# Task Management System

## Tech Stack
1. Frontend - Razor, JQuerry and Bootstrap 
2. Backend - .NET 8
3. ORM - Entity Framework
4. Authentication and Authorization - Identity Framework
5. Database - Postgresql
6. IDE - Visual Studio

## Setup

Checkout from the repository and apply migrations. I have already created the migration so you just need to apply the migration using the command _Update-Database_

## First Run
When you run the project for the first time you have to signup and make yourself admin. But for that initially, you have to bypass authorization checks.
1. To bypass I have a code commented out in Layout.cshtml which is the replica of the code that runs only after authorization which you can uncomment.
2. Then you have to also comment the authorization attribute applied directly to AdminController.
3. Now you can create roles and make yourself admin and then revert the changes that you made now to activate authorization.
4. Now the project is ready and you test all the features

## Database
Since the project uses PostgreSQL you have to set it up and import the connection string and put it in the appsettings.json or secrets.json

