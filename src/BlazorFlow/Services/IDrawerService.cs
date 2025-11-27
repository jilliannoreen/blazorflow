using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Services;

/// <summary>
/// Defines methods for interacting with Flowbite drawer components through JavaScript interop.
/// </summary>
public interface IDrawerService
{
    /// <summary>
    /// Raised when a global drawer show request is triggered.
    /// </summary>
    event Func<string, RenderFragment, Drawer.DrawerOptions?, Task>? OnShowRequested;

    /// <summary>
    /// Raised when a global drawer hide request is triggered.
    /// </summary>
    event Func<Task>? OnHideRequested;
    /// <summary>
    /// Initializes a Flowbite drawer with the given ID and options.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task InitAsync(string id, Drawer.DrawerOptions options);
    /// <summary>
    /// Shows the drawer with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task ShowAsync(string id);
    /// <summary>
    /// Hides the drawer with the specified ID.
    /// </summary>
    /// <param name="id">The HTML element ID of the drawer.</param>
    Task HideAsync(string id);
    /// <summary>
    /// Toggles the visibility of the drawer with the specified ID.
    /// </summary>
    /// <param name="id">The HTML element ID of the drawer.</param>
    Task ToggleAsync(string id);
    /// <summary>
    /// Returns whether the drawer with the given ID is currently visible.
    /// </summary>
    /// <param name="id">The HTML element ID of the drawer.</param>
    /// <returns>True if the drawer is visible; otherwise, false.</returns>
    Task<bool> IsVisibleAsync(string id);
    /// <summary>
    /// Shows a component inside a global drawer.
    /// </summary>
    Task ShowAsync<TComponent>(string title, DrawerParameters<TComponent> parameters, Drawer.DrawerOptions? options = null)
        where TComponent : IComponent;

    /// <summary>
    /// Programmatically hides the global drawer.
    /// </summary>
    Task HideAsync();

}