
namespace NlWatcher.Models;

public class FileActivity{
   public Int64 Id {get; set;}
   public String FilePath {get; set;}
   public String FileName {get; set;}
   public String Action {get; set;}
   public DateTime Created{get; set;}
}
