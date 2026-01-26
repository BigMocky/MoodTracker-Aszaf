-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Jan 26. 13:23
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `moodtracker`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `mood_entries`
--

CREATE TABLE `mood_entries` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `entry_date` date NOT NULL,
  `mood_level` tinyint(3) UNSIGNED NOT NULL,
  `note` varchar(500) NOT NULL DEFAULT '',
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `mood_entries`
--

INSERT INTO `mood_entries` (`id`, `entry_date`, `mood_level`, `note`, `created_at`) VALUES
(1, '2026-01-23', 4, 'Egész jó nap volt 😊', '2026-01-26 12:20:54'),
(2, '2026-01-24', 2, 'Fáradt voltam...', '2026-01-26 12:20:54'),
(3, '2026-01-25', 5, 'Szuper nap! 😄', '2026-01-26 12:20:54');

--
-- Eseményindítók `mood_entries`
--
DELIMITER $$
CREATE TRIGGER `trg_mood_entries_bi` BEFORE INSERT ON `mood_entries` FOR EACH ROW BEGIN
  IF NEW.mood_level < 1 OR NEW.mood_level > 5 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Mood level must be between 1 and 5';
  END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_mood_entries_bu` BEFORE UPDATE ON `mood_entries` FOR EACH ROW BEGIN
  IF NEW.mood_level < 1 OR NEW.mood_level > 5 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'Mood level must be between 1 and 5';
  END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- A nézet helyettes szerkezete `v_mood_daily_avg`
-- (Lásd alább az aktuális nézetet)
--
CREATE TABLE `v_mood_daily_avg` (
`entry_date` date
,`daily_avg` decimal(6,2)
,`entries_that_day` bigint(21)
);

-- --------------------------------------------------------

--
-- A nézet helyettes szerkezete `v_mood_history`
-- (Lásd alább az aktuális nézetet)
--
CREATE TABLE `v_mood_history` (
`id` bigint(20) unsigned
,`entry_date` date
,`mood_level` tinyint(3) unsigned
,`note` varchar(500)
,`created_at` timestamp
);

-- --------------------------------------------------------

--
-- A nézet helyettes szerkezete `v_mood_stats`
-- (Lásd alább az aktuális nézetet)
--
CREATE TABLE `v_mood_stats` (
`total_entries` bigint(21)
,`avg_mood` decimal(6,2)
,`min_mood` tinyint(3) unsigned
,`max_mood` tinyint(3) unsigned
);

-- --------------------------------------------------------

--
-- Nézet szerkezete `v_mood_daily_avg`
--
DROP TABLE IF EXISTS `v_mood_daily_avg`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_mood_daily_avg`  AS SELECT `mood_entries`.`entry_date` AS `entry_date`, round(avg(`mood_entries`.`mood_level`),2) AS `daily_avg`, count(0) AS `entries_that_day` FROM `mood_entries` GROUP BY `mood_entries`.`entry_date` ;

-- --------------------------------------------------------

--
-- Nézet szerkezete `v_mood_history`
--
DROP TABLE IF EXISTS `v_mood_history`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_mood_history`  AS SELECT `mood_entries`.`id` AS `id`, `mood_entries`.`entry_date` AS `entry_date`, `mood_entries`.`mood_level` AS `mood_level`, `mood_entries`.`note` AS `note`, `mood_entries`.`created_at` AS `created_at` FROM `mood_entries` ;

-- --------------------------------------------------------

--
-- Nézet szerkezete `v_mood_stats`
--
DROP TABLE IF EXISTS `v_mood_stats`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_mood_stats`  AS SELECT count(0) AS `total_entries`, round(avg(`mood_entries`.`mood_level`),2) AS `avg_mood`, min(`mood_entries`.`mood_level`) AS `min_mood`, max(`mood_entries`.`mood_level`) AS `max_mood` FROM `mood_entries` ;

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `mood_entries`
--
ALTER TABLE `mood_entries`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ix_mood_entries_entry_date` (`entry_date`),
  ADD KEY `ix_mood_entries_created_at` (`created_at`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `mood_entries`
--
ALTER TABLE `mood_entries`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
