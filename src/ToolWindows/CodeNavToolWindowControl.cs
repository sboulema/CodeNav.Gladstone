using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.ToolWindows;

internal class CodeNavToolWindowControl(object? dataContext, SynchronizationContext? synchronizationContext = null)
    : RemoteUserControl(dataContext, synchronizationContext)
{
}
