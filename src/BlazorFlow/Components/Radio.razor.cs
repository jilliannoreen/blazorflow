using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Radio<T>
{
    [CascadingParameter] private RadioGroup<T> RadioGroup { get; set; }
    [Parameter] public string Id { get; set; } = $"radio-{Guid.NewGuid():N}";
    [Parameter] public string Name { get; set; }
    [Parameter] public T Value { get; set; }
    [Parameter] public EventCallback<T> ValueChanged { get; set; }
    [Parameter] public EventCallback<T> OnChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
    
    
    private bool IsChecked => RadioGroup != null
        ? EqualityComparer<T>.Default.Equals(Value, (T)RadioGroup.Value)
        : false;    
    
    private async Task ToggleIfNotDisabled()
    {
        if (Disabled) return;


        if (RadioGroup != null)
        {
            await RadioGroup.SelectAsync(Value);
            return;
        }


        await ValueChanged.InvokeAsync(Value);
        if (OnChanged.HasDelegate)
            await OnChanged.InvokeAsync(Value);
    }
    
    private string InputClass => ClassBuilder
        .Default("appearance-none rounded-full border bg-no-repeat bg-center")
        .AddClass(GetSizeClass())
        .AddClass(GetBaseColorClass())
        .AddClass(GetDotClass())
        .AddClass(Disabled ? "cursor-not-allowed opacity-50" : "cursor-pointer")
        .Build();

    private string GetSizeClass()
    {
        var border = Size switch
        {
            Size.Small => "border-[4.5px]",
            Size.Medium => "border-[5.5px]",
            Size.Large => "border-[6px]",
            _ => "border-[2.5px]"
        };
        
        var size = Size switch
        {
            Size.Small => "w-4 h-4",
            Size.Medium => "w-5 h-5",
            Size.Large => "w-6 h-6",
            _ => "w-5 h-5"
        };
        
        return size + (IsChecked ? $" {border}" : "");
    }
    
    
    private string GetDotClass() 
    {
        return IsChecked ? Size switch
        {
            Size.Small => "bg-[radial-gradient(circle,_white_6px,_transparent_0)]",
            Size.Medium => "bg-[radial-gradient(circle,_white_8px,_transparent_0)]",
            Size.Large => "bg-[radial-gradient(circle,_white_12px,_transparent_0)]",
            _ => "bg-[radial-gradient(circle,_white_8px,_transparent_0)]"
        } : "";
    }
    
    
    private string GetBaseColorClass()
    {
        if (Disabled)
            return "border-[var(--gray-200)] bg-[var(--gray-25)]";
        
        return IsChecked
            ? "border-[var(--primary)]"
            : "bg-white border-[var(--gray-200)]";
    }
}