# Scheduler Toolbar

The Scheduler toolbar sits above the calendar and hosts three predefined controls: the **Today** button, the **date navigator**, and the **view switcher**. Use the `toolbar` option to reorder, remove, or augment these controls.

**API reference**: [`toolbar`](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxScheduler/Configuration/toolbar/)  
**Demo**: [Scheduler Toolbar](https://js.devexpress.com/Demos/WidgetsGallery/Demo/Scheduler/Toolbar/)

---

## Predefined Toolbar Items

| `name` value | Control |
|---|---|
| `'today'` | "Today" button — navigates to today's date |
| `'dateNavigator'` | Prev/Next arrows + current date interval label |
| `'viewSwitcher'` | Buttons to switch between configured `views[]` |

> By default all three items are displayed automatically. As soon as you define `toolbar.items`, only the items in that array are shown.

---

## Reordering or Hiding Predefined Items

---

### jQuery

```js
$('#scheduler').dxScheduler({
    // ...
    toolbar: {
        items: ['today', 'viewSwitcher', 'dateNavigator'] // reordered
    }
});
```

### Angular

```html
<!-- app.html -->
<dx-scheduler ...>
    <dxo-scheduler-toolbar>
        <dxi-scheduler-toolbar-item name="today"></dxi-scheduler-toolbar-item>
        <dxi-scheduler-toolbar-item name="viewSwitcher"></dxi-scheduler-toolbar-item>
        <dxi-scheduler-toolbar-item name="dateNavigator"></dxi-scheduler-toolbar-item>
    </dxo-scheduler-toolbar>
</dx-scheduler>
```

### Vue

```html
<!-- App.vue -->
<template>
    <DxScheduler ...>
        <DxToolbar>
            <DxItem name="today" />
            <DxItem name="viewSwitcher" />
            <DxItem name="dateNavigator" />
        </DxToolbar>
    </DxScheduler>
</template>

<script setup lang="ts">
import DxScheduler, { DxToolbar, DxItem } from 'devextreme-vue/scheduler';
</script>
```

### React

```tsx
import { Scheduler, Toolbar, Item } from 'devextreme-react/scheduler';

function App() {
    return (
        <Scheduler ...>
            <Toolbar>
                <Item name="today" />
                <Item name="viewSwitcher" />
                <Item name="dateNavigator" />
            </Toolbar>
        </Scheduler>
    );
}
```

---

## Customizing Predefined Items

Pass an `options` object alongside the item `name` to override predefined control behavior. The most common customization is reordering the **dateNavigator** sub-items (`'prev'`, `'dateInterval'`, `'next'`).

### jQuery

```js
const dateNavigatorOptions = {
    items: ['prev', 'dateInterval', 'next'] // default order; rearrange as needed
};

$('#scheduler').dxScheduler({
    toolbar: {
        items: [
            { name: 'dateNavigator', options: dateNavigatorOptions },
            'viewSwitcher'
        ]
    }
});
```

### Angular

```html
<dx-scheduler ...>
    <dxo-scheduler-toolbar>
        <dxi-scheduler-toolbar-item
            name="dateNavigator"
            [options]="dateNavigatorOptions">
        </dxi-scheduler-toolbar-item>
        <dxi-scheduler-toolbar-item name="viewSwitcher"></dxi-scheduler-toolbar-item>
    </dxo-scheduler-toolbar>
</dx-scheduler>
```

```ts
// app.ts
export class AppComponent {
    dateNavigatorOptions = { items: ['prev', 'dateInterval', 'next'] };
}
```

### Vue

```html
<template>
    <DxScheduler ...>
        <DxToolbar>
            <DxItem name="dateNavigator" :options="dateNavigatorOptions" />
            <DxItem name="viewSwitcher" />
        </DxToolbar>
    </DxScheduler>
</template>

<script setup lang="ts">
import DxScheduler, { DxToolbar, DxItem } from 'devextreme-vue/scheduler';

const dateNavigatorOptions = { items: ['prev', 'dateInterval', 'next'] };
</script>
```

### React

```tsx
import { Scheduler, Toolbar, Item } from 'devextreme-react/scheduler';

const dateNavigatorOptions = { items: ['prev', 'dateInterval', 'next'] };

function App() {
    return (
        <Scheduler ...>
            <Toolbar>
                <Item name="dateNavigator" options={dateNavigatorOptions} />
                <Item name="viewSwitcher" />
            </Toolbar>
        </Scheduler>
    );
}
```

---

## Adding a Custom Button

### jQuery

```js
$('#scheduler').dxScheduler({
    toolbar: {
        items: [
            'dateNavigator',
            'viewSwitcher',
            {
                widget: 'dxButton',
                location: 'after',
                options: {
                    text: 'Export',
                    icon: 'export',
                    onClick() { /* ... */ }
                }
            }
        ]
    }
});
```

### Angular

```html
<dx-scheduler ...>
    <dxo-scheduler-toolbar>
        <dxi-scheduler-toolbar-item name="dateNavigator"></dxi-scheduler-toolbar-item>
        <dxi-scheduler-toolbar-item name="viewSwitcher"></dxi-scheduler-toolbar-item>
        <dxi-scheduler-toolbar-item location="after">
            <dx-button text="Export" icon="export" (onClick)="onExport()"></dx-button>
        </dxi-scheduler-toolbar-item>
    </dxo-scheduler-toolbar>
</dx-scheduler>
```

### Vue

```html
<template>
    <DxScheduler ...>
        <DxToolbar>
            <DxItem name="dateNavigator" />
            <DxItem name="viewSwitcher" />
            <DxItem location="after">
                <DxButton text="Export" icon="export" @click="onExport" />
            </DxItem>
        </DxToolbar>
    </DxScheduler>
</template>

<script setup lang="ts">
import DxScheduler, { DxToolbar, DxItem } from 'devextreme-vue/scheduler';
import { DxButton } from 'devextreme-vue/button';

function onExport() { /* ... */ }
</script>
```

### React

```tsx
import { useCallback } from 'react';
import { Scheduler, Toolbar, Item } from 'devextreme-react/scheduler';
import { Button } from 'devextreme-react/button';

function App() {
    const handleExport = useCallback(() => { /* ... */ }, []);

    return (
        <Scheduler ...>
            <Toolbar>
                <Item name="dateNavigator" />
                <Item name="viewSwitcher" />
                <Item location="after">
                    <Button text="Export" icon="export" onClick={handleExport} />
                </Item>
            </Toolbar>
        </Scheduler>
    );
}
```

---

## Toolbar Options Reference

| Option | Type | Default | Description |
|---|---|---|---|
| `toolbar.items` | Array | — | Array of predefined item name strings or item config objects |
| `toolbar.visible` | boolean / undefined | `undefined` | `true` always shows, `false` always hides, `undefined` shows only when items exist |
| `toolbar.multiline` | boolean | `false` | Wraps items onto multiple rows |
| `toolbar.disabled` | boolean | `false` | Disables user interaction with all toolbar controls |

### Toolbar Item Config Object

| Property | Type | Description |
|---|---|---|
| `name` | `'today'` / `'dateNavigator'` / `'viewSwitcher'` | Predefined control to include |
| `location` | `'before'` / `'center'` / `'after'` | Positions the item in the toolbar |
| `widget` | string | DevExtreme component name (jQuery only), e.g. `'dxButton'` |
| `options` | object | Component options or predefined item overrides |
| `template` | string / function | Custom HTML template for non-component items |

---

## Hiding the Toolbar

Set `toolbar.visible: false` to remove the toolbar entirely. Remember that without the toolbar, users cannot navigate dates or switch views — you must provide alternative controls or fix the `currentDate` and `currentView`.

```js
// jQuery
$('#scheduler').dxScheduler({ toolbar: { visible: false } });
```

---

## Constraints

- **Defining `toolbar.items` removes the defaults.** If you include `items`, all three predefined controls disappear unless explicitly re-listed.
- **`location`** positions items within the toolbar's before/center/after sections. Items in `'before'` appear on the left, `'after'` on the right, `'center'` in the middle.
- **Angular uses `dxo-scheduler-toolbar` / `dxi-scheduler-toolbar-item`** (not generic `dxo-toolbar`).
- **Vue imports `DxToolbar` and `DxItem`** from `'devextreme-vue/scheduler'`, not from the generic toolbar package.

---

## Official Resources

- [Toolbar API](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxScheduler/Configuration/toolbar/)
- [Toolbar Items API](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxScheduler/Configuration/toolbar/items/)
- [Date Navigator guide](https://js.devexpress.com/Documentation/Guide/UI_Components/Scheduler/Date_Navigator/)
- [View Switcher guide](https://js.devexpress.com/Documentation/Guide/UI_Components/Scheduler/View_Switcher/)
- [Toolbar Demo](https://js.devexpress.com/Demos/WidgetsGallery/Demo/Scheduler/Toolbar/)
