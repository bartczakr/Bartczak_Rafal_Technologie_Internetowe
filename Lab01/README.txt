PROJEKT: Wypożyczalnia książek 
TECHNOLOGIA: .NET 8 (C#) + Minimal API + SQLite + Entity Framework Core
AUTOR: Rafał Bartczak

--- JAK URUCHOMIĆ PROJEKT ---

WYMAGANIA:
- Visual Studio 2022 (lub nowsze) z obsługą .NET 8
- LUB zainstalowane SDK .NET 8.0

INSTRUKCJA (VISUAL STUDIO):
1. Otwórz plik rozwiązania .sln lub folder projektu w Visual Studio
2. Poczekaj chwilę, aż VS pobierze brakujące pakiety NuGet (Restore)
3. Kliknij zielony przycisk "Start" (lub F5)
4. Przeglądarka otworzy się automatycznie na stronie głównej aplikacji

INSTRUKCJA (KONSOLA):
1. Wejdź do folderu projektu.
2. Wpisz komendę: dotnet run
3. Otwórz w przeglądarce adres podany w konsoli (np. http://localhost:5000)

--- BAZA DANYCH ---

1. Aplikacja korzysta z lekkiej bazy SQLite (plik "library.db")
2. Baza tworzy się AUTOMATYCZNIE przy pierwszym uruchomieniu (Code-First)
3. Dane testowe (Seed) również ładują się automatycznie, jeśli baza jest pusta

RESETOWANIE BAZY:
Aby przywrócić stan początkowy (zresetować wypożyczenia i użytkowników):
1. Zatrzymaj aplikację
2. Usuń plik "library.db" z folderu projektu
3. Uruchom aplikację ponownie - plik i dane zostaną odtworzone