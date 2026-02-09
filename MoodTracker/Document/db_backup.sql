-- MoodTracker DB backup (egyszerű)
CREATE DATABASE IF NOT EXISTS moodtracker CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE moodtracker;

CREATE TABLE IF NOT EXISTS mood_entries (
  id BIGINT NOT NULL AUTO_INCREMENT,
  entry_date DATE NOT NULL,
  mood_level INT NOT NULL,
  note VARCHAR(255) NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY ix_mood_entries_date (entry_date),
  KEY ix_mood_entries_mood (mood_level)
);

-- Példa nézetek (ha szeretnéd a képen látható struktúrát)
CREATE OR REPLACE VIEW v_mood_history AS
SELECT entry_date, mood_level, note, created_at FROM mood_entries ORDER BY entry_date;

CREATE OR REPLACE VIEW v_mood_daily_avg AS
SELECT entry_date, AVG(mood_level) AS avg_mood
FROM mood_entries
GROUP BY entry_date
ORDER BY entry_date;

CREATE OR REPLACE VIEW v_mood_stats AS
SELECT COUNT(*) AS total_entries, AVG(mood_level) AS overall_avg
FROM mood_entries;
