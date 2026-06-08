# 🧠 Smart Home Platform - Project Context

## 📌 Overview

This is a **multi-tenant smart home management platform** built using:

- **Backend:** ASP.NET Core (.NET 8)
- **Frontend:** React (Vite + TypeScript)
- **Database:** SQL Server (via EF Core)
- **Realtime:** SignalR
- **Logging:** Serilog (File + Console)
- **Authentication:** JWT + HttpOnly Cookies + Refresh Tokens

---

## 🏗️ Architecture

Frontend (React)
        ↓
ASP.NET Core API (.NET 8)
        ↓
Entity Framework Core
        ↓
SQL Server
        ↓
SignalR (Real-time events) + Serilog

---

## 👥 Roles & Multi-Tenant Design

### Roles:
- **SuperAdmin** → platform owner
- **Client** → customer / home owner
- **Member** → user under a client (family, guest)

---

### Tenant Hierarchy:

Client
├── Users
├── Locations (Homes)
	├── Rooms
		├── Devices
			├── DeviceConfigurations

---

## 🗄️ Core Entities

### User
- Id
- Email
- PasswordHash (BCrypt)
- Role
- ClientId
- RefreshTokens

---

### Client
- Id
- Name

---

### Location
- Id
- Name
- ClientId

---

### UserLocationAccess
- Id
- UserId
- LocationId

➡️ Enables multi-location access

---

### Room
- Id
- Name
- LocationId

---

### Device
- Id
- Name
- DeviceTypeId
- RoomId
- IsOn

---

### DeviceConfiguration
- Id
- DeviceId
- Key
- Value

---

### RefreshToken
- Id
- Token
- Expires
- IsRevoked
- UserId

---

## 🔐 Authentication System

### ✅ Implemented

- JWT generation
- HttpOnly cookie storage
- Refresh token rotation
- Secure password hashing (BCrypt)

---

### ✅ Auth Flow


Login →
backend generates JWT + refreshToken → stored in HttpOnly cookies  
→ Frontend sends requests with cookies automatically 
→ Backend reads JWT from cookie and authenticates

---

### ✅ Refresh Flow (Silent Login)


Page reload →
frontend calls /api/Auth/refresh →
backend validates refreshToken →
→ new JWT cookie issued ✅
→ new refresh token issued ✅
---

### ✅ Axios Interceptor

- Handles 401 errors automatically
- Calls `/refresh`
- Retries failed request
- Redirects to login if refresh fails
---

## 🔒 Security Model

- No tokens in localStorage ❌
- HttpOnly cookies only ✅
- CORS configured with credentials ✅
- Role-based authorization ✅
- Location-based access control ✅
- Device access linked to location ownership ✅
- Multi-tenant isolation ✅
- No client-side token storage ✅

---

## 🔁 Real-Time System

Using **SignalR**

### Hub:

/deviceHub

### Event:

ReceiveDeviceUpdate

Triggered when:
- Device state changes (toggle)

---
## 📊 Logging System (NEW ✅)

### ✅ Serilog Setup

- Console logging
- File logging (`logs/log-*.txt`)
- Daily rolling logs

---

### ✅ Log Context Includes

- CorrelationId ✅
- UserId ✅
- Route ✅
- HTTP Method ✅
- Status Code ✅
- Duration ✅

---

### ✅ Request Logging Flow


Incoming request →
CorrelationId generated →
UserId extracted from JWT →
Route + Method captured →
Request timed →
Status + duration logged

---

### ✅ Sample Log


[INF] [CorrId: abc123] [UserId: 2] [Route: /api/Devices]
[Method: POST] Request completed with Status 200 in 45ms

---

## 🗄️ Database Tables

- Users
- Clients
- Locations
- Rooms
- Devices
- DeviceConfigurations
- UserLocationAccess
- RefreshTokens

---
## 🌐 API Endpoints

### Auth
- POST /api/Auth/register
- POST /api/Auth/login
- POST /api/Auth/logout
- POST /api/Auth/refresh

---

### Clients
- POST /api/Clients
- GET /api/Clients

---

### Users
- POST /api/Users
- GET /api/Users

---

### Locations
- POST /api/Locations
- GET /api/Locations
- GET /api/Locations/full

---

### Rooms
- POST /api/Rooms
- GET /api/Rooms?locationId=

---

### Devices
- POST /api/Devices
- GET /api/Devices
- GET /api/Devices/by-room
- POST /api/Devices/{id}/toggle

---

### Device Config
- POST /api/DeviceConfigurations
- PUT /api/DeviceConfigurations
- GET /api/DeviceConfigurations/{deviceId}

---

### Access Control
- POST /api/UserLocationAccess
- GET /api/UserLocationAccess

---

## ⚛️ Frontend (React)

### Tech:
- React + TypeScript
- Vite
- Axios

---

### Implemented:

- Login page
- API integration
- Cookie-based auth
- Routing (react-router)
- Axios interceptor
- API integration
- Dashboard basic setup

---

### Axios Setup:


axios.defaults.withCredentials = true;

---

## ⚙️ Important Configurations

### Backend

✅ CORS:

.AllowCredentials()
.WithOrigins("http://localhost:5173")

✅ JWT from cookies:

context.Request.Cookies["jwt"]

---

### Frontend

✅ Env variable:


VITE_API_BASE_URL=http://localhost:5230

---

## 🚀 Current Status

✅ Backend:
- Production-grade authentication
- Refresh token rotation
- Fully functional
- Secure authentication
- Real-time system working
- Multi-tenant enforced
- Full observability (logging + tracing)

✅ Frontend:
- Login implemented
- Cookie auth integrated
- Routing working
- Axios interceptor implemented

---

## 🧭 Next Steps

- Dashboard UI (locations, rooms, devices)
- Device control components
- SignalR integration in UI
- UI/UX improvements

---

## 💡 Notes for AI / Developers

- Do NOT use localStorage for tokens
- Always use cookies (HttpOnly)
- Access must be location-based
- Multi-tenant isolation is critical
- Refresh token rotation is already implemented
- Logging system is fully structured and production-ready
---

## 🔗 Repository

https://github.com/Sagar-Gupta-01/smart-home-platform

---

## ✅ Summary

This project is a **production-ready full-stack smart home system** with:

✅ Multi-tenant architecture  
✅ Secure cookie-based authentication  
✅ Refresh token lifecycle  
✅ Axios auto-retry system  
✅ Real-time updates (SignalR)  
✅ Enterprise-grade logging & tracing  

→ This is a **production-ready backend system**

