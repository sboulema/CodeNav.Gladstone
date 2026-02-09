namespace CodeNav.Test.MapperTests;

[TestFixture]
internal class TestInterface4 : BaseTest
{
    [Test]
    public async Task TestClassImplementedInterfaceAndBaseImplementedInterfaceShouldBeOk()
    {
        var codeItems = await MapToCodeItems("TestInterface4.cs");

        Assert.That(codeItems.Any(), Is.True);

        // last item should be the implementing class
        var implementingClass = (codeItems.First() as IMembers)?.Members.Last() as CodeClassItem;

        Assert.That(implementingClass?.Kind, Is.EqualTo(CodeItemKindEnum.Class));
        Assert.That(implementingClass.Members, Has.Count.EqualTo(2));

        var method = implementingClass.Members.First() as CodeFunctionItem;

        Assert.That(method?.Kind, Is.EqualTo(CodeItemKindEnum.Method));
        Assert.That(method.Name, Is.EqualTo("ClassAMethod"));
    }
}
