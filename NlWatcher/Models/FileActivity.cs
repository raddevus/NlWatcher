
namespace NlWatcher.Models;

public class FileActivity{
   Int64 Id {get; set;}
   String FilePath {get; set;}
   String FileName {get; set;}
   String Action {get; set;}
   DateTime Created{get; set;}
}
