"use strict";
var core_1 = require('@angular/core');
var common_1 = require('@angular/common');
var ng_table_component_1 = require('./table/studiox-table.component');
var ng_table_filtering_directive_1 = require('./table/studiox-table-filtering.directive');
var ng_table_paging_directive_1 = require('./table/studiox-table-paging.directive');
var ng_table_sorting_directive_1 = require('./table/studiox-table-sorting.directive');
var StudioXTableModule = (function () {
    function StudioXTableModule() {
    }
    StudioXTableModule.decorators = [
        { type: core_1.NgModule, args: [{
                    imports: [common_1.CommonModule],
                    declarations: [ng_table_component_1.NgTableComponent, ng_table_filtering_directive_1.NgTableFilteringDirective, ng_table_paging_directive_1.NgTablePagingDirective, ng_table_sorting_directive_1.NgTableSortingDirective],
                    exports: [ng_table_component_1.NgTableComponent, ng_table_filtering_directive_1.NgTableFilteringDirective, ng_table_paging_directive_1.NgTablePagingDirective, ng_table_sorting_directive_1.NgTableSortingDirective]
                },] },
    ];
    /** @nocollapse */
    StudioXTableModule.ctorParameters = [];
    return StudioXTableModule;
}());
exports.StudioXTableModule = StudioXTableModule;
