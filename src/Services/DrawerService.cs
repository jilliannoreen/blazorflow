using System.Linq.Expressions;
using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

/// <summary>
/// Provides an implementation of <see cref="IDrawerService"/> using JavaScript interop
/// to control Flowbite drawer components.
/// </summary>
public class DrawerService : IDrawerService
{
    private readonly IJSRuntime _js;
    /// <inheritdoc/>
    public event Func<string, RenderFragment, Drawer.DrawerOptions?, Task>? OnShowRequested;
    /// <inheritdoc/>
    public event Func<Task>? OnHideRequested;


    /// <summary>
    /// Initializes a new instance of the <see cref="DrawerService"/> class.
    /// </summary>
    /// <param name="js">The JavaScript runtime used for interop.</param>
    public DrawerService(IJSRuntime js)
    {
        _js = js;
    }
    
    /// <inheritdoc />
    public async Task InitAsync(string id, Drawer.DrawerOptions options)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.drawer.init", id, options);
    }
    
    /// <inheritdoc />
    public async Task ShowAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.drawer.show", id);
    }
    /// <inheritdoc />
    public async Task HideAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.drawer.hide", id);
    }
    /// <inheritdoc />
    public async Task ToggleAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.drawer.toggle", id);
    }
    /// <inheritdoc />
    public async Task<bool> IsVisibleAsync(string id)
    {
        return await _js.InvokeAsync<bool>("flowbiteBlazorInterop.drawer.isVisible", id);
    }
    /// <inheritdoc/>
    public Task ShowAsync<TComponent>(string title, DrawerParameters<TComponent> parameters, Drawer.DrawerOptions? options = null)
        where TComponent : IComponent
    {
        var fragment = new RenderFragment(builder =>
        {
            builder.OpenComponent(0, typeof(TComponent));
            
            int seq = 1;
            foreach (var param in parameters)
                builder.AddAttribute(seq++, param.Key, param.Value);
            
            builder.CloseComponent();
        });

        return OnShowRequested?.Invoke(title, fragment, options) ?? Task.CompletedTask;
    }
    /// <inheritdoc/>
    public Task HideAsync()
    {
        return OnHideRequested?.Invoke() ?? Task.CompletedTask;
    }
}

/// <summary>
/// Represents a type-safe collection of parameters passed to a drawer component,
/// Enables strongly-typed binding of properties in the drawer using expressions.
/// </summary>
/// <typeparam name="T">The target drawer component type.</typeparam>
public class DrawerParameters<TComponent> : Dictionary<string, object?>
{
    public void Add<TValue>(Expression<Func<TComponent, TValue>> accessor, TValue value)
    {
        if (accessor.Body is MemberExpression memberExpr)
        {
            Add(memberExpr.Member.Name, value);
        }
        else if (accessor.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression member)
        {
            Add(member.Member.Name, value);
        }
        else
        {
            throw new ArgumentException("Expression must be a member expression.", nameof(accessor));
        }
    }
}