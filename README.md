# ASP.NET Core 2.0 Multi-Tenant WebApp
The ASP.NET Core 2.0 Multi-Tenant WebApp shows how to implement a Multi-Tenant Webb Application that uses a Web Api. 
Both applications are secured by Azure Active Directory.
The WebApp starts with a Welcome Page that allows the user either to signin or to signup and onboard the Application in his AAD Tenant.

# Setup 
The Application can be setup using the Powershellscript 'Deploy-Application.ps1'. The script registers both Applications in your AAD Tenant as Multi-Tenant Application. The script needs only two parameters:
    * WebAppName, the name of the Web-Application in your AAD Tenant
    * WebAppUri, the Uri of the Web-Application e.g.: "https://ad-tenant/web-app-name"

The script returns an object that contains the Application's ClientId an ClientSecret.
You have to put these values in the appsettings.json file of the Web-Application.

# OnBoarding
After calling the script you can use another AAD Tenant to test the OnBoarding process. 
The user can agree to the required Application permissions.

# Authentication Cookies
The Web-Application's Authentication Cookies are stored in a in memory Session store to keep the Cookies small.

# Token Caching
The Web-Application uses a in memory Token Cache.

# IIS Express Ports
The Application is configured to use IIS Express.
The Web Application uses IIS Express on port 44377

# Deployment to Azure
Not supported at the moment.

# Removing the Application in your AAD Tenant
To remove the Application from your AAD Tenant just call the Remove-Application.ps1 script and specify the ClientId.