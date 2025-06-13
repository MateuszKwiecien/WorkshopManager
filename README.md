# WorkshopManager

**WorkshopManager** to prosta aplikacja webowa (ASP.NET Core 8 MVC + Entity Framework Core), która ułatwia zarządzanie serwisem samochodowym:

- baza klientów, pojazdów oraz części zamiennych
- zlecenia serwisowe z listą wykonanych czynności i użytych części
- role użytkowników: **Admin**, **Mechanik**, **Recepcjonista**
- raport kosztów (klient / pojazd / miesiąc) z eksportem do PDF
- logowanie zdarzeń i wyjątków za pomocą **NLog** (do `/logs/errors.log`)

---

## • Stos technologiczny

| Warstwa   | Technologia                                                        |
| --------- | ------------------------------------------------------------------ |
| Backend   | **.NET 8**, ASP.NET Core MVC, Entity Framework Core 9 (Code‑First) |
| UI        | Bootstrap 5, Razor Views, Tag Helpers                              |
| Mapowanie | Mapster                                                            |
| PDF       | QuestPDF *(Community License)*                                     |
| Logi      | NLog 5 + NLog.Web.AspNetCore                                       |
| Baza      | SQL Server Express / LocalDB                                       |

---

## • Wymagania wstępne

- .NET SDK 8.0
- SQL Server Express **lub** LocalDB (`sqllocaldb`) zainstalowany lokalnie
- NodeJS (tylko jeśli będziesz kompilować zasoby frontend)

---

## • Instalacja / uruchomienie

```bash
# 1. Klonuj repozytorium
> git clone <repo-url>
> cd WorkshopManager

# 2. Przy pierwszym uruchomieniu – instalacja paczek
> dotnet restore

# 3. Migracje i seed bazy (wykonywane automatycznie przy starcie)
#    – jeśli chcesz ręcznie:
> cd WorkshopManager
> dotnet ef database update

# 4. Uruchom
> dotnet run
```

Aplikacja domyślnie nasłuchuje pod adresem [https://localhost:5001](https://localhost:5001).

---

## • Role i konta testowe

| Rola              | Uprawnienia                                       | Login / Hasło                |
| ----------------- | ------------------------------------------------- | ---------------------------- |
| **Admin**         | pełny dostęp                                      | `admin@wm.pl` / `Password1!` |
| **Mechanik**      | podgląd / edycja zleceń, części i czynności       | `mech@wm.pl`  / `Password1!` |
| **Recepcjonista** | dodawanie klientów, pojazdów, przyjmowanie zleceń | `rec@wm.pl`   / `Password1!` |

Kontakty testowe są seedowane podczas startu aplikacji (`IdentitySeeder`).

---

## • Struktura katalogów

```
WorkshopManager/
├── Controllers/      – logika MVC
├── Data/             – DbContext, migracje EF Core
├── DTOs/             – modele transportowe
├── Interfaces/       – kontrakty usług
├── Mappers/          – konfiguracja Mapstera
├── Models/           – encje domenowe
├── Services/         – logika aplikacyjna (serwisy, raporty, seedery)
├── Views/            – Razor Views (+ partials)
├── wwwroot/          – zasoby statyczne (css, js)
└── logs/             – pliki logów NLog
```

---

## • Logowanie (NLog)

- Konfiguracja w `nlog.config`.
- Wszystkie wyjątki i zdarzenia poziomu **Error** trafiają do `logs/errors.log`.
- W kodzie dostęp przez wstrzyknięcie `ILogger<T>`;
  ```csharp
  public class PartsController : Controller {
      private readonly ILogger<PartsController> _log;
      public PartsController(ILogger<PartsController> log) => _log = log;
  }
  ```

---

## • Raporty PDF

- `ReportsController` generuje zestawienia kosztów na podstawie widoku `ReportService`.
- Eksport PDF realizowany przez **QuestPDF**.
- Plik do pobrania: `raport.pdf`.

---

## • Migracje EF Core

Migracje są stosowane automatycznie przez `MigrationService` przy starcie aplikacji.

Generowanie nowej migracji:

```bash
> dotnet ef migrations add <Nazwa>
```

---

## • Konfiguracja (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkshopDb;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}