namespace CodeNav.Test.MapperTests;

[TestFixture]
internal class TestEmptyInterface : BaseTest
{
    [Test]
    public async Task ShouldBeOk()
    {
        var codeItems = await MapToCodeItems("TestEmptyInterface.cs");

        Assert.That(codeItems.Any(), Is.True);

        // First item should be a namespace
        Assert.That(codeItems.First().Kind, Is.EqualTo(CodeItemKindEnum.Namespace));

        // Namespace item should not have members
        Assert.That((codeItems.First() as IMembers).Members, Has.Count.EqualTo(0));
    }
}
