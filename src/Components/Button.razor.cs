using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorFlow.Components;

/// <summary>
/// A customizable button component with support for variants, colors, icons, loading state, and sizing.
/// </summary>
public partial class Button
{
    // Button behavior and style parameters
    [Parameter] public ButtonType Type { get; set; } = ButtonType.Button;
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Base;
    [Parameter] public string? Class { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public bool FullWidth { get; set; } = false;
    [Parameter] public bool FullRounded { get; set; } = false;
    [Parameter] public Variant Variant { get; set; } = Variant.Filled;
    [Parameter] public Color Color { get; set; } = Color.Primary;

    // Icon and layout
    [Parameter] public string? Icon { get; set; }
    [Parameter] public Size IconSize { get; set; } = Enums.Size.Medium;
    [Parameter] public VisualPlacement VisualPlacement { get; set; } = VisualPlacement.None;

    // Content and loading
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsLoading { get; set; } = false;
    [Parameter] public string? LoadingText { get; set; }

    // Event handling
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    // Pass any unmatched HTML attributes to the button
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Composes the final CSS class list for the button element.
    /// </summary>
    private string ButtonClass => ClassBuilder
        .Default("inline-flex justify-center items-center align-middle gap-1 font-medium leading-none text-center rounded-lg focus:outline-none")
        .AddClass(GetSizeClass())
        .AddClass(Disabled ? "cursor-not-allowed text-white bg-gray-300" : $"cursor-pointer {GetVariantColorClass()}")
        .AddClass("w-full", FullWidth)
        .AddClass("rounded-full!", FullRounded)
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Handles click events only if the button is not disabled.
    /// </summary>
    private async Task HandleClick(MouseEventArgs e)
    {
        if (OnClick.HasDelegate && !Disabled)
        {
            await OnClick.InvokeAsync(e);
        }
    }

    /// <summary>
    /// Returns the appropriate size-related CSS classes.
    /// </summary>
    private string GetSizeClass()
    {
        return Size switch
        {
            ButtonSize.ExtraSmall => "px-3 py-2 text-xs min-h-[32px]",
            ButtonSize.Small => "px-3 py-2 text-sm min-h-[36px]",
            ButtonSize.Base => "px-5 py-2.5 text-sm min-h-[40px]",
            ButtonSize.Large => "px-5 py-3 text-base min-h-[44px]",
            ButtonSize.ExtraLarge => "px-6 py-3.5 text-base min-h-[48px]",
            _ => "px-5 py-2.5 text-sm min-h-[40px]"
        };
    }

    /// <summary>
    /// Returns the appropriate loading spinner size classes.
    /// </summary>
    private string GetLoadingClass() => ClassBuilder
        .Default("inline w-4 h-4 me-3 text-white animate-spin")
        .AddClass(Size switch
        {
            ButtonSize.ExtraSmall or ButtonSize.Small => "size-4.5",
            ButtonSize.Large or ButtonSize.ExtraLarge => "size-6",
            _ => "size-5",
        })
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Returns the Tailwind-based color + variant class string.
    /// </summary>
    private string GetVariantColorClass()
    {
        var map = new Dictionary<(Color, Variant), string>
        {
            // Primary
            { (Color.Primary, Variant.Filled), "text-(--primary-foreground) bg-(--primary) hover:bg-(--primary-hover)" },
            { (Color.Primary, Variant.Outlined), "text-(--primary) border border-(--primary) hover:bg-(--primary) hover:text-(--primary-foreground) bg-transparent" },
            { (Color.Primary, Variant.Text), "text-(--primary) hover:text-(--primary-hover) bg-transparent" },

            // Secondary
            { (Color.Secondary, Variant.Filled), "text-(--secondary-foreground) bg-(--secondary) hover:bg-(--secondary-hover)" },
            { (Color.Secondary, Variant.Outlined), "text-(--secondary) border border-(--secondary) hover:bg-(--secondary) hover:text-(--secondary-foreground) bg-transparent" },
            { (Color.Secondary, Variant.Text), "text-(--secondary) hover:text-(--secondary-hover) bg-transparent" },

            // Tertiary
            { (Color.Tertiary, Variant.Filled), "text-(--tertiary-foreground) bg-(--tertiary) hover:bg-(--tertiary-hover)" },
            { (Color.Tertiary, Variant.Outlined), "text-(--tertiary) border border-(--tertiary) hover:bg-(--tertiary-hover) bg-transparent" },
            { (Color.Tertiary, Variant.Text), "text-(--tertiary) hover:text-(--tertiary-hover) bg-transparent" },

            // Success
            { (Color.Success, Variant.Filled), "text-(--success-foreground) bg-(--success) hover:bg-(--success-hover)" },
            { (Color.Success, Variant.Outlined), "text-(--success) border border-(--success) hover:bg-(--success) hover:text-(--success-foreground) bg-transparent" },
            { (Color.Success, Variant.Text), "text-(--success) hover:text-(--success-hover) bg-transparent" },

            // Info
            { (Color.Info, Variant.Filled), "text-(--info-foreground) bg-(--info) hover:bg-(--info-hover)" },
            { (Color.Info, Variant.Outlined), "text-(--info) border border-(--info) hover:bg-(--info) hover:text-(--info-foreground) bg-transparent" },
            { (Color.Info, Variant.Text), "text-(--info) hover:text-(--info-hover) bg-transparent" },

            // Warning
            { (Color.Warning, Variant.Filled), "text-(--warning-foreground) bg-(--warning) hover:bg-(--warning-hover)" },
            { (Color.Warning, Variant.Outlined), "text-(--warning) border border-(--warning) hover:bg-(--warning) hover:text-(--warning-foreground) bg-transparent" },
            { (Color.Warning, Variant.Text), "text-(--warning) hover:text-(--warning-hover) bg-transparent" },

            // Error
            { (Color.Error, Variant.Filled), "text-(--error-foreground) bg-(--error) hover:bg-(--error-hover)" },
            { (Color.Error, Variant.Outlined), "text-(--error) border border-(--error) hover:bg-(--error) hover:text-(--error-foreground) bg-transparent" },
            { (Color.Error, Variant.Text), "text-(--error) hover:text-(--error-hover) bg-transparent" },

            // Surface
            { (Color.Surface, Variant.Filled), "text-(--surface-foreground) bg-(--surface) hover:bg-(--surface-hover)" },
            { (Color.Surface, Variant.Outlined), "text-(--surface) border border-(--surface) hover:bg-(--surface) hover:bg-(--surface-foreground) bg-transparent" },
            { (Color.Surface, Variant.Text), "text-(--surface) hover:text-(--surface-hover) bg-transparent" },
            
            // Dark
            { (Color.Dark, Variant.Filled), "text-white bg-gray-700 hover:bg-gray-800" },
            { (Color.Dark, Variant.Outlined), "text-gray-700 border border-gray-700 hover:bg-gray-700 hover:text-white bg-transparent" },
            { (Color.Dark, Variant.Text), "text-gray-700 bg-transparent" },
            
            // Light
            { (Color.Light, Variant.Filled), "text-gray-900 bg-white"},
            { (Color.Light, Variant.Outlined), "text-gray-700 border border-gray-700 hover:bg-gray-700 hover:text-white bg-transparent" },
            { (Color.Light, Variant.Text), "text-gray-700 bg-transparent" },
        };

        return map.TryGetValue((Color, Variant), out var classString) ? classString : string.Empty;
    }
}