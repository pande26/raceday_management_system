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

--inserting events
insert into events (organiser_id, event_name, event_description, event_date, location, distance, event_type, banner_image_url, max_participants, entry_fee, event_status, created_at) values 
(
    1,
    'Comrades Marathon 2026',
    'The Ultimate Human Race - 90km ultra-marathon from Pietermaritzburg to Durban. This iconic event attracts runners from all over the world.',
    '2026-06-21 05:30:00',
    'Pietermaritzburg to Durban, KwaZulu-Natal',
    90.00,
    'Run',
    'https://raceday.blob.core.windows.net/events/comrades-2026.jpg',
    25000,
    1500.00,
    'Upcoming',
    getdate()
),
(
    1,
    'Cape Town Cycle Tour 2026',
    'The world''s largest timed cycling event. A spectacular 109km route around the Cape Peninsula with breathtaking views of the Atlantic Ocean.',
    '2026-03-08 06:00:00',
    'Cape Town, Western Cape',
    109.00,
    'Cycle',
    'https://raceday.blob.core.windows.net/events/cycle-tour-2026.jpg',
    35000,
    450.00,
    'Upcoming',
    getdate()
),
(
    2,
    'Soweto Marathon 2026',
    'A vibrant and culturally rich marathon through the streets of Soweto. Celebrating South African heritage, community spirit, and athletic excellence.',
    '2026-11-01 05:00:00',
    'Soweto, Gauteng',
    42.20,
    'Run',
    'https://raceday.blob.core.windows.net/events/soweto-2026.jpg',
    15000,
    750.00,
    'Upcoming',
    getdate()
),
(
    2,
    'Two Oceans Marathon 2026',
    'Known as the world''s most beautiful marathon. Takes runners through the stunning Cape Peninsula with views of both the Atlantic and Indian Oceans.',
    '2026-04-04 05:30:00',
    'Cape Town, Western Cape',
    56.00,
    'Run',
    'https://raceday.blob.core.windows.net/events/two-oceans-2026.jpg',
    12000,
    850.00,
    'Upcoming',
    getdate()
);

--inserting categories for each event
-- Categories for Comrades Marathon
insert into categories (event_id, category_name, cat_description, min_age, max_age, min_distance, max_distance, created_at) values 
(1, 'Senior Open', 'Open category for all runners', 18, 39, NULL, NULL, getdate()),
(1, 'Masters 40-49', 'Competitive category for runners aged 40-49', 40, 49, NULL, NULL, getdate()),
(1, 'Masters 50-59', 'Competitive category for runners aged 50-59', 50, 59, NULL, NULL, getdate()),
(1, 'Grand Masters 60+', 'Category for runners aged 60 and over', 60, NULL, NULL, NULL, getdate());

-- Categories for Cape Town Cycle Tour
insert into categories (event_id, category_name, cat_description, min_age, max_age, min_distance, max_distance, created_at) values 
(2, 'Elite Men', 'Competitive category for professional male cyclists', 18, 40, NULL, NULL, getdate()),
(2, 'Elite Women', 'Competitive category for professional female cyclists', 18, 40, NULL, NULL, getdate()),
(2, 'Open Men', 'For male cyclists aged 18-50', 18, 50, NULL, NULL, getdate()),
(2, 'Open Women', 'For female cyclists aged 18-50', 18, 50, NULL, NULL, getdate()),
(2, 'Veteran Men', 'For male cyclists aged 51+', 51, 99, NULL, NULL, getdate()),
(2, 'Veteran Women', 'For female cyclists aged 51+', 51, 99, NULL, NULL, getdate());

-- Categories for Soweto Marathon
insert into categories (event_id, category_name, cat_description, min_age, max_age, min_distance, max_distance, created_at) values 
(3, 'Open Men', 'Open category for men', 18, 34, NULL, NULL, getdate()),
(3, 'Open Women', 'Open category for women', 18, 34, NULL, NULL, getdate()),
(3, 'Veteran Men', 'Veteran category for men 35+', 35, NULL, NULL, NULL, getdate()),
(3, 'Veteran Women', 'Veteran category for women 35+', 35, NULL, NULL, NULL, getdate());

