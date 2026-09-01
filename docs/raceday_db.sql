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
