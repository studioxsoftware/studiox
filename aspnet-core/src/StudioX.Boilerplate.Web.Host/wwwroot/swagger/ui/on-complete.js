var studiox = studiox || {};
(function () {

    /* Swagger */

    studiox.swagger = studiox.swagger || {};

    studiox.swagger.addAuthToken = function () {
        var authToken = studiox.auth.getToken();
        if (!authToken) {
            return false;
        }
        var cookieAuth = new SwaggerClient.ApiKeyAuthorization(studiox.auth.tokenHeaderName, 'Bearer ' + authToken, 'header');
        swaggerUi.api.clientAuthorizations.add(studiox.auth.tokenHeaderName, cookieAuth);
        return true;
    }

    studiox.swagger.addCsrfToken = function () {
        var csrfToken = studiox.security.antiForgery.getToken();
        if (!csrfToken) {
            return false;
        }
        var csrfCookieAuth = new SwaggerClient.ApiKeyAuthorization(studiox.security.antiForgery.tokenHeaderName, csrfToken, 'header');
        swaggerUi.api.clientAuthorizations.add(studiox.security.antiForgery.tokenHeaderName, csrfCookieAuth);
        return true;
    }

    studiox.swagger.login = function () {
        var tenantId = window.prompt('tenantId');
        var usernameOrEmailAddress = window.prompt('usernameOrEmailAddress');
        if (!usernameOrEmailAddress) {
            return false;
        }
        var password = window.prompt('password');
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                var responseJSON = JSON.parse(xhr.responseText);
                var result = responseJSON.result;
                var expireDate = new Date(Date.now() + (result.expireInSeconds * 1000));
                studiox.auth.setToken(result.accessToken, expireDate);
                studiox.swagger.addAuthToken();
                console.log(true);
            }
        };
        xhr.open('POST', '/api/TokenAuth/Authenticate', true);
        xhr.setRequestHeader('StudioX.TenantId', tenantId);
        xhr.setRequestHeader('Content-type', 'application/json');
        xhr.send("{" +
            "usernameOrEmailAddress:'" + usernameOrEmailAddress + "'," +
            "password:'" + password + "'}"
        );
    }

})();