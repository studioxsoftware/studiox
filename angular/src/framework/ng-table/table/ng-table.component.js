"use strict";
var core_1 = require('@angular/core');
var platform_browser_1 = require('@angular/platform-browser');
var NgTableComponent = (function () {
    function NgTableComponent(sanitizer) {
        this.sanitizer = sanitizer;
        // Table values
        this.rows = [];
        // Outputs (Events)
        this.tableChanged = new core_1.EventEmitter();
        this.cellClicked = new core_1.EventEmitter();
        this.showFilterRow = false;
        this._columns = [];
        this._config = {};
    }
    Object.defineProperty(NgTableComponent.prototype, "config", {
        get: function () {
            return this._config;
        },
        set: function (conf) {
            if (!conf.className) {
                conf.className = 'table-striped table-bordered';
            }
            if (conf.className instanceof Array) {
                conf.className = conf.className.join(' ');
            }
            this._config = conf;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NgTableComponent.prototype, "columns", {
        get: function () {
            return this._columns;
        },
        set: function (values) {
            var _this = this;
            values.forEach(function (value) {
                if (value.filtering) {
                    _this.showFilterRow = true;
                }
                if (value.className && value.className instanceof Array) {
                    value.className = value.className.join(' ');
                }
                var column = _this._columns.find(function (col) { return col.name === value.name; });
                if (column) {
                    Object.assign(column, value);
                }
                if (!column) {
                    _this._columns.push(value);
                }
            });
        },
        enumerable: true,
        configurable: true
    });
    NgTableComponent.prototype.sanitize = function (html) {
        return this.sanitizer.bypassSecurityTrustHtml(html);
    };
    Object.defineProperty(NgTableComponent.prototype, "configColumns", {
        get: function () {
            var sortColumns = [];
            this.columns.forEach(function (column) {
                if (column.sort) {
                    sortColumns.push(column);
                }
            });
            return { columns: sortColumns };
        },
        enumerable: true,
        configurable: true
    });
    NgTableComponent.prototype.onChangeTable = function (column) {
        this._columns.forEach(function (col) {
            if (col.name !== column.name && col.sort !== false) {
                col.sort = '';
            }
        });
        this.tableChanged.emit({ sorting: this.configColumns });
    };
    NgTableComponent.prototype.getData = function (row, propertyName) {
        return propertyName.split('.').reduce(function (prev, curr) { return prev[curr]; }, row);
    };
    NgTableComponent.prototype.cellClick = function (row, column) {
        this.cellClicked.emit({ row: row, column: column });
    };
    NgTableComponent.decorators = [
        { type: core_1.Component, args: [{
                    selector: 'ng-table',
                    template: "\n    <table class=\"table dataTable\" ngClass=\"{{config.className || ''}}\"\n           role=\"grid\" style=\"width: 100%;\">\n      <thead>\n        <tr role=\"row\">\n          <th *ngFor=\"let column of columns\" [ngTableSorting]=\"config\" [column]=\"column\" \n              (sortChanged)=\"onChangeTable($event)\" ngClass=\"{{column.className || ''}}\">\n            {{column.title}}\n            <i *ngIf=\"config && column.sort\" class=\"pull-right fa\"\n              [ngClass]=\"{'fa-chevron-down': column.sort === 'desc', 'fa-chevron-up': column.sort === 'asc'}\"></i>\n          </th>\n        </tr>\n      </thead>\n      <tbody>\n      <tr *ngIf=\"showFilterRow\">\n        <td *ngFor=\"let column of columns\">\n          <input *ngIf=\"column.filtering\" placeholder=\"{{column.filtering.placeholder}}\"\n                 [ngTableFiltering]=\"column.filtering\"\n                 class=\"form-control\"\n                 style=\"width: auto;\"\n                 (tableChanged)=\"onChangeTable(config)\"/>\n        </td>\n      </tr>\n        <tr *ngFor=\"let row of rows\">\n          <td (click)=\"cellClick(row, column.name)\" *ngFor=\"let column of columns\" [innerHtml]=\"sanitize(getData(row, column.name))\"></td>\n        </tr>\n      </tbody>\n    </table>\n  "
                },] },
    ];
    /** @nocollapse */
    NgTableComponent.ctorParameters = [
        { type: platform_browser_1.DomSanitizer, },
    ];
    NgTableComponent.propDecorators = {
        'rows': [{ type: core_1.Input },],
        'config': [{ type: core_1.Input },],
        'tableChanged': [{ type: core_1.Output },],
        'cellClicked': [{ type: core_1.Output },],
        'columns': [{ type: core_1.Input },],
    };
    return NgTableComponent;
}());
exports.NgTableComponent = NgTableComponent;
