using System;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace DBchat
{
    class Program
    {
        static SQLiteConnection connection;
        public static SQLiteCommand command;

        static void Main(string[] args)
        {
            if (!File.Exists("myLiteDb.db"))
            {
                SQLiteConnection.CreateFile("myLiteDB.db");
            }
            connection = new SQLiteConnection("Data Source = myLiteDB.db");
            connection.Open();
            command = new SQLiteCommand(connection);
            string sql = "CREATE TABLE IF NOT EXISTS Users ('Id' integer PRIMARY KEY , 'Name' text, 'Login' text NOT NULL UNIQUE,'Password' text NOT NULL)";
            command.CommandText = sql;
            command.ExecuteNonQuery();
            command = new SQLiteCommand(connection);
            sql = "CREATE TABLE IF NOT EXISTS Messages (" +
                "'Id' integer primary key autoincrement, 'Message' text, 'DateIn' text, 'DateOut' text, 'SenderId' integer, 'RcptId' integer," +
                " FOREIGN KEY(SenderId) REFERENCES Users(ID), FOREIGN KEY(RcptId) REFERENCES Users(ID) )";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            //AddUser(1, "John", "Joh", "123");
            //AddUser(2, "Jack", "Jac", "456");

            //WriteMessage("Hi!", 1, 2);
            //WriteMessage("How are you?", 1, 2);
            Console.WriteLine(ReadMessage(2));

            connection.Close();
            Console.ReadLine();
        }

        static void AddUser(int id, string name, string login, string password)
        {
            command.CommandText = "INSERT INTO Users ('Id', 'Name', 'Login', 'Password') values ('" + id + "', '" + name + "', '" + login + "', '" + password + "')";
            command.ExecuteNonQuery();
        }

        static void WriteMessage(string message, int sendrId, int rcptId)
        {
            command.CommandText = "INSERT INTO Messages ('Message', 'DateIn', 'SenderId', 'RcptId') values ('" + message + "', '" + DateTime.Now.ToString(@"dd\.hh\:mm\:ss") + "', '" + sendrId + "', '" + rcptId + "' )";

            command.ExecuteNonQuery();
        }

        static string ReadMessage(int rcptId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            command.CommandText = "SELECT Message FROM Messages WHERE RcptId = '" + rcptId + "' ";
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read()) // stream for listening
                {
                    object id = reader.GetValue(0);

                    stringBuilder.Append(id.ToString());
                    stringBuilder.AppendLine();
                }
            }
            reader.Close();
            command.CommandText = "UPDATE Messages Set DateOut = '" + DateTime.Now.ToString(@"dd\.hh\:mm\:ss") + "' WHERE  RcptId = '" + rcptId + "'";
            command.ExecuteNonQuery();
            return stringBuilder.ToString();
        }
    }
}
