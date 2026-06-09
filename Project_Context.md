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
- Refresh token revoked server-side on logout (both cookies cleared)
- Secure password hashing (BCrypt)
- Rate limiting on `login` / `register` (brute-force protection)
- Self-registration restricted to **Client** tenants — role/ClientId from the request body are never trusted (no privilege escalation)
- Auth responses return DTOs only — `PasswordHash` and refresh tokens are never echoed back

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
- **DeviceConfiguration** access gated by location ownership ✅
- **Location / User creation scoped to the caller's tenant** (ClientId taken from the JWT claim, not the body) ✅
- Registration cannot self-assign privileged roles ✅
- Refresh token revoked server-side on logout ✅
- Rate limiting on auth endpoints (5 req/min per IP → HTTP 429) ✅
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

### Scoping (multi-tenant safe):

- The hub requires authentication (`[Authorize]`, JWT read from the cookie)
- On connect, each client auto-joins a `location-{id}` group for every location they have access to
- Device updates are broadcast **only to the owning location's group** — never to all connected clients across tenants

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
- POST /api/Auth/register  *(rate-limited; always creates a Client tenant)*
- POST /api/Auth/login      *(rate-limited)*
- POST /api/Auth/logout     *(revokes the refresh token)*
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
- Tailwind CSS (v4)

---

### Implemented:

- Login page with client-side validation + inline error messages
- Loading states and graceful error UI (no blocking `alert()` popups)
- Friendly handling of rate-limit (HTTP 429) responses on login
- Cookie-based auth
- Routing (react-router)
- Axios interceptor (401 → silent refresh → retry)
- Dashboard: location selector, rooms & devices, device toggle with optimistic update
- Empty/error states for locations and device data

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

✅ Rate limiting (fixed window, per IP):

5 requests / minute on the "auth" policy → HTTP 429
Applied to /api/Auth/login and /api/Auth/register

---

### Frontend

✅ Env variable:


VITE_API_BASE_URL=https://localhost:44305

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

- SignalR integration in the UI (live device updates via `location-{id}` groups — hub side is ready)
- Build out the `/devices` and `/settings` pages (currently placeholders)
- React error boundary + loading skeletons
- Behind a reverse proxy, add forwarded-headers support so rate limiting sees the real client IP
- Remove the unused `Room.UserId` field
- General UI/UX polish

---

## 💡 Notes for AI / Developers

- Do NOT use localStorage for tokens
- Always use cookies (HttpOnly)
- Access must be location-based
- Multi-tenant isolation is critical
- Refresh token rotation is already implemented
- Logging system is fully structured and production-ready
- `Jwt:Key` is stored in **dotnet user-secrets** (see `UserSecretsId` in the .csproj) — do NOT commit it to appsettings
- Public `register` only creates Client tenants; SuperAdmin/Member accounts are provisioned via the authenticated `Users` endpoint
- When adding endpoints, derive `ClientId` from the JWT claim — never trust it from the request body
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

