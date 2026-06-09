# Toolbar Customization

## Default Toolbar Items

DataGrid includes these built-in toolbar items (shown only when the relevant feature is enabled):

| Name | Shown when |
|---|---|
| `'addRowButton'` | `editing.allowAdding: true` |
| `'applyFilterButton'` | `filterRow` is visible |
| `'columnChooserButton'` | `columnChooser.enabled: true` |
| `'exportButton'` | `export.enabled: true` |
| `'groupPanel'` | `groupPanel.visible: true` |
| `'revertButton'` | `editing.mode: 'batch'` |
| `'saveButton'` | `editing.mode: 'batch'` |
| `'searchPanel'` | `searchPanel.visible: true` |

> **Critical**: Declaring `toolbar.items` **replaces** the default toolbar. Re-list every built-in item you still want.

---

## Customizing the Toolbar

### jQuery

```js
$('#data-grid').dxDataGrid({
    toolbar: {
        items: [
            'groupPanel',                    // built-in: group panel
            {
                location: 'after',
                widget: 'dxButton',
                options: {
                    text: 'Refresh',
                    icon: 'refresh',
                    onClick() {
                        $('#data-grid').dxDataGrid('instance').refresh();
                    }
                }
            },
            { name: 'addRowButton', showText: 'always' },   // built-in with text label
            'exportButton',
            'columnChooserButton',
            'searchPanel'
        ]
    }
});
```

### Angular

```html
<dx-data-grid>
    <dxo-data-grid-toolbar>
        <dxi-data-grid-toolbar-item name="groupPanel"></dxi-data-grid-toolbar-item>
        <dxi-data-grid-toolbar-item location="after">
            <dx-button text="Refresh" icon="refresh" (onClick)="refreshGrid()"></dx-button>
        </dxi-data-grid-toolbar-item>
        <dxi-data-grid-toolbar-item name="addRowButton" showText="always"></dxi-data-grid-toolbar-item>
        <dxi-data-grid-toolbar-item name="exportButton"></dxi-data-grid-toolbar-item>
        <dxi-data-grid-toolbar-item name="columnChooserButton"></dxi-data-grid-toolbar-item>
        <dxi-data-grid-toolbar-item name="searchPanel"></dxi-data-grid-toolbar-item>
    </dxo-data-grid-toolbar>
</dx-data-grid>
```

```ts
// app.ts
import { ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';

@ViewChild(DxDataGridComponent) gridRef!: DxDataGridComponent;

refreshGrid() {
    this.gridRef.instance.refresh();
}
```

### Vue (Composition API + TypeScript)

```vue
<template>
    <DxDataGrid ref="gridRef">
        <DxToolbar>
            <DxItem name="groupPanel" />
            <DxItem location="after">
                <template #default>
                    <DxButton text="Refresh" icon="refresh" @click="refreshGrid" />
                </template>
            </DxItem>
            <DxItem name="addRowButton" show-text="always" />
            <DxItem name="exportButton" />
            <DxItem name="columnChooserButton" />
            <DxItem name="searchPanel" />
        </DxToolbar>
    </DxDataGrid>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { DxDataGrid, DxToolbar, DxItem } from 'devextreme-vue/data-grid';
import DxButton from 'devextreme-vue/button';

const gridRef = ref<InstanceType<typeof DxDataGrid>>();

function refreshGrid() {
    gridRef.value?.instance.refresh();
}
</script>
```

### React (TSX)

```tsx
import { useRef, useCallback } from 'react';
import { DataGrid, Toolbar, Item } from 'devextreme-react/data-grid';
import Button from 'devextreme-react/button';
import dxDataGrid from 'devextreme/ui/data_grid';

function App() {
    const gridInstance = useRef<dxDataGrid | null>(null);
    const handleInitialized = useCallback((e) => { gridInstance.current = e.component; }, []);
    const handleRefresh = useCallback(() => gridInstance.current?.refresh(), []);

    return (
        <DataGrid onInitialized={handleInitialized}>
            <Toolbar>
                <Item name="groupPanel" />
                <Item location="after">
                    <Button text="Refresh" icon="refresh" onClick={handleRefresh} />
                </Item>
                <Item name="addRowButton" showText="always" />
                <Item name="exportButton" />
                <Item name="columnChooserButton" />
                <Item name="searchPanel" />
            </Toolbar>
        </DataGrid>
    );
}
```

---

## Toolbar Item Location

| `location` value | Position |
|---|---|
| `'before'` | Left side of toolbar |
| `'center'` | Center of toolbar |
| `'after'` | Right side of toolbar (default for most built-in items) |

---

## Common Toolbar Widget Types

Besides `dxButton`, you can embed any DevExtreme widget using `widget` (jQuery) or a template slot (Angular/Vue/React):

```js
// jQuery — SelectBox in toolbar
{
    location: 'before',
    widget: 'dxSelectBox',
    options: {
        dataSource: departments,
        displayExpr: 'Name',
        valueExpr: 'ID',
        onValueChanged(e) {
            // filter grid by department
        }
    }
}
```
