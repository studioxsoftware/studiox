(function (studiox, angular) {

    if (!angular) {
        return;
    }

    studiox.ng = studiox.ng || {};

    studiox.ng.http = {
        defaultError: {
            message: 'An error has occurred!',
            details: 'Error detail not sent by server.'
        },

        defaultError401: {
            message: 'You are not authenticated!',
            details: 'You should be authenticated (sign in) in order to perform this operation.'
        },

        defaultError403: {
            message: 'You are not authorized!',
            details: 'You are not allowed to perform this operation.'
        },

        defaultError404: {
            message: 'Resource not found!',
            details: 'The resource requested could not found on the server.'
        },

        logError: function (error) {
            studiox.log.error(error);
        },

        showError: function (error) {
            if (error.details) {
                return studiox.message.error(error.details, error.message || studiox.ng.http.defaultError.message);
            } else {
                return studiox.message.error(error.message || studiox.ng.http.defaultError.message);
            }
        },

        handleTargetUrl: function (targetUrl) {
            if (!targetUrl) {
                location.href = studiox.appPath;
            } else {
                location.href = targetUrl;
            }
        },

        handleNonStudioXErrorResponse: function (response, defer) {
            if (response.config.studioxHandleError !== false) {
                switch (response.status) {
                    case 401:
                        studiox.ng.http.handleUnAuthorizedRequest(
                            studiox.ng.http.showError(studiox.ng.http.defaultError401),
                            studiox.appPath
                        );
                        break;
                    case 403:
                        studiox.ng.http.showError(studiox.ajax.defaultError403);
                        break;
                    case 404:
                        studiox.ng.http.showError(studiox.ajax.defaultError404);
                        break;
                    default:
                        studiox.ng.http.showError(studiox.ng.http.defaultError);
                        break;
                }
            }

            defer.reject(response);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    studiox.ng.http.handleTargetUrl(targetUrl || studiox.appPath);
                });
            } else {
                studiox.ng.http.handleTargetUrl(targetUrl || studiox.appPath);
            }
        },

        handleResponse: function (response, defer) {
            var originalData = response.data;

            if (originalData.success === true) {
                response.data = originalData.result;
                defer.resolve(response);

                if (originalData.targetUrl) {
                    studiox.ng.http.handleTargetUrl(originalData.targetUrl);
                }
            } else if (originalData.success === false) {
                var messagePromise = null;

                if (originalData.error) {
                    if (response.config.studioxHandleError !== false) {
                        messagePromise = studiox.ng.http.showError(originalData.error);
                    }
                } else {
                    originalData.error = defaultError;
                }

                studiox.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (response.status == 401 && response.config.studioxHandleError !== false) {
                    studiox.ng.http.handleUnAuthorizedRequest(messagePromise, originalData.targetUrl);
                }
            } else { //not wrapped result
                defer.resolve(response);
            }
        }
    }

    var studioxModule = angular.module('studiox', []);

    studioxModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {

                return {

                    'request': function (config) {
                        if (endsWith(config.url, '.cshtml')) {
                            config.url = studiox.appPath + 'StudioXAppView/Load?viewUrl=' + config.url + '&_t=' + studiox.pageLoadTime.getTime();
                        }

                        return config;
                    },

                    'response': function (response) {
                        if (!response.data || !response.data.__studiox) {
                            //Non StudioX related return value
                            return response;
                        }

                        var defer = $q.defer();
                        studiox.ng.http.handleResponse(response, defer);
                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        var defer = $q.defer();

                        if (!ngError.data || !ngError.data.__studiox) {
                            studiox.ng.http.handleNonStudioXErrorResponse(ngError, defer);
                        } else {
                            studiox.ng.http.handleResponse(ngError, defer);
                        }

                        return defer.promise;
                    }

                };
            }]);
        }
    ]);

    function endsWith(str, suffix) {
        if (suffix.length > str.length) {
            return false;
        }

        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }

    studiox.event.on('studiox.dynamicScriptsInitialized', function () {
        studiox.ng.http.defaultError.message = studiox.localization.studioxWeb('DefaultError');
        studiox.ng.http.defaultError.details = studiox.localization.studioxWeb('DefaultErrorDetail');
        studiox.ng.http.defaultError401.message = studiox.localization.studioxWeb('DefaultError401');
        studiox.ng.http.defaultError401.details = studiox.localization.studioxWeb('DefaultErrorDetail401');
        studiox.ng.http.defaultError403.message = studiox.localization.studioxWeb('DefaultError403');
        studiox.ng.http.defaultError403.details = studiox.localization.studioxWeb('DefaultErrorDetail403');
        studiox.ng.http.defaultError404.message = studiox.localization.studioxWeb('DefaultError404');
        studiox.ng.http.defaultError404.details = studiox.localization.studioxWeb('DefaultErrorDetail404');
    });

})((studiox || (studiox = {})), (angular || undefined));
