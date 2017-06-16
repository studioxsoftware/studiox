var getCookieValue = function (key) {
    var equalities = document.cookie.split('; ');
    for (var i = 0; i < equalities.length; i++) {
        if (!equalities[i]) {
            continue;
        }

        var splitted = equalities[i].split('=');
        if (splitted.length !== 2) {
            continue;
        }

        if (decodeURIComponent(splitted[0]) === key) {
            return decodeURIComponent(splitted[1] || '');
        }
    }

    return null;
};

var csrfCookie = getCookieValue("StudioX.AuthToken");;
var header = 'Bearer ' + csrfCookie;
var csrfCookieAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", header, "header");
swaggerUi.api.clientAuthorizations.add("Authorization", csrfCookieAuth);