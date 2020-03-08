
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptEngine.BackEnd
{
    public static class Database
    {
        private static SQLiteConnection m_dbConnection = new SQLiteConnection(@"Data Source=HashDatabase.db;");
        public static List<string> GetHashes()
        {
            try
            {
                List<string> Entries = new List<string>();
                m_dbConnection.Open();

                string sql1 = $"Select FileName from Hashes";
                string sql2 = $"Select MD5Hash from Hashes";
                SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
                SQLiteCommand command2 = new SQLiteCommand(sql2, m_dbConnection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                SQLiteDataReader reader2 = command2.ExecuteReader();
                string name;
                string hash;
                while (reader1.Read() && reader2.Read())
                {
                    name = reader1.GetString(0);
                    hash = reader2.GetString(0);
                    Entries.Add(name + "," + hash);
                }

                m_dbConnection.Close();
                return Entries;
            }
            catch(Exception exc)
            {
                return null;
            }
        }
        public static void SendHashes(List<string> Names, List<string> Hashes)
        {
            m_dbConnection.Open();
       
            string sql1 = $"INSERT INTO Hashes VALUES(@FileName, @MD5Hash)";
            SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
            for(int i=0; i<Names.Count; i++)
            {
                command1.Parameters.AddWithValue("@FileName", Names[i]);
                command1.Parameters.AddWithValue("@MD5Hash", Hashes[i]);
                command1.ExecuteNonQuery();
            }
            m_dbConnection.Close();
        }
    }
}
