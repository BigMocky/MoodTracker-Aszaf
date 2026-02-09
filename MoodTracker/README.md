# MoodTracker (C# Console)

Egyszerű hangulatnapló parancssorban, CSV + MySQL adattárolással.

## Futtatás

1. Telepítsd a NuGet csomagot:
- `MySql.Data`

2. Állítsd be a kapcsolatot:
- `Model/Database.cs` fájlban a `_connStr` értékét írd át.

3. Indítás:
- `dotnet run`

## Adatforrások

- Külső fájl: `Document/mooddata.csv`
- MySQL tábla: `moodtracker.mood_entries`

## Menüpontok
- 1: új bejegyzés (alapból ment: CSV + ha van DB kapcsolat, DB)
- 2: előzmények táblázatosan + napi szűrés
- 3: statisztika (átlag, legjobb/legrosszabb nap, eloszlás)
- 4: keresés megjegyzésben
