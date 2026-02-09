using CodeNav.Dialogs.SettingsDialog;
using CodeNav.Helpers;
using CodeNav.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.RpcContracts.Notifications;

namespace CodeNav.ToolWindows.Commands;

[VisualStudioContribution]
internal class SettingsCommand(
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
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
        codeDocumentService.SettingsDialogData = await SettingsHelper.GetSettings(Extensibility, cancellationToken);

        var dialogResult = await Extensibility.Shell().ShowDialogAsync(
            content: new SettingsDialogControl(codeDocumentService.SettingsDialogData),
            title: "CodeNav Settings",
            options: new(DialogButton.OKCancel, DialogResult.OK),
            cancellationToken);

        if (dialogResult == DialogResult.Cancel)
        {
            return;
        }

        await SettingsHelper.SaveSettings(Extensibility, codeDocumentService.SettingsDialogData, cancellationToken);
    }
}
