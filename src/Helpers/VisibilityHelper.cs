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
        => SetCodeItemVisibility(model.CodeDocument, model.FilterText);

    /// <summary>
    /// Loop through all code items and set if the item should be visible or not.
    /// </summary>
    /// <remarks>
    /// - Check if the code item should be visible based on the fitler rules
    /// - Check if the code item's name contains the filter text
    /// </remarks>
    /// <param name="document">List of code items</param>
    /// <param name="filterText">Text that should be contained in the code items name</param>
    public static ObservableCollection<CodeItem> SetCodeItemVisibility(ObservableCollection<CodeItem> codeDocument, string filterText = "")
    {
        // TODO: We can do this easier with flatten() ?!
        if (codeDocument?.Any() != true)
        {
            return [];
        }

        try
        {
            foreach (var item in codeDocument)
            {
                if (item is IMembers hasMembersItem &&
                    hasMembersItem.Members.Any())
                {
                    SetCodeItemVisibility(hasMembersItem.Members, filterText);
                }

                item.Visibility = ShouldBeVisible(item, filterText)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        catch (Exception e)
        {
            LogHelper.Log("Error during setting visibility", e);
        }

        return codeDocument;
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
    /// Determine if a code item should be visible
    /// </summary>
    /// <remarks>
    /// - Check if the code item should be visible based on the fitler rules
    /// - Check if the code item's name contains the filter text
    /// </remarks>
    /// <param name="item">CodeItem that is checked</param>
    /// <param name="filterText">Text filter</param>
    /// <returns></returns>
    private static bool ShouldBeVisible(CodeItem item, string filterText = "")
    {
        var visible = true;

        // Check filter rules
        var filterRule = GetFilterRule(item);

        if (filterRule != null &&
            filterRule.Visible == false)
        {
            return false;
        }

        // Check filter text
        if (!string.IsNullOrEmpty(filterText))
        {
            visible = visible &&
                item.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }

        // If an item has any visible members, it should be visible.
        // If an item does not have any visible members, hide it depending on an the "Hide if empty" filter rule setting.
        if (item is IMembers hasMembersItem &&
            hasMembersItem?.Members != null)
        {
            if (hasMembersItem.Members.Any(m => m.Visibility == Visibility.Visible))
            {
                visible = true;
            }
            else if (!hasMembersItem.Members.Any(m => m.Visibility == Visibility.Visible) && filterRule != null)
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
        if (item.CodeDocumentViewModel?.FilterRules == null)
        {
            return null;
        }

        var filterRule = item.CodeDocumentViewModel?.FilterRules
            .LastOrDefault(f => (f.Access == item.Access || f.Access == CodeItemAccessEnum.All) &&
                                (f.Kind == item.Kind || f.Kind == CodeItemKindEnum.All));

        return filterRule;
    }
}
