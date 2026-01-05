using CodeNav.Constants;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Helpers;

public static class SortHelper
{
    public static ObservableList<CodeItem> Sort(CodeDocumentViewModel viewModel)
        => Sort(viewModel.CodeDocument, viewModel.SortOrder);

    public static ObservableList<CodeItem> Sort(ObservableList<CodeItem> document, SortOrderEnum sortOrder)
        => sortOrder switch
        {
            SortOrderEnum.SortByFile => SortByFile(document),
            SortOrderEnum.SortByName => SortByName(document),
            _ => document,
        };

    private static ObservableList<CodeItem> SortByName(ObservableList<CodeItem> document)
    {
        document = [.. document.OrderBy(c => c.Name)];

        foreach (var item in document)
        {
            if (item is CodeClassItem codeClassItem)
            {
                codeClassItem.Members = SortByName(codeClassItem.Members);
            }
            if (item is CodeNamespaceItem codeNamespaceItem)
            {
                codeNamespaceItem.Members = SortByName(codeNamespaceItem.Members);
            }
        }

        return document;
    }

    private static ObservableList<CodeItem> SortByFile(ObservableList<CodeItem> document)
    {
        document = [.. document.OrderBy(c => c.StartLine)];

        foreach (var item in document)
        {
            if (item is CodeClassItem codeClassItem)
            {
                codeClassItem.Members = SortByFile(codeClassItem.Members);
            }
            if (item is CodeNamespaceItem codeNamespaceItem)
            {
                codeNamespaceItem.Members = SortByFile(codeNamespaceItem.Members);
            }
        }

        return document;
    }
}
