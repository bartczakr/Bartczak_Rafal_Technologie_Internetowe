PROJEKT: Blog z systemem moderacji
TECHNOLOGIA: Node.js + Express + SQLite
AUTOR: Rafał Bartczak

--- OPIS PROJEKTU ---
Aplikacja blogowa umożliwiająca dodawanie postów i komentarzy.
Kluczową funkcjonalnością jest system moderacji: nowo dodane komentarze są domyślnie ukryte (status approved=0) i wymagają zatwierdzenia przez administratora w panelu moderacji.

--- WYMAGANIA ---
- Środowisko Node.js (wersja 18 lub nowsza)

--- INSTRUKCJA URUCHOMIENIA ---

1. INSTALACJA ZALEŻNOŚCI
   Otwórz terminal w folderze projektu i wpisz:
   npm install

2. URUCHOMIENIE SERWERA
   Wpisz komendę:
   npm run dev
   
   (Komenda ta uruchamia serwer w trybie "watch", który automatycznie restartuje aplikację po wprowadzeniu zmian w plikach).

3. PRZEGLĄDARKA
   Po uruchomieniu serwera wejdź na adres:
   http://localhost:3000

--- BAZA DANYCH ---
- Projekt wykorzystuje bazę SQLite (plik "blog.db").
- Baza oraz tabele tworzone są automatycznie przy pierwszym uruchomieniu (plik "server.js" zawiera skrypt inicjalizujący).
- Aby zresetować bazę (usunąć wszystkie dane), należy użyć komendy:
  npm run reset:db
  (lub ręcznie usunąć plik "blog.db").

--- STRUKTURA PROJEKTU ---
- /server.js       - Główny plik serwera (konfiguracja Express, połączenie z SQLite, endpointy API).
- /public          - Pliki frontendowe (HTML, CSS, JS) serwowane statycznie.
- /package.json    - Konfiguracja projektu i skrypty startowe.