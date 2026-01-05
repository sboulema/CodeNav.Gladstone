using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CodeNav.ViewModels;
using CodeNav.Constants;
using CodeNav.Models;
using CodeNav.Mappers;
using CodeNav.Extensions;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeNav.Languages.CSharp.Mappers;

/// <summary>
/// Used to map the body of a method
/// </summary>
public static class StatementMapper
{
    public static ObservableList<CodeItem> MapStatement(StatementSyntax? statement,
        SemanticModel semanticModel, Configuration configuration)
    {
        if (statement == null)
        {
            return [];
        }

        CodeItem? item;

        switch (statement.Kind())
        {
            case SyntaxKind.SwitchStatement:
                item = MapSwitch(statement as SwitchStatementSyntax, semanticModel, configuration);
                return item != null
                    ? [item]
                    : [];
            case SyntaxKind.Block:
                if (statement is not BlockSyntax blockSyntax)
                {
                    return [];
                }

                return MapStatements(blockSyntax.Statements, semanticModel, configuration);
            case SyntaxKind.TryStatement:
                if (statement is not TryStatementSyntax trySyntax)
                {
                    return [];
                }

                return MapStatement(trySyntax.Block, semanticModel, configuration);
            case SyntaxKind.LocalFunctionStatement:
                if (statement is not LocalFunctionStatementSyntax syntax)
                {
                    return [];
                }

                item = MethodMapper.MapMethod(syntax, semanticModel, configuration);
                return item != null
                    ? [item]
                    : [];
            default:
                return [];
        }
    }

    public static ObservableList<CodeItem> MapStatement(BlockSyntax? statement, SemanticModel semanticModel, Configuration configuration) 
        => MapStatement(statement as StatementSyntax, semanticModel, configuration);

    public static ObservableList<CodeItem> MapStatements(SyntaxList<StatementSyntax> statements,
        SemanticModel semanticModel, Configuration configuration)
    {
        var list = new ObservableList<CodeItem>();

        if (statements.Any() != true)
        {
            return list;
        }

        foreach (var statement in statements)
        {
            list.AddRange(MapStatement(statement, semanticModel, configuration));
        }

        return list;
    }

    /// <summary>
    /// Map a switch statement
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="control"></param>
    /// <param name="semanticModel"></param>
    /// <returns></returns>
    private static CodeItem? MapSwitch(SwitchStatementSyntax? statement,
        SemanticModel semanticModel, Configuration configuration)
    {
        if (statement == null)
        {
            return null;
        }

        var item = BaseMapper.MapBase<CodeClassItem>(statement, statement.Expression.ToString(), semanticModel, configuration);
        item.Name = $"Switch {item.Name}";
        item.Kind = CodeItemKindEnum.Switch;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.BorderColor = Colors.DarkGray.ToString();
        item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

        // Map switch cases
        foreach (var section in statement.Sections)
        {
            item.Members.AddIfNotNull(MapSwitchSection(section, semanticModel, configuration));
        }

        return item;
    }

    /// <summary>
    /// Map the individual cases within a switch statement
    /// </summary>
    /// <param name="section"></param>
    /// <param name="control"></param>
    /// <param name="semanticModel"></param>
    /// <returns></returns>
    private static CodeItem? MapSwitchSection(SwitchSectionSyntax? section,
        SemanticModel semanticModel, Configuration configuration)
    {
        if (section == null)
        {
            return null;
        }

        var item = BaseMapper.MapBase<CodePropertyItem>(section, section.Labels.First().ToString(), semanticModel, configuration);
        item.Tooltip = TooltipMapper.Map(item.Access, item.ReturnType, item.Name, string.Empty);
        item.Id = item.FullName;
        item.Kind = CodeItemKindEnum.SwitchSection;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        return item;
    }
}
