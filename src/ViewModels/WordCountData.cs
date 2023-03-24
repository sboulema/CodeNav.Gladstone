using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

namespace CodeNav.ViewModels;

/// <summary>
/// A sample data context object to use with the margin content.
/// </summary>
[DataContract]
public class WordCountData : NotifyPropertyChangedObject
{
    private int wordCount;

    /// <summary>
    /// The count of words in the text document.
    /// </summary>
    [DataMember]
    public int WordCount
    {
        get => wordCount;
        set => SetProperty(ref wordCount, value);
    }
}
