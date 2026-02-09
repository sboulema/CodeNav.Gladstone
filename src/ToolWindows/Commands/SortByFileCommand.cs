using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class SortByFileCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.SortByFileCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.SortByNamespace, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        codeDocumentService.CodeDocumentViewModel.SortOrder = SortOrderEnum.SortByFile;
        var sorted = SortHelper.Sort(codeDocumentService.CodeDocumentViewModel);
        codeDocumentService.CodeDocumentViewModel.CodeDocument.Clear();
        codeDocumentService.CodeDocumentViewModel.CodeDocument.AddRange(sorted);
    }
}
