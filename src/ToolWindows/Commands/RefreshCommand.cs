using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class RefreshCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.RefreshCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Refresh, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        var textViewSnapshot = await clientContext.GetActiveTextViewAsync(cancellationToken);

        if (textViewSnapshot == null)
        {
            return;
        }

        await codeDocumentService.UpdateCodeDocumentViewModel(clientContext.Extensibility, textViewSnapshot, cancellationToken);
    }
}
