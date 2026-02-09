using CodeNav.Dialogs.SettingsDialog;
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

    /// <summary>
    /// DataContext for the settings dialog.
    /// </summary>
    public SettingsDialogData SettingsDialogData { get; set; } = new();

    public async Task<CodeDocumentViewModel> UpdateCodeDocumentViewModel(
        VisualStudioExtensibility? extensibility,
        ITextViewSnapshot? textView,
        CancellationToken cancellationToken)
    {
        if (extensibility == null ||
            textView == null)
        {
            return CodeDocumentViewModel;
        }

        // Show loading item while we process the document
        CodeDocumentViewModel.CodeDocument.Clear();
        CodeDocumentViewModel.CodeDocument.AddRange(PlaceholderHelper.CreateLoadingItem());

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

        return CodeDocumentViewModel;
    }

    private Task SettingsObserver_ChangedAsync(CodeNavSettingsCategorySnapshot settingsSnapshot)
    {
        SettingsDialogData = SettingsHelper.GetSettings(settingsSnapshot);

        CodeDocumentViewModel.ShowFilterToolbarVisibility =
            SettingsHelper.GetShowFilterToolbarVisibility(settingsSnapshot);

        var sortOrder = SettingsHelper.GetSortOrder(settingsSnapshot);
        if (sortOrder != CodeDocumentViewModel.SortOrder)
        {
            CodeDocumentViewModel.SortOrder = sortOrder;
            var sorted = SortHelper.Sort(CodeDocumentViewModel);
            CodeDocumentViewModel.CodeDocument.Clear();
            CodeDocumentViewModel.CodeDocument.AddRange(sorted);
        }

        if (SettingsDialogData.ShowHistoryIndicators == false)
        {
            HistoryHelper.ClearHistory(CodeDocumentViewModel);
        }

        if (SettingsDialogData.AutoHighlight == false)
        {
            HighlightHelper.UnHighlight(CodeDocumentViewModel);
        }

        return Task.CompletedTask;
    }
}
