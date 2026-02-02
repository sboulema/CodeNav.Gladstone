using CodeNav.Dialogs.SettingsDialog;
using CodeNav.Helpers;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.RpcContracts.Notifications;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class SettingsCommand(VisualStudioExtensibility extensibility)
    : Command(extensibility)
{
    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%CodeNav.CodeNavToolWindow.SettingsCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Settings, IconSettings.IconOnly)
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext clientContext, CancellationToken cancellationToken)
    {
        var settingsDialogData = await SettingsHelper.GetSettings(Extensibility, cancellationToken);

        var dialogResult = await Extensibility.Shell().ShowDialogAsync(
            content: new SettingsDialogControl(settingsDialogData),
            title: "CodeNav Settings",
            options: new(DialogButton.OKCancel, DialogResult.OK),
            cancellationToken);

        if (dialogResult == DialogResult.Cancel)
        {
            return;
        }

        await SettingsHelper.SaveSettings(Extensibility, settingsDialogData, cancellationToken);
    }
}
