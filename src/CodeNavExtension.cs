using CodeNav.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace CodeNav;

/// <summary>
/// Extension entry point for the VisualStudio.Extensibility extension.
/// </summary>
[VisualStudioContribution]
internal class CodeNavExtension : Extension
{
    /// <inheritdoc />
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);

        // You can configure dependency injection here by adding services to the serviceCollection.
        serviceCollection.AddSingleton<DocumentHelper>();
        serviceCollection.AddSingleton<ConfigurationHelper>();
        serviceCollection.AddSingleton<BookmarkHelper>();
        serviceCollection.AddSingleton<HistoryHelper>();
    }
}