var studiox = studiox || {};
(function ($) {

    if (!$) {
        return;
    }

    /* JQUERY ENHANCEMENTS ***************************************************/

    // studiox.ajax -> uses $.ajax ------------------------------------------------

    studiox.ajax = function (userOptions) {
        userOptions = userOptions || {};

        var options = $.extend(true, {}, studiox.ajax.defaultOpts, userOptions);
        options.success = undefined;
        options.error = undefined;

        return $.Deferred(function ($dfd) {
            $.ajax(options)
                .done(function (data, textStatus, jqXHR) {
                    if (data.__studiox) {
                        studiox.ajax.handleResponse(data, userOptions, $dfd, jqXHR);
                    } else {
                        $dfd.resolve(data);
                        userOptions.success && userOptions.success(data);
                    }
                }).fail(function (jqXHR) {
                    if (jqXHR.responseJSON && jqXHR.responseJSON.__studiox) {
                        studiox.ajax.handleResponse(jqXHR.responseJSON, userOptions, $dfd, jqXHR);
                    } else {
                        studiox.ajax.handleNonStudioXErrorResponse(jqXHR, userOptions, $dfd);
                    }
                });
        });
    };

    $.extend(studiox.ajax, {
        defaultOpts: {
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json',
            headers: {
                 'X-Requested-With': 'XMLHttpRequest'
            }
        },

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
                return studiox.message.error(error.details, error.message);
            } else {
                return studiox.message.error(error.message || studiox.ajax.defaultError.message);
            }
        },

        handleTargetUrl: function (targetUrl) {
            if (!targetUrl) {
                location.href = studiox.appPath;
            } else {
                location.href = targetUrl;
            }
        },

        handleNonStudioXErrorResponse: function (jqXHR, userOptions, $dfd) {
            if (userOptions.studioxHandleError !== false) {
                switch (jqXHR.status) {
                    case 401:
                        studiox.ajax.handleUnAuthorizedRequest(
                            studiox.ajax.showError(studiox.ajax.defaultError401),
                            studiox.appPath
                        );
                        break;
                    case 403:
                        studiox.ajax.showError(studiox.ajax.defaultError403);
                        break;
                    case 404:
                        studiox.ajax.showError(studiox.ajax.defaultError404);
                        break;
                    default:
                        studiox.ajax.showError(studiox.ajax.defaultError);
                        break;
                }
            }

            $dfd.reject.apply(this, arguments);
            userOptions.error && userOptions.error.apply(this, arguments);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    studiox.ajax.handleTargetUrl(targetUrl);
                });
            } else {
                studiox.ajax.handleTargetUrl(targetUrl);
            }
        },

        handleResponse: function (data, userOptions, $dfd, jqXHR) {
            if (data) {
                if (data.success === true) {
                    $dfd && $dfd.resolve(data.result, data, jqXHR);
                    userOptions.success && userOptions.success(data.result, data, jqXHR);

                    if (data.targetUrl) {
                        studiox.ajax.handleTargetUrl(data.targetUrl);
                    }
                } else if (data.success === false) {
                    var messagePromise = null;

                    if (data.error) {
                        if (userOptions.studioxHandleError !== false) {
                            messagePromise = studiox.ajax.showError(data.error);
                        }
                    } else {
                        data.error = studiox.ajax.defaultError;
                    }

                    studiox.ajax.logError(data.error);

                    $dfd && $dfd.reject(data.error, jqXHR);
                    userOptions.error && userOptions.error(data.error, jqXHR);

                    if (jqXHR.status === 401 && userOptions.studioxHandleError !== false) {
                        studiox.ajax.handleUnAuthorizedRequest(messagePromise, data.targetUrl);
                    }
                } else { //not wrapped result
                    $dfd && $dfd.resolve(data, null, jqXHR);
                    userOptions.success && userOptions.success(data, null, jqXHR);
                }
            } else { //no data sent to back
                $dfd && $dfd.resolve(jqXHR);
                userOptions.success && userOptions.success(jqXHR);
            }
        },

        blockUI: function (options) {
            if (options.blockUI) {
                if (options.blockUI === true) { //block whole page
                    studiox.ui.setBusy();
                } else { //block an element
                    studiox.ui.setBusy(options.blockUI);
                }
            }
        },

        unblockUI: function (options) {
            if (options.blockUI) {
                if (options.blockUI === true) { //unblock whole page
                    studiox.ui.clearBusy();
                } else { //unblock an element
                    studiox.ui.clearBusy(options.blockUI);
                }
            }
        },

        ajaxSendHandler: function (event, request, settings) {
            var token = studiox.security.antiForgery.getToken();
            if (!token) {
                return;
            }

            if (!settings.headers || settings.headers[studiox.security.antiForgery.tokenHeaderName] === undefined) {
                request.setRequestHeader(studiox.security.antiForgery.tokenHeaderName, token);
            }
        }
    });

    $(document).ajaxSend(function (event, request, settings) {
        return studiox.ajax.ajaxSendHandler(event, request, settings);
    });

    /* JQUERY PLUGIN ENHANCEMENTS ********************************************/

    /* jQuery Form Plugin 
     * http://www.malsup.com/jquery/form/
     */

    // studioxAjaxForm -> uses ajaxForm ------------------------------------------

    if ($.fn.ajaxForm) {
        $.fn.studioxAjaxForm = function (userOptions) {
            userOptions = userOptions || {};

            var options = $.extend({}, $.fn.studioxAjaxForm.defaults, userOptions);

            options.beforeSubmit = function () {
                studiox.ajax.blockUI(options);
                userOptions.beforeSubmit && userOptions.beforeSubmit.apply(this, arguments);
            };

            options.success = function (data) {
                studiox.ajax.handleResponse(data, userOptions);
            };

            //TODO: Error?

            options.complete = function () {
                studiox.ajax.unblockUI(options);
                userOptions.complete && userOptions.complete.apply(this, arguments);
            };

            return this.ajaxForm(options);
        };

        $.fn.studioxAjaxForm.defaults = {
            method: 'POST'
        };
    }

    studiox.event.on('studiox.dynamicScriptsInitialized', function () {
        studiox.ajax.defaultError.message = studiox.localization.studioxWeb('DefaultError');
        studiox.ajax.defaultError.details = studiox.localization.studioxWeb('DefaultErrorDetail');
        studiox.ajax.defaultError401.message = studiox.localization.studioxWeb('DefaultError401');
        studiox.ajax.defaultError401.details = studiox.localization.studioxWeb('DefaultErrorDetail401');
        studiox.ajax.defaultError403.message = studiox.localization.studioxWeb('DefaultError403');
        studiox.ajax.defaultError403.details = studiox.localization.studioxWeb('DefaultErrorDetail403');
        studiox.ajax.defaultError404.message = studiox.localization.studioxWeb('DefaultError404');
        studiox.ajax.defaultError404.details = studiox.localization.studioxWeb('DefaultErrorDetail404');
    });

})(jQuery);