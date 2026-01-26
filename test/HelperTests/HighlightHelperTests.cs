using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Test.HelperTests;

[TestFixture]
internal class HighlightHelperTests : BaseTest
{
    [Test]
    public async Task CurrentItemShouldBeHighlighted()
    {
        var document = await MapToCodeDocumentViewModel("TestProperties.cs");

        HighlightHelper.HighlightCurrentItem(document, 13);

        var highlightedClass = (document.CodeDocument.First() as IMembers).Members.First() as CodeClassItem;
        var highlightedItem = highlightedClass.Members[2];

        Assert.That(highlightedItem.IsHighlighted, Is.True);
    }

    [Test]
    public async Task OnlyOneItemShouldBeHighlighted()
    {
        var document = await MapToCodeDocumentViewModel("TestProperties.cs");

        HighlightHelper.HighlightCurrentItem(document, 15);

        HighlightHelper.HighlightCurrentItem(document, 20);


        var highlightedItems = new List<CodeItem>();
        FindHighlightedItems(highlightedItems, document.CodeDocument);

        Assert.That(highlightedItems, Has.Count.EqualTo(1));
    }

    private static void FindHighlightedItems(List<CodeItem> found, ObservableList<CodeItem> source)
    {
        foreach (var item in source)
        {
            if (item.Kind == CodeItemKindEnum.Property &&
                item.IsHighlighted)
            {
                found.Add(item);
            }

            if (item is IMembers)
            {
                FindHighlightedItems(found, (item as IMembers).Members);
            }
        }
    }
}
