var studiox = studiox || {};
(function () {
    if (!moment || !moment.tz) {
        return;
    }

    /* DEFAULTS *************************************************/

    studiox.timing = studiox.timing || {};

    /* FUNCTIONS **************************************************/

    studiox.timing.convertToUserTimezone = function (date) {
        var momentDate = moment(date);
        var targetDate = momentDate.clone().tz(studiox.timing.timeZoneInfo.iana.timeZoneId);
        return targetDate;
    };

})();