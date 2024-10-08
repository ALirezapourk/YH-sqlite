using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;
namespace YH_sqlite;

internal static class Program
{
    // 1. Hämta Documents-mappen
    private readonly static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    // 2. Definiera databasmappen
    private readonly static string dbFolder = "Databaser";

    // 3. Definiera databasnamnet
    private readonly static string databaseName = "HeroTeam.Sqlite";

    // 4. Slå samman rubbet till en sökväg
    private readonly static string dbPath = Path.Combine(documentsPath, dbFolder, databaseName);

    // 4. Kombinera sökvägen till databasen
    private readonly static string connectionString = $"Data Source={dbPath}";

    
       

        private static void Main()
    {
        // Skapa databasmappen om den inte redan finns
        if (!Directory.Exists(documentsPath))
        {
            Directory.CreateDirectory(documentsPath);
        }
        // Skapa en tom fil om den inte redan finns
        if (!Directory.Exists(Path.Combine(documentsPath, databaseName)))
        {
            // SQLite gör om den till en databas sen
            File.Create(Path.Combine(documentsPath, databaseName)).Close();
        }

        // Skapa databasen och tabellen om de inte redan finns
        ExecuteSql(@"
            CREATE TABLE IF NOT EXISTS Hero (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Alias TEXT NOT NULL,
                Power TEXT NOT NULL
            );");

        // Lägg till hjältar i tabellen
        ExecuteSql("INSERT OR IGNORE INTO Hero (Id, Name, Alias, Power) VALUES (1, 'Bruce Wayne', 'Batman', 'Wealth & Intelligence');");
        ExecuteSql("INSERT OR IGNORE INTO Hero (Id, Name, Alias, Power) VALUES (2, 'Clark Kent', 'Superman', 'Super Strength');");
        ExecuteSql("INSERT OR IGNORE INTO Hero (Id, Name, Alias, Power) VALUES (3, 'Diana Prince', 'Wonder Woman', 'Super Strength & Combat Skills');");

        // Läs och skriv ut alla hjältar
        Console.WriteLine("Alla hjältar:");
        ReadHeroes();

        // Uppdatera en hjältes kraft
        ExecuteSql("UPDATE Hero SET Power = 'Super Strength, Heat Vision, Flight' WHERE Id = 2;");

        // Läs och skriv ut alla hjältar igen
        Console.WriteLine("\nUppdaterade hjältar:");
        ReadHeroes();

        // Ta bort en hjälte
        ExecuteSql("DELETE FROM Hero WHERE Id = 1;");

        // Läs och skriv ut alla hjältar igen
        Console.WriteLine("\nEfter att Batman har lämnat:");
        ReadHeroes();
    }

    private static void ExecuteSql(string sql)
    {
        // create file if not exists
        // Create the file if it does not exist
        if (!File.Exists(dbPath))
        {
            File.Create(dbPath).Close();
        }




        using (SqliteConnection connection = new(connectionString))
        {
            connection.Open();
            using (SqliteCommand command = new(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    private static void ReadHeroes()
    {
        using (SqliteConnection connection = new(connectionString))
        {
            connection.Open();
            string sql = "SELECT id, Name, Alias, Power FROM Hero;";

            using (SqliteCommand command = new(sql, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Id: {reader["Id"]}, Namn: {reader["Name"]}, Alias: {reader["Alias"]}, Kraft: {reader["Power"]}");
                    }
                }
            }
        }
    }
}