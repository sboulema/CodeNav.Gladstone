using CodeNav.Helpers;
using CodeNav.Templates;
using CodeNav.ViewModels;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace CodeNav;

[VisualStudioContribution]
internal class TextViewMarginProvider : ExtensionPart, ITextViewMarginProvider, ITextViewOpenClosedListener, ITextViewChangedListener
{
    private readonly DocumentHelper _documentHelper;
    
    private readonly Dictionary<Uri, CodeDocumentViewModel> documentModels = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TextViewMarginProvider"/> class.
    /// </summary>
    /// <param name="extension">Extension instance.</param>
    /// <param name="extensibility">Extensibility object.</param>
    public TextViewMarginProvider(Extension extension, VisualStudioExtensibility extensibility,
        DocumentHelper documentHelper)
        : base(extension, extensibility)
    {
        _documentHelper = documentHelper;
    }

    /// <summary>
    /// Configures this extension part to be applied to any code view.
    /// </summary>
    public TextViewExtensionConfiguration TextViewExtensionConfiguration
        => new()
        {
            AppliesTo = new[]
            {
                DocumentFilter.FromDocumentType(DocumentType.KnownValues.Text),
            },
        };

    /// <summary>
    /// Configures the margin to be placed to the left of the code view.
    /// </summary>
    public TextViewMarginProviderConfiguration TextViewMarginProviderConfiguration
        => new(ContainerMarginPlacement.KnownValues.Left)
        {
            GridCellLength = 200,
            GridUnitType = TextViewMarginGridUnitType.Pixel
        };

    /// <summary>
    /// Creates a visual element representing the content of the margin.
    /// </summary>
    public async Task<IRemoteUserControl> CreateVisualElementAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
    {
        var model = await _documentHelper.UpdateDocument(textView, cancellationToken);

        documentModels[textView.Uri] = model;

        return new CodeNavMarginDataTemplate(model);
    }

    /// <inheritdoc />
    public async Task TextViewChangedAsync(TextViewChangedArgs args, CancellationToken cancellationToken)
    {
        var model = await _documentHelper.UpdateDocument(args.AfterTextView, cancellationToken);
        documentModels[args.AfterTextView.Uri] = model;
    }

    /// <inheritdoc />
    public Task TextViewClosedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
    {
        documentModels.Remove(textView.Uri);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task TextViewOpenedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
