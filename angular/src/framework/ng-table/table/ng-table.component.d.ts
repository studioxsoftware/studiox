import { EventEmitter } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
export declare class NgTableComponent {
    private sanitizer;
    rows: Array<any>;
    config: any;
    tableChanged: EventEmitter<any>;
    cellClicked: EventEmitter<any>;
    showFilterRow: Boolean;
    columns: Array<any>;
    private _columns;
    private _config;
    constructor(sanitizer: DomSanitizer);
    sanitize(html: string): SafeHtml;
    readonly configColumns: any;
    onChangeTable(column: any): void;
    getData(row: any, propertyName: string): string;
    cellClick(row: any, column: any): void;
}
