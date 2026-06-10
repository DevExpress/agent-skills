# Changing Form Properties at Runtime

## Updating Form Options

Use the `option(name, value)` method (jQuery) or two-way binding (Angular/Vue/React) to change a top-level Form property at runtime.

The example below toggles `readOnly` using a CheckBox.

> **See also**: `devextreme-checkbox` skill.

### jQuery

```js
$(function() {
    const form = $('#form').dxForm({
        formData: { /* ... */ }
    }).dxForm('instance');

    $('#readOnlyCheckBox').dxCheckBox({
        text: 'Enable read-only mode',
        value: false,
        onValueChanged(e) {
            form.option('readOnly', e.value);
        }
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-form [formData]="employee" [readOnly]="isFormReadOnly"></dx-form>

<dx-check-box text="Enable read-only mode" [(value)]="isFormReadOnly"></dx-check-box>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular/ui/form';
import { DxCheckBoxComponent } from 'devextreme-angular/ui/check-box';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [DxFormComponent, DxCheckBoxComponent],
    templateUrl: './app.html'
})
export class AppComponent {
    employee = { /* ... */ };
    isFormReadOnly = false;
}
```

### Vue (Composition API + TypeScript)

```vue
<template>
    <DxForm :form-data="employee" :read-only="isFormReadOnly" />
    <DxCheckBox text="Enable read-only mode" v-model:value="isFormReadOnly" />
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { DxForm } from 'devextreme-vue/form';
import DxCheckBox from 'devextreme-vue/check-box';

const employee = reactive({ /* ... */ });
const isFormReadOnly = ref(false);
</script>
```

### React (TSX + hooks)

```tsx
import { Form } from 'devextreme-react/form';
import CheckBox from 'devextreme-react/check-box';
import { useRef, useState, useCallback } from 'react';

function App() {
    const employee = useRef({ /* ... */ });
    const [isReadOnly, setIsReadOnly] = useState(false);
    const handleReadOnlyChanged = useCallback((e) => setIsReadOnly(e.value as boolean), []);

    return (
        <>
            <Form formData={employee.current} readOnly={isReadOnly} />
            <CheckBox
                text="Enable read-only mode"
                value={isReadOnly}
                onValueChanged={handleReadOnlyChanged}
            />
        </>
    );
}
```

## Updating a Specific Item Option

Use the `itemOption(dataField, option, value)` method to change a single item property without re-rendering the whole form.

```js
// jQuery — disable the hireDate editor at runtime
const form = $('#form').dxForm({ /* ... */ }).dxForm('instance');
form.itemOption('hireDate', 'disabled', true);

// Read an item option
const isDisabled = form.itemOption('hireDate', 'disabled');
```

```ts
// Angular — get a reference via ViewChild
import { ViewChild } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';

@Component({ /* ... */ })
export class AppComponent {
    @ViewChild(DxFormComponent, { static: false }) formRef!: DxFormComponent;

    disableHireDate() {
        this.formRef.instance.itemOption('hireDate', 'disabled', true);
    }
}
```

```vue
<!-- Vue — get a reference via ref -->
<template>
    <DxForm ref="formRef" :form-data="employee" />
    <DxButton text="Disable Hire Date" @click="disableHireDate" />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { DxForm } from 'devextreme-vue/form';
import DxButton from 'devextreme-vue/button';

const formRef = ref<InstanceType<typeof DxForm>>();

function disableHireDate() {
    formRef.value?.instance.itemOption('hireDate', 'disabled', true);
}
</script>
```

```tsx
// React — get a reference via useRef + onInitialized
import dxForm from 'devextreme/ui/form';
import { useRef, useCallback } from 'react';

function App() {
    const formInstance = useRef<dxForm | null>(null);
    const handleInitialized = useCallback((e) => { formInstance.current = e.component; }, []);

    function disableHireDate() {
        formInstance.current?.itemOption('hireDate', 'disabled', true);
    }

    return (
        <Form
            formData={employee.current}
            onInitialized={handleInitialized}
        />
    );
}
```

## Updating Data Programmatically

Use `updateData(dataField, value)` to update a specific field's value in `formData` and re-render the corresponding editor.

```js
// jQuery
form.updateData('name', 'Jane Doe');

// Update multiple fields at once
form.updateData({ name: 'Jane Doe', officeNumber: 42 });
```

```ts
// Angular
this.formRef.instance.updateData('name', 'Jane Doe');
```

```ts
// Vue
formRef.value?.instance.updateData('name', 'Jane Doe');
```

```ts
// React
formInstance.current?.updateData('name', 'Jane Doe');
```
