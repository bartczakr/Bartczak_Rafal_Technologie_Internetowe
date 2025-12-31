PROJEKT: Kanban Board 
TECHNOLOGIA: .NET 8 (C#) + Minimal API + SQLite + Entity Framework Core
AUTOR: Rafał Bartczak

--- URUCHOMIENIE ---
1. Otwórz projekt w Visual Studio.
2. Kliknij Start (F5).
3. Baza danych "kanban.db" utworzy się automatycznie przy starcie, wypełniając się danymi testowymi (Seed) zgodnymi z zadaniem.

RESET: Aby wyczyścić tablicę, usuń plik "kanban.db" i zrestartuj aplikację.

--- FUNKCJONALNOŚĆ ---
1. Wyświetlanie tablicy: Dynamiczne pobieranie kolumn i zadań z bazy.
2. Dodawanie zadań: Zadania dodawane są na koniec listy w danej kolumnie.
3. Przenoszenie zadań: Za pomocą strzałek. Zadanie przenoszone jest na koniec listy w kolumnie docelowej.
4. Trwałość: Kolejność i przypisanie do kolumn jest zapisywane w bazie SQLite.