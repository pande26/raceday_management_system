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
│ ├── raceday_db.sql # SQL database script with seed data
│ └── ci-cd-green-build.png # CI/CD green build screenshot
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

4.Sample Data Included
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

🔧 CI/CD Pipeline
GitHub Actions Workflow
The CI/CD pipeline validates the repository structure for Part 1:
name: CI/CD Pipeline - RaceDay Part 1

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  validate-docs:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Check docs folder exists
      run: |
        if [ -d "docs" ]; then
          echo "✅ docs folder exists"
        else
          echo "❌ docs folder not found"
          exit 1
        fi
    
    - name: Check ERD exists
      run: |
        if [ -f "docs/ERD.png" ] || [ -f "docs/ERD.jpeg" ] || [ -f "docs/ERD.pdf" ]; then
          echo "✅ ERD file found"
        else
          echo "❌ ERD file not found"
          exit 1
        fi
    
    - name: Check API Endpoint Plan exists
      run: |
        if [ -f "docs/API-ENDPOINT-PLAN.pdf" ] || [ -f "docs/API-ENDPOINT-PLAN.md" ]; then
          echo "✅ API Endpoint Plan found"
        else
          echo "❌ API Endpoint Plan not found"
          exit 1
        fi
    
    - name: Check SQL Script exists
      run: |
        if [ -f "docs/raceday_db.sql" ]; then
          echo "✅ SQL Script found"
        else
          echo "❌ SQL Script not found"
          exit 1
        fi
    
    - name: Check README exists
      run: |
        if [ -f "README.md" ]; then
          echo "✅ README.md found"
        else
          echo "❌ README.md not found"
          exit 1
        fi
    
    - name: Check README contains required sections
      run: |
        if grep -q "system description\|role" README.md; then
          echo "✅ README contains system description"
        else
          echo "⚠️ README may be missing system description"
        fi
        
        if grep -q "CI/CD\|green build" README.md; then
          echo "✅ README contains CI/CD section"
        else
          echo "⚠️ README may be missing CI/CD section"
        fi
        
        if grep -q "youtube.com\|video" README.md; then
          echo "✅ README contains video link"
        else
          echo "⚠️ README may be missing video link"
        fi
    
    - name: Validate SQL script syntax
      run: |
        if grep -q "CREATE TABLE" docs/raceday_db.sql; then
          echo "✅ SQL script contains CREATE TABLE statements"
        else
          echo "❌ SQL script missing CREATE TABLE statements"
          exit 1
        fi
        
        if grep -q "INSERT INTO" docs/raceday_db.sql; then
          echo "✅ SQL script contains seed data (INSERT statements)"
        else
          echo "⚠️ SQL script may be missing seed data"
        fi
    
    - name: Count entities in SQL script
      run: |
        TABLE_COUNT=$(grep -c "CREATE TABLE" docs/raceday_db.sql || true)
        echo "📊 Found $TABLE_COUNT tables in SQL script"
        if [ $TABLE_COUNT -ge 6 ]; then
          echo "✅ SQL script has minimum 6 tables"
        else
          echo "❌ SQL script has less than 6 tables"
          exit 1
        fi
    
    - name: Validation Summary
      run: |
        echo "=========================================="
        echo "✅ CI/CD VALIDATION COMPLETE"
        echo "=========================================="
        echo "✅ All required files present!"
        echo "✅ Part 1 is ready for submission!"
        echo "=========================================="

✅ Successful Build Screenshot
<img width="683" height="170" alt="Screenshot 2026-09-03 132645" src="https://github.com/user-attachments/assets/94bbb3b8-60d4-40dc-acae-f57b3b091f43" />
Figure: Successful CI/CD pipeline validation showing all checks passed

🎥 Video Presentation
Part 1 - System Planning and Database
YouTube Link:

