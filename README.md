# Smart Home Platform

A scalable multi-tenant smart home management system built using **.NET 8** and **React (Vite + TypeScript)**.

---

## 🚀 Features

- Multi-tenant architecture (Client → Locations → Rooms → Devices)
- Role-based authentication (JWT + Refresh Tokens)
- Real-time device updates using SignalR
- Dynamic device configuration (Brightness, Temperature, etc.)
- Secure access control per user and location
- API-first design (mobile-ready)

---

## 🏗️ Architecture

Frontend (React)
        ↓
.NET 8 Web API
        ↓
Entity Framework Core
        ↓
SQL Server
        ↓
SignalR (Real-time)

---

## 👥 Roles

- SuperAdmin → Platform owner
- Client → Customer/home owner
- Member → Family/guests

---

## 📊 Data Flow

Client → Location → Room → Device → Configuration

---

## ⚙️ Tech Stack

- Backend: ASP.NET Core (.NET 8)
- Frontend: React + TypeScript (Vite)
- Database: SQL Server
- Realtime: SignalR
- Auth: JWT + Refresh Tokens

---

## 🚀 Setup

### Backend

```bash
cd SmartHomeAPI
dotnet run