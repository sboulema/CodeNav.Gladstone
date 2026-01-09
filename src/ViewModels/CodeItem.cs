using CodeNav.Constants;
using CodeNav.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace CodeNav.ViewModels;

[DataContract]
public class CodeItem : NotifyPropertyChangedObject
{
    public CodeItem()
    {
        ClickItemCommand = ClickItem();
        CopyNameCommand = CopyName();
        GoToDefinitionCommand = GoToDefinition();
        GoToEndCommand = GoToEnd();
        SelectInCodeCommand = SelectInCode();
        ClearHistoryCommand = ClearHistory();
        RefreshCommand = Refresh();
    }

    public CodeDocumentViewModel? CodeDocumentViewModel { get; set; }

    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type name of the data template to use for rendering the associated data.
    /// </summary>
    [DataMember]
    public string DataTemplateType { get; set; } = string.Empty;

    public int? StartLine { get; set; }

    public int? EndLine { get; set; }

    public TextSpan Span { get; set; }

    /// <summary>
    /// Icon showing the type (class, namespace, etc.) of the code item
    /// </summary>
    [DataMember]
    public ImageMoniker Moniker { get; set; }

    /// <summary>
    /// Icon showing the access (public, private, etc.) of the code item
    /// </summary>
    [DataMember]
    public ImageMoniker OverlayMoniker { get; set; }

    [DataMember]
    public string Id { get; set; } = string.Empty;

    [DataMember]
    public string Tooltip { get; set; } = string.Empty;

    public Uri? FilePath { get; set; }

    internal string FullName = string.Empty;

    public CodeItemKindEnum Kind;

    public CodeItemAccessEnum Access;

    private bool _isHighlighted;

    /// <summary>
    /// Indicator if the item should be highlighted
    /// </summary>
    [DataMember]
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    private double _opacity;

    [DataMember]
    public double Opacity
    {
        get => _opacity;
        set => SetProperty(ref _opacity, value);
    }

    #region Status Image
    private Visibility _statusMonikerVisibility = Visibility.Collapsed;

    [DataMember]
    public Visibility StatusMonikerVisibility
    {
        get => _statusMonikerVisibility;
        set => SetProperty(ref _statusMonikerVisibility, value);
    }

    private bool _statusGrayscale;

    /// <summary>
    /// Indicator if the history icon should be shown in grayscale
    /// </summary>
    [DataMember]
    public bool StatusGrayscale
    {
        get => _statusGrayscale;
        set => SetProperty(ref _statusGrayscale, value);
    }

    private double _statusOpacity;

    [DataMember]
    public double StatusOpacity
    {
        get => _statusOpacity;
        set => SetProperty(ref _statusOpacity, value);
    }

    #endregion

    public List<BookmarkStyle> BookmarkStyles
        => CodeDocumentViewModel?.BookmarkStyles ?? [];

    public bool FilterOnBookmarks
    {
        get => CodeDocumentViewModel?.FilterOnBookmarks ?? false;
        set => CodeDocumentViewModel!.FilterOnBookmarks = value;
    }

    public bool BookmarksAvailable => CodeDocumentViewModel?.Bookmarks.Any() == true;

    private bool _contextMenuIsOpen;
    public bool ContextMenuIsOpen
    {
        get => _contextMenuIsOpen;
        set => SetProperty(ref _contextMenuIsOpen, value);
    }

    #region Fonts
    private float _fontSize;

    [DataMember]
    public float FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    private float _parameterFontSize;

    [DataMember]
    public float ParameterFontSize
    {
        get => _parameterFontSize;
        set => SetProperty(ref _parameterFontSize, value);
    }

    private FontFamily? _fontFamily;

    [DataMember]
    public FontFamily? FontFamily
    {
        get => _fontFamily;
        set => SetProperty(ref _fontFamily, value);
    }

    private FontStyle _fontStyle;

    [DataMember]
    public FontStyle FontStyle
    {
        get => _fontStyle;
        set => SetProperty(ref _fontStyle, value);
    }
    #endregion

    #region IsVisible
    private Visibility _visibility;

    [DataMember]
    public Visibility IsVisible
    {
        get => _visibility;
        set => SetProperty(ref _visibility, value);
    }
    #endregion

