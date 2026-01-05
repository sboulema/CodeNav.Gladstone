using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav.Services;

public class CodeDocumentService(
    ConfigurationHelper configurationHelper)
{
    /// <summary>
    /// DataContext for the tool window.
    /// </summary>
    public CodeDocumentViewModel CodeDocumentViewModel { get; set; } = new();

    public async Task<CodeDocumentViewModel> UpdateCodeDocumentViewModel(
        VisualStudioExtensibility extensibility,
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var configuration = await configurationHelper.GetConfiguration();

        var codeItems = await DocumentMapper.MapDocument(textView.Document, configuration, CodeDocumentViewModel, cancellationToken);

        // Update the DataContext for the tool window.
        CodeDocumentViewModel.TextDocumentSnapshot = textView.Document;
        CodeDocumentViewModel.Configuration = configuration;
        CodeDocumentViewModel.SortOrder = configuration.SortOrder;
        //CodeDocumentViewModel.HistoryHelper = historyHelper;
        CodeDocumentViewModel.ConfigurationHelper = configurationHelper;
        CodeDocumentViewModel.Extensibility = extensibility;
        CodeDocumentViewModel.CodeDocumentService = this;

        CodeDocumentViewModel.CodeDocument.Clear();

        foreach (var item in codeItems)
        {
            CodeDocumentViewModel.CodeDocument.Add(item);
        }

        // Sort items
        SortHelper.Sort(CodeDocumentViewModel);

        // Apply highlights
        HighlightHelper.UnHighlight(CodeDocumentViewModel);

        // Apply current visibility settings to the document
        VisibilityHelper.SetCodeItemVisibility(CodeDocumentViewModel);

        // Apply bookmarks
        CodeDocumentViewModel.Bookmarks = ConfigurationHelper.GetFileConfiguration(CodeDocumentViewModel.Configuration, textView.Uri).Bookmarks;
        BookmarkHelper.ApplyBookmarks(CodeDocumentViewModel);

        // Apply history items
        CodeDocumentViewModel.HistoryItems = new(ConfigurationHelper.GetFileConfiguration(CodeDocumentViewModel.Configuration, textView.Uri).HistoryItems);
        HistoryHelper.ApplyHistoryIndicator(CodeDocumentViewModel);

        return CodeDocumentViewModel;
    }
}
