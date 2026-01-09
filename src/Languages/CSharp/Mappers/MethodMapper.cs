using CodeNav.Constants;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Extensibility;
using System.Windows;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers;

public static class MethodMapper
{
    public static CodeItem MapMethod(MethodDeclarationSyntax member, SemanticModel semanticModel,
        Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
        => MapMethod(member, member.Identifier, member.Modifiers,
            member.Body, member.ReturnType as ITypeSymbol, member.ParameterList,
            CodeItemKindEnum.Method, semanticModel, configuration, codeDocumentViewModel);

    public static CodeItem MapMethod(LocalFunctionStatementSyntax member,
        SemanticModel semanticModel, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
        => MapMethod(member, member.Identifier, member.Modifiers,
            member.Body, member.ReturnType as ITypeSymbol, member.ParameterList,
            CodeItemKindEnum.LocalFunction, semanticModel, configuration, codeDocumentViewModel);

    private static CodeItem MapMethod(SyntaxNode node, SyntaxToken identifier,
        SyntaxTokenList modifiers, BlockSyntax? body, ITypeSymbol? returnType,
        ParameterListSyntax parameterList, CodeItemKindEnum kind,
        SemanticModel semanticModel, Configuration configuration, CodeDocumentViewModel codeDocumentViewModel)
    {
        CodeItem item;

        var statementsCodeItems = StatementMapper.MapStatement(body, semanticModel, configuration, codeDocumentViewModel);

        VisibilityHelper.SetCodeItemVisibility(statementsCodeItems);

        if (statementsCodeItems.Any(statement => statement.IsVisible == Visibility.Visible))
        {
            // Map method as item containing statements
            item = BaseMapper.MapBase<CodeClassItem>(node, identifier,modifiers, semanticModel, codeDocumentViewModel);
            ((CodeClassItem)item).Members.AddRange(statementsCodeItems);
        }
        else
        {
            // Map method as single item
            item = BaseMapper.MapBase<CodeFunctionItem>(node, identifier, modifiers, semanticModel, codeDocumentViewModel);
            ((CodeFunctionItem)item).ReturnType = TypeMapper.Map(returnType);
            ((CodeFunctionItem)item).Parameters = ParameterMapper.MapParameters(parameterList);
            item.Tooltip = TooltipMapper.Map(item.Access, ((CodeFunctionItem)item).ReturnType, item.Name, parameterList);
        }

        item.Id = IdMapper.MapId(item.FullName, parameterList);
        item.Kind = kind;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.StartLine = BaseMapper.GetStartLine(identifier);

        if (TriviaSummaryMapper.HasSummary(node) &&
            configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(node);
        }

        return item;
    }

    public static CodeItem MapConstructor(ConstructorDeclarationSyntax member,
        SemanticModel semanticModel, CodeDocumentViewModel codeDocumentViewModel)
    {
        var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.Identifier, member.Modifiers, semanticModel, codeDocumentViewModel);
        item.Parameters = ParameterMapper.MapParameters(member.ParameterList);
        item.Tooltip = TooltipMapper.Map(item.Access, item.ReturnType, item.Name, member.ParameterList);
        item.Id = IdMapper.MapId(member.Identifier, member.ParameterList);
        item.Kind = CodeItemKindEnum.Constructor;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
        item.OverlayMoniker = ImageMoniker.KnownValues.Add;

        return item;
    }
}
