using System.Linq.Expressions;
using BlazorFlow.Enums;
using BlazorFlow.Services;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFlow.Components;

public partial class InputField : ComponentBase
{
     #region Parameters

    /// <summary>
    /// Specifies the input type (text, password, email, etc.).
    /// </summary>
    [Parameter] public required InputType Type { get; set; }

    /// <summary>
    /// Custom CSS class for the wrapper container.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// The floating label text shown above the input.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// The placeholder shown when there's no value and no label.
    /// </summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Optional helper text shown below the input.
    /// </summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>
    /// Controls whether the icon appears on the left or right.
    /// </summary>
    [Parameter] public VisualPlacement VisualPlacement { get; set; } = VisualPlacement.None;

    /// <summary>
    /// Optional SVG icon markup.
    /// </summary>
    [Parameter] public string? IconHtml { get; set; }

    /// <summary>
    /// Optional class applied to the icon container.
    /// </summary>
    [Parameter] public string? IconClass { get; set; }

    /// <summary>
    /// The visual variant of the input (Filled, Outlined, Text).
    /// </summary>
    [Parameter] public Variant Variant { get; set; } = Variant.Filled;

    /// <summary>
    /// The size of the input (Small, Medium, Large).
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// When true, the input is disabled and not editable.
    /// </summary>
    [Parameter] public bool Disabled { get; set; } = false;

    /// <summary>
    /// When true, the input is readonly but selectable.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; } = false;

    /// <summary>
    /// When true, updates the bound value immediately on input.
    /// </summary>
    [Parameter] public bool Immediate { get; set; } = false;

    /// <summary>
    /// When true, a border will be rendered (if supported by the variant).
    /// </summary>
    [Parameter] public bool Border { get; set; } = false;

    /// <summary>
    /// The input's bound value.
    /// </summary>
    [Parameter] public string? Value { get => value; set => this.value = value; }

    /// <summary>
    /// Callback invoked when the value is changed (used for two-way binding).
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Optional callback invoked anytime the value is changed, even if not bound.
    /// </summary>
    [Parameter] public EventCallback<string> OnChanged { get; set; }

    /// <summary>
    /// Expression used for validation with EditForm (e.g. @bind-Value="Model.Name").
    /// </summary>
    [Parameter] public Expression<Func<string>> ValueExpression { get; set; } = default!;

    /// <summary>
    /// The EditContext provided by the parent EditForm.
    /// </summary>
    [CascadingParameter] private EditContext? EditContext { get; set; }
    
    /// <summary>
    /// Specifies the focus color of the input when valid and focused. 
    /// This affects the border and ring color on focus, depending on the selected variant.
    /// Defaults to <c>Color.Primary</c>.
    /// </summary>
    [Parameter] public Color Color { get; set; } = Color.Primary;


    #endregion

    #region Fields & State

    /// <summary>
    /// Internal backing field for the bound value.
    /// </summary>
    private string? value;

    /// <summary>
    /// The original value used for dirty-checking or future use.
    /// </summary>
    private string? originalValue;

    /// <summary>
    /// Holds the current validation messages from the EditContext.
    /// </summary>
    public List<string> ValidationMessages { get; set; } = new();
    
    /// <summary>
    /// The unique identifier for the input element.
    /// </summary>
    private string Id { get; } = $"input-{Guid.NewGuid()}";

    #endregion

    #region Lifecycle

    /// <summary>
    /// Subscribes to validation change events on EditContext.
    /// </summary>
    protected override void OnInitialized()
    {
        if (EditContext != null)
        {
            EditContext.OnFieldChanged += HandleFieldChanged;
            EditContext.OnValidationStateChanged += HandleValidationStateChanged;
        }

        originalValue = Value;
        base.OnInitialized();
    }

