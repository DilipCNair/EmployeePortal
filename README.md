# Task Management System

## Tech Stack
1. Frontend - Razor, JQuerry and Bootstrap 
2. Backend - .NET 8
3. ORM - Entity Framework
4. Authentication and Authorization - Identity Framework
5. Database - Postgresql
6. IDE - Visual Studio

## Setup

Checkout from the repository and apply migrations. I have already created the migration so you just need to apply the migration using the command - _Update-Database_

## First Run
When you run the project for the first time you have to sign up and make yourself admin, but for that, you have to bypass authorization checks initially.
1. To bypass it, I have added codes that are initially commented out in Layout.cshtml, this code actually is the replica of the code that runs only after authorization, so uncomment that code.
2. Then you have to also comment on the authorization attribute applied directly to AdminController.
3. Now you can create roles and make yourself admin and also don't forget to revert the changes that you made so far to activate the authorization again.
4. Now the project is ready and you can test all the features

## Database
Since the project uses PostgreSQL you have to set it up and import the connection string and put it in the appsettings.json or secrets.json

## Slideshow 
[https://www.canva.com/design/DAGQQbgx3BQ/rX9Y2MKtjzWYaMP3s9mIyg/view]
