using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class CheckBox : ComponentBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter] public string Label { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public bool Disabled { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
    
    private string _id = $"checkbox-{Guid.NewGuid().ToString()}";
    
    private string InputClass => ClassBuilder
        .Default(@"appearance-none border border-(--gray-200) bg-no-repeat bg-center
                   checked:bg-[url('data:image/svg+xml,%3Csvg%20fill%3D%27none%27%20stroke%3D%27white%27%20stroke-width%3D%273%27%20stroke-linecap%3D%27round%27%20stroke-linejoin%3D%27round%27%20viewBox%3D%270%200%2024%2024%27%20xmlns%3D%27http://www.w3.org/2000/svg%27%3E%3Cpath%20d%3D%27M5%2013l4%204L19%206%27/%3E%3C/svg%3E')]")
        .AddClass(Disabled ? "cursor-pointer-disabled" : "cursor-pointer")
        .AddClass(GetSizeClass)
        .AddClass(GetBorderRadiusBySize)
        .AddClass(GetColorClass())
        .Build();
    
    private string LabelClass => ClassBuilder
        .Default("ms-2 text-sm font-medium text-gray-900")
        .Build();
    
    private string GetSizeClass => Size switch
    {
        Size.Large => "w-6 h-6 checked:bg-[length:22px_22px] ",
        Size.Small => "w-4 h-4 checked:bg-[length:14px_14px] ",
        _ => "w-5 h-5 checked:bg-[length:18px_18px] "
    };

    private string GetColorClass()
    {
        // Disabled state
        if (Disabled)
        {
            return Checked
                ? "bg-[var(--gray-100)] border-[var(--gray-100)]"
                : "bg-[var(--gray-25)] border-[var(--gray-100)]";
        }

        // Active + color variants
        return Color switch
        {
            Color.Primary => "hover:bg-[var(--secondary)] hover:border-[var(--primary)] checked:hover:bg-[var(--primary)] checked:bg-[var(--primary)] checked:border-[var(--primary)]",

            _ => throw new ArgumentOutOfRangeException(nameof(Color), Color, null)
        };
    }
    
    private string GetBorderRadiusBySize => Size switch
    {
        Size.Small => "rounded-xs",
        _ => "rounded-sm"
    };
    
    private async Task OnCheckboxChanged(ChangeEventArgs e)
    {
        if (e.Value is bool newValue)
        {
            Checked = newValue;
            await CheckedChanged.InvokeAsync(Checked);
        }
    } 
}