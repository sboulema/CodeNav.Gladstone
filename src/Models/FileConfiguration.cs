using CodeNav.ViewModels;

namespace CodeNav.Models;

public class FileConfiguration
{
    public List<CodeItem> HistoryItems = [];

    public Dictionary<string, int> Bookmarks = [];
}
