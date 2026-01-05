using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav.Services;

internal class CodeDocumentService(
    ConfigurationHelper configurationHelper)
{
    private readonly Dictionary<Uri, CodeDocumentViewModel> codeDocumentViewModels = [];

    /// <summary>
    /// DataContext for the tool window.
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

        // Update the DataContext for the tool window.
        //CodeDocumentViewModel.CodeDocument.Clear();

        foreach (var item in codeItems)
        {
            CodeDocumentViewModel.CodeDocument.Add(item);
        }

        CodeDocumentViewModel.CodeDocument.Add(new()
        {
            Name = "test"
        });

        CodeDocumentViewModel.CodeDocument.Add(new CodeNamespaceItem
        {
            Name = "namespace"
        });
    }

    private CodeDocumentViewModel GetViewModel(Uri uri)
    {
        codeDocumentViewModels.TryGetValue(uri, out var viewModel);

        viewModel ??= new CodeDocumentViewModel();

        return viewModel;
    }

    public bool RemoveViewModel(Uri uri)
        => codeDocumentViewModels.Remove(uri);
}
