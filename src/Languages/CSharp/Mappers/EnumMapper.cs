using CodeNav.Constants;
using CodeNav.Extensions;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers;

public class EnumMapper
{
    public static CodeItem MapEnumMember(EnumMemberDeclarationSyntax member,
        SemanticModel semanticModel, Configuration configuration)
    {
        var item = BaseMapper.MapBase<CodeItem>(member, member.Identifier, semanticModel, configuration);
        item.Kind = CodeItemKindEnum.EnumMember;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        return item;
    }

    public static CodeClassItem MapEnum(EnumDeclarationSyntax member,
        SemanticModel semanticModel, SyntaxTree tree, Configuration configuration)
    {
        var item = BaseMapper.MapBase<CodeClassItem>(member, member.Identifier, member.Modifiers, semanticModel, configuration);
        item.Kind = CodeItemKindEnum.Enum;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.Parameters = MapMembersToString(member.Members);
        item.BorderColor = Colors.DarkGray.ToString();

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        foreach (var enumMember in member.Members)
        {
            item.Members.AddIfNotNull(DocumentMapper.MapMember(enumMember, tree, semanticModel, configuration));
        }

        return item;
    }

    private static string MapMembersToString(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        => $"{string.Join(", ", members.Select(member => member.Identifier.Text))}";
}
