using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Toast : ComponentBase
{
    [Parameter] public required string Title { get; set; }
    [Parameter] public string? Subtitle { get; set; }
    [Parameter] public Variant Variant { get; set; }
    [Parameter] public Color Color { get; set; }
    [Parameter] public bool Dismissible { get; set; } = false;
    [Parameter] public string? Icon { get; set; } 
    
    private string Id = $"toast-{Guid.NewGuid()}";
    
    
    private string ToastClass => ClassBuilder
        .Default("flex items-center w-full max-w-sm bg-white p-4 rounded-lg shadow-sm")
        .AddClass(GetVariantColorClass())
        .Build();
    
    private string IconClass => ClassBuilder
        .Default("inline-flex items-center justify-center shrink-0 w-8 h-8 rounded-full")
        .AddClass(GetIconColorClass())
        .Build();

    private string GetIconColorClass()
    {
        return Color switch
        {
            Color.Primary => "bg-(--primary)/25 text-(--primary)",
            Color.Secondary => "bg-(--secondary)/25 text-(--secondary)",
            Color.Tertiary => "bg-(--tertiary)/25 text-(--tertiary)",
            Color.Success => "bg-(--success)/25 text-(--success)",
            Color.Info => "bg-(--info)/25 text-(--info)",
            Color.Warning => "bg-(--warning)/25 text-(--warning)",
            Color.Error => "bg-(--error)/25 text-(--error)",
            Color.Surface => "bg-(--surface)/25 text-(--surface)",
            Color.Light => "bg-gray-900/25 text-gray-900",
            Color.Dark => "bg-black/25 text-black",
            _ => "",
        };
    }
    
    private string GetVariantColorClass()
    {
        var map = new Dictionary<(Color, Variant), string>
        {
            // Primary
            { (Color.Primary, Variant.Filled), "text-(--primary)" },
            { (Color.Primary, Variant.Outlined), "text-(--primary) border border-(--primary) bg-transparent" },
            { (Color.Primary, Variant.Text), "text-(--primary) bg-transparent" },

            // Secondary
            { (Color.Secondary, Variant.Filled), "text-(--secondary)" },
            { (Color.Secondary, Variant.Outlined), "text-(--secondary) border border-(--secondary) hover:bg-(--secondary) hover:text-(--secondary-foreground) bg-transparent" },
            { (Color.Secondary, Variant.Text), "text-(--secondary) hover:text-(--secondary-hover) bg-transparent" },

            // Tertiary
            { (Color.Tertiary, Variant.Filled), "text-(--tertiary)" },
            { (Color.Tertiary, Variant.Outlined), "text-(--tertiary) border border-(--tertiary) hover:bg-(--tertiary-hover) bg-transparent" },
            { (Color.Tertiary, Variant.Text), "text-(--tertiary) hover:text-(--tertiary-hover) bg-transparent" },

            // Success
            { (Color.Success, Variant.Filled), "text-(--success)" },
            { (Color.Success, Variant.Outlined), "text-(--success) border border-(--success) hover:bg-(--success) hover:text-(--success-foreground) bg-transparent" },
            { (Color.Success, Variant.Text), "text-(--success) hover:text-(--success-hover) bg-transparent" },

            // Info
            { (Color.Info, Variant.Filled), "text-(--info)" },
            { (Color.Info, Variant.Outlined), "text-(--info) border border-(--info) hover:bg-(--info) hover:text-(--info-foreground) bg-transparent" },
            { (Color.Info, Variant.Text), "text-(--info) hover:text-(--info-hover) bg-transparent" },

            // Warning
            { (Color.Warning, Variant.Filled), "text-(--warning)" },
            { (Color.Warning, Variant.Outlined), "text-(--warning) border border-(--warning) hover:bg-(--warning) hover:text-(--warning-foreground) bg-transparent" },
            { (Color.Warning, Variant.Text), "text-(--warning) hover:text-(--warning-hover) bg-transparent" },

            // Error
            { (Color.Error, Variant.Filled), "text-(--error)" },
            { (Color.Error, Variant.Outlined), "text-(--error) border border-(--error) hover:bg-(--error) hover:text-(--error-foreground) bg-transparent" },
            { (Color.Error, Variant.Text), "text-(--error) hover:text-(--error-hover) bg-transparent" },

            // Surface
            { (Color.Surface, Variant.Filled), "text-(--surface)" },
            { (Color.Surface, Variant.Outlined), "text-(--surface) border border-(--surface) hover:bg-(--surface) hover:bg-(--surface-foreground) bg-transparent" },
            { (Color.Surface, Variant.Text), "text-(--surface) hover:text-(--surface-hover) bg-transparent" },
            
            // Dark
            { (Color.Dark, Variant.Filled), "text-black" },
            { (Color.Dark, Variant.Outlined), "text-gray-700 border border-gray-700 hover:bg-gray-700 hover:text-white bg-transparent" },
            { (Color.Dark, Variant.Text), "text-gray-700 bg-transparent" },
            
            // Light
            { (Color.Light, Variant.Filled), "text-gray-900" },
            { (Color.Light, Variant.Outlined), "text-gray-700 border border-gray-700 hover:bg-gray-700 hover:text-white bg-transparent" },
            { (Color.Light, Variant.Text), "text-gray-700 bg-transparent" },
        };

        return map.TryGetValue((Color, Variant), out var classString) ? classString : string.Empty;
    }

}