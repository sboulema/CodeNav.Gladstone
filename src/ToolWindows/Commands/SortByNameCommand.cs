using CodeNav.Constants;
using CodeNav.Helpers;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class SortByNameCommand(VisualStudioExtensibility extensibility)
    : Command(extensibility)
{

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.SortByNameCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.SortAscending, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
        => await SettingsHelper.SaveSortOrder(Extensibility, SortOrderEnum.SortByName, cancellationToken);
}
