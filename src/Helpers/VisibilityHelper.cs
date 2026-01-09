using CodeNav.Constants;
using CodeNav.Interfaces;
using CodeNav.Models;
using CodeNav.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace CodeNav.Helpers;

public static class VisibilityHelper
{
    public static ObservableCollection<CodeItem> SetCodeItemVisibility(CodeDocumentViewModel model)
        => SetCodeItemVisibility(model.CodeDocument, model.FilterText, model.FilterOnBookmarks, model.Bookmarks);

    /// <summary>
    /// Loop through all codeItems and look into Settings to see if the item should be visible or not.
    /// </summary>
    /// <param name="document">List of codeItems</param>
    /// <param name="name">Filters items by name</param>
    /// <param name="filterOnBookmarks">Filters items by being bookmarked</param>
    /// <param name="bookmarks">List of bookmarked items</param>
    public static ObservableCollection<CodeItem> SetCodeItemVisibility(ObservableCollection<CodeItem> document, string name = "",
        bool filterOnBookmarks = false, Dictionary<string, int>? bookmarks = null)
    {
        if (document?.Any() != true)
        {
            // No code items have been found to filter on by name
            return [];
        }

        try
        {
            foreach (var item in document)
            {
                if (item is IMembers hasMembersItem && hasMembersItem.Members.Any())
                {
                    SetCodeItemVisibility(hasMembersItem.Members, name, filterOnBookmarks, bookmarks);
                }

                item.IsVisible = ShouldBeVisible(item, name, filterOnBookmarks, bookmarks)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        catch (Exception e)
        {
            LogHelper.Log("Error during setting visibility", e);
        }

        return document;
    }

    public static bool IsEmpty(List<CodeItem> document)
    {
        if (document?.Any() != true)
        {
            return true;
        }

        var isEmpty = true;

        foreach (var item in document)
        {
            if (item is IMembers membersItem)
            {
                isEmpty = !membersItem.Members.Any();
            }
        }

        return isEmpty;
    }

    /// <summary>
    /// Determine if an item should be visible
    /// </summary>
    /// <param name="item">CodeItem that is checked</param>
    /// <param name="name">Text filter</param>
    /// <param name="filterOnBookmarks">Are we only showing bookmarks?</param>
    /// <param name="bookmarks">List of current bookmarks</param>
    /// <returns></returns>
    private static bool ShouldBeVisible(CodeItem item, string name = "",
        bool filterOnBookmarks = false, Dictionary<string, int>? bookmarks = null)
    {
        var visible = true;

        var filterRule = GetFilterRule(item);

        if (filterRule != null && filterRule.Visible == false)
        {
            return false;
        }

        if (filterOnBookmarks)
        {
            visible = BookmarkHelper.IsBookmark(bookmarks, item);
        }

        if (!string.IsNullOrEmpty(name))
        {
            visible = visible && item.Name.Contains(name, StringComparison.OrdinalIgnoreCase);
        }

        // If an item has any visible members, it should be visible.
        // If an item does not have any visible members, hide it depending on an option
        if (item is IMembers hasMembersItem &&
            hasMembersItem?.Members != null)
        {
            if (hasMembersItem.Members.Any(m => m.IsVisible == Visibility.Visible))
            {
                visible = true;
            }
            else if (!hasMembersItem.Members.Any(m => m.IsVisible == Visibility.Visible) && filterRule != null)
            {
                visible = !filterRule.HideIfEmpty;
            }
        }

        return visible;
    }

    public static Visibility GetIgnoreVisibility(CodeItem item)
    {
        var filterRule = GetFilterRule(item);

        if (filterRule == null)
        {
            return Visibility.Visible;
        }

        return filterRule.Ignore ? Visibility.Collapsed : Visibility.Visible;
    }

    private static FilterRule? GetFilterRule(CodeItem item)
    {
        if (item.CodeDocumentViewModel?.Configuration.FilterRules == null)
        {
            return null;
        }

        var filterRule = item.CodeDocumentViewModel?.Configuration.FilterRules
            .LastOrDefault(f => (f.Access == item.Access || f.Access == CodeItemAccessEnum.All) &&
                                (f.Kind == item.Kind || f.Kind == CodeItemKindEnum.All));

        return filterRule;
    }
}
