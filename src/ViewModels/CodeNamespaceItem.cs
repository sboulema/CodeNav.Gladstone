using System.Windows;

namespace CodeNav.ViewModels;

public class CodeNamespaceItem : CodeClassItem
{
    public Visibility IgnoreVisibility { get; set; }

    public Visibility NotIgnoreVisibility
        => IgnoreVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
}
