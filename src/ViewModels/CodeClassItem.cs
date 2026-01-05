using CodeNav.Interfaces;
using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using System.Windows;

namespace CodeNav.ViewModels;

[DataContract]
public class CodeClassItem : CodeItem, IMembers, ICodeCollapsible
{
    public CodeClassItem()
    {
        DataTemplateType = "Class";
    }

    [DataMember]
    public ObservableList<CodeItem> Members { get; set; } = [];

    public string Parameters { get; set; } = string.Empty;

    private string _borderColor = string.Empty;

    [DataMember]
    public string BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }

    public event EventHandler? IsExpandedChanged;
    private bool _isExpanded;

    /// <summary>
    /// Gets or sets a value indicating whether the item is expanded in the tool window.
    /// </summary>
    [DataMember]
    public bool IsExpanded
    {
        get { return _isExpanded; }
        set
        {
            if (_isExpanded != value)
            {
                SetProperty(ref _isExpanded, value);   
                IsExpandedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Do we have any members that are not null and should be visible?
    /// If we don't hide the expander +/- symbol and the header border
    /// </summary>
    [DataMember]
    public Visibility HasMembersVisibility
        => Members.Any(m => m.IsVisible == Visibility.Visible)
            ? Visibility.Visible
            : Visibility.Collapsed;
}
