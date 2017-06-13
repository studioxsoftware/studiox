var studiox = studiox || {};
(function () {

    if (!toastr) {
        return;
    }

    /* DEFAULTS *************************************************/

    toastr.options.positionClass = 'toast-bottom-right';

    /* NOTIFICATION *********************************************/

    var showNotification = function (type, message, title, options) {
        toastr[type](message, title, options);
    };

    studiox.notify.success = function (message, title, options) {
        showNotification('success', message, title, options);
    };

    studiox.notify.info = function (message, title, options) {
        showNotification('info', message, title, options);
    };

    studiox.notify.warn = function (message, title, options) {
        showNotification('warning', message, title, options);
    };

    studiox.notify.error = function (message, title, options) {
        showNotification('error', message, title, options);
    };

})();