# Export to Excel and PDF

## Dependencies

| Format | Package | Install |
|---|---|---|
| Excel (.xlsx) | `devextreme-exceljs-fork` + `file-saver` | `npm install devextreme-exceljs-fork file-saver` |
| PDF | `jspdf` | `npm install jspdf` |

---

## Basic Export Setup (all frameworks)

Enable the Export button with `export.enabled`. Configure available formats with `export.formats`. Handle the `onExporting` event to trigger the export libraries.

### jQuery

```html
<!-- Add scripts before DevExtreme -->
<script src="https://cdn.jsdelivr.net/npm/devextreme-exceljs-fork@4.4.1/dist/dx-exceljs-fork.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/FileSaver.js/2.0.2/FileSaver.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.3.1/jspdf.umd.min.js"></script>
```

```js
// index.js
$(function() {
    $('#data-grid').dxDataGrid({
        export: {
            enabled: true,
            formats: ['xlsx', 'pdf']
        },
        onExporting(e) {
            if (e.format === 'xlsx') {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Employees');
                DevExpress.excelExporter.exportDataGrid({
                    worksheet,
                    component: e.component
                }).then(() => {
                    workbook.xlsx.writeBuffer().then(buffer => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Employees.xlsx');
                    });
                });
            } else if (e.format === 'pdf') {
                const doc = new jsPDF();
                DevExpress.pdfExporter.exportDataGrid({
                    jsPDFDocument: doc,
                    component: e.component
                }).then(() => doc.save('Employees.pdf'));
            }
        }
    });
});
```

### Angular

```ts
// app.ts
import { Component } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';
import { Workbook } from 'devextreme-exceljs-fork';
import { saveAs } from 'file-saver-es';
import { jsPDF } from 'jspdf';
import { exportDataGrid as exportToExcel } from 'devextreme/excel_exporter';
import { exportDataGrid as exportToPdf  } from 'devextreme/pdf_exporter';
import { DxDataGridTypes } from 'devextreme-angular/ui/data-grid';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxDataGridComponent]
})
export class AppComponent {
    exportConfig = { enabled: true, formats: ['xlsx', 'pdf'] };

    onExporting(e: DxDataGridTypes.ExportingEvent) {
        if (e.format === 'xlsx') {
            const workbook = new Workbook();
            const worksheet = workbook.addWorksheet('Employees');
            exportToExcel({ worksheet, component: e.component }).then(() => {
                workbook.xlsx.writeBuffer().then((buffer) => {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Employees.xlsx');
                });
            });
        } else if (e.format === 'pdf') {
            const doc = new jsPDF();
            exportToPdf({ jsPDFDocument: doc, component: e.component }).then(() => doc.save('Employees.pdf'));
        }
    }
}
```

```html
<!-- app.html -->
<dx-data-grid
    [export]="exportConfig"
    (onExporting)="onExporting($event)">
</dx-data-grid>
```

### Vue (Composition API + TypeScript)

```vue
<template>
    <DxDataGrid
        :export="exportConfig"
        @exporting="onExporting"
    />
</template>

<script setup lang="ts">
import { DxDataGrid, DxDataGridTypes } from 'devextreme-vue/data-grid';
import { Workbook } from 'devextreme-exceljs-fork';
import { saveAs } from 'file-saver-es';
import { jsPDF } from 'jspdf';
import { exportDataGrid as exportToExcel } from 'devextreme/excel_exporter';
import { exportDataGrid as exportToPdf  } from 'devextreme/pdf_exporter';

const exportConfig = { enabled: true, formats: ['xlsx', 'pdf'] };

function onExporting(e: DxDataGridTypes.ExportingEvent) {
    if (e.format === 'xlsx') {
        const workbook = new Workbook();
        const worksheet = workbook.addWorksheet('Employees');
        exportToExcel({ worksheet, component: e.component }).then(() => {
            workbook.xlsx.writeBuffer().then(buffer => {
                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Employees.xlsx');
            });
        });
    } else if (e.format === 'pdf') {
        const doc = new jsPDF();
        exportToPdf({ jsPDFDocument: doc, component: e.component }).then(() => doc.save('Employees.pdf'));
    }
}
</script>
```

### React (TSX)

```tsx
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { DataGrid, type DataGridTypes } from 'devextreme-react/data-grid';
import { Workbook } from 'devextreme-exceljs-fork';
import { saveAs } from 'file-saver-es';
import { jsPDF } from 'jspdf';
import { exportDataGrid as exportToExcel } from 'devextreme/excel_exporter';
import { exportDataGrid as exportToPdf  } from 'devextreme/pdf_exporter';

const exportConfig = { enabled: true, formats: ['xlsx', 'pdf'] };

function onExporting(e: DataGridTypes.ExportingEvent) {
    if (e.format === 'xlsx') {
        const workbook = new Workbook();
        const worksheet = workbook.addWorksheet('Employees');
        exportToExcel({ worksheet, component: e.component }).then(() => {
            workbook.xlsx.writeBuffer().then(buffer => {
                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Employees.xlsx');
            });
        });
    } else if (e.format === 'pdf') {
        const doc = new jsPDF();
        exportToPdf({ jsPDFDocument: doc, component: e.component }).then(() => doc.save('Employees.pdf'));
    }
}

function App() {
    return <DataGrid export={exportConfig} onExporting={onExporting} />;
}
```

---

## Export Only Selected Rows

Pass `selectedRowsOnly: true` in the exporter options:

```ts
exportToExcel({ worksheet, component: e.component, selectedRowsOnly: true });
```

---

## Common `export` Options

| Option | Type | Description |
|---|---|---|
| `enabled` | `Boolean` | Shows the Export button in the toolbar |
| `formats` | `Array` | `['xlsx']`, `['pdf']`, or `['xlsx', 'pdf']` |
| `fileName` | `String` | Default file name (without extension) |
| `allowExportSelectedData` | `Boolean` | Adds "Export selected rows" option to the button dropdown |
