using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.ViewModels
{
    [DataContract]
    public class CodeDocumentViewModel : NotifyPropertyChangedObject
    {
        public CodeDocumentViewModel()
        {
            SortCommand = Sort();
        }

        private List<CodeItem> _codeDocument = new();

        [DataMember]
        public List<CodeItem> CodeDocument
        {
            get => _codeDocument;
            set => SetProperty(ref _codeDocument, value);
        }

        public ITextViewSnapshot? TextView { get; set; }

        public ITextDocumentSnapshot? TextDocumentSnapshot { get; set; }

        public Configuration Configuration { get; set; } = new();

        public DocumentHelper? DocumentHelper { get; set; }

        public HistoryHelper? HistoryHelper { get; set; }

        public ConfigurationHelper? ConfigurationHelper { get; set; }

        public bool ShowFilterToolbar => Configuration.ShowFilterToolbar;

        public Visibility ShowFilterToolbarVisibility => Configuration.ShowFilterToolbar
            ? Visibility.Visible
            : Visibility.Collapsed;

        public SortOrderEnum SortOrder;

        public Visibility BookmarksAvailable
            => Bookmarks.Any() ? Visibility.Visible : Visibility.Collapsed;

        public void AddBookmark(string id, int bookmarkStyleIndex)
        {
            if (Bookmarks.ContainsKey(id))
            {
                Bookmarks.Remove(id);
            }

            Bookmarks.Add(id, bookmarkStyleIndex);

            //NotifyPropertyChanged("BookmarksAvailable");
        }

        public void RemoveBookmark(string id)
        {
            Bookmarks.Remove(id);

            //NotifyPropertyChanged("BookmarksAvailable");
        }

        public void ClearBookmarks()
        {
            BookmarkHelper.ClearBookmarks(this);

            //NotifyPropertyChanged("BookmarksAvailable");
        }

        public Visibility ClearFilterVisibility =>
            string.IsNullOrEmpty(FilterText) ?
            Visibility.Collapsed : Visibility.Visible;

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                //NotifyPropertyChanged("ClearFilterVisibility");
            }
        }

        private Dictionary<string, int> _bookmarks = new();

        [DataMember]
        public Dictionary<string, int> Bookmarks
        {
            get => _bookmarks;
            set
            {
                _bookmarks = value;
                //NotifyPropertyChanged("BookmarksAvailable");
            }
        }

        public bool FilterOnBookmarks;

        [DataMember]
        public Uri? FilePath { get; set; }

        [DataMember]
        public List<BookmarkStyle> BookmarkStyles = new();

        [DataMember]
        public ObservableCollection<CodeItem> HistoryItems = new();

        [DataMember]
        public AsyncCommand SortCommand { get; }
        public AsyncCommand Sort()
        {
            return new AsyncCommand(async (parameter, cancellationToken) =>
            {
                if (parameter is not SortOrderEnum sortOrder)
                {
                    return;
                }

                Configuration.SortOrder = sortOrder;
                SortOrder = sortOrder;
                SortHelper.Sort(this);

                await ConfigurationHelper.SaveConfiguration(Configuration, cancellationToken);
            });
        }
    }
}
