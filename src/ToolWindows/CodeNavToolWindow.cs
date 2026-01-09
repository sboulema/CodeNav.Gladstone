using CodeNav.Services;
using CodeNav.ToolWindows.Commands;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace CodeNav.ToolWindows;

[VisualStudioContribution]
internal class CodeNavToolWindow(CodeDocumentService documentService) : ToolWindow
{
    /// <inheritdoc />
    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.Floating,
        Toolbar = new ToolWindowToolbar(Toolbar),
        VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveSelectionFileName, @"\.cs$"),
    };

    [VisualStudioContribution]
    private static ToolbarConfiguration Toolbar => new("%CodeNav.CodeNavToolWindow.Toolbar.DisplayName%")
    {
        Children = [
            ToolbarChild.Command<RefreshCommand>(),
            ToolbarChild.Separator,
            ToolbarChild.Command<SortByFileCommand>(),
            ToolbarChild.Command<SortByNameCommand>(),
            // TODO: Add settings command
        ],
    };

    /// <inheritdoc />
    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        Title = "CodeNav";

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        => Task.FromResult<IRemoteUserControl>(new CodeNavToolWindowControl(documentService.CodeDocumentViewModel));
}
