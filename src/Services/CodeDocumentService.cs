using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Settings.Settings;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav.Services;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

public class CodeDocumentService
{
    public CodeDocumentService(CodeNavSettingsCategoryObserver settingsObserver)
    {
        settingsObserver.Changed += SettingsObserver_ChangedAsync;
    }

    /// <summary>
    /// DataContext for the tool window.
    /// </summary>
    public CodeDocumentViewModel CodeDocumentViewModel { get; set; } = new();

    public async Task<CodeDocumentViewModel> UpdateCodeDocumentViewModel(
        VisualStudioExtensibility extensibility,
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var codeItems = await DocumentMapper.MapDocument(textView.Document, CodeDocumentViewModel, cancellationToken);

        // Update the DataContext for the tool window.
        CodeDocumentViewModel.CodeDocument.Clear();
        CodeDocumentViewModel.CodeDocument.AddRange(codeItems);
        CodeDocumentViewModel.Extensibility = extensibility;
        CodeDocumentViewModel.CodeDocumentService = this;

        // Sort items
        SortHelper.Sort(CodeDocumentViewModel);

        // Apply highlights
        HighlightHelper.UnHighlight(CodeDocumentViewModel);

        // Apply current visibility settings to the document
        VisibilityHelper.SetCodeItemVisibility(CodeDocumentViewModel);

        // Apply history items
        HistoryHelper.ApplyHistoryIndicator(CodeDocumentViewModel);

        // TODO: Until we implement syncing collapse to text editor better to expand all
        OutliningHelper.ExpandAll(CodeDocumentViewModel);

        return CodeDocumentViewModel;
    }

    private Task SettingsObserver_ChangedAsync(CodeNavSettingsCategorySnapshot settingsSnapshot)
    {
        CodeDocumentViewModel.ShowFilterToolbarVisibility =
            SettingsHelper.GetShowFilterToolbarVisibility(settingsSnapshot);

        CodeDocumentViewModel.SortOrder =
            SettingsHelper.GetSortOrder(settingsSnapshot);

        return Task.CompletedTask;
    }
}
