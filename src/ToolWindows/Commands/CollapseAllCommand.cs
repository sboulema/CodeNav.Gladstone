using CodeNav.Helpers;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class CollapseAllCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.CollapseAllCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.CollapseAll, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        OutliningHelper.CollapseAll(codeDocumentService.CodeDocumentViewModel);
    }
}