-- Categories for Two Oceans Marathon
insert into categories (event_id, category_name, cat_description, min_age, max_age, min_distance, max_distance, created_at) values 
(4, 'Senior Men', 'Open category for men', 20, 39, NULL, NULL, getdate()),
(4, 'Senior Women', 'Open category for women', 20, 39, NULL, NULL, getdate()),
(4, 'Masters Men', 'Masters category for men 40+', 40, NULL, NULL, NULL, getdate()),
(4, 'Masters Women', 'Masters category for women 40+', 40, NULL, NULL, NULL, getdate());

--inserting enrolments
--enrolments for Comrades Marathon
insert into enrolments (participant_id, event_id, category_id, enrolment_date, enr_status, payment_status, payment_amount, comments) values 
(3, 1, 1, dateadd(day, -45, getdate()), 'Confirmed', 'Paid', 1500.00, 'Training for first ultra'),
(4, 1, 2, dateadd(day, -40, getdate()), 'Confirmed', 'Paid', 1500.00, NULL),
(5, 1, 1, dateadd(day, -35, getdate()), 'Pending', 'Pending', 1500.00, 'Awaiting medical clearance'),
(6, 1, 3, dateadd(day, -30, getdate()), 'Withdrawn', 'Refunded', 1500.00, 'Withdrew due to injury'),
(7, 1, 2, dateadd(day, -25, getdate()), 'Confirmed', 'Paid', 1500.00, NULL),
(8, 1, 4, dateadd(day, -20, getdate()), 'Pending', 'Pending', 1500.00, 'First Comrades');

--enrolments for Cape Town Cycle Tour
insert into enrolments (participant_id, event_id, category_id, enrolment_date, enr_status, payment_status, payment_amount, comments) values 
(3, 2, 3, dateadd(day, -60, getdate()), 'Confirmed', 'Paid', 450.00, 'Cycling with club team'),
(4, 2, 4, dateadd(day, -55, getdate()), 'Confirmed', 'Paid', 450.00, NULL),
(5, 2, 3, dateadd(day, -50, getdate()), 'Confirmed', 'Paid', 450.00, 'Ready for the challenge'),
(7, 2, 5, dateadd(day, -45, getdate()), 'Pending', 'Pending', 450.00, NULL),
(8, 2, 6, dateadd(day, -40, getdate()), 'Confirmed', 'Paid', 450.00, 'Cycling with friends');

--enrolments for Soweto Marathon
insert into enrolments (participant_id, event_id, category_id, enrolment_date, enr_status, payment_status, payment_amount, comments) values 
(3, 3, 1, dateadd(day, -30, getdate()), 'Confirmed', 'Paid', 750.00, 'Running for charity'),
(4, 3, 2, dateadd(day, -28, getdate()), 'Confirmed', 'Paid', 750.00, NULL),
(5, 3, 3, dateadd(day, -25, getdate()), 'Pending', 'Pending', 750.00, 'First Soweto marathon'),
(6, 3, 1, dateadd(day, -20, getdate()), 'Confirmed', 'Paid', 750.00, 'Ready to race'),
(7, 3, 4, dateadd(day, -15, getdate()), 'Confirmed', 'Paid', 750.00, NULL),
(8, 3, 3, dateadd(day, -10, getdate()), 'Pending', 'Pending', 750.00, 'Last minute entry');

--enrolments for Two Oceans Marathon
insert into enrolments (participant_id, event_id, category_id, enrolment_date, enr_status, payment_status, payment_amount, comments) values 
(3, 4, 1, dateadd(day, -90, getdate()), 'Confirmed', 'Paid', 850.00, 'Training going well'),
(4, 4, 2, dateadd(day, -85, getdate()), 'Confirmed', 'Paid', 850.00, NULL),
(5, 4, 3, dateadd(day, -80, getdate()), 'Confirmed', 'Paid', 850.00, 'Running with running club'),
(6, 4, 1, dateadd(day, -75, getdate()), 'Pending', 'Pending', 850.00, 'Awaiting confirmation'),
(7, 4, 4, dateadd(day, -70, getdate()), 'Confirmed', 'Paid', 850.00, NULL),
(8, 4, 3, dateadd(day, -65, getdate()), 'Confirmed', 'Paid', 850.00, 'Excited for the race');

