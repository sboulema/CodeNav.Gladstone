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

        CodeDocumentViewModel.CodeDocument.Clear();
        CodeDocumentViewModel.CodeDocument.AddRange(codeItems);

        // Update the DataContext for the tool window.
        CodeDocumentViewModel.Configuration = configuration;
        CodeDocumentViewModel.SortOrder = configuration.SortOrder;
        CodeDocumentViewModel.ConfigurationHelper = configurationHelper;
        CodeDocumentViewModel.Extensibility = extensibility;
        CodeDocumentViewModel.CodeDocumentService = this;

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
        HistoryHelper.ApplyHistoryIndicator(CodeDocumentViewModel);

        // TODO: Until we implement syncing collapse to text editor better to expand all
        OutliningHelper.ExpandAll(CodeDocumentViewModel);

        return CodeDocumentViewModel;
    }
}
