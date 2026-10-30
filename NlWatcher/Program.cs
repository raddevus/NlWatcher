using NlWatcher.Models;

   if (args.Length < 1) {Console.WriteLine("Please provide a path to watch."); return;}
   Console.WriteLine($"{Environment.CurrentDirectory}");
   var allExclusionPaths = LoadExcludePaths();
   using var watcher = new FileSystemWatcher(args[0]);
   watcher.InternalBufferSize = 65536;

   // We need the following filters so that all Events
   // (Created, Deleted, etc.) fire.  But I've discovered
   // that the NotifyFilters.LastAccess generates way
   // too much output on Linux systems. Way to many file accesses & overloads this app.
   watcher.NotifyFilter =
                        NotifyFilters.LastWrite |
                        NotifyFilters.Size |
                        NotifyFilters.FileName |
                        NotifyFilters.DirectoryName;

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
  // if e.FullPath represents a directory then we ignore
  if (Directory.Exists(e.FullPath)) return;
  if (IsExclusionPath(e.FullPath, allExclusionPaths)) return;
if (e.Name.Contains("NlWatcher.db", StringComparison.OrdinalIgnoreCase)) {return; }
   if (e.ChangeType != WatcherChangeTypes.Changed)
   {
       return;
   }
   FileActivity fa = new FileActivity{FilePath=Path.GetDirectoryName(e.FullPath), FileName=Path.GetFileName(e.FullPath), Created=DateTime.Now, Action="Changed"};
   FileActivityContext fac = new();
   fac.Add(fa);
   fac.SaveChanges();
   Console.WriteLine($"Changed: {e.FullPath}");
}

void OnCreated(object sender, FileSystemEventArgs e)
{
  // if e.FullPath represents a directory then we ignore
  if (Directory.Exists(e.FullPath)) return;
  if (IsExclusionPath(e.FullPath, allExclusionPaths)) return;
if (e.Name.Contains("NlWatcher.db", StringComparison.OrdinalIgnoreCase)) {return; }
   string value = $"Created: {e.FullPath}";
   FileActivity fa = new FileActivity{FilePath=Path.GetDirectoryName(e.FullPath), FileName=Path.GetFileName(e.FullPath), Created=DateTime.Now, Action="Created"};
   FileActivityContext fac = new();
   fac.Add(fa);
   fac.SaveChanges();
   Console.WriteLine(value);
}

void OnDeleted(object sender, FileSystemEventArgs e){
  // if e.FullPath represents a directory then we ignore
  if (Directory.Exists(e.FullPath)) return;
  if (IsExclusionPath(e.FullPath, allExclusionPaths)) return;
if (e.Name.Contains("NlWatcher.db", StringComparison.OrdinalIgnoreCase)) {return; }
   FileActivity fa = new FileActivity{FilePath=Path.GetDirectoryName(e.FullPath), FileName=Path.GetFileName(e.FullPath), Created=DateTime.Now, Action="Deleted"};
   FileActivityContext fac = new();
   fac.Add(fa);
   fac.SaveChanges();
   Console.WriteLine($"Deleted: {e.FullPath}"); 
}

void OnRenamed(object sender, RenamedEventArgs e)
{
  // if e.FullPath represents a directory then we ignore
  if (Directory.Exists(e.FullPath)) return;
  if (IsExclusionPath(e.FullPath, allExclusionPaths)) return;
if (e.Name.Contains("NlWatcher.db", StringComparison.OrdinalIgnoreCase)) {return; }
   FileActivity fa = new FileActivity{FilePath=Path.GetDirectoryName(e.FullPath), FileName=Path.GetFileName(e.FullPath), Created=DateTime.Now, Action="Renamed"};
   FileActivityContext fac = new();
   fac.Add(fa);
   fac.SaveChanges();
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
       //PrintException(ex.InnerException);
        if (ex is InternalBufferOverflowException)
        {
          Console.WriteLine("It's buffer overflow");
        }
        else{
          Console.WriteLine(ex.GetType());
        }
   }
}

List<string> LoadExcludePaths(){
   Console.WriteLine("Loading exclusion paths.");
   if (!File.Exists("excludePaths.txt")){
      Console.WriteLine("No excludePaths.txt found.");
      return new List<string>();
   }
   return File.ReadAllLines("excludePaths.txt").ToList<string>();
}

bool IsExclusionPath(string path, List<string> excludePaths){
   foreach (string p in excludePaths){
      if (path.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0){
         return true;
      }
   }
   return false;
}

