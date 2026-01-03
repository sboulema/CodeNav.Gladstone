using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav.Services;

internal class CodeDocumentService(
    VisualStudioExtensibility extensibility,
    ConfigurationHelper configurationHelper)
{
    private readonly Dictionary<Uri, CodeDocumentViewModel> codeDocumentViewModels = [];

    /// <summary>
    /// DataContext for the CodeNav tool window.
    /// </summary>
    public CodeDocumentViewModel CodeDocumentViewModel { get; set; } = new();

    public async Task UpdateCodeDocumentViewModel(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var configuration = await configurationHelper.GetConfiguration();

        var codeItems = await DocumentMapper.MapDocument(textView.Document, configuration, cancellationToken);

        var codeDocumentViewModel = GetViewModel(textView.Uri);

        codeDocumentViewModel.CodeDocument = [.. codeItems];

        UpdateDataContext(codeDocumentViewModel);
    }

    private CodeDocumentViewModel GetViewModel(Uri uri)
    {
        codeDocumentViewModels.TryGetValue(uri, out var viewModel);

        viewModel ??= new CodeDocumentViewModel();

        return viewModel;
    }

    /// <summary>
    /// Update the DataContext of the CodeNav tool window.
    /// </summary>
    /// <remarks>
    /// Update individual properties to avoid replacing the entire DataContext
    /// and not triggering notifications in the UI.
    /// </remarks>
    /// <param name="codeDocumentViewModel"></param>
    private void UpdateDataContext(CodeDocumentViewModel codeDocumentViewModel)
    {
        //CodeDocumentViewModel.CodeDocument = codeDocumentViewModel.CodeDocument;

        //CodeDocumentViewModel.CodeDocument = new()
        //{
        //    new CodeItem
        //    {
        //        Name = "MyClass",
        //        Kind = CodeItemKindEnum.Class,
        //        StartLine = 1,
        //        EndLine = 20,
        //    },
        //};

        codeDocumentViewModel
            .CodeDocument
            .Add(new()
            {
                Name = "MyClass",
                Kind = CodeItemKindEnum.Class,
                StartLine = 1,
                EndLine = 20,
            });
    }

    public bool RemoveViewModel(Uri uri)
        => codeDocumentViewModels.Remove(uri);
}
