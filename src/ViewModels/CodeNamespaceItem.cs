using System.Runtime.Serialization;
using System.Windows;

namespace CodeNav.ViewModels;

[DataContract]
public class CodeNamespaceItem : CodeClassItem
{
    public CodeNamespaceItem()
    {
        DataTemplateType = "Namespace";
    }

    public Visibility IgnoreVisibility { get; set; }

    public Visibility NotIgnoreVisibility
        => IgnoreVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
}
