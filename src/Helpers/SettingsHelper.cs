using CodeNav.Constants;
using CodeNav.Settings;
using CodeNav.Settings.Settings;
using System.Windows;

namespace CodeNav.Helpers;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

public static class SettingsHelper
{
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
}
