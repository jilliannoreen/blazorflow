using System.Linq.Expressions;
using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

/// <summary>
/// Provides a service for showing and hiding dialogs using JavaScript interop and component rendering.
/// </summary>
public class DialogService : IDialogService
{
    /// <summary>
    /// JavaScript runtime abstraction used for interop with browser features (e.g., focus management, DOM manipulation).
    /// </summary>
    private readonly IJSRuntime _js;

    /// <summary>
    /// Reference to the parent dialog provider that manages dialog lifecycle and rendering.
    /// </summary>
    private DialogProvider? _provider;

    /// <inheritdoc/>
    public event Func<string, RenderFragment, DialogOptions?, Task>? OnShowRequested;
    /// <inheritdoc/>
    public event Func<Task>? OnHideRequested;


    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="js">The JavaScript runtime used for interop.</param>
    public DialogService(IJSRuntime js)
    {
        _js = js;
    }
    /// <summary>
    /// Registers the <see cref="DialogProvider"/> used to render dialogs dynamically.
    /// </summary>
    /// <param name="provider">The provider responsible for dialog rendering.</param>
    public void RegisterProvider(DialogProvider provider)
    {
        _provider = provider;
    }

    
    /// <inheritdoc />
    public async Task InitAsync(string id, DialogOptions options, DotNetObjectReference<Dialog> dotNetRef)
    {
        // Initializes the dialog via JavaScript interop (e.g., attaches event listeners).
        await _js.InvokeVoidAsync("blazorFlowInterop.dialog.init", id, options, dotNetRef);
    }
    
    /// <inheritdoc />
    public async Task ShowAsync(string id)
    {
        // Shows the dialog identified by the given ID.
        await _js.InvokeVoidAsync("blazorFlowInterop.dialog.show", id);
    }
    /// <inheritdoc />
    public async Task HideAsync(string id)
    {
        // Hides the dialog identified by the given ID.
        await _js.InvokeVoidAsync("blazorFlowInterop.dialog.hide", id);
    }
    /// <inheritdoc />
    public async Task ToggleAsync(string id)
    {
        // Toggles the visibility of the dialog.
        await _js.InvokeVoidAsync("blazorFlowInterop.dialog.toggle", id);
    }
    /// <inheritdoc />
    public async Task<bool> IsVisibleAsync(string id)
    {
        // Checks if the dialog is currently visible.
        return await _js.InvokeAsync<bool>("blazorFlowInterop.dialog.isVisible", id);
    }
    /// <inheritdoc/>
    public async Task<DialogReference> ShowAsync<TDialog>(
        string title,
        Dictionary<string, object?> parameters,
        DialogOptions? options = null
    ) where TDialog : ComponentBase
    {
        if (_provider is null)
            throw new InvalidOperationException("DialogProvider is not registered.");

        // Dynamically shows a Blazor component as a dialog.
        return await _provider.ShowAsync<TDialog>(title, parameters, options);
    }
    
    /// <inheritdoc/>
    public Task HideAsync()
    {
        return OnHideRequested?.Invoke() ?? Task.CompletedTask;
    }
}

/// <summary>
/// Represents a strongly-typed dictionary of parameters passed to a dialog component.
/// </summary>
/// <typeparam name="TComponent">The dialog component type.</typeparam>
public class DialogParameters<TComponent> : Dictionary<string, object?>
{
    /// <summary>
    /// Adds a strongly-typed parameter using a lambda expression to access the property.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="accessor">Expression accessing the component property (e.g., x => x.Property).</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <exception cref="ArgumentException">Thrown if the expression is not a valid member expression.</exception>
    public void Add<TValue>(Expression<Func<TComponent, TValue>> accessor, TValue value)
    {
        if (accessor.Body is MemberExpression memberExpr)
        {
            Add(memberExpr.Member.Name, value);
        }
        else if (accessor.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression member)
        {
            // Handles nullable or casted property expressions.
            Add(member.Member.Name, value);
        }
        else
        {
            throw new ArgumentException("Expression must be a member expression.", nameof(accessor));
        }
    }
}