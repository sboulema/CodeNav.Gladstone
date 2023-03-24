﻿using CodeNav.Constants;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers;

public static class FieldMapper
{
    public static CodeItem MapField(FieldDeclarationSyntax member, SemanticModel semanticModel,
        Configuration configuration)
    {
        var item = BaseMapper.MapBase<CodeItem>(member, member.Declaration.Variables.First().Identifier,
            member.Modifiers, semanticModel, configuration);

        item.Kind = IsConstant(member.Modifiers)
            ? CodeItemKindEnum.Constant
            : CodeItemKindEnum.Variable;
        item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

        if (TriviaSummaryMapper.HasSummary(member) && configuration.UseXMLComments)
        {
            item.Tooltip = TriviaSummaryMapper.Map(member);
        }

        return item;
    }

    private static bool IsConstant(SyntaxTokenList modifiers)
        => modifiers.Any(m => m.RawKind == (int)SyntaxKind.ConstKeyword);
}
