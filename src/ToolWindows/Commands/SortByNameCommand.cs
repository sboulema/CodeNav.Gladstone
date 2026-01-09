using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class SortByNameCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.SortByNameCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.SortAscending, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        SortHelper.Sort(codeDocumentService.CodeDocumentViewModel.CodeDocument, SortOrderEnum.SortByName);
    }
}
