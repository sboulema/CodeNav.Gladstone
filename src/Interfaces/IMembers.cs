using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Interfaces;

public interface IMembers
{
    ObservableList<CodeItem> Members { get; set; }

    bool IsExpanded { get; set; }
}
