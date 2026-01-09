using CodeNav.Constants;
using CodeNav.ViewModels;

namespace CodeNav.Models;

public class Configuration
{
    public bool UseXMLComments { get; set; } = false;

    public bool ShowFilterToolbar { get; set; } = true;

    public SortOrderEnum SortOrder { get; set; }

    public List<FilterRule> FilterRules { get; set; } = [];

    public List<BookmarkStyle> BookmarkStyles { get; set; } = [];

    public Dictionary<Uri, FileConfiguration> FileConfigurations { get; set; } = [];
}
