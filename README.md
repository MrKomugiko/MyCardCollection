# MyCardCollection

Website created to help with managing Magic The Gathering cards collection,  
browsing for new inspiration and sharing briew ideas.  

###  Table of Contents  
- [Demo](#demo)  
- [Tech stack](#stack)  
- [Features](#Features)  
- [Run locally](#run)  
- [Overview (screens)](#screens)  


<a name="demo"/>

## Demo  

| **Website**       | https://demo-mrkomugiko.herokuapp.com           | 
|:-------------|:-------------| 
| **Login**        | MrKomugiko           | 
| **Password**       | Test123!           | 

**INFO:** *possible interruptions in the availability of the test page due to the cyclical rewriting   
of the Postgress database by the Heroku host and the need to manually refresh the access keys.*

<a name="stack"/>

## Tech Stack

**Language** C#, JS, JQuery 

**UI** Bootstrap 5 & Bootstrap Premium "Wingman" Theme

**Framework** ASP.NET Core MVC (.NET 6) + EntityFramework Core

**Database** Postgress

**Hosting** Heroku

<a name="Features"/>

## Features
&check; Fetching data from www.scryfall.com API.  
&check; Storing Users collection in database.  
&check; Displaying lists of all available card sets and its content.  
&check; Importing cards via *.txt file or in website single add input.  
```format: <Count> <SET> <Number> : 1 MID 123, 3 VOW 20 ... ```  
&check; Switching beetween list of cards data and image gallery.   
&check; Creating and customizing your own deck (from imported-owned cards).  
&check; Paginations, dynamic searching bar.  
&check; Creating personal accounts.  
&check; Displaying collection value and most valuable cards.  
&check; List of Other registered colelctioners with sorting by date,collection size or colelction value  
&check; Collectioners profile with customized background, easy acces to user top cards and his decks  
&check; Decks preview with charts - cost, color, set, type  
&check; Commenting other users decks and adding nested replies
&cross; Live updating card values (currently, price is locked with date of import).  

<a name="run"/>

## Run Locally
0. Prerequisits:
* Visual Studio 2022 (optional) 
* Microsoft SQL Managment Studio (local database)

1. Clone the project

```bash
  git clone https://github.com/MrKomugiko/MyCardCollection
```

2. Go to the project directory

```bash
  cd MyCardCollection
```

3. Install dependencies

```bash
  npm install
```

4. Insert connection string to your SQL database
Edit File: Program.cs
```
11: string connectionString = "<your connection string>";
```
5. Select database 
UseNpgsql if Postgres  
UseSqlServer if SQL  
```
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```
6. Remove content of Migrations folder
7. Prepare your database tables run commands
```
npm add-migration init
npm update-database
```

8. Start the server

```bash
  npm dotnet run
```

9. Browse website on localhost
```
https://localhost:7011/
```

<a name="screens"/>

## Overwiew - screens
![Manage page](MCC%20album/ManagePAge.png)
![Profile](MCC%20album/ProfilePage.png)
![Comments(MCC%20album/CommentsSystem.png)
![Collectioners](MCC%20album/CollectionersPage.png)
![Home](MCC%20album/HomePage.png)
![Deckspage](MCC%20album/DecksPage.png)
![Deck edition](MCC%20album/Decks_Edition.png)
![Deck selecting new bg image](MCC%20album/Decks_Edition-selectbg.png)
![Home_Set Gallery](MCC%20album/Home_Set-Gallery.png)
![Home Set list](MCC%20album/Home-Set-List.png)
![MyCollection](MCC%20album/MyCollectionPage.png)
![Deck builder](MCC%20album/DeckBuilderPage.png)
![Deck overview modal](MCC%20album/DeckOverviewModal.png)
![CardDetailPage_1](MCC%20album/CardDetailPage_1.png)
![CardDetailPage_2](MCC%20album/CardDetailPage_2.png)

