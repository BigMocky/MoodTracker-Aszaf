using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MoodTracker.Model
{
    internal class Database
    {
        // ⚠️ Írd át a saját adataidra!
        private readonly string _connStr =
            "Server=localhost;Database=moodtracker;Uid=root;Pwd=;SslMode=None;";

        public bool CheckConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Mood> SelectMoods()
        {
            List<Mood> list = new List<Mood>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    string sql = @"SELECT id, entry_date, mood_level, note, created_at
                                   FROM mood_entries
                                   ORDER BY entry_date ASC, id ASC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            Mood m = new Mood(
                                r.GetDateTime("entry_date"),
                                r.GetInt32("mood_level"),
                                r.IsDBNull(r.GetOrdinal("note")) ? "" : r.GetString("note")
                            );

                            m.Id = r.GetInt64("id");
                            m.CreatedAt = r.GetDateTime("created_at");
                            list.Add(m);
                        }
                    }
                }
            }
            catch
            {
                // ha gond van, üres listával visszatérünk (tanulói szint)
            }

            return list;
        }

        public bool InsertMood(Mood mood)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    string sql = @"INSERT INTO mood_entries(entry_date, mood_level, note)
                                   VALUES (@d, @m, @n)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@d", mood.EntryDate.Date);
                        cmd.Parameters.AddWithValue("@m", mood.MoodLevel);
                        cmd.Parameters.AddWithValue("@n", mood.Note ?? "");
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteMood(long id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    string sql = @"DELETE FROM mood_entries WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
