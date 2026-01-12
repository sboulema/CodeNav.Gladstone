using System.Runtime.Serialization;
using System.Windows;
using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Models;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.ViewModels;

[DataContract]
public class CodeDocumentViewModel : NotifyPropertyChangedObject
{
    public CodeDocumentViewModel()
    {
        SortCommand = Sort();
    }

    public VisualStudioExtensibility Extensibility { get; set; }

    public CodeDocumentService CodeDocumentService { get; set; }

    public SortOrderEnum SortOrder;

    public List<string> HistoryItemIds = [];

    public List<FilterRule> FilterRules = [];

    #region Dependency properties

    [DataMember]
    public ObservableList<CodeItem> CodeDocument { get; set; } = [];

    private Visibility _showFilterToolbarVisibility = Visibility.Visible;

    /// <summary>
    /// Visibility of the filter toolbar.
    /// </summary>
    [DataMember]
    public Visibility ShowFilterToolbarVisibility
    {
        get => _showFilterToolbarVisibility;
        set => SetProperty(ref _showFilterToolbarVisibility, value);
    }

    private string _filterText = string.Empty;

    /// <summary>
    /// Text to filter code items by name.
    /// </summary>
    [DataMember]
    public string FilterText
    {
        get => _filterText;
        set
        {
            SetProperty(ref _filterText, value);
            VisibilityHelper.SetCodeItemVisibility(this);
        }
    }

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

            // TODO: How are we going to sort, custom toolbar or default toolbar?
            //Configuration.SortOrder = sortOrder;
            SortOrder = sortOrder;
            SortHelper.Sort(this);

            //await ConfigurationHelper.SaveConfiguration(Configuration, cancellationToken);
        });
    }

    #endregion
}
