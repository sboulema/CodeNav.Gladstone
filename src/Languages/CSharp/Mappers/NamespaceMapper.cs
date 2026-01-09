using CodeNav.Constants;
using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers;

public static class NamespaceMapper
{
    public static CodeNamespaceItem MapNamespace(NamespaceDeclarationSyntax member,
        SemanticModel semanticModel, SyntaxTree tree, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = BaseMapper.MapBase<CodeNamespaceItem>(member, member.Name, semanticModel, codeDocumentViewModel);
        item.Kind = CodeItemKindEnum.Namespace;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.IgnoreVisibility = VisibilityHelper.GetIgnoreVisibility(item);

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        var regions = RegionMapper.MapRegions(tree, member.Span, configuration);

        foreach (var namespaceMember in member.Members)
        {
            var memberItem = DocumentMapper.MapMember(namespaceMember, tree, semanticModel, configuration, codeDocumentViewModel);
            if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
            {
                item.Members.AddIfNotNull(memberItem);
            }
        }

        // Add regions to namespace if they are not present in any children of the namespace
        if (regions.Any())
        {
            foreach (var region in regions)
            {
                if (item.Members.Flatten().FilterNull().Any(i => i.Id == region?.Id) == false)
                {
                    item.Members.AddIfNotNull(region);
                }
            }
        }

        return item;
    }

    public static CodeNamespaceItem MapNamespace(BaseNamespaceDeclarationSyntax member,
        SemanticModel semanticModel, SyntaxTree tree, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = BaseMapper.MapBase<CodeNamespaceItem>(member, member.Name, semanticModel, codeDocumentViewModel);
        item.Kind = CodeItemKindEnum.Namespace;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.IgnoreVisibility = VisibilityHelper.GetIgnoreVisibility(item);

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        var regions = RegionMapper.MapRegions(tree, member.Span, configuration);

        foreach (var namespaceMember in member.Members)
        {
            var memberItem = DocumentMapper.MapMember(namespaceMember, tree, semanticModel, configuration, codeDocumentViewModel);
            if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
            {
                item.Members.AddIfNotNull(memberItem);
            }
        }

        // Add regions to namespace if they are not present in any children of the namespace
        if (regions.Any())
        {
            foreach (var region in regions)
            {
                if (item.Members.Flatten().FilterNull().Any(i => i.Id == region?.Id) == false)
                {
                    item.Members.AddIfNotNull(region);
                }
            }
        }

        return item;
    }
}
