<#
.SYNOPSIS
    Lists DI wiring mistakes that compile but break at run time.

.DESCRIPTION
    Scans a WPF project (skipping bin, obj, .git) and prints one line per finding:
      MISSING-REGISTRATION <Type>  <file>   a view resolves <Type> from the container ({dxmvvm:Ioc}, custom
                                            markup extension, GetService/GetRequiredService) but nothing registers it
      NEW-OUTSIDE-ROOT     <Type>  <file>   a registered type is still created with `new` outside the composition root
      VIEWMODELSOURCE-IN-XAML <Type> <file>    a view still creates a migrated view model with dxmvvm:ViewModelSource
      STATIC-SOURCE-OUTSIDE-ROOT <Call> <file> a static call whose result is registered is still made outside the composition root
      POCO-PLAIN-REGISTRATION, NO-PUBLIC-CONSTRUCTOR, VIEWMODELSOURCE-CREATE-NOT-NEW, IOC-PROVIDER-NOT-CONFIGURED,
      STARTUPURI-NULL                           registrations and startup code that throw or misbehave at run time
    In a git working tree it also puts CRLF line endings back into the files under Path that the agent changed
    and that were CRLF in git (EOL-RESTORED lines; not counted as findings). Files that already had uncommitted
    changes when -Baseline ran are never touched; without a baseline, switched files are only listed (EOL-CHANGED).
    Prints "OK" when there are no findings. Exit code = number of findings.

.PARAMETER Path
    Project or solution folder. Defaults to the current directory.

.PARAMETER Project
    The WPF app project being migrated (its .csproj name without extension), when the folder has several.
    The checks cover that project and the projects it references, so registrations in tests or other apps
    do not count.

.PARAMETER Baseline
    Record which files already have uncommitted changes (in the git directory, not the working tree) and exit.
    Run once before changing anything.
#>
param([string] $Path = '.', [string] $Project, [switch] $Baseline)

$repo = git -C $Path rev-parse --show-toplevel 2>$null
$baselineFile = $repo ? (git -C $Path rev-parse --path-format=absolute --git-path dx-di-baseline.txt 2>$null) : $null
if ($Baseline) {
    if (-not $baselineFile) { 'BASELINE  not a git working tree - line endings will only be listed'; exit 0 }
    $dirty = @(git -C $repo -c core.quotepath=off status --porcelain --untracked-files=all 2>$null | ForEach-Object { $_.Substring(3) -replace '^.* -> ', '' })
    # written even when empty: an empty baseline means "restore every file the agent changes"
    [System.IO.File]::WriteAllLines($baselineFile, [string[]]$dirty)
    "BASELINE  $($dirty.Count) file(s) with uncommitted changes recorded; they will not be touched"
    exit 0
}

# Scope: the WPF app project and the projects it references (not tests or other apps in the folder)
$projects = @(Get-ChildItem $Path -Recurse -File -Filter *.csproj | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$apps = @($projects | Where-Object {
    $xml = Get-Content $_.FullName -Raw
    $xml -match '(?i)<OutputType>\s*WinExe\s*</OutputType>' -and
        $xml -match '(?i)<UseWPF>\s*true\s*</UseWPF>|60dc8134-eba5-43b8-bcc9-bb4bc16c2548|Include="PresentationFramework"' -and
        (-not $Project -or $_.BaseName -eq $Project)
})
if ($apps.Count -gt 1) {
    "AMBIGUOUS  several WPF app projects: $(($apps | ForEach-Object BaseName) -join ', ') - rerun with -Project <name of the app you migrate>"
    exit 1
}
$scope = @()
if ($apps) {
    $queue = [System.Collections.Generic.Queue[string]]::new(); $queue.Enqueue($apps[0].FullName)
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    while ($queue.Count) {
        $proj = $queue.Dequeue()
        if (-not $seen.Add($proj) -or -not (Test-Path $proj)) { continue }
        $scope += Split-Path $proj
        foreach ($m in [regex]::Matches((Get-Content $proj -Raw), '<ProjectReference\s+Include="([^"]+)"')) {
            $queue.Enqueue([System.IO.Path]::GetFullPath((Join-Path (Split-Path $proj) $m.Groups[1].Value)))
        }
    }
}
$inScope = { param($file) -not $scope -or ($scope | Where-Object { $file.StartsWith($_ + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase) }) }

$files = Get-ChildItem $Path -Recurse -File -Include *.cs, *.xaml |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj|\.git)[\\/]' -and (& $inScope $_.FullName) }
$code = @{}
foreach ($f in $files) {
    $text = Get-Content $f.FullName -Raw
    if ($null -eq $text) { $text = '' }
    if ($f.Extension -eq '.cs') { $text = [regex]::Replace([regex]::Replace($text, '(?s)/\*.*?\*/', ''), '(?m)(?<![:"])//.*$', '') }
    else { $text = [regex]::Replace($text, '(?s)<!--.*?-->', '') }
    $code[$f.FullName] = $text
}
$rel = { param($p) [System.IO.Path]::GetRelativePath((Resolve-Path $Path), $p) }

