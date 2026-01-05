using System.Windows;
using System.Windows.Media;
using CodeNav.Extensions;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace CodeNav.Helpers;

public static class HighlightHelper
{
    //private static string _foregroundColor = EnvironmentColors.ToolWindowTabSelectedTextColorKey.ToString();
    //private static string _borderColor = EnvironmentColors.FileTabButtonDownSelectedActiveColorKey.ToString();
    //private static string _regularForegroundColor = EnvironmentColors.ToolWindowTextColorKey.ToString();

    /// <summary>
    /// Highlight code items that contain the current line number
    /// </summary>
    /// <param name="codeDocumentViewModel">Code document</param>
    /// <param name="backgroundColor">Background color to use when highlighting</param>
    /// <param name="lineNumber">Current line number</param>
    public static void HighlightCurrentItem(CodeDocumentViewModel codeDocumentViewModel,
        string backgroundColor, int lineNumber)
    {
        if (codeDocumentViewModel == null)
        {
            return;
        }

        try
        {
            UnHighlight(codeDocumentViewModel);
            Highlight(codeDocumentViewModel, lineNumber, backgroundColor);
        }
        catch (Exception e)
        {
            LogHelper.Log("Error highlighting current item", e);
        }
    }

    /// <summary>
    /// Remove highlight from all code items
    /// </summary>
    /// <remarks>Will restore bookmark foreground color when unhighlighting a bookmarked item</remarks>
    /// <param name="codeDocumentViewModel">Code document</param>
    public static void UnHighlight(CodeDocumentViewModel codeDocumentViewModel)
        => codeDocumentViewModel.CodeDocument
            .Flatten()
            .FilterNull()
            .ToList()
            .ForEach(item =>
            {
                item.FontWeight = FontWeights.Regular;
                item.NameBackgroundColor = Brushes.Transparent.Color.ToString();
                item.IsHighlighted = false;
                item.ForegroundColor = BookmarkHelper.IsBookmark(codeDocumentViewModel.Bookmarks, item)
                    ? codeDocumentViewModel.BookmarkStyles[codeDocumentViewModel.Bookmarks[item.Id]].ForegroundColor
                    : "red";

                if (item is CodeClassItem classItem)
                {
                    classItem.BorderColor = Colors.DarkGray.ToString();
                }
            });

    /// <summary>
    /// Highlight code items that contain the current line number
    /// </summary>
    /// <remarks>
    /// Highlighting changes the foreground, font weight and background of a code item
    /// Deepest highlighted code item will be scrolled to, to ensure it is in view
    /// </remarks>
    /// <param name="document">Code document</param>
    /// <param name="ids">List of unique code item ids</param>
    private static void Highlight(CodeDocumentViewModel codeDocumentViewModel,
        int lineNumber, string backgroundColor)
    {
        var itemsToHighlight = codeDocumentViewModel
            .CodeDocument
            .Flatten()
            .FilterNull()
            .Where(item => item.StartLine <= lineNumber && item.EndLine >= lineNumber)
            .OrderBy(item => item.StartLine);

        foreach (var item in itemsToHighlight)
        {
            item.ForegroundColor = "red";
            item.FontWeight = FontWeights.Bold;
            item.NameBackgroundColor = backgroundColor;
            item.IsHighlighted = true;

            if (item is CodeClassItem classItem)
            {
                classItem.BorderColor = "red";
            }
        }
    }

    ///// <summary>
    ///// Get background highlight color from settings
    ///// </summary>
    ///// <remarks>
    ///// Should be used separate from the actual highlighting to avoid,
    ///// reading settings every time we highlight
    ///// </remarks>
    ///// <returns>Background color</returns>
    //public static async Task<Color> GetBackgroundHighlightColor()
    //{
    //    var general = await General.GetLiveInstanceAsync();

    //    var highlightBackgroundColor = general.HighlightColor;

    //    if (highlightBackgroundColor.IsNamedColor &&
    //        highlightBackgroundColor.Name.Equals("Transparent"))
    //    {
    //        return ColorHelper.ToMediaColor(EnvironmentColors.SystemHighlightColorKey);
    //    }

    //    return ColorHelper.ToMediaColor(highlightBackgroundColor);
    //}
}
