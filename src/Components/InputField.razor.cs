using System.Linq.Expressions;
using BlazorFlow.Enums;
using BlazorFlow.Services;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;

public partial class InputField : ComponentBase, IDisposable
{
    #region Parameters

    /// <summary>
    /// Specifies the input type (text, password, email, etc.).
    /// </summary>
    [Parameter] public required InputType Type { get; set; } = InputType.Text;

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
    [Parameter] public string? Icon { get; set; }

    /// <summary>
    /// Optional class applied to the icon container.
    /// </summary>
    [Parameter] public string? IconClass { get; set; }
    /// <summary>
    /// Size of Icon
    /// </summary>
    [Parameter] public Size IconSize { get; set; } = Enums.Size.Medium;

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
    [Parameter] public string? Value { get => _value; set => this._value = value; }

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
    
    /// <summary>
    /// Gets or sets the custom error message to display below the input field.
    /// If set, this overrides any built-in validation messages.
    /// </summary>
    [Parameter] public string? ErrorText { get; set; }
    /// <summary>
    /// Add autocomplete control
    /// </summary>
    [Parameter] public bool AutoComplete { get; set; } = false;
    /// <summary>
    /// Additional arbitrary attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?> UserAttributes { get; set; } = new Dictionary<string, object?>();

    #endregion

    #region Fields & State

    /// <summary>
    /// Internal backing field for the bound value.
    /// </summary>
    private string? _value;

    /// <summary>
    /// The original value used for dirty-checking or future use.
    /// </summary>
    private string? _originalValue;

    /// <summary>
    /// Holds the current validation messages from the EditContext.
    /// </summary>
    public List<string> ValidationMessages { get; set; } = new();
    
    /// <summary>
    /// The unique identifier for the input element.
    /// </summary>
    private string Id { get; } = $"input-{Guid.NewGuid()}";
    /// <summary>
    /// Determines whether the input field should display an error state.
    /// Returns <c>true</c> if a custom error message is provided via <see cref="ErrorText"/>
    /// or if any validation messages are present from the form context.
    /// </summary>
    private bool HasError => !string.IsNullOrEmpty(ErrorText) || ValidationMessages.Any();

    /// <summary>
    /// Dynamically sets aria-describedby to match visible helper or error text
    /// </summary>
    private string? AriaDescribedById =>
        HasError ? $"{Id}-error"
        : !string.IsNullOrWhiteSpace(HelperText) ? $"{Id}-helper-text"
        : null;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


    /// <summary>
    /// Represents a reference to the HTML input element in the DOM.
    /// This is used to pass the actual DOM element to JavaScript functions,
    /// for example, to attach event listeners or trap focus.
    /// </summary>
    private ElementReference _inputElementRef;

    /// <summary>
    /// Provides a .NET object reference that can be passed to JavaScript.
    /// This allows JavaScript to invoke C# methods on this specific Blazor component instance.
    /// It's crucial for JavaScript to 'call back' into Blazor, e.g., for debounced input events.
    /// </summary>
    private DotNetObjectReference<InputField>? _dotNetRef;


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

