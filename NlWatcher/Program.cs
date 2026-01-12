using NlWatcher.Models;

   if (args.Length < 1) {Console.WriteLine("Need path."); return;}
   
   using var watcher = new FileSystemWatcher(args[0]);

   watcher.NotifyFilter = NotifyFilters.Attributes
                        | NotifyFilters.CreationTime
                        | NotifyFilters.DirectoryName
                        | NotifyFilters.FileName
                        | NotifyFilters.LastAccess
                        | NotifyFilters.LastWrite
                        | NotifyFilters.Security
                        | NotifyFilters.Size;

   watcher.Changed += OnChanged;
   watcher.Created += OnCreated;
   watcher.Deleted += OnDeleted;
   watcher.Renamed += OnRenamed;
   watcher.Error += OnError;

   watcher.Filter = "*.*";
   watcher.IncludeSubdirectories = true;
   watcher.EnableRaisingEvents = true;

   Console.WriteLine("Press enter to exit.");
   Console.ReadLine();

void OnChanged(object sender, FileSystemEventArgs e)
{
   if (e.ChangeType != WatcherChangeTypes.Changed)
   {
       return;
   }
   FileActivity fa = new FileActivity{FilePath=Path.GetDirectoryName(e.FullPath), FileName=Path.GetFileName(e.FullPath), Created=DateTime.Now, Action="Changed"};
   Console.WriteLine($"Changed: {e.FullPath}");
}

void OnCreated(object sender, FileSystemEventArgs e)
{
   string value = $"Created: {e.FullPath}";
   Console.WriteLine(value);
}

void OnDeleted(object sender, FileSystemEventArgs e) =>
   Console.WriteLine($"Deleted: {e.FullPath}");

void OnRenamed(object sender, RenamedEventArgs e)
{
   Console.WriteLine($"Renamed:");
   Console.WriteLine($"    Old: {e.OldFullPath}");
   Console.WriteLine($"    New: {e.FullPath}");
}

void OnError(object sender, ErrorEventArgs e) =>
   PrintException(e.GetException());

void PrintException(Exception? ex)
{
   if (ex != null)
   {
       Console.WriteLine($"Message: {ex.Message}");
       Console.WriteLine("Stacktrace:");
       Console.WriteLine(ex.StackTrace);
       Console.WriteLine();
       PrintException(ex.InnerException);
   }
}
