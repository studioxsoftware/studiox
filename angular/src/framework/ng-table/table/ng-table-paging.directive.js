"use strict";
var core_1 = require('@angular/core');
var NgTablePagingDirective = (function () {
    function NgTablePagingDirective() {
        this.ngTablePaging = true;
        this.tableChanged = new core_1.EventEmitter();
    }
    Object.defineProperty(NgTablePagingDirective.prototype, "config", {
        get: function () {
            return this.ngTablePaging;
        },
        set: function (value) {
            this.ngTablePaging = value;
        },
        enumerable: true,
        configurable: true
    });
    NgTablePagingDirective.prototype.onChangePage = function (event) {
        // Object.assign(this.config, event);
        if (this.ngTablePaging) {
            this.tableChanged.emit({ paging: event });
        }
    };
    NgTablePagingDirective.decorators = [
        { type: core_1.Directive, args: [{ selector: '[ngTablePaging]' },] },
    ];
    /** @nocollapse */
    NgTablePagingDirective.ctorParameters = [];
    NgTablePagingDirective.propDecorators = {
        'ngTablePaging': [{ type: core_1.Input },],
        'tableChanged': [{ type: core_1.Output },],
        'config': [{ type: core_1.Input },],
        'onChangePage': [{ type: core_1.HostListener, args: ['pagechanged', ['$event'],] },],
    };
    return NgTablePagingDirective;
}());
exports.NgTablePagingDirective = NgTablePagingDirective;
