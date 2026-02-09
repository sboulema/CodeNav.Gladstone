using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;

namespace CodeNav.Settings;

#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

internal static class SettingsDefinition
{
    [VisualStudioContribution]
    internal static SettingCategory CodeNavSettingsCategory { get; } = new("codeNavSettings", "%CodeNav.Settings.Category.DisplayName%")
    {
        Description = "%CodeNav.Settings.Category.Description%",
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    internal static Setting.Boolean ShowFilterToolbarSetting { get; } = new(
        "showFilterToolbar",
        "%CodeNav.Settings.ShowFilterToolbar.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: true)
    {
        Description = "%CodeNav.Settings.ShowFilterToolbar.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Boolean ShowRegionsSetting { get; } = new(
        "showRegions",
        "%CodeNav.Settings.ShowRegions.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: true)
    {
        Description = "%CodeNav.Settings.ShowRegions.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Boolean ShowHistoryIndicatorsSetting { get; } = new(
        "showHistoryIndicators",
        "%CodeNav.Settings.ShowHistoryIndicators.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: true)
    {
        Description = "%CodeNav.Settings.ShowHistoryIndicators.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Boolean AutoHighlightSetting { get; } = new(
        "autoHighlight",
        "%CodeNav.Settings.AutoHighlight.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: true)
    {
        Description = "%CodeNav.Settings.AutoHighlight.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Boolean UpdateWhileTypingSetting { get; } = new(
        "updateWhileTyping",
        "%CodeNav.Settings.UpdateWhileTyping.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: true)
    {
        Description = "%CodeNav.Settings.UpdateWhileTyping.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Integer AutoLoadLineThresholdSetting { get; } = new(
        "autoLoadLineThreshold",
        "%CodeNav.Settings.AutoLoadLineThreshold.DisplayName%",
        CodeNavSettingsCategory,
        defaultValue: 0)
    {
        Description = "%CodeNav.Settings.AutoLoadLineThreshold.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.Enum SortOrderSetting { get; } = new(
        "sortOrder",
        "%CodeNav.Settings.SortOrder.DisplayName%",
        CodeNavSettingsCategory,
        [
            new("Unknown", "%CodeNav.Settings.SortOrder.Unknown%"),
            new("SortByFile", "%CodeNav.Settings.SortOrder.SortByFile%"),
            new("SortByName", "%CodeNav.Settings.SortOrder.SortByName%")
        ],
        defaultValue: "Unknown")
    {
        Description = "%CodeNav.Settings.SortOrder.Description%",
    };

    [VisualStudioContribution]
    internal static Setting.ObjectArray FilterRulesSetting { get; } = new(
        "filterRules",
        "%CodeNav.Settings.FilterRules.DisplayName%",
        CodeNavSettingsCategory,
        [
            new ArraySettingItemProperty.Boolean("filterRuleVisible", "%CodeNav.Settings.FilterRules.Visible.DisplayName%", true),
        ],
        defaultValue: [])
    {
        Description = "%CodeNav.Settings.FilterRules.Description%",
    };
}
