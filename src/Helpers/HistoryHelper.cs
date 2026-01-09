using CodeNav.Extensions;
using CodeNav.Interfaces;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Extensibility.Editor;
using System.Collections.ObjectModel;
using System.Windows;

namespace CodeNav.Helpers;

public class HistoryHelper
{
    private const int MaxHistoryItems = 5;

    /// <summary>
    /// Add code item to history items based on text edits
    /// </summary>
    /// <remarks>Used when adding item to history based on text changes</remarks>
    /// <param name="model">Document holding all code items</param>
    /// <param name="textEdits">List of text edits made</param>
    public static void AddItemToHistory(CodeDocumentViewModel model, IEnumerable<TextEdit> textEdits)
    {
        foreach (var textEdit in textEdits)
        {
            try
            {
                var item = model
                    .CodeDocument
                    .Flatten()
                    .FilterNull()
                    .Where(item => item is not IMembers)
                    .FirstOrDefault(item => item.Span.Contains(textEdit.Range.Start));

                AddItemToHistory(item);
            }
            catch (Exception e)
            {
                LogHelper.Log("Error adding item to history", e);
            }
        }
    }

    /// <summary>
    /// Add code item to history items based on its code item
    /// </summary>
    /// <remarks>Used when adding item to history based on clicking a code item</remarks>
    /// <param name="item">Code item that was clicked</param>
    public static void AddItemToHistory(CodeItem item)
    {
        var model = item.CodeDocumentViewModel;

        if (model == null)
        {
            return;
        }

        // Clear current indicators
        foreach (var historyItem in model.HistoryItems)
        {
            if (historyItem == null)
            {
                continue;
            }

            historyItem.StatusMonikerVisibility = Visibility.Collapsed;
        }

        // Add new indicator, only keep the five latest history items
        model.HistoryItems.Remove(item);
        model.HistoryItems.Insert(0, item);
        model.HistoryItems = new ObservableCollection<CodeItem>(model.HistoryItems.Take(MaxHistoryItems));

        ApplyHistoryIndicator(model);
    }

    /// <summary>
    /// Apply history indicator to all code items in the history list
    /// </summary>
    /// <remarks>Uses Flatten() to do this recursively for child code items</remarks>
    /// <param name="model">Document holding all code items</param>
    public static void ApplyHistoryIndicator(CodeDocumentViewModel model)
    {
        var historyItems = model.HistoryItems
            .Where(i => i != null)
            .Select((historyItem, i) => (historyItem, i));

        foreach (var (historyItem, i) in historyItems)
        {
            var codeItem = model.CodeDocument
                .Flatten()
                .Where(item => item != null)
                .FirstOrDefault(item => item.Id == historyItem.Id);

            if (codeItem == null)
            {
                continue;
            }

            ApplyHistoryIndicator(codeItem, i);
        }
    }

    /// <summary>
    /// Apply history indicator to a single code item
    /// </summary>
    /// <remarks>Index determines color and opacity of the history indicator</remarks>
    /// <param name="item">Code item in history list</param>
    /// <param name="index">Index in history list</param>
    private static void ApplyHistoryIndicator(CodeItem item, int index = 0)
    {
        item.StatusMonikerVisibility = Visibility.Visible;

        // Show the icon in grayscale if it is not the most recent history item
        item.StatusGrayscale = index > 0;

        item.StatusOpacity = GetOpacity(index);
    }

    /// <summary>
    /// Get opacity based on history list index
    /// </summary>
    /// <param name="index">Index in history list</param>
    /// <remarks>
    /// 0: latest history item => 100%
    /// 1-2: => 90%
    /// 3-4: => 70%
    /// </remarks>
    /// <returns>Double between 0 and 1</returns>
    private static double GetOpacity(int index)
        => index switch
        {
            0 => 1,
            1 or 2 => 0.9,
            3 or 4 => 0.7,
            _ => 1,
        };

    /// <summary>
    /// Delete all history item indicators
    /// </summary>
    /// <param name="item">Code item on which the context menu was invoked</param>
    public static async Task ClearHistory(CodeItem item, CancellationToken cancellationToken)
    {
        if (item.CodeDocumentViewModel == null)
        {
            return;
        }

        item.CodeDocumentViewModel.HistoryItems.Clear();
    }
}