    #region Colors
    private string _backgroundColor = string.Empty;

    [DataMember]
    public string BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            SetProperty(ref _backgroundColor, value);
        }
    }
    #endregion

    #region Commands
    [DataMember]
    public AsyncCommand ClickItemCommand { get; }
    public AsyncCommand ClickItem()
    {
        return new AsyncCommand(async (obj, clientContext, cancellationToken) =>
        {
            HistoryHelper.AddItemToHistory(this);
            await ScrollToLine(Span.Start, clientContext, cancellationToken);
        });
    }

    [DataMember]
    public AsyncCommand GoToDefinitionCommand { get; }
    public AsyncCommand GoToDefinition()
    {
        return new AsyncCommand(async (obj, clientContext, cancellationToken) =>
        {
            await ScrollToLine(Span.Start, clientContext, cancellationToken);
        });
    }

    [DataMember]
    public AsyncCommand ClearHistoryCommand { get; }
    public AsyncCommand ClearHistory()
    {
        return new AsyncCommand(async (parameter, cancellationToken) =>
        {
            await HistoryHelper.ClearHistory(this, cancellationToken);
        });
    }

    [DataMember]
    public AsyncCommand GoToEndCommand { get; }
    public AsyncCommand GoToEnd()
    {
        return new AsyncCommand(async (obj, clientContext, cancellationToken) =>
        {
            await ScrollToLine(Span.End, clientContext, cancellationToken);
        });
    }

    [DataMember]
    public AsyncCommand SelectInCodeCommand { get; }
    public AsyncCommand SelectInCode()
    {
        return new AsyncCommand(async (obj, clientContext, cancellationToken) =>
        {
            await SelectLines(clientContext, cancellationToken);
        });
    }

    [DataMember]
    public AsyncCommand CopyNameCommand { get; }
    public AsyncCommand CopyName()
        => new(async (parameter, cancellationToken) =>
        {
            Clipboard.SetText(Name);
            await Task.CompletedTask;
        });

    [DataMember]
    public AsyncCommand RefreshCommand { get; }
    public AsyncCommand Refresh()
    {
        return new AsyncCommand(async (parameter, clientContext, cancellationToken) =>
        {
            var textView = await clientContext.GetActiveTextViewAsync(cancellationToken);
            await CodeDocumentViewModel!
                .CodeDocumentService!
                .UpdateCodeDocumentViewModel(CodeDocumentViewModel.Extensibility, textView, cancellationToken);
        });
    }

    //public ICommand ExpandAllCommand => new DelegateCommand(ExpandAll);
    //public void ExpandAll(object args) => Control?.ToggleAll(true, new List<CodeItem>() { this });

    //public ICommand CollapseAllCommand => new DelegateCommand(CollapseAll);
    //public void CollapseAll(object args) => Control?.ToggleAll(false, new List<CodeItem>() { this });

    ///// <summary>
    ///// Add a single bookmark
    ///// </summary>
    //public ICommand BookmarkCommand => new DelegateCommand(Bookmark);
    //public void Bookmark(object args) => BookmarkAsync(args).FireAndForget();

    //public async Task BookmarkAsync(object args)
    //{
    //    try
    //    {
    //        if (args is not BookmarkStyle bookmarkStyle ||
    //            CodeDocumentViewModel == null)
    //        {
    //            return;
    //        }

    //        BookmarkHelper.ApplyBookmarkStyle(this, bookmarkStyle);

    //        var bookmarkStyleIndex = BookmarkHelper.GetIndex(CodeDocumentViewModel, bookmarkStyle);

    //        CodeDocumentViewModel.AddBookmark(Id, bookmarkStyleIndex);

    //        await SolutionStorageHelper.SaveToSolutionStorage(CodeDocumentViewModel);

    //        ContextMenuIsOpen = false;

    //        NotifyPropertyChanged("BookmarksAvailable");
    //    }
    //    catch (Exception e)
    //    {
    //        LogHelper.Log("CodeItem.Bookmark", e);
    //    }
    //}

    ///// <summary>
    ///// Delete a single bookmark
    ///// </summary>
    //public ICommand DeleteBookmarkCommand => new DelegateCommand(DeleteBookmark);
    //public void DeleteBookmark(object args)
    //{
    //    try
    //    {
    //        BookmarkHelper.ClearBookmark(this);

    //        CodeDocumentViewModel?.RemoveBookmark(Id);

    //        SolutionStorageHelper.SaveToSolutionStorage(CodeDocumentViewModel).FireAndForget();

    //        NotifyPropertyChanged("BookmarksAvailable");
    //    }
    //    catch (Exception e)
    //    {
    //        LogHelper.Log("CodeItem.DeleteBookmark", e);
    //    }
    //}

    ///// <summary>
    ///// Clear all bookmarks
    ///// </summary>
    //public ICommand ClearBookmarksCommand => new DelegateCommand(ClearBookmarks);
    //public void ClearBookmarks(object args)
    //{
    //    try
    //    {
    //        CodeDocumentViewModel?.ClearBookmarks();

    //        SolutionStorageHelper.SaveToSolutionStorage(CodeDocumentViewModel).FireAndForget();

    //        NotifyPropertyChanged("BookmarksAvailable");
    //    }
    //    catch (Exception e)
    //    {
    //        LogHelper.Log("CodeItem.ClearBookmarks", e);
    //    }
    //}

    //public ICommand FilterBookmarksCommand => new DelegateCommand(FilterBookmarks);
    //public void FilterBookmarks(object args) => Control?.FilterBookmarks();

    //public ICommand CustomizeBookmarkStylesCommand => new DelegateCommand(CustomizeBookmarkStyles);
    //public void CustomizeBookmarkStyles(object args)
    //{
    //    if (CodeDocumentViewModel == null)
    //    {
    //        return;
    //    }

    //    new BookmarkStylesWindow(CodeDocumentViewModel).ShowDialog();
    //    BookmarkHelper.ApplyBookmarks(CodeDocumentViewModel);
    //}
    #endregion

    private async Task ScrollToLine(
        int position,
        IClientContext clientContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var textViewSnapshot = await clientContext.GetActiveTextViewAsync(cancellationToken);

            if (textViewSnapshot == null)
            {
                return;
            }

            var textDocumentSnapshot = textViewSnapshot?.Document;

            // If the code item has a different file path, open that document
            if (FilePath != null &&
                textViewSnapshot?.Uri != FilePath)
            {
                textDocumentSnapshot = await clientContext.Extensibility
                    .Documents()
                    .OpenTextDocumentAsync(FilePath, cancellationToken);
            }

            if (textDocumentSnapshot == null)
            {
                return;
            }

            // Scroll to the requested line
            await clientContext.Extensibility.Editor().EditAsync(batch =>
            {
                var caret = new TextPosition(textDocumentSnapshot, position);
                textViewSnapshot!.AsEditable(batch).SetSelections(
                [
                    new Selection(activePosition: caret, anchorPosition: caret, insertionPosition: caret)
                ]);
            },
            cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
    }

    private async Task SelectLines(IClientContext clientContext, CancellationToken cancellationToken)
    {
        try
        {
            var textViewSnapshot = await clientContext.GetActiveTextViewAsync(cancellationToken);

            if (textViewSnapshot == null)
            {
                return;
            }

            var textDocumentSnapshot = textViewSnapshot?.Document;

            // If the code item has a different file path, open that document
            if (FilePath != null &&
                textViewSnapshot?.Uri != FilePath)
            {
                textDocumentSnapshot = await clientContext.Extensibility
                    .Documents()
                    .OpenTextDocumentAsync(FilePath, cancellationToken);
            }

            if (textDocumentSnapshot == null)
            {
                return;
            }

            // Select all lines corresponding to the code item
            await clientContext.Extensibility.Editor().EditAsync(batch =>
            {
                textViewSnapshot!.AsEditable(batch).SetSelections(
                [
                    new Selection(
                        new TextRange(
                            new TextPosition(textDocumentSnapshot, Span.Start),
                            new TextPosition(textDocumentSnapshot, Span.End)))
                ]);
            },
            cancellationToken);
        }
        catch (Exception)
        {
            // Ignore
        }
    }
}
