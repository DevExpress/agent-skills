# Getting Started with dxForm

## Initialization (all frameworks)

The Form auto-generates label-editor pairs from the `formData` object.

**Auto-selected editor types:**
| Value type | Default editor |
|---|---|
| `string` | dxTextBox |
| `number` | dxNumberBox |
| `Date` | dxDateBox |
| `boolean` | dxCheckBox |

### jQuery

```html
<!-- index.html -->
<div id="form"></div>
```

```js
// index.js
$(function() {
    $('#form').dxForm({
        formData: {
            name: 'John Heart',
            officeNumber: 901,
            hireDate: new Date(2012, 4, 13)
        }
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-form [formData]="employee"></dx-form>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular/ui/form';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [DxFormComponent],
    templateUrl: './app.html'
})
export class AppComponent {
    employee = {
        name: 'John Heart',
        officeNumber: 901,
        hireDate: new Date(2012, 4, 13)
    };
}
```

### Vue (Composition API + TypeScript)

```vue
<!-- App.vue -->
<template>
    <DxForm :form-data="employee" />
</template>

<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { reactive } from 'vue';
import { DxForm } from 'devextreme-vue/form';

const employee = reactive({
    name: 'John Heart',
    officeNumber: 901,
    hireDate: new Date(2012, 4, 13)
});
</script>
```

### React (TSX + hooks)

```tsx
// App.tsx
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { Form } from 'devextreme-react/form';
import { useRef } from 'react';

const employeeData = {
    name: 'John Heart',
    officeNumber: 901,
    hireDate: new Date(2012, 4, 13)
};

function App() {
    const employee = useRef(employeeData);

    return <Form formData={employee.current} />;
}

export default App;
```

## Listening for field changes

Use `onFieldDataChanged` to react to any field value change. The event argument provides `dataField` and `value`.

```js
// jQuery
$('#form').dxForm({
    formData: { /* ... */ },
    onFieldDataChanged(e) {
        console.log(`${e.dataField} changed to`, e.value);
    }
});
```

```html
<!-- Angular -->
<dx-form [formData]="employee" (onFieldDataChanged)="onFieldChanged($event)"></dx-form>
```

```ts
// Angular component
import { DxFormTypes } from 'devextreme-angular/ui/form';

onFieldChanged(e: DxFormTypes.FieldDataChangedEvent) {
    console.log(`${e.dataField} changed to`, e.value);
}
```

```vue
<!-- Vue -->
<DxForm :form-data="employee" @field-data-changed="onFieldChanged" />
```

```tsx
// React
function onFieldDataChanged(e) {
    console.log(e.dataField, e.value);
}
// ...
<Form formData={employee.current} onFieldDataChanged={onFieldDataChanged} />
```
