using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace NlWatcher.Models;

public class FileActivityContext : DbContext{
   // The variable name must match the name of the table.
    public DbSet<FileActivity> FileActivity { get; set; }
    public string DbPath{get;set;}
   
    public FileActivityContext(){
      var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "NlWatcher.db");
        Console.WriteLine(DbPath);

        SqliteConnection connection = new SqliteConnection($"Data Source={DbPath}");
        // ########### FYI THE DB is created when it is OPENED ########
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        FileInfo fi = new FileInfo(DbPath);
        // check to see if db file is 0 length, if so, it needs to have table added
        if (fi.Length == 0){
           EnsureDatabaseInitialized();
        }
    }
    // configures the database for use by EF
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    protected String [] allTableCreation = {
        @"CREATE TABLE FileActivity 
            (
            [ID] INTEGER NOT NULL PRIMARY KEY,
            [FilePath] NVARCHAR(256) NOT NULL check(length(FilePath) <= 256),
            [FileName] NVARCHAR(256) NOT NULL check(length(FileName) <= 256),
            [Action] NVARCHAR(50) NOT NULL check(length(Action) <= 50),
            [Created] NVARCHAR(30) default (datetime('now','localtime')) 
                      check(length(Created) <= 30)
            )"
    };

    /// <summary>
    /// Initializes the database, enables WAL mode, and creates custom tables if they don't exist.
    /// </summary>
    private void EnsureDatabaseInitialized()
    {
        // 1. Ensure the database file is created
 //       Database.EnsureCreated();

        // 2. Turn on Write-Ahead Logging (WAL) mode
        Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");

        // (Optional performance tweak often paired with WAL mode)
        // Thought it should be set to FULL but NORMAL is 
        // fine and shaves off a few milliseconds.
        // For our purposes this is safe enough (not quite ACID)
//        Database.ExecuteSqlRaw("PRAGMA synchronous=FULL;");

        // 3. Execute your table creation scripts
        foreach (var sql in allTableCreation)
        {
            Database.ExecuteSqlRaw(sql);
        }
    }
}
