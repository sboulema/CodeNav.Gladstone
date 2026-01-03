using CodeNav.Constants;
using CodeNav.Services;
using CodeNav.ViewModels;
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
    private static ToolbarConfiguration Toolbar => new("%ToolWindowSample.MyToolWindow.Toolbar.DisplayName%")
    {
        Children = [
            //ToolbarChild.Command<CommitToolbarCommand>(),
            //ToolbarChild.Command<RevertToolbarCommand>(),
            //ToolbarChild.Command<HideFilesToolbarCommand>(),
            //ToolbarChild.Command<RefreshToolbarCommand>(),
        ],
    };

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        Title = "CodeNav";
        //var viewModel = await documentService.GetViewModel();

        //_dataContext.CodeDocument = new()
        //{
        //    new CodeItem
        //    {
        //        Name = "MyClass",
        //        Kind = CodeItemKindEnum.Class,
        //        StartLine = 1,
        //        EndLine = 20,
        //    },
        //};
        //_dataContext.CodeDocument = viewModel.CodeDocument;

        //documentService.CodeDocumentViewModel.CodeDocument = new()
        //{
        //    new CodeItem
        //    {
        //        Name = "MyClass",
        //        Kind = CodeItemKindEnum.Class,
        //        StartLine = 1,
        //        EndLine = 20,
        //    },
        //};
    }

    /// <inheritdoc />
    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        => Task.FromResult<IRemoteUserControl>(new CodeNavToolWindowControl(documentService.CodeDocumentViewModel));
}
