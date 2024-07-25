# System Info Solution Overview

This project is a *Windows only* C# client/server solution designed to gather information about systems where the client app is run on. 
The clients centralize the data through the server-side app's API for storage in a database. 
It is designed for companies that need to keep track of their customers' systems.

## Features

The client gathers information such as:
  - System name
  - Drives info (space, format, name, serial number...)
  - Operating system info (drive path, version, build number...)
  - Installed applications info (version, path, name...)

And uses the server-side app's API which:
  - Stores the current state of the system in a database
  - Create a history of all previous states of the system

Secured with argon2 hashing and JWT authentication.

## Installation

### Server API Setup

1. Edit the appsettings.json file in the 'SystemInfoApi' folder with your settings and database information.
2. If you kept auto-migrations to false, make sure you execute the SQL script in the "Migrations" folder to create the tables in your database.
3. Insert your customers names into the newly created "Client" table in your database.
4. Insert the application names into the newly created "Application" table in your database.

### Client Setup

Open and edit the settings.json file in the 'SystemInfoClient' folder as follows:
1. Set the CustomerId to the ID of the customer who owns the machine where the client app will be running.
2. Set the ApiUrl to the URL of the server API.
3. Edit the placeholder applications to the ones you want to keep track of and their correct paths. (Make sure the IDs correctly correspond to the IDs in the "Application" table in your database.)
4. Create a Windows user environment variable called "SysInfoApp".
5. Put the ApiPassword you chose in the appsettings.json file for the API as value for this environment variable. (This clear password will be hashed once the app runs for the first time.)

Make sure the server API is running, and execute the client app.