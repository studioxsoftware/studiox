"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
var Observable_1 = require("rxjs/Observable");
var message_service_1 = require("./message/message.service");
var log_service_1 = require("./log/log.service");
var token_service_1 = require("./auth/token.service");
var utils_service_1 = require("./utils/utils.service");
require("rxjs/add/operator/catch");
require("rxjs/add/operator/map");
var StudioXHttpConfiguration = (function () {
    function StudioXHttpConfiguration(_messageService, _logService) {
        this._messageService = _messageService;
        this._logService = _logService;
        this.defaultError = {
            message: 'An error has occurred!',
            details: 'Error detail not sent by server.'
        };
        this.defaultError401 = {
            message: 'You are not authenticated!',
            details: 'You should be authenticated (sign in) in order to perform this operation.'
        };
        this.defaultError403 = {
            message: 'You are not authorized!',
            details: 'You are not allowed to perform this operation.'
        };
        this.defaultError404 = {
            message: 'Resource not found!',
            details: 'The resource requested could not found on the server.'
        };
    }
    StudioXHttpConfiguration.prototype.logError = function (error) {
        this._logService.error(error);
    };
    StudioXHttpConfiguration.prototype.showError = function (error) {
        if (error.details) {
            return this._messageService.error(error.details, error.message || this.defaultError.message);
        }
        else {
            return this._messageService.error(error.message || this.defaultError.message);
        }
    };
    StudioXHttpConfiguration.prototype.handleTargetUrl = function (targetUrl) {
        if (!targetUrl) {
            location.href = '/';
        }
        else {
            location.href = targetUrl;
        }
    };
    StudioXHttpConfiguration.prototype.handleUnAuthorizedRequest = function (messagePromise, targetUrl) {
        var _this = this;
        var self = this;
        if (messagePromise) {
            messagePromise.done(function () {
                _this.handleTargetUrl(targetUrl || '/');
            });
        }
        else {
            self.handleTargetUrl(targetUrl || '/');
        }
    };
    StudioXHttpConfiguration.prototype.handleNonStudioXErrorResponse = function (response) {
        var self = this;
        switch (response.status) {
            case 401:
                self.handleUnAuthorizedRequest(self.showError(self.defaultError401), '/');
                break;
            case 403:
                self.showError(self.defaultError403);
                break;
            case 404:
                self.showError(self.defaultError404);
                break;
            default:
                self.showError(self.defaultError);
                break;
        }
    };
    StudioXHttpConfiguration.prototype.handleStudioXResponse = function (response, ajaxResponse) {
        var newResponse = new http_1.ResponseOptions({
            url: response.url,
            body: ajaxResponse,
            headers: response.headers,
            status: response.status,
            statusText: response.statusText,
            type: response.type
        });
        if (ajaxResponse.success) {
            newResponse.body = ajaxResponse.result;
            if (ajaxResponse.targetUrl) {
                this.handleTargetUrl(ajaxResponse.targetUrl);
                ;
            }
        }
        else {
            if (!ajaxResponse.error) {
                ajaxResponse.error = this.defaultError;
            }
            this.logError(ajaxResponse.error);
            this.showError(ajaxResponse.error);
            if (response.status === 401) {
                this.handleUnAuthorizedRequest(null, ajaxResponse.targetUrl);
            }
        }
        return new http_1.Response(newResponse);
    };
    StudioXHttpConfiguration.prototype.getStudioXAjaxResponseOrNull = function (response) {
        var contentType = response.headers.get('Content-Type');
        if (!contentType) {
            this._logService.warn('Content-Type is not sent!');
            return null;
        }
        if (contentType.indexOf("application/json") < 0) {
            this._logService.warn('Content-Type is not application/json: ' + contentType);
            return null;
        }
        var responseObj = response.json();
        if (!responseObj.__studiox) {
            return null;
        }
        return responseObj;
    };
    StudioXHttpConfiguration.prototype.handleResponse = function (response) {
        var ajaxResponse = this.getStudioXAjaxResponseOrNull(response);
        if (ajaxResponse == null) {
            return response;
        }
        return this.handleStudioXResponse(response, ajaxResponse);
    };
    StudioXHttpConfiguration.prototype.handleError = function (error) {
        var ajaxResponse = this.getStudioXAjaxResponseOrNull(error);
        if (ajaxResponse != null) {
            this.handleStudioXResponse(error, ajaxResponse);
        }
        else {
            this.handleNonStudioXErrorResponse(error);
        }
        return Observable_1.Observable.throw('HTTP error: ' + error.status + ', ' + error.statusText);
    };
    return StudioXHttpConfiguration;
}());
StudioXHttpConfiguration = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [message_service_1.MessageService,
        log_service_1.LogService])
], StudioXHttpConfiguration);
exports.StudioXHttpConfiguration = StudioXHttpConfiguration;
var StudioXHttp = (function (_super) {
    __extends(StudioXHttp, _super);
    function StudioXHttp(backend, defaultOptions, configuration) {
        var _this = _super.call(this, backend, defaultOptions) || this;
        _this._tokenService = new token_service_1.TokenService();
        _this._utilsService = new utils_service_1.UtilsService();
        _this.configuration = configuration;
        return _this;
    }
    StudioXHttp.prototype.get = function (url, options) {
        var _this = this;
        if (!options) {
            options = {};
        }
        this.normalizeRequestOptions(options);
        return _super.prototype.get.call(this, url, options)
            .map(function (response) { return _this.configuration.handleResponse(response); })
            .catch(function (error) { return _this.configuration.handleError(error); });
    };
    StudioXHttp.prototype.post = function (url, body, options) {
        var _this = this;
        if (!options) {
            options = {};
        }
        this.normalizeRequestOptions(options);
        return _super.prototype.post.call(this, url, body, options)
            .map(function (response) { return _this.configuration.handleResponse(response); })
            .catch(function (error) { return _this.configuration.handleError(error); });
    };
    StudioXHttp.prototype.put = function (url, body, options) {
        var _this = this;
        if (!options) {
            options = {};
        }
        this.normalizeRequestOptions(options);
        return _super.prototype.put.call(this, url, body, options)
            .map(function (response) { return _this.configuration.handleResponse(response); })
            .catch(function (error) { return _this.configuration.handleError(error); });
    };
    StudioXHttp.prototype.delete = function (url, options) {
        var _this = this;
        if (!options) {
            options = {};
        }
        this.normalizeRequestOptions(options);
        return _super.prototype.delete.call(this, url, options)
            .map(function (response) { return _this.configuration.handleResponse(response); })
            .catch(function (error) { return _this.configuration.handleError(error); });
    };
    StudioXHttp.prototype.request = function (url, options) {
        var _this = this;
        if (!options) {
            options = {};
        }
        this.normalizeRequestOptions(options);
        return _super.prototype.request.call(this, url, options)
            .map(function (response) { return _this.configuration.handleResponse(response); })
            .catch(function (error) { return _this.configuration.handleError(error); });
    };
    StudioXHttp.prototype.normalizeRequestOptions = function (options) {
        if (!options.headers) {
            options.headers = new http_1.Headers();
        }
        this.addAuthorizationHeaders(options);
        this.addAcceptLanguageHeader(options);
        this.addTenantIdHeader(options);
    };
    StudioXHttp.prototype.addAcceptLanguageHeader = function (options) {
        var cookieLangValue = this._utilsService.getCookieValue("StudioX.Localization.CultureName");
        if (cookieLangValue && !options.headers.has('Accept-Language')) {
            options.headers.append('Accept-Language', cookieLangValue);
        }
    };
    StudioXHttp.prototype.addTenantIdHeader = function (options) {
        var cookieTenantIdValue = this._utilsService.getCookieValue('StudioX.TenantId');
        if (cookieTenantIdValue && !options.headers.has('StudioX.TenantId')) {
            options.headers.append('StudioX.TenantId', cookieTenantIdValue);
        }
    };
    StudioXHttp.prototype.addAuthorizationHeaders = function (options) {
        var authorizationHeaders = options.headers.getAll('Authorization');
        if (!authorizationHeaders) {
            authorizationHeaders = [];
        }
        if (!this.itemExists(authorizationHeaders, function (item) { return item.indexOf('Bearer ') == 0; })) {
            var token = this._tokenService.getToken();
            if (token) {
                options.headers.append('Authorization', 'Bearer ' + token);
            }
        }
    };
    StudioXHttp.prototype.itemExists = function (items, predicate) {
        for (var i = 0; i < items.length; i++) {
            if (predicate(items[i])) {
                return true;
            }
        }
        return false;
    };
    return StudioXHttp;
}(http_1.Http));
StudioXHttp = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.XHRBackend !== "undefined" && http_1.XHRBackend) === "function" && _a || Object, typeof (_b = typeof http_1.RequestOptions !== "undefined" && http_1.RequestOptions) === "function" && _b || Object, StudioXHttpConfiguration])
], StudioXHttp);
exports.StudioXHttp = StudioXHttp;
var _a, _b;
//# sourceMappingURL=studioxHttp.js.map