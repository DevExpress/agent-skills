# Validation and Form Submission

## Adding Validation Rules

Specify `validationRules[]` on a simple item to apply validation. Use `isRequired: true` as shorthand for a RequiredRule.

**Common validation rule types:**

| Type | Description |
|---|---|
| `'required'` | Value must not be empty |
| `'numeric'` | Value must be numeric |
| `'email'` | Value must be a valid email address |
| `'range'` | Value must be within `min`/`max` |
| `'stringLength'` | String length within `min`/`max` |
| `'pattern'` | Value must match a regex |
| `'custom'` | Custom validation function |

### jQuery

```js
$('#form').dxForm({
    formData: { name: '', officeNumber: null, email: '' },
    colCount: 2,
    items: [
        {
            dataField: 'name',
            isRequired: true
        },
        {
            dataField: 'officeNumber',
            validationRules: [{ type: 'numeric' }]
        },
        {
            dataField: 'email',
            validationRules: [{ type: 'email' }]
        }
    ]
});
```

### Angular

```html
<!-- app.html -->
<dx-form [formData]="employee" [colCount]="2">
    <dxi-form-simple-item dataField="name" [isRequired]="true"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="officeNumber">
        <dxi-form-validation-rule type="numeric"></dxi-form-validation-rule>
    </dxi-form-simple-item>
    <dxi-form-simple-item dataField="email">
        <dxi-form-validation-rule type="email"></dxi-form-validation-rule>
    </dxi-form-simple-item>
</dx-form>
```

### Vue (Composition API + TypeScript)

```vue
<template>
    <DxForm :form-data="employee" :col-count="2">
        <DxSimpleItem data-field="name" :is-required="true" />
        <DxSimpleItem data-field="officeNumber">
            <DxNumericRule />
        </DxSimpleItem>
        <DxSimpleItem data-field="email">
            <DxEmailRule />
        </DxSimpleItem>
    </DxForm>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import { DxForm, DxSimpleItem, DxNumericRule, DxEmailRule } from 'devextreme-vue/form';

const employee = reactive({ name: '', officeNumber: null as number | null, email: '' });
</script>
```

### React (TSX + hooks)

```tsx
import { Form, SimpleItem, RequiredRule, NumericRule, EmailRule } from 'devextreme-react/form';
import { useRef } from 'react';

interface Employee { name: string; officeNumber: number | null; email: string; }

function App() {
    const employee = useRef<Employee>({ name: '', officeNumber: null, email: '' });

    return (
        <Form formData={employee.current} colCount={2}>
            <SimpleItem dataField="name">
                <RequiredRule />
            </SimpleItem>
            <SimpleItem dataField="officeNumber">
                <NumericRule />
            </SimpleItem>
            <SimpleItem dataField="email">
                <EmailRule />
            </SimpleItem>
        </Form>
    );
}
```

## Showing a Validation Summary

Set `showValidationSummary` to `true` to display a summary of all errors at the bottom of the form.

```js
// jQuery
$('#form').dxForm({ showValidationSummary: true, /* ... */ });
```

```html
<!-- Angular -->
<dx-form [showValidationSummary]="true"></dx-form>
```

## Submitting the Form

Add a `ButtonItem` with `buttonOptions.useSubmitBehavior: true`. Wrap the Form in an HTML `<form>` element and call `preventDefault()` on submit.

> **See also**: `devextreme-button` skill — for `buttonOptions` configuration details.

### jQuery

```html
<!-- index.html -->
<form action="/employee-page" id="form-container">
    <div id="form"></div>
</form>
```

```js
// index.js
$(function() {
    $('#form').dxForm({
        formData: { /* ... */ },
        colCount: 2,
        items: [
            {
                itemType: 'group',
                caption: 'Personal Information',
                colCount: 2,
                items: [
                    { dataField: 'name', isRequired: true },
                    { dataField: 'officeNumber', validationRules: [{ type: 'numeric' }] },
                    { dataField: 'email', validationRules: [{ type: 'email' }] }
                ]
            },
            {
                itemType: 'button',
                buttonOptions: {
                    text: 'Submit',
                    useSubmitBehavior: true
                }
            }
        ]
    });

    $('#form-container').on('submit', function(e) {
        e.preventDefault();
        // handle submission
    });
});
```

### Angular

```html
<!-- app.html -->
<form action="/employee-page" (submit)="handleSubmit($event)">
    <dx-form [formData]="employee" [colCount]="2">
        <dxi-form-group-item caption="Personal Information" [colCount]="2">
            <dxi-form-simple-item dataField="name" [isRequired]="true"></dxi-form-simple-item>
            <dxi-form-simple-item dataField="officeNumber">
                <dxi-form-validation-rule type="numeric"></dxi-form-validation-rule>
            </dxi-form-simple-item>
            <dxi-form-simple-item dataField="email">
                <dxi-form-validation-rule type="email"></dxi-form-validation-rule>
            </dxi-form-simple-item>
        </dxi-form-group-item>
        <dxi-form-button-item [buttonOptions]="submitButtonOptions"></dxi-form-button-item>
    </dx-form>
</form>
```

```ts
// app.ts
submitButtonOptions = {
    text: 'Submit',
    useSubmitBehavior: true
};

handleSubmit(e: Event) {
    e.preventDefault();
    // handle submission
}
```

### Vue (Composition API + TypeScript)

```vue
<template>
    <form action="/employee-page" @submit.prevent="handleSubmit">
        <DxForm :form-data="employee" :col-count="2">
            <DxGroupItem caption="Personal Information" :col-count="2">
                <DxSimpleItem data-field="name" :is-required="true" />
                <DxSimpleItem data-field="officeNumber">
                    <DxNumericRule />
                </DxSimpleItem>
                <DxSimpleItem data-field="email">
                    <DxEmailRule />
                </DxSimpleItem>
            </DxGroupItem>
            <DxButtonItem :button-options="submitButtonOptions" />
        </DxForm>
    </form>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import {
    DxForm, DxGroupItem, DxSimpleItem, DxButtonItem,
    DxNumericRule, DxEmailRule
} from 'devextreme-vue/form';

const employee = reactive({ name: '', officeNumber: null as number | null, email: '' });

const submitButtonOptions = { text: 'Submit', useSubmitBehavior: true };

function handleSubmit() {
    // handle submission
}
</script>
```

### React (TSX + hooks)

```tsx
import { Form, GroupItem, SimpleItem, ButtonItem, RequiredRule, NumericRule, EmailRule } from 'devextreme-react/form';
import { useRef } from 'react';

function App() {
    const employee = useRef({ name: '', officeNumber: null as number | null, email: '' });
    const submitButtonOptions = { text: 'Submit', useSubmitBehavior: true };

    function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        // handle submission
    }

    return (
        <form action="/employee-page" onSubmit={handleSubmit}>
            <Form formData={employee.current} colCount={2}>
                <GroupItem caption="Personal Information" colCount={2}>
                    <SimpleItem dataField="name">
                        <RequiredRule />
                    </SimpleItem>
                    <SimpleItem dataField="officeNumber">
                        <NumericRule />
                    </SimpleItem>
                    <SimpleItem dataField="email">
                        <EmailRule />
                    </SimpleItem>
                </GroupItem>
                <ButtonItem buttonOptions={submitButtonOptions} />
            </Form>
        </form>
    );
}
```
