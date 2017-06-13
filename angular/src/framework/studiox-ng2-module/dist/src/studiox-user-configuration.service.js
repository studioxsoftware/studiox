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
var http_1 = require("@angular/http");
var StudioXUserConfigurationService = (function () {
    function StudioXUserConfigurationService(_http) {
        this._http = _http;
    }
    StudioXUserConfigurationService.prototype.initialize = function () {
        this._http.get('/StudioXUserConfiguration/GetAll')
            .subscribe(function (result) {
            jQuery.extend(true, studiox, result.json());
        });
    };
    return StudioXUserConfigurationService;
}());
StudioXUserConfigurationService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.Http !== "undefined" && http_1.Http) === "function" && _a || Object])
], StudioXUserConfigurationService);
exports.StudioXUserConfigurationService = StudioXUserConfigurationService;
var _a;
//# sourceMappingURL=studiox-user-configuration.service.js.map