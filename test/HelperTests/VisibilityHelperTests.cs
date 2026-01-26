using System.Windows;

namespace CodeNav.Test.HelperTests;

[TestFixture]
internal class VisibilityHelperTests : BaseTest
{
    [TestCase(false, Visibility.Visible)]
    [TestCase(true, Visibility.Collapsed)]
    public async Task EmptyItemsShouldRespectSetting(bool hideItemsWithoutChildren, Visibility expectedVisibility)
    {
        var codeDocumentViewModel = new CodeDocumentViewModel
        {
            FilterRules =
            [
                new()
                {
                    Access = CodeItemAccessEnum.All,
                    Kind = CodeItemKindEnum.Class,
                    Visible = true,
                    HideIfEmpty = hideItemsWithoutChildren
                }
            ]
        };

        codeDocumentViewModel = await MapToCodeDocumentViewModel("TestVisibility.cs", codeDocumentViewModel);

        VisibilityHelper.SetCodeItemVisibility(codeDocumentViewModel);

        var firstClass = (codeDocumentViewModel.CodeDocument.First() as IMembers).Members.First() as CodeClassItem;          

        Assert.That(firstClass.Visibility, Is.EqualTo(expectedVisibility));
    }
}
