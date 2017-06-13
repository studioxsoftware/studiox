"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var NotifyService = (function () {
    function NotifyService() {
    }
    NotifyService.prototype.info = function (message, title, options) {
        studiox.notify.info(message, title, options);
    };
    NotifyService.prototype.success = function (message, title, options) {
        studiox.notify.success(message, title, options);
    };
    NotifyService.prototype.warn = function (message, title, options) {
        studiox.notify.warn(message, title, options);
    };
    NotifyService.prototype.error = function (message, title, options) {
        studiox.notify.error(message, title, options);
    };
    return NotifyService;
}());
NotifyService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [])
], NotifyService);
exports.NotifyService = NotifyService;
//# sourceMappingURL=notify.service.js.map