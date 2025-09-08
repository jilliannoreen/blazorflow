using System.ComponentModel;

namespace BlazorFlow.Enums;

/// <summary>
/// Indicates the size of a component.
/// </summary>
public enum Size
{
    /// <summary>
    /// The smallest size.
    /// </summary>
    [Description("Extra Small")]
    ExtraSmall,
    
    [Description("small")]
    Small,

    /// <summary>
    /// A medium size.
    /// </summary>
    [Description("medium")]
    Medium,

    /// <summary>
    /// The largest size.
    /// </summary>
    [Description("large")]
    Large,
}