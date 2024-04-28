
![Logo](https://my.unshackled.fitness/icon_x128.png)


# Unshackled Fitness

A workout planner/tracker for weight lifters. It is a cross-platform hosted WebAssembly Blazor PWA with a vertical slice architecture that supports MS SQL Server, MySQL, and PostgreSql databases through Entity Framework.



## Demo

A hosted version is available at https://unshackled.fitness. A 30-day, no credit card required, free trial is available.


## Run Locally

Install the [.NET SDK](https://dotnet.microsoft.com/en-us/download) for your platform. Then, install the EF Core tools for the .NET CLI.
```bash
dotnet tool install --global dotnet-ef
```

Clone the project

```bash
  git clone https://github.com/unshackled-fitness/Unshackled.Fitness.git Unshackled.Fitness
```

Go to the project directory

```bash
  cd Unshackled.Fitness
```

### Create App Settings files ###
Copy and rename the sample-appsettings.json files for the Blazor server and client projects

```bash
# Windows
copy .\src\Unshackled.Fitness.My\sample-appsettings.json .\src\Unshackled.Fitness.My\appsettings.json
copy .\src\Unshackled.Fitness.My\sample-appsettings.Development.json .\src\Unshackled.Fitness.My\appsettings.Development.json
copy .\src\Unshackled.Fitness.My.Client\wwwroot\sample-appsettings.json .\src\Unshackled.Fitness.My.Client\wwwroot\appsettings.json
copy .\src\Unshackled.Fitness.My.Client\wwwroot\sample-appsettings.Development.json .\src\Unshackled.Fitness.My.Client\wwwroot\appsettings.Development.json

# Mac OS/Linux
cp ./src/Unshackled.Fitness.My/sample-appsettings.json ./src/Unshackled.Fitness.My/appsettings.json
cp ./src/Unshackled.Fitness.My/sample-appsettings.Development.json ./src/Unshackled.Fitness.My/appsettings.Development.json
cp ./src/Unshackled.Fitness.My.Client/wwwroot/sample-appsettings.json ./src/Unshackled.Fitness.My.Client/wwwroot/appsettings.json
cp ./src/Unshackled.Fitness.My.Client/wwwroot/sample-appsettings.Development.json ./src/Unshackled.Fitness.My.Client/wwwroot/appsettings.Development.json
```

### Configure App Settings ###

Open the solution file or project folder in your favorite editor and complete the newly created appsettings files.

**Unshackled.Fitness.My/appsettings.json**

*DbConfiguration*
```json
"DbConfiguration": {
	"DatabaseType": "mssql",
	"TablePrefix": "uf_"
}
```
* DatabaseType: The database you will be using. Use mssql, mysql, or postgresql as the value.
* TablePrefix: The table name prefix in the database. Recommend that you leave this as "uf_" unless you have a reason to change it.

*HashIdConfiguration*
```json
"HashIdConfiguration": {
	"Alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890",
	"Salt": "randomstringofcharacters",
	"MinLength": 12
},
```
* Alphabet: The list of possible characters to use while hashing database IDs for display in URLs.
* Salt: A random string of characters to salt your hash. Make this sufficiently long (at least 15 chars)
* MinLength: The minimum length you want the resultant hash to be. Larger IDs may produce longer hashes, but lower ID numbers will not fall below this minimum.

*SiteConfiguration*
```json
"SiteConfiguration": {
	"SiteName": "Unshackled Fitness",
	"AppThemeColor": "#194480",
	"AllowRegistration": true,
	"RequireConfirmedAccount": true,
	"PasswordStrength": {
		"RequireDigit": true,
		"RequireLowercase": true,
		"RequireNonAlphanumeric": true,
		"RequireUppercase": true,
		"RequiredLength": 10,
		"RequiredUniqueChars": 7
	}
}
```
* SiteName: The default title of your app.
* AppThemeColor: The color of the loading screen when your PWA is first displayed.
* AllowRegistration: True, allows users to register for an account. False, prevents new user accounts registration.
* RequireConfirmedAccount: True, requires new users to confirm their email adddress before logging in. False, new users can log in immediately.
* PasswordStrength: Set the requirements you want for password strength.

*SmtpSettings*
```json
"SmtpSettings": {
	"Host": "smtp.host.domain",
	"Port": 587,
	"UseSSL": true,
	"DefaultReplyTo": "email@domain.com",
	"Username": "",
	"Password": ""
}
```
* Host: Your SMTP host URL.
* Port: 587 is the default. 465 if required by your host. 25 if your host doesn't support SSL
* UseSSL: Leave to true unless required otherwise.
* DefaultReplyTo: The default reply address to be used when sending emails.
* Username: Your host username. **This setting should be moved to a secrets file, key vault, or environment variable**
* PasswordStrength: Your host password. **This setting should be moved to a secrets file, key vault, or environment variable**

*ConnectionStrings*

**Unshackled.Fitness.My/appsettings.Development.json**

```json
"ConnectionStrings": {
	"DefaultDatabase": ""
},
```
* DefaultDatabase: The connection string for the database you chose in the DbConfiguration section.

**Unshackled.Fitness.My.Client/wwwroot/appsettings.json**

*ClientConfiguration*
```json
"ClientConfiguration": {
	"SiteName": "Unshackled Fitness",
	"LibraryApiUrl": "https://my.unshackled.fitness/api/"
},
```
* SiteName: The default title of your app.
* LibraryApiUrl: The URL for accessing our library of exercises. Leave this as is or set it to "" if you don't wish to allow your users to import exercises from the hosted library.

### Database Migrations ###

In your terminal, move to the Unshackled.Fitness.Core.Data directory from the main project directory
```bash
cd ./src/Unshackled.Fitness.Core.Data
```
Add a migration for the current release and your database.

```bash
# MS SQL Server
dotnet ef migrations add v1.1.0 -c MsSqlServerDbContext -s ../Unshackled.Fitness.Web -o Migrations
# MySQL Server
dotnet ef migrations add v1.1.0 -c MySqlServerDbContext -s ../Unshackled.Fitness.Web -o Migrations
# PostgreSQL Server
dotnet ef migrations add v1.1.0 -c PostgresSqlServerDbContext -s ../Unshackled.Fitness.Web -o Migrations
```

Apply the migration
```bash
# MS SQL Server
dotnet ef database update --context MsSqlServerDbContext -s ../Unshackled.Fitness.Web
# MySQL Server
dotnet ef database update --context MySqlServerDbContext -s ../Unshackled.Fitness.Web
# PostgreSQL Server
dotnet ef database update --context PostgreSqlServerDbContext -s ../Unshackled.Fitness.Web
```

### Run the Website ###
Move to the Unshackled.Fitness.My directory from the main project directory.
```bash
cd ./src/Unshackled.Fitness.My
```
By default, the website will run at https://localhost:5580. You can change these settings under Properties/launchSettings.json.

Start the server
```bash
dotnet run
```
 ### Upgrading to new versions ###
 Review the release notes for the new version (and any previous versions you may have skipped) to see if new database migrations are required. Add and apply new migrations using the same commands as before while changing the version number to the new version.

## Acknowledgements

Unshackled Fitness is built on top of these other great projects.

 - [AutoMapper](https://automapper.org/)
 - [Blazored.FluentValidation](https://github.com/Blazored/FluentValidation)
 - [Blazored.LocalStorage](https://github.com/Blazored/LocalStorage)
 - [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
 - [Hashids.Net](https://github.com/ullmark/hashids.net)
 - [MudBlazor](https://mudblazor.com/)
 - [MediatR](https://github.com/jbogard/MediatR)
 - [TimeZoneNames](https://github.com/mattjohnsonpint/TimeZoneNames)
 - [Z.EntityFramework.Plus.EfCore](https://entityframework-plus.net/)
