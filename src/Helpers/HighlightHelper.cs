using CodeNav.Extensions;
using CodeNav.ViewModels;

namespace CodeNav.Helpers;

public static class HighlightHelper
{
    /// <summary>
    /// Highlight code items that contain the current line number
    /// </summary>
    /// <param name="codeDocumentViewModel">Code document</param>
    /// <param name="lineNumber">Current line number</param>
    public static void HighlightCurrentItem(CodeDocumentViewModel codeDocumentViewModel,
        int lineNumber)
    {
        if (codeDocumentViewModel == null)
        {
            return;
        }

        try
        {
            UnHighlight(codeDocumentViewModel);
            Highlight(codeDocumentViewModel, lineNumber);
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
                item.IsHighlighted = false;
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
    private static void Highlight(CodeDocumentViewModel codeDocumentViewModel, int lineNumber)
        => codeDocumentViewModel
            .CodeDocument
            .Flatten()
            .FilterNull()
            .Where(item => item.StartLine <= lineNumber && item.EndLine >= lineNumber)
            .ToList()
            .ForEach(item => item.IsHighlighted = true);
}
