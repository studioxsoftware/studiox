
1. The studiox.js included is a lite version.
2. The on-complete.js adds 3 helper methods in the studiox.swagger namespace:

	+ addAuthToken() adds the authToken, which can be set via studiox.auth.setToken(authToken, expireDate).
	+ addCsrfToken() adds the csrfToken, if the user is logged in to the application.
	+ login() prompts for tenantId, usernameOrEmailAddress and password, logs in via token-based authentication at 
		/api/TokenAuth/Authenticate and then calls addAuthToken() automatically.

3. Example flow:
3.1. Example flow 1: User is logged into application

Run studiox.swagger.addCsrfToken() in the console to authenticate
Refresh to unauthenticate

3.2. Example flow 2: User is logged in via Postman

Run studiox.auth.setToken(authToken) and studiox.swagger.addAuthToken() in the console to authenticate
Refresh to unauthenticate
Subsequently, run studiox.swagger.addAuthToken() in the console to authenticate

3.3. Example flow 3: User is not logged in

Run studiox.swagger.login() in the console to authenticate
Refresh to unauthenticate
Subsequently, run studiox.swagger.addAuthToken() in the console to authenticate