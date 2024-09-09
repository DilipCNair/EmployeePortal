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
To bypass I have a code commented out in Layout.cshtml which is the replica of the code that runs only after authorization which you can uncomment.
Then you have to also comment the authorization attribute applied directly to AdminController.
No you can create roles and make yourself admin and then revert the code
