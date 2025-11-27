using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Chip : ComponentBase
{
    [Parameter] public Color Color { get; set; } = Color.Surface;
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter] public bool FullRounded { get; set; } = false;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string ChipClass => ClassBuilder
        .Default("font-medium w-auto text-center")
        .AddClass("rounded-full", FullRounded)
        .AddClass(Size switch 
            {
                Size.Large => "text-sm px-3 py-1",
                Size.Small => "text-xs px-2.5 py-0.5",
                _ => "text-xs px-3 py-1"
            }
        )
        .AddClass(Color switch
            {
                Color.Error => "text-red-500 bg-red-100",
                Color.Info => "text-blue-500 bg-blue-100",
                Color.Success => "text-green-600 bg-green-100",
                Color.Surface => "text-gray-500 bg-gray-100",
                _ => "text-gray-500 bg-gray-100"
            }
        )
        .Build();

}