# Types the project registers: AddX<T>, AddX<I, T>, AddX(typeof(T)), AddX(typeof(I), ...GetPOCOType(typeof(T))).
# $services holds what the container hands out (the service type: I for AddX<I, T>) - what {dxmvvm:Ioc} can resolve;
# $registered also holds implementation types - what the migration moved into the container.
$registered = [System.Collections.Generic.HashSet[string]]::new()
$services = [System.Collections.Generic.HashSet[string]]::new()
foreach ($text in $code.Values) {
    foreach ($m in [regex]::Matches($text, 'Add(?:Singleton|Transient|Scoped)\s*<\s*([\w.]+)\s*(?:,\s*([\w.]+)\s*)?>')) {
        [void]$services.Add(($m.Groups[1].Value -split '\.')[-1])
        foreach ($g in 1, 2) { if ($m.Groups[$g].Success) { [void]$registered.Add(($m.Groups[$g].Value -split '\.')[-1]) } }
    }
    foreach ($m in [regex]::Matches($text, 'Add(?:Singleton|Transient|Scoped)\s*\(\s*typeof\s*\(\s*([\w.]+)\s*\)')) {
        [void]$services.Add(($m.Groups[1].Value -split '\.')[-1])
        [void]$registered.Add(($m.Groups[1].Value -split '\.')[-1])
    }
    # factory registrations: AddX(sp => new T(...)) / AddX(sp => ViewModelSource.Create(() => new T(...))) / AddX(sp => T.Create(...)),
    # also with a block body: AddX(sp => { ...; return ...; })
    # (a generic AddX<I>(sp => new T(...)) already registered I as the service; T only counts as registered)
    foreach ($m in [regex]::Matches($text, 'Add(?:Singleton|Transient|Scoped)\s*(<[^;>]*>)?\s*\(\s*\w+\s*=>\s*(\{)?([^;]*)')) {
        $generic = $m.Groups[1].Success
        $body = $m.Groups[3].Value
        if ($m.Groups[2].Success) {
            $depth = 1; $i = $m.Groups[2].Index + 1
            while ($i -lt $text.Length -and $depth -gt 0) { if ($text[$i] -eq '{') { $depth++ } elseif ($text[$i] -eq '}') { $depth-- }; $i++ }
            $body = $text.Substring($m.Groups[2].Index, $i - $m.Groups[2].Index)
        }
        # The service type of a non-generic AddX is what the factory returns: the first type created in the
        # returned expression (the body itself, or the last `return` of a block). Everything created counts as registered.
        $creation = 'new\s+(?:[\w.]+\.)?(\w+)\s*\(|(?:[\w.]+\.)?(\w+)\s*\.\s*Create\s*\('
        $returned = $m.Groups[2].Success ? ([regex]::Matches($body, '\breturn\s+([^;]+);') | Select-Object -Last 1 | ForEach-Object { $_.Groups[1].Value }) : $body
        foreach ($expression in @(@{ Text = $body; Service = $false }, @{ Text = "$returned"; Service = -not $generic })) {
            $first = $true
            foreach ($created in [regex]::Matches($expression.Text, $creation)) {
                foreach ($g in 1, 2) {
                    if (-not $created.Groups[$g].Success -or $created.Groups[$g].Value -eq 'ViewModelSource') { continue }
                    [void]$registered.Add($created.Groups[$g].Value)
                    if ($expression.Service -and $first) { [void]$services.Add($created.Groups[$g].Value); $first = $false }
                }
            }
        }
    }
}

