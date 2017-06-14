import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NgTableComponent } from './table/studiox-table.component';
import { NgTableFilteringDirective } from './table/studiox-table-filtering.directive';
import { NgTablePagingDirective } from './table/studiox-table-paging.directive';
import { NgTableSortingDirective } from './table/studiox-table-sorting.directive';

@NgModule({
  imports: [CommonModule],
  declarations: [NgTableComponent, NgTableFilteringDirective, NgTablePagingDirective, NgTableSortingDirective],
  exports: [NgTableComponent, NgTableFilteringDirective, NgTablePagingDirective, NgTableSortingDirective]
})
export class Ng2TableModule {
}