--inserting results
--results for Comrades Marathon
insert into results (enrolment_id, event_id, participant_id, finish_time, overall_position, category_position, total_finishers, category_total, is_disqualified, disqualification_reason, recorded_by, recorded_at) values 
(1, 1, 3, '07:15:22.000', 125, 28, 18500, 4500, 0, NULL, 1, dateadd(day, 1, getdate())),
(2, 1, 4, '08:12:45.000', 345, 89, 18500, 4500, 0, NULL, 1, dateadd(day, 1, getdate())),
(5, 1, 7, '09:28:15.000', 678, 156, 18500, 4500, 0, NULL, 1, dateadd(day, 1, getdate()));

--results for Cape Town Cycle Tour
insert into results (enrolment_id, event_id, participant_id, finish_time, overall_position, category_position, total_finishers, category_total, is_disqualified, disqualification_reason, recorded_by, recorded_at) values 
(7, 2, 3, '03:45:30.000', 1245, 87, 24000, 4500, 0, NULL, 1, dateadd(day, 2, getdate())),
(8, 2, 4, '04:12:45.000', 3456, 234, 24000, 4500, 0, NULL, 1, dateadd(day, 2, getdate())),
(10, 2, 8, '05:22:15.000', 5678, 456, 24000, 4500, 0, NULL, 1, dateadd(day, 2, getdate()));

--results for Soweto Marathon
insert into results (enrolment_id, event_id, participant_id, finish_time, overall_position, category_position, total_finishers, category_total, is_disqualified, disqualification_reason, recorded_by, recorded_at) values 
(12, 3, 3, '03:56:15.000', 78, 15, 12000, 3200, 0, NULL, 2, dateadd(day, 3, getdate())),
(13, 3, 4, '04:10:32.000', 156, 42, 12000, 3200, 0, NULL, 2, dateadd(day, 3, getdate())),
(15, 3, 7, '05:45:12.000', 567, 89, 12000, 3200, 0, NULL, 2, dateadd(day, 3, getdate()));

--results for Two Oceans Marathon
insert into results (enrolment_id, event_id, participant_id, finish_time, overall_position, category_position, total_finishers, category_total, is_disqualified, disqualification_reason, recorded_by, recorded_at) values 
(16, 4, 3, '05:12:30.000', 156, 34, 9500, 2000, 0, NULL, 2, dateadd(day, 4, getdate())),
(17, 4, 4, '05:45:12.000', 289, 78, 9500, 2000, 0, NULL, 2, dateadd(day, 4, getdate())),
(18, 4, 5, '04:58:47.000', 134, 28, 9500, 2000, 0, NULL, 2, dateadd(day, 4, getdate())),
(20, 4, 7, '06:15:30.000', 456, 89, 9500, 2000, 0, NULL, 2, dateadd(day, 4, getdate()));

--inserting event_images
insert into event_images (event_id, image_url, is_primary, caption, uploaded_at, uploaded_by) values 
(1, 'https://raceday.blob.core.windows.net/gallery/comrades-start.jpg', 1, 'Comrades Marathon start line at Pietermaritzburg', getdate(), 1),
(1, 'https://raceday.blob.core.windows.net/gallery/comrades-finish.jpg', 0, 'Finishers at Durban City Hall', getdate(), 1),
(2, 'https://raceday.blob.core.windows.net/gallery/cycle-tour-start.jpg', 1, 'Mass start at Cape Town Stadium', getdate(), 1),
(2, 'https://raceday.blob.core.windows.net/gallery/cycle-tour-chapmans.jpg', 0, 'Cyclists along Chapmans Peak Drive', getdate(), 1),
(3, 'https://raceday.blob.core.windows.net/gallery/soweto-start.jpg', 1, 'Vibrant start in Soweto', getdate(), 2),
(3, 'https://raceday.blob.core.windows.net/gallery/soweto-crowd.jpg', 0, 'Supportive crowds cheering runners', getdate(), 2),
(4, 'https://raceday.blob.core.windows.net/gallery/two-oceans-start.jpg', 1, 'Starting line at Newlands', getdate(), 2),
(4, 'https://raceday.blob.core.windows.net/gallery/two-oceans-view.jpg', 0, 'Scenic views along the Cape coastline', getdate(), 2);