$findings = [System.Collections.Generic.List[string]]::new()

# Types resolved from the container
# Each pattern with the type names it is trusted for. GetService<T> also serves the DevExpress service
# container (GetService<IDialogService>()), whose services must NOT be registered, so only views and
# view models are checked there.
$resolvePatterns = @(
    @('[{<]\w+:\w+\s+(?:Type=)?"?\{x:Type\s+(?:\w+:)?(\w+)\}', 'ViewModel$|View$|Window$|Service$|Repository$|Store$|Client$'),  # {dxmvvm:Ioc Type={x:Type vm:X}}, <dxmvvm:Ioc Type="{x:Type vm:X}"/>
    @('\{\w+:(?:Ioc|DISource|Resolve\w*)\s+(?:Type=)?\w+:(\w+)\}', 'ViewModel$|View$|Window$|Service$|Repository$|Store$|Client$'),  # {local:DISource Type=vm:X}
    @('Get(?:Required)?Service\s*<\s*(?:[\w.]+\.)?(\w+)\s*>', 'ViewModel$|View$|Window$')
)
foreach ($entry in $code.GetEnumerator()) {
    foreach ($pattern in $resolvePatterns) {
        foreach ($m in [regex]::Matches($entry.Value, $pattern[0])) {
            $type = $m.Groups[1].Value
            if ($type -notmatch $pattern[1]) { continue }
            if (-not $services.Contains($type)) {
                $hint = $registered.Contains($type) ? '  (registered only as the implementation of another service type; register it as itself)' : ''
                $findings.Add("MISSING-REGISTRATION $type  $(& $rel $entry.Key)$hint")
            }
        }
    }
}

# Views that still create a view model themselves with dxmvvm:ViewModelSource although the migration moved it:
# it is registered (the container's instance is never used) or it can no longer be created without arguments
$allCode = $code.Values -join "`n"
foreach ($entry in $code.GetEnumerator()) {
    if ($entry.Key -notlike '*.xaml') { continue }
    foreach ($m in [regex]::Matches($entry.Value, '[{<]\w+:ViewModelSource\s+(?:Type\s*=\s*)?"?(?:\{x:Type\s+)?(?:\w+:)?(\w+)')) {
        $type = $m.Groups[1].Value
        $ctors = [regex]::Matches($allCode, "(?m)^\s*(?:public|protected|private|internal)[\w\s]*?\s$type\s*\(([^)]*)\)")
        $needsArguments = $ctors.Count -gt 0 -and -not ($ctors | Where-Object {
            $parameters = $_.Groups[1].Value.Trim()
            -not $parameters -or -not ($parameters -split ',' | Where-Object { $_ -notmatch '=' })
        })
        if ($registered.Contains($type) -or $needsArguments) {
            $why = $registered.Contains($type) ? 'it is registered, but this view creates its own instance' : 'its constructor now needs arguments'
            $findings.Add("VIEWMODELSOURCE-IN-XAML $type  $(& $rel $entry.Key)  ($why - resolve it with {dxmvvm:Ioc Type={x:Type ...}})")
        }
    }
}

