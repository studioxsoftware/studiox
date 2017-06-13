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
var MessageService = (function () {
    function MessageService() {
    }
    MessageService.prototype.info = function (message, title) {
        return studiox.message.info(message, title);
    };
    MessageService.prototype.success = function (message, title) {
        return studiox.message.success(message, title);
    };
    MessageService.prototype.warn = function (message, title) {
        return studiox.message.warn(message, title);
    };
    MessageService.prototype.error = function (message, title) {
        return studiox.message.error(message, title);
    };
    MessageService.prototype.confirm = function (message, titleOrCallBack, callback) {
        if (typeof titleOrCallBack == 'string') {
            return studiox.message.confirm(message, titleOrCallBack, callback);
        }
        else {
            return studiox.message.confirm(message, titleOrCallBack);
        }
    };
    return MessageService;
}());
MessageService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [])
], MessageService);
exports.MessageService = MessageService;
//# sourceMappingURL=message.service.js.map