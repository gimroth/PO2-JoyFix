# 📘 JoyFix – System Zarządzania Serwisem Konsol

## Spis treści
- [Wprowadzenie](#wprowadzenie)
- [Opis funkcjonalności](#opis-funkcjonalności)
- [Architektura systemu](#architektura-systemu)
- [Modele danych](#modele-danych)
- [Konfiguracja bazy danych](#konfiguracja-bazy-danych)
- [Logika biznesowa](#logika-biznesowa)
- [Interfejs użytkownika (Blazor)](#interfejs-użytkownika-blazor)
- [Instrukcja obsługi](#instrukcja-obsługi)
- [Wymagania systemowe](#wymagania-systemowe)
- [Konfiguracja i uruchomienie](#konfiguracja-i-uruchomienie)

---

## Wprowadzenie
**JoyFix** to aplikacja webowa wspierająca zarządzanie serwisem naprawy konsol i urządzeń elektronicznych. Umożliwia obsługę klientów, prowadzenie zgłoszeń serwisowych, przypisywanie techników i monitorowanie przebiegu napraw.

> Technologia: **ASP.NET Core 8 (Blazor Server)** + **PostgreSQL**

---

## Opis funkcjonalności
- Dodawanie i edycja klientów
- Rejestrowanie urządzeń
- Tworzenie i zarządzanie zgłoszeniami napraw
- Przypisywanie techników według specjalizacji
- Ewidencjonowanie napraw (koszty, części, opis pracy)
- Walidacja danych (frontend + backend)
- Interfejs w oparciu o Bootstrap: tabele, formularze, modale

---

## Architektura systemu
- **Frontend**: Blazor Server
- **Backend**: ASP.NET Core (.NET 8)
- **ORM**: Entity Framework Core
- **Baza danych**: PostgreSQL
- **UI**: Bootstrap

---

## Modele danych

**Customer**
```csharp
using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public ICollection<Device> Devices { get; set; } = new HashSet<Device>();
        public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    }
}
```
**Device**
```csharp
using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Device
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string DeviceType { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }

        public Customer Customer { get; set; }
        public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    }
}
```
**RepairRequest**
```csharp
using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class RepairRequest
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public int DeviceId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string DeviceType { get; set; }

        [Required]
        public required string IssueDescription { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "oczekuje";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Repair? Repair { get; set; }

        public Customer Customer { get; set; } = null!;
        public Device Device { get; set; } = null!;
    }
}
```
**Repair**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoyFix.Data
{
    public class Repair
    {
        [Key]
        [ForeignKey("RepairRequest")] 
        public int RepairRequestId { get; set; } 

        public int TechnicianId { get; set; }

        [Required]
        public required string WorkDescription { get; set; }

        public string? PartsUsed { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Cost { get; set; }

        public DateTime RepairDate { get; set; } = DateTime.UtcNow;
        public RepairRequest RepairRequest { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
    }
}
```
**Specialization**
```csharp
namespace JoyFix.Data
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<TechnicianSpecialization> Technicians { get; set; } = new HashSet<TechnicianSpecialization>();
    }

    public class TechnicianSpecialization
    {
        public int TechnicianId { get; set; }
        public int SpecializationId { get; set; }
        public DateTime AcquiredDate { get; set; }

        public Technician Technician { get; set; }
        public Specialization Specialization { get; set; }
    }
}
```
**Technician**
```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Technician
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public ICollection<Repair> Repairs { get; set; } = new HashSet<Repair>();
        public ICollection<TechnicianSpecialization> Specializations { get; set; } = new HashSet<TechnicianSpecialization>();
    }
}
```

---

## Konfiguracja bazy danych
**ContextDB**

ContextDB to główny kontekst bazy danych oparty na Entity Framework Core. Odpowiada za konfigurację encji, relacji pomiędzy nimi oraz ustawienie ograniczeń i indeksów.
```csharp
using Microsoft.EntityFrameworkCore;

namespace JoyFix.Data
{
public class ContextDB : DbContext
{
public ContextDB(DbContextOptions<ContextDB> options) : base(options)
{
}
public DbSet<Customer> Customers { get; set; }
public DbSet<Device> Devices { get; set; }
public DbSet<RepairRequest> RepairRequests { get; set; }
public DbSet<Repair> Repairs { get; set; }
public DbSet<Technician> Technicians { get; set; }
public DbSet<Specialization> Specializations { get; set; }
public DbSet<TechnicianSpecialization> TechnicianSpecializations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Devices)
                .WithOne(d => d.Customer)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.RepairRequests)
                .WithOne(rr => rr.Customer)
                .HasForeignKey(rr => rr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>()
                .HasMany(d => d.RepairRequests)
                .WithOne(rr => rr.Device)
                .HasForeignKey(rr => rr.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RepairRequest>()
                .HasOne(rr => rr.Repair)
                .WithOne(r => r.RepairRequest)
                .HasForeignKey<Repair>(r => r.RepairRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Repair>()
                .HasOne(r => r.Technician)
                .WithMany(t => t.Repairs)
                .HasForeignKey(r => r.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasKey(ts => new { ts.TechnicianId, ts.SpecializationId });

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasOne(ts => ts.Technician)
                .WithMany(t => t.Specializations)
                .HasForeignKey(ts => ts.TechnicianId);

            modelBuilder.Entity<TechnicianSpecialization>()
                .HasOne(ts => ts.Specialization)
                .WithMany(s => s.Technicians)
                .HasForeignKey(ts => ts.SpecializationId);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<RepairRequest>()
                .HasIndex(rr => rr.Status);

            modelBuilder.Entity<Technician>()
                .HasIndex(t => t.Email)
                .IsUnique();

        }
    }
}
```
**Customer** posiada wiele Devices i RepairRequests (cascade delete).

**Device** powiązany jest z wieloma RepairRequests.

**RepairRequest** ma dokładnie jeden Repair (relacja jeden-do-jednego).

**Repair** przypisany jest do jednego Technician (bez kasowania kaskadowego).

**Technician** może mieć wiele Specializations poprzez tabelę pośredniczącą TechnicianSpecialization (relacja wiele-do-wielu).

Dodano unikalne indeksy dla pól: Email (Customer i Technician) oraz SerialNumber (Device).

Indeks utworzono również dla pola Status w RepairRequest.

**Wartości domyślne:** Status zgłoszenia naprawy ustawiany domyślnie na oczekuje.

Pola wymagane i ograniczenia walidacyjne definiowane przy pomocy adnotacji (DataAnnotations) w modelach.

**Seedy danych**

System zawiera zdefiniowane seedy startowe, które pozwalają szybko zainicjalizować testowe środowisko napraw konsolowych.

**Klasa:** ```DatabaseSeeder```

Centralna klasa agregująca wywołanie wszystkich seederów:
```csharp
using JoyFix.Data;
using JoyFix.Data.Seeders;

namespace JoyFix.Seeders
{
public static class DatabaseSeeder
{
public static void Seed(ContextDB context)
{
CustomerSeeder.Seed(context);
DeviceSeeder.Seed(context);
TechnicianSeeder.Seed(context);
RepairRequestSeeder.Seed(context);
RepairSeeder.Seed(context);
}
}
}
```
**Zakres seedowanych danych:**

**CustomerSeeder** – Dodaje 3 przykładowych klientów z danymi kontaktowymi.

**DeviceSeeder** – Tworzy 6 urządzeń przypisanych do klientów (różne typy konsol Nintendo, PlayStation, Xbox).

**TechnicianSeeder** – Trzech techników z przypisanymi specjalizacjami odpowiadającymi markom i typom usterek.

**RepairRequestSeeder** – Generuje zgłoszenia serwisowe w zależności od typu urządzenia i przypisuje je do odpowiednich klientów.

**RepairSeeder** – Tworzy rekordy napraw powiązane z istniejącymi zgłoszeniami oraz przypisuje je technikom na podstawie dopasowania specjalizacji.

---

## Logika Biznesowa

**Ogólna struktura**

- Każda klasa Service korzysta z DynamicDbContextFactory, co pozwala dynamicznie zarządzać wieloma kontekstami baz danych — dobry wybór przy np. wielu tenantach.

- Każdy Service implementuje metody CRUD, walidacje oraz relacje encji przez Include() — co poprawia czytelność i obsługę złożonych struktur danych.

**CustomerService**

- Pobiera klientów wraz z ich urządzeniami i zgłoszeniami (Devices, RepairRequests).

- Waliduje unikalność e-maila i numeru telefonu (tylko cyfry) przy tworzeniu/edycji.

-  tu bardziej zaawansowanej walidacji numeru telefonu (np. długość, format międzynarodowy itp.) — możesz to rozbudować w przyszłości.

**DeviceService**

- Sprawdza unikalność numeru seryjnego.

- Chroni przed usunięciem urządzenia, które ma powiązane zgłoszenia napraw.

- Działa dobrze w kontekście powiązań Device ↔ Customer i RepairRequest.

**RepairRequestService**

- Obsługuje zgłoszenia napraw, przypisane do Device, Customer, opcjonalnie Repair.

- Walidacja istnienia urządzenia i klienta.

- Blokuje usunięcie zgłoszenia, jeśli ma już przypisaną naprawę (Repair).

- Używa statusów: "oczekuje", "w trakcie" — można dodać enum dla lepszej kontroli stanu.
 
**RepairService**

- Zarządza naprawami i automatycznie aktualizuje status RepairRequest przy tworzeniu/edycji/usunięciu naprawy.

- Dobry mechanizm aktualizacji statusu: "oczekuje" ↔ "w trakcie".

- Waliduje istnienie technika i zgłoszenia przed dodaniem naprawy.

- Unika duplikowania naprawy dla jednego zgłoszenia.

**SpecializationService**

- Zabezpiecza przed usunięciem specjalizacji przypisanej do techników.

- Dobrze zorganizowana logika walidacji unikalności nazw.

- Obsługuje relację Specialization ↔ TechnicianSpecialization.

**TechnicianService**

- Obsługuje techników, ich specjalizacje oraz przypisane naprawy.

- Chroni przed usunięciem technika, który ma przypisane naprawy.

- Umożliwia przypisywanie nowych specjalizacji do techników — bardzo dobrze!

- Również waliduje unikalność e-maila i format numeru telefonu.

---

## Interfejs użytkownika (Blazor)

Aplikacja podzielona jest na oddzielne strony z podstronami

**Customers**

```/customers ``` - lista klientów

```/customeradd ``` - dodawanie klientów

```/customeredit ``` - edytowanie klientów

**Devices**

```/devices ``` - lista urządzeń

```/deviceadd ``` - dodawanie urządzeń

```/deviceedit ``` - edytowanie urządzeń

**RepairRequests**

```/repairrequests ``` - lista zgłoszeń napraw

```/repairrequestadd ``` - dodawanie zgłoszeń napraw
 
```/repairrequestedit ``` - edytowanie zgłoszeń napraw

**Repairs**

```/repairs ``` - lista napraw

```/repairadd ``` - dodawanie napraw

```/repairedit ``` - edytowanie napraw

**Specializations**

```/specializations ``` - lista specjalizacji

```/specializationadd ``` - dodawanie specjalizacji

```/specializationedit ``` - edytowanie specjalizacji

**Technicians**

```/technicians ``` - lista techników

```/technicianadd ``` - dodawanie techników

```/technicianedit ``` - edytowanie techników

Każda ze stron pozwala na:

- Wyświetlanie list klientów, urządzeń, techników, napraw itp.

- Filtrowanie listy (wyszukiwanie po nazwie, emailu, typie urządzenia itd.).

- Dodawanie, edycję i usuwanie.

- Potwierdzanie usunięcia przez użytkownika.

--- 

## Instrukcja obsługi

Każdy komponent listowy (np. lista klientów, produktów, usług) umożliwia użytkownikowi:

- przeglądanie danych w formie tabeli,

- filtrowanie i wyszukiwanie rekordów po wybranych kryteriach,

- dodawanie nowych rekordów,

- edycję istniejących,

 - usuwanie niepotrzebnych danych.

**Filtrowanie**

Na górze strony znajduje się formularz filtrowania, który umożliwia przeszukiwanie danych po różnych polach (np. imię, email, telefon, adres). Użytkownik może:

- wpisać dane w dowolne pole (np. fragment imienia lub numer telefonu),

- kliknąć przycisk „Search”, aby zastosować filtry,

- kliknąć „Clear all”, aby wyczyścić pola i zobaczyć wszystkie rekordy ponownie.

Filtry działają niezależnie od siebie – można użyć jednego lub kilku jednocześnie.

**Tabela danych**

- Lista wszystkich pasujących rekordów wyświetlana jest w formie tabeli.

- Kolumny prezentują kluczowe dane (np. imię, email, adres).

- Dla każdego wiersza dostępne są przyciski:

  - Edytuj – umożliwia modyfikację rekordu,

  - Usuń – wywołuje potwierdzenie usunięcia.

**Dodawanie nowego rekordu**

- Przycisk „Add new” nad tabelą przenosi do formularza tworzenia.

- Formularz zawiera wymagane pola i przycisk „Zapisz”.

**Edycja istniejącego rekordu**

- Kliknięcie „Edit” przy wybranym wierszu otwiera formularz z danymi do edycji.

- Po zapisaniu zmian użytkownik wraca do listy.

**Usuwanie rekordu**

- Kliknięcie „Delete” wyświetla pytanie o potwierdzenie.

- Po zatwierdzeniu rekord zostaje usunięty z bazy, a lista odświeżona.

**Dodatkowe informacje**

| Element            | Opis                                                                    |
|--------------------|-------------------------------------------------------------------------|
| Ładowanie danych   | Komunikat „Loading...” pojawia się, gdy dane są pobierane.              |
| Obsługa błędów     | W przypadku problemów z API – pojawia się komunikat o błędzie.          |
| Brak wyników       | Jeśli żaden rekord nie spełnia kryteriów filtrowania.                   |
| Potwierdzenia      | Po edycji, dodaniu lub usunięciu użytkownik widzi zaktualizowaną listę. |

---

## Wymagania systemowe

- .NET 8 lub nowszy

- PostgreSQL (serwer lokalny lub zdalny)

- Przeglądarka obsługująca Blazor Server (Chrome, Edge, Firefox)

- System operacyjny: Windows / Linux / macOS

--- 

## Konfiguracja i uruchomienie

Aby uruchomić aplikację lokalnie, wykonaj poniższe kroki:

**Wymagania wstępne**

Upewnij się, że masz zainstalowane:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- IDE, np. [Visual Studio 2022+](https://visualstudio.microsoft.com/pl/) lub [Rider](https://www.jetbrains.com/rider/)
- Narzędzia Entity Framework (jeśli chcesz ręcznie wykonywać migracje):
  ```bash
  dotnet tool install --global dotnet-ef
  ```
**Konfiguracja**

1. Ustawienie łańcucha połączenia do bazy danych:
   - W pliku appsettings.json dodaj dane dostępowe do swojej instancji PostgreSQL.


2. Dodanie migracji i aktualizacja bazy danych (opcjonalnie):

   - Jeśli chcesz ręcznie zarządzać schematem bazy:
      ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
      ```

3. Automatyczna migracja i seeding:

   - Aplikacja automatycznie wykonuje migracje i wstępne wypełnienie bazy danych przy pierwszym uruchomieniu:


4. Uruchomienie aplikacji

   - Otwórz terminal w katalogu projektu i uruchom aplikację:
      ```bash
     dotnet run
      ```

   - Aplikacja będzie dostępna pod adresem:
     https://localhost:5001 (lub innym, zależnie od konfiguracji)

     
**Użyte rozszerzenia / biblioteki**

- PostgreSQL – baza danych

- Entity Framework Core (EF Core) – ORM (obsługa migracji, seeding)

- ASP.NET Core Razor Components (Blazor Server) – interfejs użytkownika

- ProtectedBrowserStorage – przechowywanie sesji po stronie klienta

- Bootstrap 5 – stylowanie (przy pomocy klas CSS)