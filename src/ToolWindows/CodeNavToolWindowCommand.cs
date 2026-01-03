using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeNav.ToolWindows;

[VisualStudioContribution]
public class PendingChangesToolWindowCommand : Command
{
    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindowCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.DocumentOutline, IconSettings.IconAndText),
        EnabledWhen = ActivationConstraint.SolutionState(SolutionState.Exists),
        Placements = [CommandPlacement.KnownPlacements.ViewOtherWindowsMenu],
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await Extensibility.Shell().ShowToolWindowAsync<CodeNavToolWindow>(activate: true, cancellationToken);
    }
}
