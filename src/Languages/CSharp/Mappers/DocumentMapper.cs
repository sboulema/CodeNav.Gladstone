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
        CodeDocumentViewModel codeDocumentViewModel,
        CancellationToken cancellationToken)
    {
        var tree = CSharpSyntaxTree.ParseText(documentSnapshot.Text.CopyToString(), cancellationToken: cancellationToken);
        var msCorLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create("CodeNavCompilation", [tree], [msCorLib]);
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = (CompilationUnitSyntax)await tree.GetRootAsync(cancellationToken);

        return root.Members
            .Where(member => member != null)
            .Select(member => MapMember(member, tree, semanticModel, configuration, codeDocumentViewModel))
            .Where(codeItem => codeItem != null)
            .Cast<CodeItem>();
    }

    public static SemanticModel? GetCSharpSemanticModel(SyntaxTree tree)
    {
        SemanticModel semanticModel;

        try
        {
            var msCorLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("CodeNavCompilation", [tree], [msCorLib]);
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
        Configuration configuration, 
        CodeDocumentViewModel codeDocumentViewModel,
        bool mapBaseClass = true)
        => member.Kind() switch
            {
                SyntaxKind.MethodDeclaration when member is MethodDeclarationSyntax memberSyntax
                    => MethodMapper.MapMethod(memberSyntax, semanticModel, configuration, codeDocumentViewModel),
                SyntaxKind.EnumDeclaration when member is EnumDeclarationSyntax enumSyntax
                    => EnumMapper.MapEnum(enumSyntax, semanticModel, tree, configuration, codeDocumentViewModel),
                SyntaxKind.EnumMemberDeclaration when member is EnumMemberDeclarationSyntax enumMemberSyntax
                    => EnumMapper.MapEnumMember(enumMemberSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.InterfaceDeclaration when member is InterfaceDeclarationSyntax interfaceSyntax
                    => InterfaceMapper.MapInterface(interfaceSyntax, semanticModel, tree, configuration, codeDocumentViewModel),
                SyntaxKind.FieldDeclaration when member is FieldDeclarationSyntax fieldSyntax
                    => FieldMapper.MapField(fieldSyntax, semanticModel, configuration, codeDocumentViewModel),
                SyntaxKind.PropertyDeclaration when member is PropertyDeclarationSyntax propertySyntax
                    => PropertyMapper.MapProperty(propertySyntax, semanticModel, configuration, codeDocumentViewModel),
                SyntaxKind.StructDeclaration when member is StructDeclarationSyntax structSyntax
                    => StructMapper.MapStruct(structSyntax, semanticModel, tree, configuration, codeDocumentViewModel),
                SyntaxKind.ClassDeclaration when member is ClassDeclarationSyntax classSyntax
                    => ClassMapper.MapClass(classSyntax, semanticModel, tree, configuration, codeDocumentViewModel, mapBaseClass),
                SyntaxKind.EventFieldDeclaration when member is EventFieldDeclarationSyntax eventFieldSyntax
                    => DelegateEventMapper.MapEvent(eventFieldSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.DelegateDeclaration when member is DelegateDeclarationSyntax delegateSyntax
                    => DelegateEventMapper.MapDelegate(delegateSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.FileScopedNamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, configuration, codeDocumentViewModel),
                SyntaxKind.NamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, configuration, codeDocumentViewModel),
                SyntaxKind.RecordDeclaration when member is RecordDeclarationSyntax recordSyntax
                    => RecordMapper.MapRecord(recordSyntax, semanticModel, configuration, codeDocumentViewModel),
                SyntaxKind.ConstructorDeclaration when member is ConstructorDeclarationSyntax constructorSyntax
                    => MethodMapper.MapConstructor(constructorSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.IndexerDeclaration when member is IndexerDeclarationSyntax indexerSyntax
                    => IndexerMapper.MapIndexer(indexerSyntax, semanticModel, configuration, codeDocumentViewModel),
                _ => null,
            };
}