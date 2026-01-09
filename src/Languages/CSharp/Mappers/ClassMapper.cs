using CodeNav.Constants;
using CodeNav.Extensions;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers;

public class ClassMapper
{
    public static CodeClassItem MapClass(ClassDeclarationSyntax member,
        SemanticModel semanticModel, SyntaxTree tree, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel,
        bool mapBaseClass)
    {
        var item = BaseMapper.MapBase<CodeClassItem>(member, member.Identifier, member.Modifiers, semanticModel, configuration, codeDocumentViewModel);
        item.Kind = CodeItemKindEnum.Class;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.Parameters = MapInheritance(member);
        item.BorderColor = Colors.DarkGray.ToString();
        item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

        if (TriviaSummaryMapper.HasSummary(member) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        var regions = RegionMapper.MapRegions(tree, member.Span, configuration);
        var implementedInterfaces = InterfaceMapper.MapImplementedInterfaces(member, semanticModel, tree, configuration, codeDocumentViewModel);

        // Map members from the base class
        if (mapBaseClass)
        {
            MapMembersFromBaseClass(member, regions, semanticModel, configuration, codeDocumentViewModel);
        }

        // Map class members
        foreach (var classMember in member.Members)
        {
            var memberItem = DocumentMapper.MapMember(classMember, tree, semanticModel, configuration, codeDocumentViewModel);
            if (memberItem != null && !InterfaceMapper.IsPartOfImplementedInterface(implementedInterfaces, memberItem)
                && !RegionMapper.AddToRegion(regions, memberItem))
            {
                item.Members.Add(memberItem);
            }
        }

        // Add implemented interfaces to class or region if they have a interface member inside them
        if (implementedInterfaces.Any())
        {
            foreach (var interfaceItem in implementedInterfaces)
            {
                if (interfaceItem.Members.Any())
                {
                    if (!RegionMapper.AddToRegion(regions, interfaceItem))
                    {
                        item.Members.Add(interfaceItem);
                    }
                }
            }
        }

        // Add regions to class if they have a region member inside them
        if (regions.Any())
        {
            foreach (var region in regions)
            {
                if (region?.Members.Any() == true)
                {
                    item.Members.Add(region);
                }
            }
        }

        return item;
    }

    private static string MapInheritance(ClassDeclarationSyntax member)
    {
        if (member?.BaseList == null)
        {
            return string.Empty;
        }

        var inheritanceList = (from BaseTypeSyntax bases in member.BaseList.Types select bases.Type.ToString()).ToList();

        return !inheritanceList.Any() ? string.Empty : $" : {string.Join(", ", inheritanceList)}";
    }

    private static void MapMembersFromBaseClass(ClassDeclarationSyntax member,
        ObservableCollection<CodeRegionItem> regions, SemanticModel semanticModel, Configuration configuration,
        CodeDocumentViewModel codeDocumentViewModel)
    {
        var classSymbol = semanticModel.GetDeclaredSymbol(member);
        var baseType = classSymbol?.BaseType;

        if (baseType == null ||
            baseType.SpecialType == SpecialType.System_Object)
        {
            return;
        }

        var baseRegion = new CodeRegionItem
        {
            Name = baseType.Name,
            FullName = baseType.Name,
            Id = baseType.Name,
            Tooltip = baseType.Name,
            BorderColor = Colors.DarkGray.ToString(),
            FontSize = configuration.FontSize - 2,
            Kind = CodeItemKindEnum.BaseClass,
        };

        regions.Add(baseRegion);

        var baseSyntaxTree = baseType.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;

        if (baseSyntaxTree == null)
        {
            return;
        }

        var baseSemanticModel = DocumentMapper.GetCSharpSemanticModel(baseSyntaxTree);

        if (baseSemanticModel == null)
        {
            return;
        }

        var baseTypeMembers = baseType?.GetMembers();

        if (baseTypeMembers == null)
        {
            return;
        }

        foreach (var inheritedMember in baseTypeMembers)
        {
            var syntaxNode = inheritedMember.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            if (syntaxNode.IsKind(SyntaxKind.VariableDeclarator))
            {
                syntaxNode = syntaxNode?.Parent?.Parent;
            }

            if (syntaxNode == null)
            {
                continue;
            }

            var memberItem = DocumentMapper.MapMember(syntaxNode, syntaxNode.SyntaxTree,
                baseSemanticModel, configuration, codeDocumentViewModel, mapBaseClass: false);

            baseRegion.Members.AddIfNotNull(memberItem);
        }
    }

}
