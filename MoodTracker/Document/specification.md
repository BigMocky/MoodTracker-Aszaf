# Előzetes specifikáció – MoodTracker CLI

## Cél
Parancssoros hangulatnapló, ahol a felhasználó naponta rögzíti a hangulatát (1–5) és egy rövid megjegyzést. A program megjeleníti az előzményeket, statisztikát számol, és motiváló üzenetet ad.

## Adatforrások (kötelező)
- **Külső fájl (CSV):** `Document/mooddata.csv`  
  Formátum: `entry_date;mood_level;note`
- **MySQL adatbázis:** `moodtracker` adatbázis, `mood_entries` tábla.

## Osztályok
- `MoodTracker.Model.Mood`
  - `EntryDate`, `MoodLevel`, `Note`, plusz DB-hez: `Id`, `CreatedAt`
  - 2+ paraméteres konstruktorok
  - `ToString()` felüldefiniálva
- `MoodTracker.Model.Database`
  - `CheckConnection()`
  - `SelectMoods()`
  - `InsertMood(Mood mood)`
  - `DeleteMood(long id)`

## Adatszerkezetek
- `List<Mood>` – a program memóriában tárolja és kezeli az adatokat
- `Dictionary<int,int>` – statisztikai eloszlás (1–5 gyakoriság)
- (DB-ből beolvasás a `SelectMoods()`-ban, a fő program listába merge-öl)

## Kötelező programozási elemek
- `while` ciklus (főmenü)
- `switch-case` (menü választás)
- `for` és `foreach` ciklusok
- `if` elágazások és **ternary** operátor (motiváló üzenet tónusa)
- Függvények paraméterekkel + `out` használat (`CalcSumAndAvg`)

## Funkciók
1. Új hangulat rögzítése (dátum, hangulat, megjegyzés) – **alapból ment CSV + DB**
2. Előzmények listázása (táblázatosan + emoji)
3. Statisztika (átlag, legjobb/legrosszabb, eloszlás)
4. Keresés a megjegyzésekben

## Megjegyzés
A cél egy 12.-es tanulói szintű, jól olvasható, egyszerű megoldás.