        _originalValue = Value;
        base.OnInitialized();
    }

    /// <summary>
    /// Initializes JS interop after the component has rendered.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            if (Immediate)
            {
                // Debounce for 300ms. Adjust as needed.
                await JSRuntime.InvokeVoidAsync("flowbiteBlazorInterop.input.setupInputDebounce", _dotNetRef, _inputElementRef, nameof(HandleDebouncedInputFromJs), 300);
            }
        }
    }

    /// <summary>
    /// Detaches event handlers from EditContext.
    /// </summary>
    public void Dispose()
    {
        if (EditContext != null)
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
            EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        }

        _dotNetRef?.Dispose(); // Dispose DotNetObjectReference
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

    /// <summary>
    /// This method is called from JavaScript after the debounce delay.
    /// </summary>
    /// <param name="newValue">The debounced value from the input field.</param>
    [JSInvokable] // Mark as invokable from JavaScript
    public Task HandleDebouncedInputFromJs(string newValue)
    {
        // This is where the actual update to the Blazor component's state happens
        // after the client-side debounce.
        return SetValueAsync(newValue);
    }


    /// <summary>
    /// Updates internal state, triggers binding, and notifies EditContext.
    /// </summary>
    private async Task SetValueAsync(string newValue)
    {
        if (_value != newValue)
        {
            _value = newValue;

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
        var newMessages = EditContext.GetValidationMessages(fieldIdentifier).ToList();

        if (!ValidationMessages.SequenceEqual(newMessages))
        {
            ValidationMessages = newMessages;
            StateHasChanged();
        }
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
        .AddClass(GetClassByVariantSize(
            hasStartIcon: VisualPlacement == VisualPlacement.Start, 
            hasEndIcon: VisualPlacement == VisualPlacement.End, 
            hasLabel: !string.IsNullOrWhiteSpace(Label))) // ⬅ handles padding/bg/font
        .Build();


    /// <summary>
    /// Builds the label class with floating animation support.
    /// </summary>
    private string LabelClass => ClassBuilder
        .Default("absolute left-[16px] text-sm text-gray-600 transition-all z-10 origin-[0] scale-75 cursor-text px-1.5")
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
            (Variant.Filled, Size.Small) => "sm-input-filled top-[14px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Medium) => "md-input-filled top-[16px] -translate-y-3 peer-focus:-translate-y-3",
            (Variant.Filled, Size.Large) => "lg-input-filled top-[20px] -translate-y-4 peer-focus:-translate-y-4",

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
        var colorClass = GetFocusColorClass();

        return Variant switch
        {
            Variant.Filled => HasError
                ? "border-red-600 focus:border-red-600"
                : $"border-0",
            
            Variant.Text => HasError
                ? "border-0 border-b-2 border-red-600 focus:border-red-600"
                : $"border-0 border-b-2 border-gray-300 {colorClass}",

            Variant.Outlined => HasError
                ? "border border-red-600 focus:border-red-600"
                : $"border border-gray-300 {colorClass}",

            _ => string.Empty
        };
    }

    /// <summary>
    /// Returns the background class based on the <see cref="Variant"/>.
    /// </summary>
    private string GetBgClassByVariant()
    {
        return Variant switch
        {
            Variant.Filled => "bg-white",
            _ => "bg-transparent"
        };
    }
    /// <summary>
    /// Returns the border radius class based on the <see cref="Size"/>.
    /// If <see cref="Variant"/> is <c>Text</c>, no border radius is applied.
    /// </summary>
    private string GetBorderRadiusClassBySize()
    {
        // If Variant is Text, return no border radius
        if (Variant == Variant.Text)
            return string.Empty;

        return Size switch
        {
            Size.Large or Size.Medium => "rounded-xl",
            _ => "rounded-lg"
        };
    }

    /// <summary>
    /// Returns the text size class based on the <see cref="Size"/>.
    /// </summary>
    private string GetTextValueSizeClass()
    {
        return Size switch
        {
            Size.Small or Size.Medium => "text-sm",
            Size.Large => "text-base",
            _ => "text-sm"
        };
    }


    /// <summary>
    /// Returns padding, font size, rounding, and background classes based on size and variant.
    /// Border styles are handled separately in <see cref="GetBorderClassByVariant"/>.
    /// </summary>
    private string GetClassByVariantSize(bool hasStartIcon = false, bool hasEndIcon = false, bool hasLabel = true)
    {

        // Icon Padding
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

        // Vertical padding (merged top and bottom)
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
                Size.Small => "py-2 pt-4.5", // original behavior
                Size.Medium => "py-2 pt-5.5",
                Size.Large => "py-2.5 pt-5.5",
                _ => string.Empty
            },
            Variant.Outlined => Size switch
            {
                Size.Small => "py-3",
                Size.Medium => "py-3.5",
                Size.Large => "p-4",
                _ => string.Empty
            },
            Variant.Text => Size switch
            {
                Size.Small => "py-3",
                Size.Medium => "py-3.5",
                Size.Large => "p-4",
                _ => string.Empty
            },
            _ => string.Empty

        };
        var bgClass = GetBgClassByVariant();
        var borderRadiusClass = GetBorderRadiusClassBySize();
        var textClass = GetTextValueSizeClass();

        return $"{iconPaddingStart} {textClass} {verticalPadding} {bgClass} {borderRadiusClass} {iconPaddingEnd}".Trim();

    }

    /// <summary>
    /// Returns the label color class based on validation and focus states.
    /// </summary>
    private string GetLabelColorClass()
    {
        if (HasError)
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