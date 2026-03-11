using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using Newtonsoft.Json;
using System;

namespace MAB_Mailer
{
    public class DatabaseService
    {
        private string _dbPath = "MAB_Data.db";
        private string _connectionString;

        public DatabaseService()
        {
            SQLitePCL.Batteries_V2.Init();
            _connectionString = $"Data Source={_dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath)) { using (File.Create(_dbPath)) { } }

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();

                string sqlRecipients = @"CREATE TABLE IF NOT EXISTS Recipients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                Ad TEXT, Soyad TEXT, Email TEXT, GroupName TEXT, ExtraDataJson TEXT)";
                using (var cmd = new SqliteCommand(sqlRecipients, conn)) { cmd.ExecuteNonQuery(); }

                string sqlSettings = @"CREATE TABLE IF NOT EXISTS AppSettings (
                Key TEXT PRIMARY KEY, Value TEXT)";
                using (var cmd = new SqliteCommand(sqlSettings, conn)) { cmd.ExecuteNonQuery(); }

                string sqlGroups = @"CREATE TABLE IF NOT EXISTS RecipientGroups (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                GroupName TEXT, MemberEmailsJson TEXT)";
                using (var cmd = new SqliteCommand(sqlGroups, conn)) { cmd.ExecuteNonQuery(); }

                string sqlAccounts = @"CREATE TABLE IF NOT EXISTS EmailAccounts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                DisplayName TEXT, 
                Email TEXT, 
                Password TEXT, 
                SmtpHost TEXT, 
                SmtpPort INTEGER, 
                EnableSsl INTEGER)";
                using (var cmd = new SqliteCommand(sqlAccounts, conn)) { cmd.ExecuteNonQuery(); }

