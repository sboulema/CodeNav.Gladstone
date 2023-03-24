using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Templates;

internal class CodeNavMarginDataTemplate : RemoteUserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeNavMarginDataTemplate" /> class.
    /// </summary>
    /// <param name="dataContext">
    /// Data context of the remote control which can be referenced from xaml through data binding.
    /// </param>
    public CodeNavMarginDataTemplate(object? dataContext)
        : base(dataContext)
    {
    }
}
