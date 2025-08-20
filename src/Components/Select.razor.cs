using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFlow.Components;

public partial class Select<TItem, TValue>
{
    #region Parameters

    [Parameter] public string? Label { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public IEnumerable<TItem> Items { get; set; } = new List<TItem>();
    [Parameter, EditorRequired] public Func<TItem, TValue> ValueSelector { get; set; } = default!;
    [Parameter, EditorRequired] public Func<TItem, string> DisplaySelector { get; set; } = default!;
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    [Parameter] public Func<TValue, string>? ItemTextSelector { get; set; }
    [Parameter] public Variant Variant { get; set; } = Variant.Outlined;
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public string Icon { get; set; } = string.Empty;
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public string? ErrorText { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public Size IconSize { get; set; } = Size.Medium;
    [Parameter] public VisualPlacement VisualPlacement { get; set; } = VisualPlacement.End;
    [Parameter] public string? HelperText { get; set; }

    /// <summary>
    /// Expression used for validation with EditForm (e.g., @bind-Value="Model.Property").
    /// </summary>
    [Parameter] public Expression<Func<TValue>>? ValueExpression { get; set; }

    #endregion

    #region Cascading and State

    [CascadingParameter] private EditContext? EditContext { get; set; }
    public string Id { get; set; } = $"select-{Guid.NewGuid()}";

    private bool IsOpen { get; set; }
    private string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Cached options for efficiency: avoids recomputing Value and Display repeatedly.
    /// </summary>
    private List<(TValue Value, string Display)> CachedOptions { get; set; } = new();

    public List<string> ValidationMessages { get; set; } = new();

    private bool HasError => !string.IsNullOrEmpty(ErrorText) || ValidationMessages.Any();

    #endregion

    #region Lifecycle

    protected override void OnInitialized()
    {
        if (EditContext != null)
        {
            EditContext.OnFieldChanged += HandleFieldChanged;
            EditContext.OnValidationStateChanged += HandleValidationStateChanged;
        }

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (Items != null && ValueSelector != null && DisplaySelector != null)
        {
            // Efficiently cache Value + Display
            CachedOptions = Items.Select(i => (ValueSelector(i), DisplaySelector(i))).ToList();
        }

        UpdateSelectedDisplay();
    }

    #endregion

    #region UI Interaction

    private void ToggleDropdown()
    {
        if (!Disabled && !ReadOnly)
            IsOpen = !IsOpen;
    }

    private async Task SelectItem(TValue item)
    {
        Value = item;
        IsOpen = false;

        await ValueChanged.InvokeAsync(item);

        if (EditContext != null && ValueExpression != null)
        {
            var fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            EditContext.NotifyFieldChanged(fieldIdentifier);
        }

        UpdateSelectedDisplay();
    }

    private void UpdateSelectedDisplay()
    {
        DisplayText = CachedOptions
            .FirstOrDefault(x => EqualityComparer<TValue>.Default.Equals(x.Value, Value))
            .Display ?? string.Empty;
    }

    private string GetDisplayValue(TValue item) =>
        ItemTextSelector?.Invoke(item) ?? item?.ToString() ?? string.Empty;

    #endregion

    #region CSS Builders

    private string InputClass => ClassBuilder
        .Default("peer w-full font-normal text-gray-900 appearance-none focus:outline-none focus:ring-0")
        .AddClass(GetBorderClassByVariant())
        .AddClass("opacity-50 cursor-not-allowed bg-gray-100 text-gray-500", Disabled)
        .AddClass("bg-gray-100 text-gray-500 cursor-default", ReadOnly && !Disabled)
        .AddClass(GetClassByVariantSize(
            hasStartIcon: VisualPlacement == VisualPlacement.Start,
            hasEndIcon: VisualPlacement == VisualPlacement.End,
            hasLabel: !string.IsNullOrWhiteSpace(Label)))
        .Build();

    private string LabelClass => ClassBuilder
        .Default("absolute left-[16px] text-sm text-gray-600 transition-all z-10 origin-[0] scale-75 cursor-text px-1.5")
        .AddClass("bg-white", Variant == Variant.Outlined)
        .AddClass(GetLabelPositionClass())
        .AddClass("peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0")
        .AddClass("peer-focus:scale-75")
        .AddClass(GetLabelColorClass())
        .Build();

    private string GetLabelPositionClass()
    {
        return (Variant, Size) switch
        {
            (Variant.Filled, Size.Small) => "sm-input-filled top-[14px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Medium) => "md-input-filled top-[16px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Large) => "lg-input-filled top-[20px] -translate-y-4 peer-focus:-translate-y-4",
            (Variant.Outlined, Size.Small) or (Variant.Text, Size.Small) => "sm-input top-[14px] -translate-y-[22px] peer-focus:-translate-y-[22px]",
            (Variant.Outlined, Size.Medium) or (Variant.Text, Size.Medium) => "md-input top-[16px] -translate-y-6 peer-focus:-translate-y-6",
            (Variant.Outlined, Size.Large) or (Variant.Text, Size.Large) => "lg-input top-[20px] -translate-y-7 peer-focus:-translate-y-7",
            _ => "top-[12px] -translate-y-4 peer-focus:-translate-y-4"
        };
    }

    private string GetFocusColorClass() => Color switch
    {
        Color.Primary => "focus:border-(--primary)",
        Color.Secondary => "focus:border-(--secondary)",
        Color.Tertiary => "focus:border-(--tertiary)",
        Color.Success => "focus:border-(--success)",
        Color.Warning => "focus:border-(--warning)",
        Color.Info => "focus:border-(--info)",
        Color.Error => "focus:border-(--error)",
        _ => "focus:border-(--primary)"
    };

    private string GetBorderClassByVariant()
    {
        var colorClass = GetFocusColorClass();
        return Variant switch
        {
            Variant.Filled => HasError
                ? "border-red-600 focus:border-red-600"
                : "border-0",
            Variant.Text => HasError
                ? "border-0 border-b-2 border-red-600 focus:border-red-600"
                : $"border-0 border-b-2 border-gray-300 {colorClass}",
            Variant.Outlined => HasError
                ? "border border-red-600 focus:border-red-600"
                : $"border border-gray-300 {colorClass}",
            _ => string.Empty
        };
    }

    private string GetLabelColorClass()
    {
        if (HasError) return "peer-focus:text-red-600 text-red-600";

        var colorVar = Color switch
        {
            Color.Primary => "--primary",
            Color.Secondary => "--secondary",
            Color.Tertiary => "--tertiary",
            Color.Success => "--success",
            Color.Warning => "--warning",
            Color.Info => "--info",
            Color.Error => "--error",
            _ => "--primary"
        };

        return $"peer-focus:text-({colorVar})";
    }

    private string GetClassByVariantSize(bool hasStartIcon = false, bool hasEndIcon = false, bool hasLabel = true)
    {
        var iconPaddingStart = hasStartIcon
            ? IconSize switch
            {
                Size.Small => "pl-9",
                Size.Large => "pl-11",
                _ => "pl-10"
            }
            : string.Empty;

        var iconPaddingEnd = hasEndIcon
            ? IconSize switch
            {
                Size.Small => "pr-9",
                Size.Large => "pr-11",
                _ => "pr-10"
            }
            : string.Empty;

        string verticalPadding = Variant switch
        {
            Variant.Filled when !hasLabel => Size switch
            {
                Size.Small => "py-3",
                Size.Medium => "py-3.5",
                Size.Large => "py-4",
                _ => string.Empty
            },
            Variant.Filled => Size switch
            {
                Size.Small => "py-2 pt-4.5",
                Size.Medium => "py-2 pt-5.5",
                Size.Large => "py-2.5 pt-5.5",
                _ => string.Empty
            },
            Variant.Outlined or Variant.Text => Size switch
            {
                Size.Small => "py-3",
                Size.Medium => "py-3.5",
                Size.Large => "p-4",
                _ => string.Empty
            },
            _ => string.Empty
        };

        var bgClass = Variant == Variant.Filled ? "bg-gray-100" : "bg-transparent";
        var borderRadiusClass = (Variant == Variant.Text) ? string.Empty : (Size == Size.Large || Size == Size.Medium ? "rounded-xl" : "rounded-lg");
        var textClass = Size == Size.Large ? "text-base" : "text-sm";

        return $"{iconPaddingStart} {textClass} {verticalPadding} {bgClass} {borderRadiusClass} {iconPaddingEnd}".Trim();
    }

    private string GetOptionClass(bool isSelected)
    {
        return ClassBuilder
            .Default("px-4 py-2 cursor-pointer transition-colors")
            .AddClass("hover:bg-gray-100", !isSelected)
            .AddClass(Color switch
            {
                Color.Primary => "bg-(--primary-hover) text-(--primary-foreground) font-semibold",
                Color.Secondary => "bg-(--secondary-hover) text-(--secondary-foreground) font-semibold",
                Color.Tertiary => "bg-(--tertiary-hover) text-(--tertiary-foreground) font-semibold",
                Color.Success => "bg-(--success-hover) text-(--success-foreground) font-semibold",
                Color.Warning => "bg-(--warning-hover) text-(--warning-foreground) font-semibold",
                Color.Info => "bg-(--info-hover) text-(--info-foreground) font-semibold",
                Color.Error => "bg-(--error-hover) text-(--error-foreground) font-semibold",
                _ => "bg-(--primary-hover) text-(--primary-foreground) font-semibold"
            }, isSelected)
            .Build();
    }
    
    private string GetIconClass => ClassBuilder
        .Default("absolute top-1/2 -translate-y-1/2")
        .AddClass(VisualPlacement == VisualPlacement.Start ? "left-4" : "right-4")
        .AddClass(HasError ? "text-red-600" : "text-gray-400")
        .Build();

    #endregion

    #region Validation

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e) =>
        GetApplicableValidationMessages();

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e) =>
        GetApplicableValidationMessages();

    private void GetApplicableValidationMessages()
    {
        if (EditContext == null) return;

        var fieldIdentifier = FieldIdentifier.Create(ValueExpression);
        var newMessages = EditContext.GetValidationMessages(fieldIdentifier).ToList();

        if (!ValidationMessages.SequenceEqual(newMessages))
        {
            ValidationMessages = newMessages;
            StateHasChanged();
        }
    }

    #endregion
}
