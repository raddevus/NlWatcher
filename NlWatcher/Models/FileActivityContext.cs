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
            foreach (String tableCreate in allTableCreation){
                command.CommandText = tableCreate;
                command.ExecuteNonQuery();
            }
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
}
