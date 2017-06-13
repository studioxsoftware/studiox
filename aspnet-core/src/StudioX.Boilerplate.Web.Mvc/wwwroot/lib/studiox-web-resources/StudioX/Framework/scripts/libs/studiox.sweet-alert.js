var studiox = studiox || {};
(function ($) {
    if (!sweetAlert || !$) {
        return;
    }

    /* DEFAULTS *************************************************/

    studiox.libs = studiox.libs || {};
    studiox.libs.sweetAlert = {
        config: {
            'default': {

            },
            info: {
                type: 'info'
            },
            success: {
                type: 'success'
            },
            warn: {
                type: 'warning'
            },
            error: {
                type: 'error'
            },
            confirm: {
                type: 'warning',
                title: 'Are you sure?',
                showCancelButton: true,
                cancelButtonText: 'Cancel',
                confirmButtonColor: "#DD6B55",
                confirmButtonText: 'Yes'
            }
        }
    };

    /* MESSAGE **************************************************/

    var showMessage = function (type, message, title) {
        if (!title) {
            title = message;
            message = undefined;
        }

        var opts = $.extend(
            {},
            studiox.libs.sweetAlert.config.default,
            studiox.libs.sweetAlert.config[type],
            {
                title: title,
                text: message
            }
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts, function () {
                $dfd.resolve();
            });
        });
    };

    studiox.message.info = function (message, title) {
        return showMessage('info', message, title);
    };

    studiox.message.success = function (message, title) {
        return showMessage('success', message, title);
    };

    studiox.message.warn = function (message, title) {
        return showMessage('warn', message, title);
    };

    studiox.message.error = function (message, title) {
        return showMessage('error', message, title);
    };

    studiox.message.confirm = function (message, titleOrCallback, callback) {
        var userOpts = {
            text: message
        };

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            userOpts.title = titleOrCallback;
        };

        var opts = $.extend(
            {},
            studiox.libs.sweetAlert.config.default,
            studiox.libs.sweetAlert.config.confirm,
            userOpts
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts, function (isConfirmed) {
                callback && callback(isConfirmed);
                $dfd.resolve(isConfirmed);
            });
        });
    };

    studiox.event.on('studiox.dynamicScriptsInitialized', function () {
        studiox.libs.sweetAlert.config.confirm.title = studiox.localization.studioxWeb('AreYouSure');
        studiox.libs.sweetAlert.config.confirm.cancelButtonText = studiox.localization.studioxWeb('Cancel');
        studiox.libs.sweetAlert.config.confirm.confirmButtonText = studiox.localization.studioxWeb('Yes');
    });

})(jQuery);