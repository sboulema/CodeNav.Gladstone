using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeNav.Mappers;
using CodeNav.ViewModels;
using CodeNav.Constants;
using CodeNav.Models;

namespace CodeNav.Languages.CSharp.Mappers;

public class PropertyMapper
{
    public static CodePropertyItem MapProperty(PropertyDeclarationSyntax member,
        SemanticModel semanticModel, Configuration configuration)
    {
        var item = BaseMapper.MapBase<CodePropertyItem>(member, member.Identifier, member.Modifiers, semanticModel, configuration);
        item.Type = TypeMapper.Map(member.Type);

        if (member.AccessorList != null)
        {
            if (member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration))
            {
                item.Parameters += "get";
            }

            if (member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration))
            {
                item.Parameters += string.IsNullOrEmpty(item.Parameters) ? "set" : ",set";
            }

            if (!string.IsNullOrEmpty(item.Parameters))
            {
                item.Parameters = $" {{{item.Parameters}}}";
            }
        }

        item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, item.Parameters);
        item.Kind = CodeItemKindEnum.Property;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        return item;
    }
}
