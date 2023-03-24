using CodeNav.Constants;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using System.ComponentModel;

namespace CodeNav.Mappers;

public static class IconMapper
{
    public static ImageMoniker MapMoniker(CodeItemKindEnum kind, CodeItemAccessEnum access)
    {
        var accessString = GetEnumDescription(access);

        string monikerString = kind switch
        {
            CodeItemKindEnum.Namespace => $"Namespace{accessString}",
            CodeItemKindEnum.Class => $"Class{accessString}",
            CodeItemKindEnum.Constant => $"Constant{accessString}",
            CodeItemKindEnum.Delegate => $"Delegate{accessString}",
            CodeItemKindEnum.Enum => $"Enumeration{accessString}",
            CodeItemKindEnum.EnumMember => $"EnumerationItem{accessString}",
            CodeItemKindEnum.Event => $"Event{accessString}",
            CodeItemKindEnum.Interface => $"Interface{accessString}",
            CodeItemKindEnum.Constructor or CodeItemKindEnum.Method => $"Method{accessString}",
            CodeItemKindEnum.Property or CodeItemKindEnum.Indexer => $"Property{accessString}",
            CodeItemKindEnum.Struct or CodeItemKindEnum.Record => $"Structure{accessString}",
            CodeItemKindEnum.Variable => $"Field{accessString}",
            CodeItemKindEnum.Switch => "FlowSwitch",
            CodeItemKindEnum.SwitchSection => "FlowDecision",
            CodeItemKindEnum.StyleRule => "Rule",
            CodeItemKindEnum.PageRule => "PageStyle",
            CodeItemKindEnum.NamespaceRule => "Namespace",
            CodeItemKindEnum.MediaRule => "Media",
            CodeItemKindEnum.FontFaceRule => "Font",
            _ => $"Property{accessString}",
        };

        var monikers = typeof(KnownMonikers).GetProperties();

        var imageMoniker = monikers.FirstOrDefault(m => monikerString.Equals(m.Name))?.GetValue(null, null);

        if (imageMoniker != null)
        {
            return (ImageMoniker)imageMoniker;
        }

        return KnownMonikers.QuestionMark;
    }

    private static string GetEnumDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
        {
            return string.Empty;
        }

        return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is not DescriptionAttribute attribute
            ? value.ToString() : attribute.Description;
    }
}
