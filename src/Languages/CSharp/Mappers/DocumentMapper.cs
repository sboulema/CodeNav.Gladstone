using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Extensibility.Editor;
using CodeNav.ViewModels;

namespace CodeNav.Languages.CSharp.Mappers;

internal class DocumentMapper
{
    public static async Task<IEnumerable<CodeItem>> MapDocument(ITextDocumentSnapshot documentSnapshot,
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
            .Select(member => MapMember(member, tree, semanticModel, codeDocumentViewModel))
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
        CodeDocumentViewModel codeDocumentViewModel,
        bool mapBaseClass = true)
        => member.Kind() switch
            {
                SyntaxKind.MethodDeclaration when member is MethodDeclarationSyntax memberSyntax
                    => MethodMapper.MapMethod(memberSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.EnumDeclaration when member is EnumDeclarationSyntax enumSyntax
                    => EnumMapper.MapEnum(enumSyntax, semanticModel, tree, codeDocumentViewModel),
                SyntaxKind.EnumMemberDeclaration when member is EnumMemberDeclarationSyntax enumMemberSyntax
                    => EnumMapper.MapEnumMember(enumMemberSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.InterfaceDeclaration when member is InterfaceDeclarationSyntax interfaceSyntax
                    => InterfaceMapper.MapInterface(interfaceSyntax, semanticModel, tree, codeDocumentViewModel),
                SyntaxKind.FieldDeclaration when member is FieldDeclarationSyntax fieldSyntax
                    => FieldMapper.MapField(fieldSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.PropertyDeclaration when member is PropertyDeclarationSyntax propertySyntax
                    => PropertyMapper.MapProperty(propertySyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.StructDeclaration when member is StructDeclarationSyntax structSyntax
                    => StructMapper.MapStruct(structSyntax, semanticModel, tree, codeDocumentViewModel),
                SyntaxKind.ClassDeclaration when member is ClassDeclarationSyntax classSyntax
                    => ClassMapper.MapClass(classSyntax, semanticModel, tree, codeDocumentViewModel, mapBaseClass),
                SyntaxKind.EventFieldDeclaration when member is EventFieldDeclarationSyntax eventFieldSyntax
                    => DelegateEventMapper.MapEvent(eventFieldSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.DelegateDeclaration when member is DelegateDeclarationSyntax delegateSyntax
                    => DelegateEventMapper.MapDelegate(delegateSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.FileScopedNamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, codeDocumentViewModel),
                SyntaxKind.NamespaceDeclaration when member is BaseNamespaceDeclarationSyntax namespaceSyntax
                    => NamespaceMapper.MapNamespace(namespaceSyntax, semanticModel, tree, codeDocumentViewModel),
                SyntaxKind.RecordDeclaration when member is RecordDeclarationSyntax recordSyntax
                    => RecordMapper.MapRecord(recordSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.ConstructorDeclaration when member is ConstructorDeclarationSyntax constructorSyntax
                    => MethodMapper.MapConstructor(constructorSyntax, semanticModel, codeDocumentViewModel),
                SyntaxKind.IndexerDeclaration when member is IndexerDeclarationSyntax indexerSyntax
                    => IndexerMapper.MapIndexer(indexerSyntax, semanticModel, codeDocumentViewModel),
                _ => null,
            };
}