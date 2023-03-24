using CodeNav.Extensions;
using CodeNav.ViewModels;
using System.Windows.Media;

namespace CodeNav.Helpers;

public class BookmarkHelper
{
    private readonly ConfigurationHelper _configurationHelper;

    public BookmarkHelper(ConfigurationHelper configurationHelper)
    {
        _configurationHelper = configurationHelper;
    }

    /// <summary>
    /// Apply bookmark style to all code items that are bookmarked
    /// </summary>
    /// <param name="codeDocumentViewModel"></param>
    public static void ApplyBookmarks(CodeDocumentViewModel codeDocumentViewModel)
    {
        try
        {
            if (!codeDocumentViewModel.CodeDocument.Any())
            {
                return;
            }

            foreach (var bookmark in codeDocumentViewModel.Bookmarks)
            {
                var codeItem = codeDocumentViewModel.CodeDocument
                    .Flatten()
                    .FirstOrDefault(i => i.Id.Equals(bookmark.Key));

                if (codeItem == null)
                {
                    continue;
                }

                ApplyBookmarkStyle(codeItem, codeDocumentViewModel.BookmarkStyles[bookmark.Value]);
            }
        }
        catch (Exception e)
        {
            LogHelper.Log("ApplyBookmarks", e);
        }
    }

    /// <summary>
    /// Revert all code items to previous styling and delete all bookmarks
    /// </summary>
    /// <param name="codeDocumentViewModel">view model</param>
    public static void ClearBookmarks(CodeDocumentViewModel codeDocumentViewModel)
    {
        try
        {
            foreach (var bookmark in codeDocumentViewModel.Bookmarks)
            {
                var codeItem = codeDocumentViewModel.CodeDocument
                    .Flatten()
                    .FirstOrDefault(i => i.Id.Equals(bookmark.Key));
                ClearBookmark(codeItem);
            }

            codeDocumentViewModel.Bookmarks.Clear();
        }
        catch (Exception e)
        {
            LogHelper.Log("ClearBookmarks", e);
        }
    }

    /// <summary>
    /// Apply bookmark style foreground and background to code item
    /// </summary>
    /// <param name="codeItem">code item</param>
    /// <param name="bookmarkStyle">bookmark style</param>
    public static void ApplyBookmarkStyle(CodeItem codeItem, BookmarkStyle bookmarkStyle)
    {
        codeItem.BackgroundColor = bookmarkStyle.BackgroundColor;
        codeItem.ForegroundColor = bookmarkStyle.ForegroundColor;
    }

    /// <summary>
    /// Revert code item foreground and background to previous state
    /// </summary>
    /// <param name="codeItem">code item</param>
    public static void ClearBookmark(CodeItem codeItem)
    {
        if (codeItem == null)
        {
            return;
        }

        codeItem.BackgroundColor = Brushes.Transparent.Color.ToString();
        codeItem.ForegroundColor = "red";
    }

    /// <summary>
    /// Is a code item bookmarked
    /// </summary>
    /// <param name="codeDocumentViewModel">view model</param>
    /// <param name="codeItem">code item</param>
    /// <returns>if code item is bookmarked</returns>
    public static bool IsBookmark(CodeDocumentViewModel codeDocumentViewModel, CodeItem codeItem)
        => codeDocumentViewModel.Bookmarks.ContainsKey(codeItem.Id);

    /// <summary>
    /// Is a code item bookmarked
    /// </summary>
    /// <param name="bookmarks">List of bookmarks</param>
    /// <param name="codeItem">code item</param>
    /// <returns>if code item is bookmarked</returns>
    public static bool IsBookmark(Dictionary<string, int>? bookmarks, CodeItem codeItem)
        => bookmarks?.ContainsKey(codeItem.Id) == true;

    /// <summary>
    /// Default available bookmark styles
    /// </summary>
    /// <returns>List of bookmark styles</returns>
    public static List<BookmarkStyle> GetBookmarkStyles(CodeDocumentViewModel codeDocumentViewModel)
    {
        var bookmarkStyles = codeDocumentViewModel.Configuration.BookmarkStyles;

        if (bookmarkStyles.Any() != true)
        {
            codeDocumentViewModel.BookmarkStyles = GetDefaultBookmarkStyles();
        }

        //if (bookmarkStyles.Any(style => style.BackgroundColor.A != 0) == true)
        //{
        //    codeDocumentViewModel.BookmarkStyles = storageItem.BookmarkStyles;
        //}

        if (codeDocumentViewModel.BookmarkStyles?.Any() != true)
        {
            codeDocumentViewModel.BookmarkStyles = GetDefaultBookmarkStyles();
        }

        return codeDocumentViewModel.BookmarkStyles;
    }

    //public static async Task SetBookmarkStyles(CodeDocumentViewModel codeDocumentViewModel, ControlCollection controls)
    //{
    //    var styles = new List<BookmarkStyle>();

    //    foreach (var item in controls)
    //    {
    //        if (!(item is Label label))
    //        {
    //            continue;
    //        }

    //        styles.Add(new BookmarkStyle(ColorHelper.ToMediaColor(label.BackColor), ColorHelper.ToMediaColor(label.ForeColor)));
    //    }

    //    codeDocumentViewModel.BookmarkStyles = styles;

    //    await SolutionStorageHelper.SaveToSolutionStorage(codeDocumentViewModel);
    //}

    public async Task SetBookmarkStyles(CodeDocumentViewModel codeDocumentViewModel,
        List<BookmarkStyle> bookmarkStyles, CancellationToken cancellationToken)
    {
        codeDocumentViewModel.BookmarkStyles = bookmarkStyles;
        codeDocumentViewModel.Configuration.BookmarkStyles = bookmarkStyles;

        await _configurationHelper.SaveConfiguration(codeDocumentViewModel.Configuration, cancellationToken);
    }

    public static int GetIndex(CodeDocumentViewModel codeDocumentViewModel, BookmarkStyle bookmarkStyle)
    {
        return codeDocumentViewModel.BookmarkStyles.FindIndex(b =>
            b.BackgroundColor == bookmarkStyle.BackgroundColor &&
            b.ForegroundColor == bookmarkStyle.ForegroundColor);
    }

    private static List<BookmarkStyle> GetDefaultBookmarkStyles()
        => new()
        {
            new BookmarkStyle(Brushes.LightYellow.Color.ToString(), Brushes.Black.Color.ToString()),
            new BookmarkStyle(Brushes.PaleVioletRed.Color.ToString(), Brushes.White.Color.ToString()),
            new BookmarkStyle(Brushes.LightGreen.Color.ToString(), Brushes.Black.Color.ToString()),
            new BookmarkStyle(Brushes.LightBlue.Color.ToString(), Brushes.Black.Color.ToString()),
            new BookmarkStyle(Brushes.MediumPurple.Color.ToString(), Brushes.White.Color.ToString()),
            new BookmarkStyle(Brushes.LightGray.Color.ToString(), Brushes.Black.Color.ToString()),
        };
}
