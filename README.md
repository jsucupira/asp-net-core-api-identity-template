Web API template projects
===================


This is a template project I created for boiler plate code for Web API with Identity.  There are two projects here, one for .NET 4.6 and another for .NET Core

----------

 - .NET 4.6
	- I am using EntityFramework 6.1 and Microsoft.AspNet.Identity.EntityFramework for the Authentication piece
	- Also using CacheCow for the caching layer
 - .NET Core
	 - I am using EntityFramework Core 1.1 and Microsoft.AspNetCore.Identity.EntityFramework for the Authentication piece 
		 - before running the project for the first time you will need to run EF database update command, which is dotnet ef database update (https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations).

## Postman ##
There are some Postman web request export in the project as well.  I use those for testing.

**Setup**

 - Import the file Web API Template.postman_collection.json
 - Create Local Variables
	 - "host": this is the url used on your project
	 - "access_token": you won't need to set this value.  It will be automatically set once you call the Login end-point

