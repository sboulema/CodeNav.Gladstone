using CodeNav.Constants;
using CodeNav.Dialogs.SettingsDialog;
using CodeNav.Settings;
using CodeNav.Settings.Settings;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;
using System.Windows;

namespace CodeNav.Helpers;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

public static class SettingsHelper
{
    public static async Task<SettingsDialogData> GetSettings(
        VisualStudioExtensibility extensibility,
        CancellationToken cancellationToken)
    {
        var settingCategory = await extensibility
            .Settings()
            .ReadEffectiveValuesAsync(
            [
                SettingsDefinition.CodeNavSettingsCategory,
            ],
            cancellationToken);

        return new()
        {
            AutoHighlight = GetSettingValueOrDefault(settingCategory, SettingsDefinition.AutoHighlightSetting),
        };
    }

    public static async Task<SettingsWriteResponse> SaveSettings(
        VisualStudioExtensibility extensibility,
        SettingsDialogData settings,
        CancellationToken cancellationToken)
        => await extensibility
            .Settings()
            .WriteAsync(
                batch =>
                {
                    batch.WriteSetting(SettingsDefinition.AutoHighlightSetting, settings.AutoHighlight);
                    batch.WriteSetting(SettingsDefinition.AutoLoadLineThreshold, settings.AutoLoadLineThreshold);
                    batch.WriteSetting(SettingsDefinition.ShowFilterToolbarSetting, settings.ShowFilterToolbar);
                    batch.WriteSetting(SettingsDefinition.ShowRegionsSetting, settings.ShowRegions);
                    batch.WriteSetting(SettingsDefinition.ShowHistoryIndicatorsSetting, settings.ShowHistoryIndicators);
                },
                description: "Settings saved via CodeNav dialog",
                cancellationToken);

    public static Visibility GetShowFilterToolbarVisibility(CodeNavSettingsCategorySnapshot settingsSnapshot)
    {
        if (settingsSnapshot.ShowFilterToolbarSetting == null)
        {
            return BoolToVisibility(SettingsDefinition.ShowFilterToolbarSetting.DefaultValue);
        }

        return BoolToVisibility(settingsSnapshot.ShowFilterToolbarSetting.ValueOrDefault(SettingsDefinition.ShowFilterToolbarSetting.DefaultValue));

        static Visibility BoolToVisibility(bool value)
            => value ? Visibility.Visible : Visibility.Collapsed;
    }

    public static SortOrderEnum GetSortOrder(CodeNavSettingsCategorySnapshot settingsSnapshot)
    {
        if (settingsSnapshot.SortOrderSetting == null)
        {
            return Enum.Parse<SortOrderEnum>(SettingsDefinition.SortOrderSetting.DefaultValue);
        }

        return Enum.Parse<SortOrderEnum>(settingsSnapshot.SortOrderSetting.ValueOrDefault(SettingsDefinition.SortOrderSetting.DefaultValue));
    }

    /// <summary>
    /// Get the value of a setting based on its full id and a list of current setting values. 
    /// </summary>
    /// <param name="settingValues">Dictionary of setting key value pairs</param>
    /// <param name="setting">Boolean setting definition</param>
    /// <returns></returns>
    private static bool GetSettingValueOrDefault(SettingValues? settingValues, Setting.Boolean setting)
    {
        if (settingValues == null)
        {
            return setting.DefaultValue;
        }

        return settingValues[setting.FullId].ValueOrDefault(setting.DefaultValue);

        return settingValues
            .FirstOrDefault(settingValue => settingValue.Key == setting.FullId)
            .Value
            .ValueOrDefault(setting.DefaultValue);
    }
}
