﻿using CodeNav.Languages.CSharp.Mappers;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav.Helpers;

public class DocumentHelper
{
    private readonly ConfigurationHelper _configurationHelper;
    private readonly HistoryHelper _historyHelper;
    
    private readonly VisualStudioExtensibility _extensibility;

    public DocumentHelper(ConfigurationHelper configurationHelper,
        HistoryHelper historyHelper,
        VisualStudioExtensibility extensibility)
    {
        _configurationHelper = configurationHelper;
        _historyHelper = historyHelper;
        _extensibility = extensibility;
    }

    public async Task<CodeDocumentViewModel> UpdateDocument(ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var documentSnapshot = await textView.GetTextDocumentAsync(cancellationToken);

        var configuration = await _configurationHelper.GetConfiguration();

        var codeItems = await DocumentMapper.MapDocument(documentSnapshot, configuration, cancellationToken);

        var codeDocumentViewModel = new CodeDocumentViewModel
        {
            TextDocumentSnapshot = documentSnapshot,
            Configuration = configuration,
            CodeDocument = codeItems.ToList(),
            SortOrder = configuration.SortOrder,
            DocumentHelper = this,
            HistoryHelper = _historyHelper,
            ConfigurationHelper = _configurationHelper
        };

        // set codedocument viewmodel on all codeitems

        // Sort items
        SortHelper.Sort(codeDocumentViewModel);

        // Apply highlights
        HighlightHelper.UnHighlight(codeDocumentViewModel);

        // Apply current visibility settings to the document
        VisibilityHelper.SetCodeItemVisibility(codeDocumentViewModel);

        // Apply bookmarks
        codeDocumentViewModel.Bookmarks = ConfigurationHelper.GetFileConfiguration(codeDocumentViewModel.Configuration, textView.Uri).Bookmarks;
        BookmarkHelper.ApplyBookmarks(codeDocumentViewModel);

        // Apply history items
        codeDocumentViewModel.HistoryItems = new(ConfigurationHelper.GetFileConfiguration(codeDocumentViewModel.Configuration, textView.Uri).HistoryItems);
        HistoryHelper.ApplyHistoryIndicator(codeDocumentViewModel);

        return codeDocumentViewModel;
    }

    public async Task ScrollToLine(
        CodeItem codeItem,
        int position,
        CancellationToken cancellationToken)
    {
        try
        {
            var documentSnapshot = codeItem.CodeDocumentViewModel?.TextDocumentSnapshot;

            if (codeItem.FilePath != null &&
                documentSnapshot?.Uri != codeItem.FilePath)
            {
                documentSnapshot = await _extensibility.Documents().OpenTextDocumentAsync(codeItem.FilePath, cancellationToken);
            }

            await _extensibility.Editor().EditAsync(
                batch =>
                {
                    codeItem.CodeDocumentViewModel?.TextView?.AsEditable(batch).SetSelections(new List<Selection> 
                    { 
                        new Selection(new TextRange(new TextPosition(documentSnapshot, position), 0))
                    });
                },
                cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
    }

    public async Task SelectLines(
        CodeItem codeItem,
        CancellationToken cancellationToken)
    {
        try
        {
            var documentSnapshot = codeItem.CodeDocumentViewModel?.TextDocumentSnapshot;

            if (codeItem.FilePath != null &&
                documentSnapshot?.Uri != codeItem.FilePath)
            {
                documentSnapshot = await _extensibility.Documents().OpenTextDocumentAsync(codeItem.FilePath, cancellationToken);
            }

            await _extensibility.Editor().EditAsync(
                batch =>
                {
                    codeItem.CodeDocumentViewModel?.TextView?.AsEditable(batch).SetSelections(new List<Selection>
                    {
                        new Selection(new TextRange(
                            new TextPosition(documentSnapshot, codeItem.Span.Start),
                            new TextPosition(documentSnapshot, codeItem.Span.End)))
                    });
                },
                cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
    }
}
