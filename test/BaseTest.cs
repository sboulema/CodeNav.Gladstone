namespace CodeNav.Test;

internal abstract class BaseTest
{
    internal static async Task<IEnumerable<CodeItem>> MapToCodeItems(
        string fileName,
        CodeDocumentViewModel? codeDocumentViewModel = null)
    {
        var fileText = await GetFileText(fileName);

        var document = await DocumentMapper.MapDocument(fileText, codeDocumentViewModel ?? new(), default);

        return document;
    }

    internal static async Task<CodeDocumentViewModel> MapToCodeDocumentViewModel(
        string fileName,
        CodeDocumentViewModel? codeDocumentViewModel = null)
    {
        codeDocumentViewModel ??= new CodeDocumentViewModel();

        var codeItems = await MapToCodeItems(fileName, codeDocumentViewModel);
        codeDocumentViewModel.CodeDocument.AddRange(codeItems);
        return codeDocumentViewModel;
    }

    private static async Task<string> GetFileText(string fileName)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Files", fileName);
        return await File.ReadAllTextAsync(filePath);
    }
}
