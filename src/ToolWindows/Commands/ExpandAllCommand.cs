using CodeNav.Helpers;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class ExpandAllCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.ExpandAllCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.ExpandAll, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        OutliningHelper.ExpandAll(codeDocumentService.CodeDocumentViewModel);
    }
}
