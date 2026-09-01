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

--creating the events table that stores event/race information
create table events(
event_id int primary key identity(1,1),
organiser_id int not null,
event_name varchar(100) not null,
event_description text not null,
event_date datetime not null,
location varchar(200) not null,
distance decimal(5,2) not null,
event_type varchar(20) not null check (event_type in ('Run', 'Walk', 'Cycle')),
banner_image_url varchar(500) null,
max_participants int null,
entry_fee decimal(10,2) null,
event_status varchar(20) not null default 'Upcoming' check (event_status in ('Upcoming', 'Ongoing', 'Completed', 'Cancelled')),
created_at datetime not null default getdate(),
updated_at datetime null,
foreign key (organiser_id) references users(user_id)
);

--creating the categories table to stores age/distance categories for events
create table categories(
category_id int primary key identity(1,1),
event_id int not null,
category_name varchar(50) not null,
cat_description varchar(200) null,
min_age int null,
max_age int null,
min_distance decimal(5,2) null,
max_distance decimal(5,2) null,
created_at datetime not null default getdate(),
foreign key (event_id) references events(event_id)
);

--created the enrolments that stores participant event registrations
create table enrolments(
enrolment_id int primary key identity(1,1),
participant_id int not null,
event_id int not null,
category_id int not null,
enrolment_date datetime not null default getdate(),
enr_status varchar(20) not null default 'Pending' check (enr_status in ('Pending', 'Confirmed', 'Completed', 'Withdrawn')),
payment_status varchar(20) null default 'Pending' check (payment_status in ('Pending', 'Paid', 'Refunded')),
payment_amount decimal(10,2) null,
comments varchar(500) null,
updated_at datetime null,
foreign key (participant_id) references users(user_id),
foreign key (event_id) references events(event_id),
foreign key (category_id) references categories(category_id)
);

--created the results table to stores race results for participants
create table results(
result_id int primary key identity(1,1),
enrolment_id int not null unique,
event_id int not null,
participant_id int not null,
finish_time time(3) not null,
overall_position int not null,
category_position int null,
total_finishers int not null,
category_total int null,
is_disqualified bit not null default 0,
disqualification_reason varchar(200) null,
recorded_by int not null,
recorded_at datetime not null default getdate(),
updated_at datetime null,
foreign key (enrolment_id) references enrolments(enrolment_id),
foreign key (event_id) references events(event_id),
foreign key (participant_id) references users(user_id),
foreign key (recorded_by) references users(user_id)
);

--creating the event_images table that stores additional event images
create table event_images(
image_id int primary key identity(1,1),
event_id int not null,
image_url varchar(500) not null,
is_primary bit not null default 0,
caption varchar(200) null,
uploaded_at datetime not null default getdate(),
uploaded_by int not null,
foreign key (event_id) references events(event_id),
foreign key (uploaded_by) references users(user_id)
);

--inserting data into the tables 
--inserting orgernisers
insert into users (email, password_hash, firstname, surname, role, date_of_birth, phone_number, created_at, is_active) values 
('thabo.mokoena@raceday.co.za', 'hashed_password_123', 'Thabo', 'Mokoena', 'Organiser', '1985-03-15', '0821234567', getdate(), 1),
('sarah.van.der.merwe@raceday.co.za', 'hashed_password_456', 'Sarah', 'Van Der Merwe', 'Organiser', '1990-07-22', '0839876543', getdate(), 1);

--inserting participants
insert into users (email, password_hash, firstname, surname, role, date_of_birth, phone_number, created_at, is_active) values 
('david.nkosi@gmail.com', 'hashed_password_789', 'David', 'Nkosi', 'Participant', '1992-11-10', '0734567890', getdate(), 1),
('lindiwe.mthembu@gmail.com', 'hashed_password_101', 'Lindiwe', 'Mthembu', 'Participant', '1995-05-28', '0781234567', getdate(), 1),
('pieter.du.toit@gmail.com', 'hashed_password_102', 'Pieter', 'Du Toit', 'Participant', '1988-09-02', '0723456789', getdate(), 1),
('zanele.khumalo@gmail.com', 'hashed_password_103', 'Zanele', 'Khumalo', 'Participant', '1993-12-14', '0712345678', getdate(), 1),
('james.oosthuizen@gmail.com', 'hashed_password_104', 'James', 'Oosthuizen', 'Participant', '1990-06-30', '0798765432', getdate(), 1),
('grace.masondo@gmail.com', 'hashed_password_105', 'Grace', 'Masondo', 'Participant', '1997-04-05', '0745678901', getdate(), 1),
('sipho.dlamini@gmail.com', 'hashed_password_106', 'Sipho', 'Dlamini', 'Participant', '1994-08-19', '0824567890', getdate(), 1),
('maria.smit@gmail.com', 'hashed_password_107', 'Maria', 'Smit', 'Participant', '1991-10-25', '0835678901', getdate(), 1);
