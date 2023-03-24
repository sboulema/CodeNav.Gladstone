﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace CodeNav.Languages.CSharp.Mappers;

/// <summary>
/// Creates an unique id for a CodeItem based on its name and parameters
/// </summary>
public static class IdMapper
{
    public static string MapId(SyntaxToken identifier, ParameterListSyntax parameters)
    {
        return MapId(identifier.Text, parameters);
    }

    public static string MapId(string name, ParameterListSyntax parameters)
    {
        return name + ParameterMapper.MapParameters(parameters, true, false);
    }

    public static string MapId(string name, ImmutableArray<IParameterSymbol> parameters, bool useLongNames, bool prettyPrint)
    {
        return name + MapParameters(parameters, useLongNames, prettyPrint);
    }

    private static string MapParameters(ImmutableArray<IParameterSymbol> parameters, bool useLongNames = false, bool prettyPrint = true)
    {
        var paramList = (from IParameterSymbol parameter in parameters select TypeMapper.Map(parameter.Type, useLongNames)).ToList();
        return prettyPrint ? $"({string.Join(", ", paramList)})" : string.Join(string.Empty, paramList);
    }
}
