# Kursplattform – Mittuniversitetet

En webbaserad kursplattform byggd som skolprojekt. Studenter kan söka kurser, anmäla sig och hantera dokument. Administratörer hanterar kurser, anmälningar och dokumentförfrågningar.

## Tekniker

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core + SQLite
- ASP.NET Identity (autentisering och roller)
- Bootstrap 5.3 (med inbyggt dark mode)
- Razor Views

## Kör projektet lokalt

### Installation

```bash
git clone https://github.com/arlaspresident/projekt.net.git
cd courses
dotnet run --project Courses.Web
```

Öppna `https://localhost:5001` i webbläsaren.

Databasen skapas automatiskt vid första start och fylls med exempeldata (kurser från Mittuniversitetet).

## Testanvändare

| Roll    | E-post                        | Lösenord     |
|---------|-------------------------------|--------------|
| Admin   | admin@courses.se              | Test123!     |
| Student | student@courses.se            | Student123!  |

## Registrera eget konto

Roller tilldelas automatiskt baserat på e-postdomän:

| Domän                    | Roll    |
|--------------------------|---------|
| `@student.courses.se`    | Student |
| `@courses.se`            | Admin   |
| Övriga                   | Visitor |

## Funktioner

**Publik**
- Bläddra och sök bland kurser (filtrering på kategori, poäng, söksträng)

**Student**
- Anmäl dig till kurser
- Se status på anmälningar (väntar / godkänd)
- Ta emot dokumentförfrågningar från admin och ladda upp PDF-svar

**Admin**
- Hantera kurser, kategorier och lärare (CRUD)
- Se och godkänna studentanmälningar
- Skicka dokumentförfrågningar till studenter och ladda ned uppladdade filer

