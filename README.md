ASP.NET 5.0 API - JWT Authentication Server with Refresh Tokens
 
# Installation

1. Create the target database, i.e. _JwtAuth_.

2. Build the solution.

```
dotnet build .\JwtAuthRestApi.sln --Configuration Release
```

3. Run __JwtAuthServer.RegistrationTool__  to install migrations and then required users and roles.

Please use one of the next commands:
```
JwtAuthServer.RegistrationTool.exe add-roles InputFiles/Roles.xml
JwtAuthServer.RegistrationTool.exe add-users InputFiles/Users.xml
```

1. Check the database <br>
The following tables will appear:
- AspNetRoles
- AspNetUsers
- AspNetRoleClaims
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens
- AppUserRefreshTokens
- RevokedRefreshTokens -- collection of revoked tokens after rotation

2. Run the server __JwtAuthServer__
```
dotnet run JwtAuthServer.Api.dll --Configuration Release
```

The server provides API to authenticate users, validate tokens or refresh (with enabled rotation) them.

# Test the web application

1. Run the web app __JwtDemoWebApp__
```
dotnet run JwtDemoWebApp.dll --Configuration Release
```

2. Open the browser and login as a tester.
```
https://localhost:5021
```

Use the credentials below
Login: _tester_ or _user_ or _MixUser_
Password: _P@ssw0rd!_

3. Open the document tab to see available documents for the user.
Different users see different documents

# Test the web application api controller

1. Run the web app __JwtDemoWebApp__

2. Run the client app __JwtDemoClientApp__
```
dotnet run JwtDemoClientApp.dll --Configuration Release
```

The client application authenticates as a tester and reads the avaliable set of documents.
The expected output is 
```
["doc-01.txt","doc-03.txt"]
```