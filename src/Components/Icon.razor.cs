using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Icon
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?> UserAttributes { get; set; } = new Dictionary<string, object?>();
    [Parameter]
    public string? Class { get; set; } 
    [Parameter]
    public string? Style { get; set; } 
    [Parameter]
    public string? Title { get; set; } 
    [Parameter]
    public string SvgIcon { get; set; } 
    [Parameter]
    public Size Size { get; set; } = Size.Medium;
    [Parameter]
    public Color Color { get; set; } = Color.Inherit;
    [Parameter]
    public string ViewBox { get; set; } = "0 0 24 24";
    

    private string IconClass => ClassBuilder
        .Default(string.Empty)
        .AddClass(Size switch
        {
            Size.Small => "size-4.5",
            Size.Large => "size-6",
            _ => "size-5"
        })
        .AddClass(Class)
        .Build();
}