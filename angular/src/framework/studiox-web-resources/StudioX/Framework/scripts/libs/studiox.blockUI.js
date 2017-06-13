var studiox = studiox || {};
(function () {

    if (!$.blockUI) {
        return;
    }

    $.extend($.blockUI.defaults, {
        message: ' ',
        css: { },
        overlayCSS: {
            backgroundColor: '#AAA',
            opacity: 0.3,
            cursor: 'wait'    
        }
    });
    
    studiox.ui.block = function (elm) {
        if (!elm) {
            $.blockUI();
        } else {
            $(elm).block();
        }
    };

    studiox.ui.unblock = function (elm) {
        if (!elm) {
            $.unblockUI();
        } else {
            $(elm).unblock();
        }
    };

})();