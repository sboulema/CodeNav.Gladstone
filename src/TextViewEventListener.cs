using CodeNav.Services;
using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeNav;

/// <summary>
/// Listener for text view lifetime events to start CodeNav on new documents or changed documents.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextViewEventListener"/> class.
/// </remarks>
/// <param name="extension">Extension instance.</param>
/// <param name="extensibility">Extensibility object.</param>
/// <param name="diagnosticsProvider">Local diagnostics provider service instance.</param>
[VisualStudioContribution]
internal class TextViewEventListener(
    CodeNavExtension extension,
    VisualStudioExtensibility extensibility,
    CodeDocumentService codeDocumentService)
    : ExtensionPart(extension, extensibility), ITextViewOpenClosedListener, ITextViewChangedListener
{
    private readonly CodeDocumentService codeDocumentService = Requires.NotNull(codeDocumentService, nameof(codeDocumentService));

    /// <inheritdoc/>
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo =
        [
            DocumentFilter.FromGlobPattern("**/*.cs", true),
        ],
    };

    /// <inheritdoc />
    public Task TextViewChangedAsync(TextViewChangedArgs args, CancellationToken cancellationToken)
        => codeDocumentService.UpdateCodeDocumentViewModel(args.AfterTextView, cancellationToken);

    /// <inheritdoc />
    public async Task TextViewClosedAsync(ITextViewSnapshot textViewSnapshot, CancellationToken cancellationToken)
        => codeDocumentService.RemoveViewModel(textViewSnapshot.Uri);

    /// <inheritdoc />
    public Task TextViewOpenedAsync(ITextViewSnapshot textViewSnapshot, CancellationToken cancellationToken)
        => codeDocumentService.UpdateCodeDocumentViewModel(textViewSnapshot, cancellationToken);
}