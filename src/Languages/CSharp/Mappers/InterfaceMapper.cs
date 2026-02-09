using CodeNav.Constants;
using CodeNav.Mappers;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Extensibility;
using System.Collections.Immutable;

namespace CodeNav.Languages.CSharp.Mappers;

public static class InterfaceMapper
{
    public static bool IsPartOfImplementedInterface(IEnumerable<CodeImplementedInterfaceItem> implementedInterfaces,
        CodeItem item)
    {
        return item != null && implementedInterfaces.SelectMany(i => i.Members.Select(m => m.Id)).Contains(item.Id);
    }

    public static List<CodeImplementedInterfaceItem> MapImplementedInterfaces(SyntaxNode member,
        SemanticModel semanticModel, SyntaxTree tree, CodeDocumentViewModel codeDocumentViewModel)
    {
        var implementedInterfaces = new List<CodeImplementedInterfaceItem>();

        ISymbol? symbol;
        try
        {
            symbol = semanticModel.GetDeclaredSymbol(member);
        }
        catch (Exception)
        {
            return implementedInterfaces;
        }

        if (symbol is not INamedTypeSymbol classSymbol)
        {
            return implementedInterfaces;
        }

        var interfacesList = new List<INamedTypeSymbol>();
        GetInterfaces(interfacesList, classSymbol.Interfaces);

        foreach (INamedTypeSymbol implementedInterface in interfacesList.Distinct())
        {
            implementedInterfaces.Add(MapImplementedInterface(implementedInterface.Name,
                implementedInterface.GetMembers(), classSymbol, member, semanticModel, tree, codeDocumentViewModel));
        }

        return implementedInterfaces;
    }

    /// <summary>
    /// Recursively get the interfaces implemented by the class.
    /// This ignores interfaces implemented by any base class, contrary to the .Allinterfaces behavior
    /// </summary>
    /// <param name="interfacesFound">List of all interfaces found</param>
    /// <param name="source">Implemented interfaces</param>
    private static void GetInterfaces(List<INamedTypeSymbol> interfacesFound, ImmutableArray<INamedTypeSymbol> source)
    {
        interfacesFound.AddRange(source);
        foreach (var interfaceItem in source)
        {
            GetInterfaces(interfacesFound, interfaceItem.Interfaces);
        }
    }

    public static CodeImplementedInterfaceItem MapImplementedInterface(string name,
        ImmutableArray<ISymbol> members, INamedTypeSymbol implementingClass, SyntaxNode currentClass,
        SemanticModel semanticModel, SyntaxTree tree, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = new CodeImplementedInterfaceItem
        {
            Name = name,
            FullName = name,
            Id = name,
            Kind = CodeItemKindEnum.ImplementedInterface,
            IsExpanded = true
        };

        foreach (var member in members)
        {
            var implementation = implementingClass.FindImplementationForInterfaceMember(member);
            if (implementation == null || !implementation.DeclaringSyntaxReferences.Any())
            {
                continue;
            }

            // Ignore interface members not directly implemented in the current class
            if (!implementation.ContainingSymbol.Equals(implementingClass))
            {
                continue;
            }

            // Ignore interface members not directly implemented in the current file (partial class)
            if (implementation.DeclaringSyntaxReferences != null &&
                implementation.DeclaringSyntaxReferences.Any() &&
                implementation.DeclaringSyntaxReferences.First().SyntaxTree.FilePath != currentClass.SyntaxTree.FilePath)
            {
                continue;
            }

            var reference = implementation.DeclaringSyntaxReferences.First();
            var declarationSyntax = reference.GetSyntax();

            if (declarationSyntax is not MemberDeclarationSyntax memberDeclaration)
            {
                continue;
            }

            var interfaceMember = DocumentMapper.MapMember(memberDeclaration, tree, semanticModel, codeDocumentViewModel);
            if (interfaceMember == null)
            {
                continue;
            }

            interfaceMember.OverlayMoniker = ImageMoniker.KnownValues.InterfacePublic;
            item.Members.Add(interfaceMember);
        }

        if (item.Members.Any())
        {
            item.StartLine = item.Members.Min(i => i.StartLine);
            item.EndLine = item.Members.Max(i => i.EndLine);
        }

        return item;
    }

    public static CodeInterfaceItem MapInterface(InterfaceDeclarationSyntax? member,
        SemanticModel semanticModel, SyntaxTree tree, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = BaseMapper.MapBase<CodeInterfaceItem>(member, member.Identifier,
            member.Modifiers, semanticModel, codeDocumentViewModel);

        item.Kind = CodeItemKindEnum.Interface;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        if (TriviaSummaryMapper.HasSummary(member))
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        var regions = RegionMapper.MapRegions(tree, member.Span, codeDocumentViewModel);

        foreach (var interfaceMember in member.Members)
        {
            var memberItem = DocumentMapper.MapMember(interfaceMember, tree, semanticModel, codeDocumentViewModel);
            if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
            {
                item.Members.Add(memberItem);
            }
        }

        // Add regions to interface if they have a region member inside them
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
}