    /// <summary>
    /// Detaches event handlers from EditContext.
    /// </summary>
    private void Dispose()
    {
        if (EditContext != null)
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
            EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        }
    }

    #endregion

    #region Input Handlers

    /// <summary>
    /// Called on input event if Immediate=true.
    /// </summary>
    private async Task HandleInput(ChangeEventArgs e)
    {
        if (Immediate)
        {
            var newValue = e.Value?.ToString() ?? string.Empty;
            await SetValueAsync(newValue);
        }
    }
    
    private async Task HandleChange(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        if (!Immediate) await SetValueAsync(newValue);
        
    }

    /// <summary>
    /// Updates internal state, triggers binding, and notifies EditContext.
    /// </summary>
    private async Task SetValueAsync(string newValue)
    {
        if (value != newValue)
        {
            value = newValue;

            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(newValue);

            if (OnChanged.HasDelegate)
                await OnChanged.InvokeAsync(newValue);

            if (EditContext != null && ValueExpression != null)
            {
                var fieldIdentifier = FieldIdentifier.Create(ValueExpression);
                EditContext.NotifyFieldChanged(fieldIdentifier);
            }
        }
    }

    #endregion

    #region Validation

    /// <summary>
    /// Triggered when the field changes; updates validation messages.
    /// </summary>
    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
        => GetApplicableValidationMessages();

    /// <summary>
    /// Triggered when the form's validation state changes.
    /// </summary>
    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        => GetApplicableValidationMessages();

    /// <summary>
    /// Retrieves messages for the bound field and updates local state.
    /// </summary>
    private void GetApplicableValidationMessages()
    {
        if (EditContext == null) return;

        var fieldIdentifier = FieldIdentifier.Create(ValueExpression);
        ValidationMessages = EditContext.GetValidationMessages(fieldIdentifier).ToList();

        StateHasChanged();
    }

    #endregion

    #region Style & Class Builders

    /// <summary>
    /// Returns the placeholder only if no label is set.
    /// </summary>
    private string? PlaceholderValue => string.IsNullOrWhiteSpace(Label) ? Placeholder : " ";

    /// <summary>
    /// Returns the CSS class for the wrapper div.
    /// </summary>
    private string InputWrapperClass => ClassBuilder
        .Default("relative w-full")
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Builds the final input class string based on props and state.
    /// </summary>
    private string InputClass => ClassBuilder
        .Default("peer w-full font-normal text-gray-900 appearance-none focus:outline-none focus:ring-0")
        .AddClass(GetBorderClassByVariant()) // ⬅ handles border logic
        .AddClass("opacity-50 cursor-not-allowed bg-gray-100 text-gray-500", Disabled)
        .AddClass("bg-gray-100 text-gray-500 cursor-default", ReadOnly && !Disabled && !ValidationMessages.Any())
        .AddClass(GetClassByVariantSize()) // ⬅ handles padding/bg/font
        .Build();


    /// <summary>
    /// Builds the label class with floating animation support.
    /// </summary>
    private string LabelClass => ClassBuilder
        .Default("absolute left-[16px] text-sm text-gray-600 transition-all z-10 origin-[0] scale-75 cursor-text")
        .AddClass("bg-white", Variant == Variant.Outlined)
        .AddClass(GetLabelPositionClass()) // dynamically set label top offset
        .AddClass("peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0") // if empty, reset
        .AddClass("peer-focus:scale-75") // float up on focus
        .AddClass(GetLabelColorClass()) // color on focus or error
        .Build();
    
    private string GetLabelPositionClass()
    {
        // Base label float positions based on variant and size
        return (Variant, Size) switch
        {
            // Filled
            // (Variant.Outlined, Size.Small) or (Variant.Text, Size.Small) => "top-[14px] -translate-y-[22px]  peer-focus:-translate-y-[22px]",
            // (Variant.Outlined, Size.Medium) or (Variant.Text, Size.Medium) => "top-[16px] -translate-y-6 peer-focus:-translate-y-6",
            // (Variant.Outlined, Size.Large) or (Variant.Text, Size.Large) => "top-[20px] -translate-y-7 peer-focus:-translate-y-7",
            (Variant.Filled, Size.Small) => "sm-input-filled top-[12px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Medium) => "md-input-filled top-[14px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Large) => "lg-input-filled top-[18px] -translate-y-4 peer-focus:-translate-y-4",

            // Outlined floats a bit higher
            (Variant.Outlined, Size.Small) or (Variant.Text, Size.Small) => "sm-input top-[14px] -translate-y-[22px]  peer-focus:-translate-y-[22px]",
            (Variant.Outlined, Size.Medium) or (Variant.Text, Size.Medium) => "md-input top-[16px] -translate-y-6 peer-focus:-translate-y-6",
            (Variant.Outlined, Size.Large) or (Variant.Text, Size.Large) => "lg-input top-[20px] -translate-y-7 peer-focus:-translate-y-7",

            // Fallback
            _ => "top-[12px] -translate-y-4 peer-focus:-translate-y-4"
        };
    }
    
    /// <summary>
    /// Returns focus border and ring classes using custom CSS variables for the selected color.
    /// </summary>
    private string GetFocusColorClass() => Color switch
    {
        Color.Primary => "focus:border-(--primary)",
        Color.Secondary => "focus:border-(--secondary)",
        Color.Tertiary => "focus:border-(--tertiary) ",
        Color.Success => "focus:border-(--success)",
        Color.Warning => "focus:border-(--warning)",
        Color.Info => "focus:border-(--info)",
        Color.Error => "focus:border-(--error)",
        _ => "focus:border-(--primary)"
    };
    
    /// <summary>
    /// Returns the appropriate Tailwind border classes based on the selected <see cref="Variant"/>,
    /// current validation state, and configured <see cref="Color"/>.
    /// This method dynamically applies bottom-only or full-border styles and error handling.
    /// </summary>
    /// <returns>Tailwind utility class string representing border and focus styling.</returns>
    private string GetBorderClassByVariant()
    {
        var hasError = ValidationMessages.Any();
        var colorClass = GetFocusColorClass();

        return Variant switch
        {
            Variant.Filled => hasError
                ? "border-red-600 focus:border-red-600"
                : $"border-0",
            
            Variant.Text => hasError
                ? "border-0 border-b-2 border-red-600 focus:border-red-600"
                : $"border-0 border-b-2 border-gray-300 {colorClass}",

            Variant.Outlined => hasError
                ? "border border-red-600 focus:border-red-600"
                : $"border border-gray-300 {colorClass}",

            _ => string.Empty
        };
    }


    /// <summary>
    /// Returns padding, font size, rounding, and background classes based on size and variant.
    /// Border styles are handled separately in <see cref="GetBorderClassByVariant"/>.
    /// </summary>
    private string GetClassByVariantSize()
    {
        var map = new Dictionary<(Size, Variant), string>
        {
            // Filled
            // { (Size.Small, Variant.Filled), "px-4 pb-1.5 pt-4 text-sm rounded-t-lg bg-gray-200" },
            // { (Size.Medium, Variant.Filled), "px-4 pb-1.5 pt-5 text-sm rounded-t-xl bg-gray-200" },
            // { (Size.Large, Variant.Filled), "px-4 pb-2 pt-5 text-base rounded-t-xl bg-gray-200" },
            { (Size.Small, Variant.Filled), "px-4 pb-1.5 pt-4 text-sm rounded-lg bg-gray-100" },
            { (Size.Medium, Variant.Filled), "px-4 pb-1.5 pt-5 text-sm rounded-xl bg-gray-100" },
            { (Size.Large, Variant.Filled), "px-4 pb-2 pt-5 text-base rounded-xl bg-gray-100" },

            // Outlined
            { (Size.Small, Variant.Outlined), "px-4 py-3 text-sm rounded-lg bg-transparent" },
            { (Size.Medium, Variant.Outlined), "px-4 py-3.5 text-sm rounded-xl bg-transparent" },
            { (Size.Large, Variant.Outlined), "p-4 text-base rounded-xl bg-transparent" },

            // Text
            { (Size.Small, Variant.Text), "px-4 py-3 text-sm bg-transparent" },
            { (Size.Medium, Variant.Text), "px-4 py-3.5 text-sm bg-transparent" },
            { (Size.Large, Variant.Text), "p-4 text-base bg-transparent" },
        };

        return map.TryGetValue((Size, Variant), out var classString) ? classString : string.Empty;
    }
    
    /// <summary>
    /// Returns the label color class based on validation and focus states.
    /// </summary>
    private string GetLabelColorClass()
    {
        if (ValidationMessages.Any())
        {
            return "peer-focus:text-red-600 text-red-600";
        }

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
    #endregion
}