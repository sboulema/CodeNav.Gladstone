using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Templates;

/// <summary>
/// Initializes a new instance of the <see cref="CodeNavMarginDataTemplate" /> class.
/// </summary>
/// <param name="dataContext">
/// Data context of the remote control which can be referenced from xaml through data binding.
/// </param>
internal class CodeNavMarginDataTemplate(object? dataContext) : RemoteUserControl(dataContext)
{
}
