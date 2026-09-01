--creating a database
create database raceday_db;

--use database
use [raceday_db];

--creating a table that will store the user's information
create table users(
user_id int primary key identity(1,1),
email varchar(100) not null unique,
password_hash varchar(255) not null,
firstname varchar(55) not null,
surname varchar(50) not null,
role varchar(20) not null check (role in ('Organiser', 'Participant')),
profile_picture_url varchar(500) null,
date_of_birth date null,
phone_number varchar(20) null,
created_at datetime not null default getdate(),
updated_at datetime null,
is_active bit not null default 1
);