using CodeNav.Models;
using Microsoft.VisualStudio.Extensibility;

namespace CodeNav.Helpers;

public class ConfigurationHelper(VisualStudioExtensibility extensibility)
{
    private Configuration? _configuration;

    public async Task<Configuration> GetConfiguration()
        => _configuration ??= await GetPersistedState();

    public static FileConfiguration GetFileConfiguration(Configuration configuration, Uri filePath)
        => configuration.FileConfigurations.GetValueOrDefault(filePath) ?? new();

    private async Task<Configuration> GetPersistedState(CancellationToken cancellationToken = default)
        => await extensibility.Configuration().GetPersistedStateAsync("codeNavConfiguration", new Configuration(), cancellationToken)
            ?? new Configuration();

    public async Task SaveConfiguration(Configuration configuration, CancellationToken cancellationToken)
        => await extensibility.Configuration().WritePersistedStateAsync("codeNavConfiguration", configuration, cancellationToken);
}
