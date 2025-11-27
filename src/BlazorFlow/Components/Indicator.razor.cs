using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Indicator : ComponentBase
{
    [Parameter] public Size Size { get; set; }
    [Parameter] public Color Color { get; set; }
    [Parameter] public string Icon { get; set; }

    private string IndicatorClass => ClassBuilder
        .Default("flex justify-center items-center mx-1 rounded-full")
        .AddClass(Size switch
        {
            Size.ExtraSmall => "size-4",
            Size.Small => "size-5",
            Size.Large => "size-6.5",
            _ => "size-5.5"
        })
        .AddClass(Color switch
        {
            Color.Error => "bg-red-100",
            Color.Info => "bg-blue-100",
            Color.Success => "bg-green-100",
            Color.Surface => "bg-gray-100",
            Color.Warning => "bg-amber-400 text-white",
            _ => "text-gray-500 bg-gray-100"
        })
        .Build();}