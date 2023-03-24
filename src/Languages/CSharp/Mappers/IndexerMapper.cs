using CodeNav.Constants;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers;

public class IndexerMapper
{
    public static CodeItem MapIndexer(IndexerDeclarationSyntax member,
        SemanticModel semanticModel, Configuration configuration)
    {
        var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.ThisKeyword, member.Modifiers, semanticModel, configuration);
        item.Type = TypeMapper.Map(member.Type);
        item.Parameters = ParameterMapper.MapParameters(member.ParameterList);
        item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, item.Parameters);
        item.Kind = CodeItemKindEnum.Indexer;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        return item;
    }
}
