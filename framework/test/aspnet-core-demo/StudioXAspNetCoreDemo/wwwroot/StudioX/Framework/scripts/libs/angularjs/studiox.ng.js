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
            location.href = targetUrl;
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    if (!targetUrl) {
                        location.reload();
                    } else {
                        studiox.ng.http.handleTargetUrl(targetUrl);
                    }
                });
            } else {
                if (!targetUrl) {
                    location.reload();
                } else {
                    studiox.ng.http.handleTargetUrl(targetUrl);
                }
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
                    messagePromise = studiox.ng.http.showError(originalData.error);
                } else {
                    originalData.error = defaultError;
                }

                studiox.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (originalData.unAuthorizedRequest) {
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
                            return response;
                        }

                        var defer = $q.defer();
                        studiox.ng.http.handleResponse(response, defer);
                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        if (!ngError.data || !ngError.data.__studiox) {
                            studiox.ng.http.showError(studiox.ng.http.defaultError);
                            return ngError;
                        }

                        var defer = $q.defer();
                        studiox.ng.http.handleResponse(ngError, defer);
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
    });

})((studiox || (studiox = {})), (angular || undefined));