define(function () {
    return {
        load: function (name, req, onload, config) {
            var url = studiox.appPath + 'api/StudioXServiceProxies/Get?name=' + name;
            req([url], function (value) {
                onload(value);
            });
        }
    };
});