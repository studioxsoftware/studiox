var studiox = studiox || {};
(function ($) {

    //Check if SignalR is defined
    if (!$ || !$.connection) {
        return;
    }

    //Create namespaces
    studiox.signalr = studiox.signalr || {};
    studiox.signalr.hubs = studiox.signalr.hubs || {};

    //Get the common hub
    studiox.signalr.hubs.common = $.connection.studioxCommonHub;

    var commonHub = studiox.signalr.hubs.common;
    if (!commonHub) {
        return;
    }

    //Register to get notifications
    commonHub.client.getNotification = function (notification) {
        studiox.event.trigger('studiox.notifications.received', notification);
    };

    //Connect to the server
    studiox.signalr.connect = function() {
        $.connection.hub.start().done(function () {
            studiox.log.debug('Connected to SignalR server!'); //TODO: Remove log
            studiox.event.trigger('studiox.signalr.connected');
            commonHub.server.register().done(function () {
                studiox.log.debug('Registered to the SignalR server!'); //TODO: Remove log
            });
        });
    };

    if (studiox.signalr.autoConnect === undefined) {
        studiox.signalr.autoConnect = true;
    }

    if (studiox.signalr.autoConnect) {
        studiox.signalr.connect();
    }

})(jQuery);