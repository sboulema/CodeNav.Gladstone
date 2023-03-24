using CodeNav.ViewModels;

namespace CodeNav.Models;

public class FileConfiguration
{
    public List<CodeItem> HistoryItems = new();

    public Dictionary<string, int> Bookmarks = new();
}
