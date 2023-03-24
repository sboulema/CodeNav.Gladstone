using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

namespace CodeNav.ViewModels;

public class BookmarkStyle : NotifyPropertyChangedObject
{
    private string _backgroundColor = string.Empty;

    [DataMember]
    public string BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    private string _foregroundColor = string.Empty;

    [DataMember]
    public string ForegroundColor
    {
        get => _foregroundColor;
        set => SetProperty(ref _foregroundColor, value);
    }

    public BookmarkStyle()
    {

    }

    public BookmarkStyle(string backgroundColor, string foregroundColor)
    {
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
    }
}
