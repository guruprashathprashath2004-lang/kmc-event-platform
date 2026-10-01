# KMC Event Platform

Kandy Municipal Council event platform built with ASP.NET Core.

- `KMC_API_Guruprashath` - Web API (JWT auth, EF Core, SQL Server LocalDB), runs on https://localhost:7080
- `KMC_Client_Guruprashath` - Razor Pages client that consumes the API

## Setup
1. Install Visual Studio with the ASP.NET workload and SQL Server LocalDB.
2. In `KMC_API_Guruprashath`, copy `appsettings.Development.example.json` to `appsettings.Development.json` and set `Jwt:Key` to a random string of 32+ characters.
3. Run the API first (https profile), then run the client.
4. The database is created and seeded automatically on first run.

## Demo accounts (seeded)
| Role | Username | Password |
|------|----------|----------|
| Admin | kmc.admin | Kmc@2026 |
| Organizer | organizer.demo | Organizer@2026 |
| Citizen | citizen.demo | Citizen@2026 |

These are demo accounts only. Do not reuse them anywhere real.
