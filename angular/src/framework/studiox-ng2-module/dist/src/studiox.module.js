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
var studiox_session_service_1 = require("./session/studiox-session.service");
var permission_checker_service_1 = require("./auth/permission-checker.service");
var feature_checker_service_1 = require("./features/feature-checker.service");
var localization_service_1 = require("./localization/localization.service");
var setting_service_1 = require("./settings/setting.service");
var notify_service_1 = require("./notify/notify.service");
var message_service_1 = require("./message/message.service");
var log_service_1 = require("./log/log.service");
var studiox_multi_tenancy_service_1 = require("./multi-tenancy/studiox-multi-tenancy.service");
var studioxHttp_1 = require("./studioxHttp");
var studiox_user_configuration_service_1 = require("./studiox-user-configuration.service");
var token_service_1 = require("./auth/token.service");
var utils_service_1 = require("./utils/utils.service");
function studioxHttpFactory(xhrBackend, requestOptions, configuration) {
    return new studioxHttp_1.StudioXHttp(xhrBackend, requestOptions, configuration);
}
exports.studioxHttpFactory = studioxHttpFactory;
exports.StudioXHttpProvider = {
    provide: http_1.Http,
    useFactory: studioxHttpFactory,
    deps: [http_1.XHRBackend, http_1.RequestOptions, studioxHttp_1.StudioXHttpConfiguration]
};
var StudioXModule = (function () {
    function StudioXModule() {
    }
    return StudioXModule;
}());
StudioXModule = __decorate([
    core_1.NgModule({
        imports: [
            http_1.HttpModule,
            http_1.JsonpModule
        ],
        declarations: [],
        providers: [
            studiox_session_service_1.StudioXSessionService,
            permission_checker_service_1.PermissionCheckerService,
            feature_checker_service_1.FeatureCheckerService,
            localization_service_1.LocalizationService,
            setting_service_1.SettingService,
            notify_service_1.NotifyService,
            message_service_1.MessageService,
            log_service_1.LogService,
            studiox_multi_tenancy_service_1.StudioXMultiTenancyService,
            studiox_user_configuration_service_1.StudioXUserConfigurationService,
            studioxHttp_1.StudioXHttpConfiguration,
            exports.StudioXHttpProvider,
            token_service_1.TokenService,
            utils_service_1.UtilsService
        ]
    }),
    __metadata("design:paramtypes", [])
], StudioXModule);
exports.StudioXModule = StudioXModule;
//# sourceMappingURL=studiox.module.js.map