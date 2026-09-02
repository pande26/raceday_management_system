# 🏃 RaceDay - Event Management System

## 📋 Overview

**RaceDay** is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform bridges the gap between Event Organisers and Participants, streamlining the entire event lifecycle from creation to results publishing.

### 🌟 Key Features
- **Event Management**: Create, edit, and manage events with categories
- **Participant Registration**: Browse events and enrol with category selection
- **Results Tracking**: Record and publish race results with finishing positions
- **Role-Based Access**: Separate interfaces for Organisers and Participants
- **Cloud Storage**: Azure Blob Storage integration for event banners and profile pictures
- **Containerized**: Fully Dockerized for easy deployment

---

## 👥 User Roles

### 🎯 Organiser
- Create, edit, and delete events
- Manage event categories (age/distance groups)
- View all event enrolments
- Capture and publish participant results
- Upload event banner images

### 🏃 Participant
- Create account and log in
- Browse upcoming events with filtering
- Enrol in events by selecting a category
- View personal enrolment history
- Track personal race results with finishing positions
- Update profile and upload profile picture

---

## 📁 Project Structure (Part 1)
RaceDay/
├── README.md # This file
├── docs/ # Documentation folder
│ ├── ERD.png # Entity Relationship Diagram
│ ├── API-ENDPOINT-PLAN.pdf # Full API endpoint specification
│ └── raceday_db.sql # SQL database script with seed data
└── .github/
└── workflows/
└── ci-cd.yml # GitHub Actions CI/CD workflow


---

## 🗄️ Database Schema

### Entities (7 Tables)
1. **Users** - Authentication and user information
2. **Events** - Event/race management
3. **Categories** - Age/distance categories per event
4. **Enrolments** - Participant registrations
5. **Results** - Race results and finishing positions
6. **Event_Images** - Gallery images for events
7. **Profiles** - User personal information (optional)

### ERD Highlights
- ✅ Minimum 6 entities included
- ✅ Primary and Foreign keys clearly identified
- ✅ Cardinality indicated for all relationships
- ✅ CHECK constraints for data validation
- ✅ Soft delete support (is_active, event_status)
- ✅ Audit fields (created_at, updated_at)

---

## 🔌 API Endpoint Plan

### Endpoint Summary (38 Total)

| Category | GET | POST | PUT | DELETE | Total |
|----------|-----|------|-----|--------|-------|
| Authentication | 1 | 2 | 0 | 1 | 4 |
| User Profile | 2 | 1 | 1 | 0 | 4 |
| Events | 4 | 2 | 1 | 1 | 8 |
| Categories | 2 | 1 | 1 | 1 | 5 |
| Enrolments | 3 | 1 | 3 | 0 | 7 |
| Results | 4 | 1 | 2 | 1 | 8 |
| Dashboard | 2 | 0 | 0 | 0 | 2 |
| **TOTAL** | **18** | **8** | **8** | **4** | **38** |

### Key Endpoints
- **Authentication**: `/api/auth/register`, `/api/auth/login`, `/api/auth/logout`
- **Events**: `/api/events` (CRUD), `/api/events/{id}/banner` (upload)
- **Categories**: `/api/events/{id}/categories` (CRUD)
- **Enrolments**: `/api/events/{id}/enrol`, `/api/enrolments/me`
- **Results**: `/api/events/{id}/results`, `/api/results/me`, `/api/results/{id}/publish`

---

## 📊 Database Setup Instructions

### Prerequisites
- SQL Server (2019 or later)
- SQL Server Management Studio (SSMS)

### Steps to Create Database

1. **Open SQL Server Management Studio (SSMS)**

2. **Run the SQL Script**
   - Open `docs/raceday_db.sql` in SSMS
   - Click **Execute** (F5) to run the script

3. **Verify Database Creation**
   ```sql
   USE raceday_db;
   SELECT * FROM users;
   SELECT * FROM events;
   SELECT * FROM enrolments;

4. Sample Data Included
-2 Organisers (Thabo Mokoena, Sarah Van Der Merwe)
-8 Participants with South African names
-4 Events (Comrades, Cycle Tour, Soweto, Two Oceans)
-Categories for each event
-Enrolments with various statuses
-Results with finish times and positions

Seed Data Statistics
Entity	Count	Description
Organisers	2	Event creators
Participants	8	Registered users
Events	4	Upcoming events
Categories	24	Categories per event
Enrolments	24	Participant registrations
Results	11	Race results
Event Images	8	Gallery images