# Runtime-generated POCO view models registered as their plain class: bindable `public virtual`
# properties, no notifying base class, no [GenerateViewModel] -> needs ViewModelSource.GetPOCOType(typeof(T))
$allCode = $code.Values -join "`n"
foreach ($text in $code.Values) {
    foreach ($m in [regex]::Matches($text, '(?s)((?:\[[^\]]*\]\s*)*)public\s+(?:partial\s+)?class\s+(\w+)\s*(?::\s*([\w.<>, ]+))?\s*\{')) {
        $type = $m.Groups[2].Value
        if ($m.Groups[1].Value -match 'GenerateViewModel') { continue }
        if ($m.Groups[3].Success -and $m.Groups[3].Value -match 'ViewModelBase|BindableBase|INotifyPropertyChanged|ObservableObject') { continue }
        if (-not $registered.Contains($type)) { continue }
        # this class's own body only: from its opening brace to the matching closing one
        $open = $m.Index + $m.Length - 1; $depth = 1; $i = $open + 1
        while ($i -lt $text.Length -and $depth -gt 0) { if ($text[$i] -eq '{') { $depth++ } elseif ($text[$i] -eq '}') { $depth-- }; $i++ }
        $body = $text.Substring($open, $i - $open)
        if ($body -notmatch 'public\s+virtual\s+[\w<>?,\s]+\s+\w+\s*\{\s*get') { continue }
        $pocoRegistered = $allCode -match "GetPOCOType\s*\(\s*typeof\s*\(\s*(?:[\w.]+\.)?$type\s*\)" -or
            $allCode -match "ViewModelSource\s*\.\s*Create\s*\(\s*\(\s*\)\s*=>\s*new\s+(?:[\w.]+\.)?$type\s*\(" -or
            $allCode -match "ViewModelSource\s*\.\s*Create\s*<\s*(?:[\w.]+\.)?$type\s*>" -or
            $allCode -match "Add(?:Singleton|Transient|Scoped)\s*(?:<[^>]*>)?\s*\(\s*\w+\s*=>\s*(?:[\w.]+\.)?$type\s*\.\s*Create\s*\("
        if (-not $pocoRegistered) {
            $findings.Add("POCO-PLAIN-REGISTRATION $type  (register ViewModelSource.GetPOCOType(typeof($type)) under $type)")
        }
    }
}

# Registered as a class the container cannot construct: no public constructor (DevExpress scaffolded
# view models have protected constructors and a static Create method that builds the POCO type)
foreach ($text in $code.Values) {
    foreach ($m in [regex]::Matches($text, '(?s)\bclass\s+(\w+)\b[^{]*\{')) {
        $type = $m.Groups[1].Value
        if (-not $registered.Contains($type)) { continue }
        $ctors = [regex]::Matches($text, "(?m)^\s*((?:public|protected|private|internal)(?:\s+\w+)*)\s+$type\s*\(")
        if (-not $ctors.Count -or ($ctors | Where-Object { $_.Groups[1].Value -match '\bpublic\b' })) { continue }
        $plain = $allCode -match "Add(?:Singleton|Transient|Scoped)\s*<\s*(?:[\w.]+\s*,\s*)?(?:[\w.]+\.)?$type\s*>\s*\(\s*\)" -or
            $allCode -match "Add(?:Singleton|Transient|Scoped)\s*\(\s*typeof\s*\(\s*(?:[\w.]+\.)?$type\s*\)\s*\)"
        if (-not $plain -or $allCode -match "GetPOCOType\s*\(\s*typeof\s*\(\s*(?:[\w.]+\.)?$type\s*\)") { continue }
        $hint = $text -match "static\s+$type\s+Create\s*\(" ? "register a factory that calls $type.Create(...)" : 'it has no public constructor'
        $findings.Add("NO-PUBLIC-CONSTRUCTOR $type  (the container cannot create it - $hint)")
    }
}

# ViewModelSource.Create builds the POCO type from a `new` expression only; anything else throws at run time
foreach ($entry in $code.GetEnumerator()) {
    if ($entry.Key -like '*.cs' -and $entry.Value -match 'ViewModelSource\s*\.\s*Create\s*(?:<[^>]*>)?\s*\(\s*\(\s*\)\s*=>(?!\s*new\b)') {
        $findings.Add("VIEWMODELSOURCE-CREATE-NOT-NEW  $(& $rel $entry.Key)  (ViewModelSource.Create(() => ...) needs 'new X(...)' as the lambda body)")
    }
}

# Startup wiring that compiles but throws when the app starts
if ($allCode -match '\{dxmvvm:Ioc\b|<dxmvvm:Ioc\b' -and $allCode -notmatch 'IocServiceProvider\s*\.\s*Default\s*\.\s*ConfigureServices\s*\(') {
    $findings.Add('IOC-PROVIDER-NOT-CONFIGURED  {dxmvvm:Ioc} is used but the provider is never handed to IocServiceProvider.Default.ConfigureServices(...) - every view that uses it throws at startup')
}
foreach ($entry in $code.GetEnumerator()) {
    if ($entry.Key -like '*.cs' -and $entry.Value -match 'StartupUri\s*=\s*null') {
        $findings.Add("STARTUPURI-NULL  $(& $rel $entry.Key)  (assigning null throws; remove the StartupUri attribute from App.xaml instead)")
    }
}