                string sqlTemplates = @"CREATE TABLE IF NOT EXISTS MessageTemplates (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                Title TEXT, 
                Subject TEXT, 
                Body TEXT)";
                using (var cmd = new SqliteCommand(sqlTemplates, conn)) { cmd.ExecuteNonQuery(); }
            }
        }

        public void ImportRecipient(Recipient r)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string jsonData = JsonConvert.SerializeObject(r.DataFields);

                string checkSql = "SELECT Id FROM Recipients WHERE Email = @mail";
                int? existingId = null;

                using (var cmdCheck = new SqliteCommand(checkSql, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@mail", r.Email);
                    var result = cmdCheck.ExecuteScalar();
                    if (result != null) existingId = int.Parse(result.ToString());
                }

                if (existingId != null)
                {
                    string updateSql = @"UPDATE Recipients 
                                         SET Ad = @ad, Soyad = @soyad, GroupName = @grp, ExtraDataJson = @json 
                                         WHERE Id = @id";
                    using (var cmd = new SqliteCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", r.Ad ?? "");
                        cmd.Parameters.AddWithValue("@soyad", r.Soyad ?? "");
                        cmd.Parameters.AddWithValue("@grp", r.GroupName ?? "");
                        cmd.Parameters.AddWithValue("@json", jsonData ?? "");
                        cmd.Parameters.AddWithValue("@id", existingId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insertSql = @"INSERT INTO Recipients (Ad, Soyad, Email, GroupName, ExtraDataJson) 
                                         VALUES (@ad, @soyad, @mail, @grp, @json)";
                    using (var cmd = new SqliteCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", r.Ad ?? "");
                        cmd.Parameters.AddWithValue("@soyad", r.Soyad ?? "");
                        cmd.Parameters.AddWithValue("@mail", r.Email);
                        cmd.Parameters.AddWithValue("@grp", r.GroupName ?? "");
                        cmd.Parameters.AddWithValue("@json", jsonData ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }

                if (!string.IsNullOrEmpty(r.GroupName))
                {
                    SyncGroup(r.GroupName, r.Email, conn);
                }
            }
        }

        public void AddRecipient(Recipient r)
        {
            ImportRecipient(r);
        }

        public void UpdateRecipientDetails(Recipient r)
        {
            ImportRecipient(r);
        }

        public List<Recipient> GetAllRecipients()
        {
            var list = new List<Recipient>();
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Recipients ORDER BY Id DESC";
                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var recipient = new Recipient
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Ad = reader["Ad"].ToString(),
                            Soyad = reader["Soyad"].ToString(),
                            Email = reader["Email"].ToString(),
                            GroupName = reader["GroupName"].ToString(),
                            ExtraDataJson = reader["ExtraDataJson"].ToString()
                        };
                        if (!string.IsNullOrEmpty(recipient.ExtraDataJson))
                            recipient.DataFields = JsonConvert.DeserializeObject<Dictionary<string, string>>(recipient.ExtraDataJson);
                        list.Add(recipient);
                    }
                }
            }
            return list;
        }

        public void ClearAll()
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand("DELETE FROM Recipients", conn)) { cmd.ExecuteNonQuery(); }
                using (var cmd = new SqliteCommand("DELETE FROM RecipientGroups", conn)) { cmd.ExecuteNonQuery(); }
            }
        }

        private void SyncGroup(string groupName, string email, SqliteConnection conn)
        {
            string checkSql = "SELECT MemberEmailsJson FROM RecipientGroups WHERE GroupName = @name";
            string currentJson = null;

            using (var cmd = new SqliteCommand(checkSql, conn))
            {
                cmd.Parameters.AddWithValue("@name", groupName);
                var result = cmd.ExecuteScalar();
                if (result != null) currentJson = result.ToString();
            }

            List<string> emails = new List<string>();
            if (!string.IsNullOrEmpty(currentJson))
            {
                emails = JsonConvert.DeserializeObject<List<string>>(currentJson) ?? new List<string>();
            }

            if (!emails.Contains(email))
            {
                emails.Add(email);
                string newJson = JsonConvert.SerializeObject(emails);

                if (currentJson == null)
                {
                    string insertSql = "INSERT INTO RecipientGroups (GroupName, MemberEmailsJson) VALUES (@name, @json)";
                    using (var cmd = new SqliteCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", groupName);
                        cmd.Parameters.AddWithValue("@json", newJson);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string updateSql = "UPDATE RecipientGroups SET MemberEmailsJson = @json WHERE GroupName = @name";
                    using (var cmd = new SqliteCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", groupName);
                        cmd.Parameters.AddWithValue("@json", newJson);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<RecipientGroup> GetGroups()
        {
            var list = new List<RecipientGroup>();
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    var cmd = new SqliteCommand("SELECT * FROM RecipientGroups", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var group = new RecipientGroup
                            {
                                Id = int.Parse(reader["Id"].ToString()),
                                GroupName = reader["GroupName"].ToString()
                            };
                            string json = reader["MemberEmailsJson"].ToString();
                            if (!string.IsNullOrEmpty(json)) group.MemberEmails = JsonConvert.DeserializeObject<List<string>>(json);
                            list.Add(group);
                        }
                    }
                }
                catch { }
            }
            return list;
        }

        public void DeleteGroup(string groupName)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("DELETE FROM RecipientGroups WHERE GroupName = @name", conn);
                cmd.Parameters.AddWithValue("@name", groupName);
                cmd.ExecuteNonQuery();
            }
        }

        public void SaveAccount(EmailAccount account)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO EmailAccounts (DisplayName, Email, Password, SmtpHost, SmtpPort, EnableSsl) 
                               VALUES (@dn, @mail, @pass, @host, @port, @ssl)";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dn", account.DisplayName ?? "");
                    cmd.Parameters.AddWithValue("@mail", account.Email);
                    cmd.Parameters.AddWithValue("@pass", account.Password);
                    cmd.Parameters.AddWithValue("@host", account.SmtpHost);
                    cmd.Parameters.AddWithValue("@port", account.SmtpPort);
                    cmd.Parameters.AddWithValue("@ssl", account.EnableSsl ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<EmailAccount> GetAccounts()
        {
            var list = new List<EmailAccount>();
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    var cmd = new SqliteCommand("SELECT * FROM EmailAccounts", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EmailAccount
                            {
                                Id = int.Parse(reader["Id"].ToString()),
                                DisplayName = reader["DisplayName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                SmtpHost = reader["SmtpHost"].ToString(),
                                SmtpPort = int.Parse(reader["SmtpPort"].ToString()),
                                EnableSsl = reader["EnableSsl"].ToString() == "1"
                            });
                        }
                    }
                }
                catch { }
            }
            return list;
        }

        public void DeleteAccount(int id)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("DELETE FROM EmailAccounts WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void SaveTemplate(string title, string subject, string body)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO MessageTemplates (Title, Subject, Body) VALUES (@t, @s, @b)";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@s", subject);
                    cmd.Parameters.AddWithValue("@b", body);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<MessageTemplate> GetTemplates()
        {
            var list = new List<MessageTemplate>();
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    var cmd = new SqliteCommand("SELECT * FROM MessageTemplates", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MessageTemplate
                            {
                                Id = int.Parse(reader["Id"].ToString()),
                                Title = reader["Title"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                Body = reader["Body"].ToString()
                            });
                        }
                    }
                }
                catch { }
            }
            return list;
        }

        public void DeleteTemplate(int id)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("DELETE FROM MessageTemplates WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void SaveSignature(string signature)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO AppSettings (Key, Value) VALUES ('UserSignature', @val) 
                       ON CONFLICT(Key) DO UPDATE SET Value = @val";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@val", signature);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string GetSignature()
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT Value FROM AppSettings WHERE Key = 'UserSignature'";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "";
                }
            }
        }
        public void DeleteRecipient(int id)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("DELETE FROM Recipients WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}