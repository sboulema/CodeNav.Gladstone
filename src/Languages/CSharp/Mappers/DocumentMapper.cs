using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Extensibility.Editor;
using CodeNav.ViewModels;
using CodeNav.Models;

namespace CodeNav.Languages.CSharp.Mappers;

internal class DocumentMapper
{
    public static async Task<IEnumerable<CodeItem>> MapDocument(ITextDocumentSnapshot documentSnapshot,
        Configuration configuration,
        CancellationToken cancellationToken)
    {
        var tree = CSharpSyntaxTree.ParseText(documentSnapshot.Text.CopyToString(), cancellationToken: cancellationToken);
        var msCorLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { msCorLib });
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = (CompilationUnitSyntax)await tree.GetRootAsync(cancellationToken);

        return root.Members
            .Where(member => member != null)
            .Select(member => MapMember(member, tree, semanticModel, configuration))
            .Where(codeItem => codeItem != null)
            .Cast<CodeItem>();
    }

    public static SemanticModel? GetCSharpSemanticModel(SyntaxTree tree)
    {
        SemanticModel semanticModel;

        try
        {
            var msCorLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { msCorLib });
            semanticModel = compilation.GetSemanticModel(tree);
        }
        catch (ArgumentException)
        {
            return null;
        }

        return semanticModel;
    }

    public static CodeItem? MapMember(
        SyntaxNode member, SyntaxTree tree, SemanticModel semanticModel,
        Configuration configuration, bool mapBaseClass = true)
        => member.Kind() switch
            {
                SyntaxKind.MethodDeclaration when member is MethodDeclarationSyntax memberSyntax
                    => MethodMapper.MapMethod(memberSyntax, semanticModel, configuration),
                SyntaxKind.EnumDeclaration when member is EnumDeclarationSyntax enumSyntax
                    => EnumMapper.MapEnum(enumSyntax, semanticModel, tree, configuration),
                SyntaxKind.EnumMemberDeclaration when member is EnumMemberDeclarationSyntax enumMemberSyntax
                    => EnumMapper.MapEnumMember(enumMemberSyntax, semanticModel, configuration),
                SyntaxKind.InterfaceDeclaration when member is InterfaceDeclarationSyntax interfaceSyntax
                    => InterfaceMapper.MapInterface(interfaceSyntax, semanticModel, tree, configuration),
                SyntaxKind.FieldDeclaration when member is FieldDeclarationSyntax fieldSyntax
                    => FieldMapper.MapField(fieldSyntax, semanticModel, configuration),
                SyntaxKind.PropertyDeclaration when member is PropertyDeclarationSyntax propertySyntax
                    => PropertyMapper.MapProperty(propertySyntax, semanticModel, configuration),
                SyntaxKind.StructDeclaration when member is StructDeclarationSyntax structSyntax
                    => StructMapper.MapStruct(structSyntax, semanticModel, tree, configuration),
                SyntaxKind.ClassDeclaration when member is ClassDeclarationSyntax classSyntax
                    => ClassMapper.MapClass(classSyntax, semanticModel, tree, configuration, mapBaseClass),
                SyntaxKind.EventFieldDeclaration when member is EventFieldDeclarationSyntax eventFieldSyntax
                    => DelegateEventMapper.MapEvent(eventFieldSyntax, semanticModel, configuration),
                SyntaxKind.DelegateDeclaration when member is DelegateDeclarationSyntax delegateSyntax
                    => DelegateEventMapper.MapDelegate(delegateSyntax, semanticModel, configuration),
                SyntaxKind.FileScopedNamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, configuration),
                SyntaxKind.NamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, configuration),
                SyntaxKind.RecordDeclaration when member is RecordDeclarationSyntax recordSyntax
                    => RecordMapper.MapRecord(recordSyntax, semanticModel, configuration),
                SyntaxKind.ConstructorDeclaration when member is ConstructorDeclarationSyntax constructorSyntax
                    => MethodMapper.MapConstructor(constructorSyntax, semanticModel, configuration),
                SyntaxKind.IndexerDeclaration when member is IndexerDeclarationSyntax indexerSyntax
                    => IndexerMapper.MapIndexer(indexerSyntax, semanticModel, configuration),
                _ => null,
            };
}