# Registered concrete types still created with new outside the composition root
$rootPattern = 'IServiceCollection|new\s+ServiceCollection\s*\(|Host\.Create(?:Default|Application)Builder'
foreach ($entry in $code.GetEnumerator()) {
    if ($entry.Key -notlike '*.cs' -or $entry.Value -match $rootPattern) { continue }
    foreach ($type in $registered) {
        if ($type -match '^I[A-Z]') { continue }
        if ($entry.Value -match "new\s+(?:[\w.]+\.)?$type\s*\(" -and $entry.Value -notmatch "class\s+$type\b") {
            $findings.Add("NEW-OUTSIDE-ROOT $type  $(& $rel $entry.Key)")
        }
    }
}

# A static source the container now provides, still called directly outside the composition root:
# AddX(Source.Get()) / AddX(sp => Source.Get()) / AddX(sp => { ...; return Source.Get(); }) registers what
# Source.Get() returns, so any other Source.Get( call bypasses the container (a static locator left behind)
$sources = [System.Collections.Generic.HashSet[string]]::new()
foreach ($m in [regex]::Matches($allCode, 'Add(?:Singleton|Transient|Scoped)\s*(?:<[^;]*?>)?\s*\(\s*(?:\w+\s*=>\s*(?:\{[^}]*?\breturn\s+)?)?(?:global::)?((?:[A-Z]\w*\.)*[A-Z]\w*)\s*\.\s*(\w+)\s*\(')) {
    $class = ($m.Groups[1].Value -split '\.')[-1]; $method = $m.Groups[2].Value
    if ($class -in 'ViewModelSource', 'ActivatorUtilities', 'Activator', 'Options', 'Host' -or ($method -eq 'Create' -and $registered.Contains($class))) { continue }
    [void]$sources.Add("$class.$method")
}
foreach ($entry in $code.GetEnumerator()) {
    if ($entry.Key -notlike '*.cs' -or $entry.Value -match $rootPattern) { continue }
    foreach ($source in $sources) {
        $class, $method = $source -split '\.'
        if ($entry.Value -match "class\s+$class\b") { continue }
        if ($entry.Value -match "\b$class\s*\.\s*$method\s*\(") {
            $findings.Add("STATIC-SOURCE-OUTSIDE-ROOT $source  $(& $rel $entry.Key)  (the container provides what it returns - inject it instead of calling it)")
        }
    }
}

# Line endings: a file that was CRLF in git and is saved with LF now shows every line as changed.
# Restored here instead of reported, so no extra turn is spent on it.
# Only files under Path, and only those the agent changed: files listed in the baseline are left alone.
if ($repo) {
    $before = ($baselineFile -and (Test-Path $baselineFile)) ? @(Get-Content $baselineFile) : $null
    foreach ($line in git -C $Path -c core.quotepath=off ls-files --eol -m --full-name -- . 2>$null) {
        if ($line -notmatch '^i/crlf\s+w/(lf|mixed)\s+\S*\s+(.+)$') { continue }
        $name = $Matches[2].Trim()
        if ($null -eq $before) { "EOL-CHANGED  $name  (was CRLF in git; restore it if you edited this file)"; continue }
        if ($before -contains $name) { continue }
        $file = Join-Path $repo $name
        $text = [System.IO.File]::ReadAllText($file)
        $bom = [System.IO.File]::ReadAllBytes($file)[0..2] -join ',' -eq '239,187,191'
        [System.IO.File]::WriteAllText($file, ($text -replace '\r?\n', "`r`n"), [System.Text.UTF8Encoding]::new($bom))
        "EOL-RESTORED  $name  (CRLF line endings put back)"
    }
}

$findings = $findings | Sort-Object -Unique
if ($findings) { $findings } else { 'OK' }
exit @($findings).Count
