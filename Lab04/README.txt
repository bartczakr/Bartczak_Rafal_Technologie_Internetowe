PROJEKT: Filmy i Oceny 
TECHNOLOGIA: Node.js + Express + SQLite
AUTOR: Rafał Bartczak

--- OPIS PROJEKTU ---
System rankingowy filmów. Użytkownicy mogą dodawać filmy oraz wystawiać im oceny (1-5).
Aplikacja automatycznie wylicza średnią ocen oraz liczbę głosów i sortuje filmy od najlepiej ocenianych.

--- INSTRUKCJA URUCHOMIENIA ---

1. INSTALACJA
   Wpisz w terminalu: npm install

2. START
   Wpisz w terminalu: npm run dev
   (Serwer uruchomi się na http://localhost:3000)

3. RESET DANYCH
   Aby usunąć bazę: npm run reset:db

--- FUNKCJONALNOŚĆ ---

1. Ranking (GET /api/movies):
   - Zwraca listę filmów zawierającą: Tytuł, Rok, Liczbę głosów (votes), Średnią (avg_score).
   - Lista jest domyślnie posortowana malejąco według średniej oceny.
   - Bonus: Obsługa filtrowania po roku (?year=2010) oraz limitu wyników (?limit=5).

2. Ocenianie (POST /api/ratings):
   - Walidacja po stronie serwera: Ocena musi być liczbą całkowitą z przedziału 1-5.
   - Po dodaniu oceny ranking aktualizuje się natychmiastowo.

3. Frontend:
   - Widok tabelaryczny z wizualizacją średniej (kolorowe odznaki).
   - Formularze dodawania filmów i ocen bez przeładowania strony (fetch API).