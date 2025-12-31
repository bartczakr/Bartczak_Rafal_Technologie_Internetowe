PROJEKT: Notatki z Tagowaniem 
TECHNOLOGIA: .NET 8 (C#) + SQLite + Entity Framework Core
AUTOR: Rafał Bartczak

--- URUCHOMIENIE ---
1. Otwórz projekt w Visual Studio.
2. Uruchom (F5). Baza "notes.db" utworzy się automatycznie.

--- FUNKCJONALNOŚĆ ---
1. Wyszukiwanie (Full Text Like):
   - Dynamiczne filtrowanie notatek po wpisaniu frazy w pole wyszukiwania.
   - Wyszukiwanie obejmuje Tytuł oraz Treść notatki.
   - BONUS: Wyszukana fraza jest podświetlana na żółto w wynikach (frontend).

2. Tagowanie (Relacja Wiele-do-Wielu):
   - Podczas dodawania notatki można wpisać listę tagów po przecinku.
   - System inteligentnie sprawdza tagi: jeśli tag już istnieje w bazie, podpina go. Jeśli nie istnieje - tworzy nowy.
   - Jeden tag może być przypisany do wielu notatek, a notatka może mieć wiele tagów.

3. Model Danych:
   - Zgodny z wymaganiami (Notes, Tags, tabela łącząca NoteTags).
   - Tabela NoteTags jest obsługiwana automatycznie przez EF Core.