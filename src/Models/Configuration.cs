using CodeNav.Constants;
using CodeNav.ViewModels;

namespace CodeNav.Models;

public class Configuration
{
    public bool UseXMLComments { get; set; } = false;

    public string FontFamilyName { get; set; } = "Segoe UI";

    public float FontSize { get; set; } = 11.25f;

    public bool ShowFilterToolbar { get; set; } = true;

    public bool ShowMargin { get; set; } = true;

    public double MarginWidth { get; set; } = 200;

    public SortOrderEnum SortOrder { get; set; }

    public List<FilterRule> FilterRules { get; set; } = new();

    public List<BookmarkStyle> BookmarkStyles { get; set; } = new();

    public Dictionary<Uri, FileConfiguration> FileConfigurations { get; set; } = new();
}
