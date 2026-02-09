using CodeNav.Constants;
using CodeNav.Interfaces;
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
        document = [.. document.OrderBy(codeItem => codeItem.Name)];

        foreach (var item in document)
        {
            if (item is IMembers membersItem)
            {
                membersItem.Members = SortByName(membersItem.Members);
            }
        }

        return document;
    }

    private static ObservableList<CodeItem> SortByFile(ObservableList<CodeItem> document)
    {
        document = [.. document.OrderBy(codeItem => codeItem.Span.Start)];

        foreach (var item in document)
        {
            if (item is IMembers membersItem)
            {
                membersItem.Members = SortByFile(membersItem.Members);
            }
        }

        return document;
    }
}
