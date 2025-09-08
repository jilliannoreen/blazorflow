using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

/// <summary>
/// Renders an SVG icon with configurable size, color, and additional attributes.
/// </summary>
public partial class Icon
{
    /// <summary>
    /// Additional arbitrary attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?> UserAttributes { get; set; } = new Dictionary<string, object?>();
    /// <summary>
    /// Custom CSS classes to apply to the icon.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }
    /// <summary>
    /// Inline CSS styles for the icon.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }
    /// <summary>
    /// Tooltip or title attribute for the icon.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }
    /// <summary>
    /// The SVG path or content that defines the icon shape.
    /// </summary>
    [Parameter]
    public string SvgIcon { get; set; }
    /// <summary>
    /// The predefined size of the icon.
    /// </summary>
    [Parameter]
    public Size Size { get; set; } = Size.Medium;
    /// <summary>
    /// The color category to apply to the icon.
    /// </summary>
    [Parameter]
    public Color Color { get; set; } = Color.Inherit;
    /// <summary>
    /// The SVG viewBox attribute that defines the coordinate system.
    /// </summary>
    [Parameter]
    public string ViewBox { get; set; } = "0 0 24 24";

    /// <summary>
    /// Builds the final CSS class string for the icon, based on size and user input.
    /// </summary>
    private string IconClass => ClassBuilder
        .Default(string.Empty)
        .AddClass(Size switch
        {
            Size.ExtraSmall => "size-3.5",
            Size.Small => "size-4.5",
            Size.Large => "size-6",
            _ => "size-5"
        })
        .AddClass(Class)
        .Build();
}