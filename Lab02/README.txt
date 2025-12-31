PROJEKT: Sklep internetowy
TECHNOLOGIA: .NET 8 (C#) + Minimal API + SQLite + Entity Framework Core
AUTOR: Rafał Bartczak

--- JAK URUCHOMIĆ PROJEKT ---

WYMAGANIA:
- Visual Studio 2022 (lub nowsze) z obsługą .NET 8
- LUB zainstalowane SDK .NET 8.0

INSTRUKCJA (VISUAL STUDIO):
1. Otwórz plik rozwiązania .sln w Visual Studio
2. Upewnij się, że w menedżerze pakietów NuGet pobrane są zależności
3. Kliknij zielony przycisk "Start" (lub F5)
4. Przeglądarka otworzy się automatycznie na stronie głównej sklepu

INSTRUKCJA (KONSOLA):
1. Wejdź do folderu projektu
2. Wpisz komendę: dotnet run
3. Otwórz w przeglądarce adres podany w konsoli (np. http://localhost:5000)

--- BAZA DANYCH ---

1. Projekt wykorzystuje bazę SQLite (plik "shop.db").
2. Baza tworzy się AUTOMATYCZNIE przy uruchomieniu (podejście Code-First)
3. Aplikacja automatycznie wypełnia bazę produktami testowymi (Seed): Klawiatura, Mysz, Monitor (zgodnie z treścią zadania)

RESETOWANIE BAZY:
Aby wyczyścić zamówienia i koszyk:
1. Zatrzymaj aplikację
2. Usuń plik "shop.db" z folderu projektu
3. Uruchom ponownie – struktura i produkty zostaną odtworzone