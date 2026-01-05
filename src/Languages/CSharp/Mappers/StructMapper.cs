using CodeNav.Constants;
using CodeNav.Extensions;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers;

public static class StructMapper
{
    public static CodeClassItem MapStruct(StructDeclarationSyntax member,
        SemanticModel semanticModel, SyntaxTree tree, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = BaseMapper.MapBase<CodeClassItem>(member, member.Identifier, member.Modifiers, semanticModel, configuration, codeDocumentViewModel);
        item.Kind = CodeItemKindEnum.Struct;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.BorderColor = Colors.DarkGray.ToString();

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        foreach (var structMember in member.Members)
        {
            item.Members.AddIfNotNull(DocumentMapper.MapMember(structMember, tree, semanticModel, configuration, codeDocumentViewModel));
        }

        return item;
    }
}
