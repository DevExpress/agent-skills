// DevExpress WPF AI Chat Control — Quickstart (C#)
// Mirrors the official DevExpress CLI template: `dotnet new dx.wpf.aichat` (v26.1).
// Shows App.xaml.cs startup: DevExpress theme setup + Azure OpenAI client registration.
//
// Credentials are read from App.config (see App.config in this folder). The demo endpoint
// is the same one shipped in DevExpress demos / CLI templates — it is rate limited and
// for evaluation only. DevExpress does not offer a REST API and ships no built-in LLMs/SLMs;
// using the demo credentials in production is prohibited.
//
// Requires: .NET 8+, the Microsoft.NET.Sdk.Razor project SDK, and the WebView2 runtime.

using System;
using System.Configuration;
using System.Windows;
using Microsoft.Extensions.AI;
using DevExpress.Xpf.Core;
using DevExpress.AIIntegration;

namespace DxAiChatDemo {
    // ------------------------------------------------------------------
    // App.xaml.cs — register the IChatClient before any window opens.
    // ------------------------------------------------------------------
    public partial class App : Application {
        static App() {
            CompatibilitySettings.UseLightweightThemes = true;
            ApplicationThemeHelper.Preload(PreloadCategories.Core);
            ApplicationThemeHelper.ApplicationThemeName = Theme.Win11Light.Name;

            // --- Azure OpenAI (matches the dx.wpf.aichat template) ---
            var azureEndpoint   = ConfigurationManager.AppSettings["AzureOpenAI.Endpoint"];
            var azureKey        = ConfigurationManager.AppSettings["AzureOpenAI.Key"];
            var azureDeployment = ConfigurationManager.AppSettings["AzureOpenAI.Deployment"];
            if (string.IsNullOrEmpty(azureEndpoint) || string.IsNullOrEmpty(azureKey) || string.IsNullOrEmpty(azureDeployment))
                throw new InvalidOperationException(
                    "Specify the Azure OpenAI endpoint, key, and deployment name in the 'App.config' file.");

            var azureOpenAIClient = new Azure.AI.OpenAI.AzureOpenAIClient(
                new Uri(azureEndpoint), new Azure.AzureKeyCredential(azureKey));
            IChatClient chatClient = azureOpenAIClient.GetChatClient(azureDeployment).AsIChatClient();

            AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
        }
    }

    // ------------------------------------------------------------------
    // MainWindow.xaml.cs — code-behind (empty after setup). See quickstart.xaml.
    // ------------------------------------------------------------------
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
        }
    }
}

// ------------------------------------------------------------------
// Other providers — same pattern, just build a different IChatClient and
// call RegisterChatClient. Full snippets live in references/getting-started.md.
//
//   OpenAI:
//     IChatClient chatClient = new OpenAI.OpenAIClient(apiKey)
//         .GetChatClient("gpt-4o-mini").AsIChatClient();
//
//   Ollama (dotnet add package OllamaSharp):
//     IChatClient chatClient = new OllamaApiClient(
//         new Uri("http://localhost:11434/"), "llama3.1");
//
//   AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
// ------------------------------------------------------------------
