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
var StudioXSessionService = (function () {
    function StudioXSessionService() {
    }
    Object.defineProperty(StudioXSessionService.prototype, "userId", {
        get: function () {
            return studiox.session.userId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StudioXSessionService.prototype, "tenantId", {
        get: function () {
            return studiox.session.tenantId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StudioXSessionService.prototype, "impersonatorUserId", {
        get: function () {
            return studiox.session.impersonatorUserId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StudioXSessionService.prototype, "impersonatorTenantId", {
        get: function () {
            return studiox.session.impersonatorTenantId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(StudioXSessionService.prototype, "multiTenancySide", {
        get: function () {
            return studiox.session.multiTenancySide;
        },
        enumerable: true,
        configurable: true
    });
    return StudioXSessionService;
}());
StudioXSessionService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [])
], StudioXSessionService);
exports.StudioXSessionService = StudioXSessionService;
//# sourceMappingURL=studiox-session.service.js.map