# CodeNav.Gladstone
A out-of-proc rewrite of [CodeNav](https://github.com/sboulema/CodeNav) using [VisualStudio.Extensibility](https://github.com/microsoft/VSExtensibility). 

# CodeNav 
Visual Studio extension to show the code structure of your current document

[![Sponsor](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/sboulema)

## Features
- Quickly see all the important methods and properties in your document.
- Never get lost during a refactoring of a super long document.
- Show/Dock as a separate tool window.
- Cursor position will be reflected by highlighting the current method in the list
- Clicking on an item in the list will take you to that location in the document.
- Dark theme support
- Collapse/Expand all ranges
- History/edit indicators

## Features broken
- Sort by file order or by name 
- Filter items by kind (method, property), access (public, private), name
- Synced collapsing/expanding ranges
- Regions

## Features dropped
- Top Margin
- Toggle visibility by double-clicking the splitter bar
- Show as an editor margin (left side / right side / top side / hidden) 
- Visual studio 2017, 2019 support
- Customizable fonts
- Set code item opacity through filter window
- Bookmarks

## Language support
- C#

## Languages dropped
- Visual Basic
- JavaScript
- CSS

## Installing
- No public build yet

## Known shortcomings of [VisualStudio.Extensibility](https://github.com/microsoft/VSExtensibility)

### [RemoteUI](https://github.com/microsoft/VSExtensibility/blob/205f50bec40166533d30b13a281af3ab9cd288ff/docs/new-extensibility-model/inside-the-sdk/remote-ui.md)
- Remote UI doesn't allow referencing your own custom controls.
- A Remote user control is fully defined in a single XAML file which references a single (but potentially complex and nested) data context object.
- A Remote user control is actually instantiated in the Visual Studio process, not the process hosting the extension: the XAML cannot reference types and assemblies from the extension but can reference types and assemblies from the Visual Studio process.

## Screenshots
![Preview](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Preview.png) ![Preview-Dark](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Preview-dark.png)

#### Filter window
![Filter window](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Filters.png) 

#### Options window
![Options window](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Options%20-%20General.png) ![Options window](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Options%20-%20Fonts.png)

#### Tool window
![Tool window](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/ToolWindow.png) 

#### Bookmarks
![Bookmarks](https://i.imgur.com/SqLgsXw.png) ![Bookmark styles](https://raw.githubusercontent.com/sboulema/CodeNav/main/Resources/Bookmark%20styles.png)

# Links

[DataTriggers in a XAML ItemsControl](https://github.com/microsoft/VSExtensibility/issues/456)

[Splitting up the tool window XAML in resource dictionaries](https://github.com/MicrosoftDocs/visualstudio-docs/blob/main/docs/extensibility/visualstudio.extensibility/inside-the-sdk/advanced-remote-ui.md#user-xaml-resource-dictionaries)

[Getting a clientContext in an AsyncCommand bound to a XAML Command property](https://github.com/microsoft/VSExtensibility/issues/520)

[Settings Sample](https://github.com/microsoft/VSExtensibility/blob/2f74457eb241552cd725c8ca544fd99797ef546e/New_Extensibility_Model/Samples/SettingsSample/README.md)

[Settings Sample 2](https://github.com/MicrosoftDocs/visualstudio-docs/blob/c70d685e945ff6cea8dd0f7bbb54f780ef67170e/docs/extensibility/visualstudio.extensibility/settings/settings.md?plain=1#L201)

[Fix having multiple projects](https://github.com/microsoft/VSExtensibility/issues/388)

[Making a WPF TextBox binding fire on each new character](https://stackoverflow.com/questions/10619596/making-a-wpf-textbox-binding-fire-on-each-new-character)