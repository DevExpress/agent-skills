# Items, Layout, Groups, and Tabs

## Configuring Items

The `items[]` array drives the form layout. A string entry creates a default simple item for the named field. An object entry gives you full control.

```js
// jQuery — mix of string shorthand and explicit config
$('#form').dxForm({
    formData: {
        name: 'John Heart',
        officeNumber: 901,
        hireDate: new Date(2012, 4, 13)
    },
    items: [
        'name',
        'officeNumber',
        {
            dataField: 'hireDate',
            editorOptions: { disabled: true }
        }
    ]
});
```

### Overriding the editor type

Use `editorType` to select a specific editor. Use `editorOptions` to pass the editor's own options.

> **See also**: editor-specific skills — `devextreme-selectbox`, `devextreme-checkbox`, `devextreme-textarea` — for valid `editorOptions` properties.

```js
// jQuery — use a SelectBox for a string field
{
    dataField: 'department',
    editorType: 'dxSelectBox',
    editorOptions: {
        dataSource: ['IT', 'HR', 'Finance'],
        placeholder: 'Select department'
    }
}
```

```html
<!-- Angular -->
<dxi-form-simple-item dataField="department" editorType="dxSelectBox" [editorOptions]="deptOptions"></dxi-form-simple-item>
```

```ts
// Angular component
deptOptions = { dataSource: ['IT', 'HR', 'Finance'], placeholder: 'Select department' };
```

```vue
<!-- Vue -->
<DxSimpleItem data-field="department" editor-type="dxSelectBox" :editor-options="deptOptions" />
```

```tsx
// React
<SimpleItem dataField="department" editorType="dxSelectBox" editorOptions={deptOptions} />
```

### Custom item template

When you need an editor or content that is not a standard DevExtreme widget:

```html
<!-- Angular -->
<dxi-form-simple-item dataField="notes" [template]="'notesTemplate'">
</dxi-form-simple-item>
<div *dxTemplate="let data of 'notesTemplate'">
    <dx-text-area [(value)]="employee.notes"></dx-text-area>
</div>
```

## Columns

Use `colCount` for a fixed number of columns. Use `colSpan` on individual items to make them span multiple columns.

```js
// jQuery — 2-column layout, notes item spans both columns
$('#form').dxForm({
    formData: { name: '', position: '', notes: '' },
    colCount: 2,
    items: ['name', 'position', { dataField: 'notes', colSpan: 2 }]
});
```

```html
<!-- Angular -->
<dx-form [formData]="employee" [colCount]="2">
    <dxi-form-simple-item dataField="name"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="position"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="notes" [colSpan]="2"></dxi-form-simple-item>
</dx-form>
```

```vue
<!-- Vue -->
<DxForm :form-data="employee" :col-count="2">
    <DxSimpleItem data-field="name" />
    <DxSimpleItem data-field="position" />
    <DxSimpleItem data-field="notes" :col-span="2" />
</DxForm>
```

```tsx
// React
<Form formData={employee} colCount={2}>
    <SimpleItem dataField="name" />
    <SimpleItem dataField="position" />
    <SimpleItem dataField="notes" colSpan={2} />
</Form>
```

### Responsive columns

```js
// jQuery — 1 col on phones, 2 on tablets, 3 on desktops
$('#form').dxForm({
    colCountByScreen: { xs: 1, sm: 2, md: 3, lg: 3 }
});
```

```html
<!-- Angular -->
<dx-form [colCountByScreen]="colCountByScreen"></dx-form>
```

```ts
// Angular component
colCountByScreen = { xs: 1, sm: 2, md: 3, lg: 3 };
```

## Groups

Add a `GroupItem` to organize related fields under a heading. Groups can have their own `colCount`.

```js
// jQuery — two side-by-side groups
$('#form').dxForm({
    formData: { name: '', position: '', hireDate: null, phone: '', email: '' },
    colCount: 2,
    items: [
        {
            itemType: 'group',
            caption: 'Personal Information',
            items: ['name', 'position', 'hireDate']
        },
        {
            itemType: 'group',
            caption: 'Contacts',
            items: ['phone', 'email']
        }
    ]
});
```

```html
<!-- Angular -->
<dx-form [formData]="employee" [colCount]="2">
    <dxi-form-group-item caption="Personal Information">
        <dxi-form-simple-item dataField="name"></dxi-form-simple-item>
        <dxi-form-simple-item dataField="position"></dxi-form-simple-item>
        <dxi-form-simple-item dataField="hireDate"></dxi-form-simple-item>
    </dxi-form-group-item>
    <dxi-form-group-item caption="Contacts">
        <dxi-form-simple-item dataField="phone"></dxi-form-simple-item>
        <dxi-form-simple-item dataField="email"></dxi-form-simple-item>
    </dxi-form-group-item>
</dx-form>
```

```vue
<!-- Vue -->
<DxForm :form-data="employee" :col-count="2">
    <DxGroupItem caption="Personal Information">
        <DxSimpleItem data-field="name" />
        <DxSimpleItem data-field="position" />
        <DxSimpleItem data-field="hireDate" />
    </DxGroupItem>
    <DxGroupItem caption="Contacts">
        <DxSimpleItem data-field="phone" />
        <DxSimpleItem data-field="email" />
    </DxGroupItem>
</DxForm>
```

```tsx
// React
import { Form, SimpleItem, GroupItem } from 'devextreme-react/form';

<Form formData={employee} colCount={2}>
    <GroupItem caption="Personal Information">
        <SimpleItem dataField="name" />
        <SimpleItem dataField="position" />
        <SimpleItem dataField="hireDate" />
    </GroupItem>
    <GroupItem caption="Contacts">
        <SimpleItem dataField="phone" />
        <SimpleItem dataField="email" />
    </GroupItem>
</Form>
```

## Tabs

Use `TabbedItem` when you want to show related items in switchable panels.

```js
// jQuery
$('#form').dxForm({
    formData: { name: '', phone: '', email: '' },
    items: [{
        itemType: 'tabbed',
        tabs: [
            { title: 'Personal', items: ['name'] },
            { title: 'Contacts', items: ['phone', 'email'] }
        ]
    }]
});
```

```html
<!-- Angular -->
<dx-form [formData]="employee">
    <dxi-form-tabbed-item>
        <dxi-form-tab title="Personal">
            <dxi-form-simple-item dataField="name"></dxi-form-simple-item>
        </dxi-form-tab>
        <dxi-form-tab title="Contacts">
            <dxi-form-simple-item dataField="phone"></dxi-form-simple-item>
            <dxi-form-simple-item dataField="email"></dxi-form-simple-item>
        </dxi-form-tab>
    </dxi-form-tabbed-item>
</dx-form>
```

```vue
<!-- Vue -->
import { DxForm, DxTabbedItem, DxTab, DxSimpleItem } from 'devextreme-vue/form';

<DxForm :form-data="employee">
    <DxTabbedItem>
        <DxTab title="Personal">
            <DxSimpleItem data-field="name" />
        </DxTab>
        <DxTab title="Contacts">
            <DxSimpleItem data-field="phone" />
            <DxSimpleItem data-field="email" />
        </DxTab>
    </DxTabbedItem>
</DxForm>
```

```tsx
// React
import { Form, TabbedItem, Tab, SimpleItem } from 'devextreme-react/form';

<Form formData={employee}>
    <TabbedItem>
        <Tab title="Personal">
            <SimpleItem dataField="name" />
        </Tab>
        <Tab title="Contacts">
            <SimpleItem dataField="phone" />
            <SimpleItem dataField="email" />
        </Tab>
    </TabbedItem>
</Form>
```
