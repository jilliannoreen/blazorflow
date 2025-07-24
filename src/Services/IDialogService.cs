using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

/// <summary>
/// Provides methods for showing, hiding, and managing dialogs (drawers) in a Blazor application.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Raised when a global drawer show request is triggered.
    /// </summary>
    event Func<string, RenderFragment, DialogOptions?, Task>? OnShowRequested;

    /// <summary>
    /// Raised when a global drawer hide request is triggered.
    /// </summary>
    event Func<Task>? OnHideRequested;
    /// <summary>
    /// Registers a dialog provider instance with the service.
    /// </summary>
    /// <param name="provider">The dialog provider to register.</param>
    void RegisterProvider(DialogProvider provider);
    /// <summary>
    /// Initializes a Flowbite drawer with the given ID and options.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task InitAsync(string id, DialogOptions options, DotNetObjectReference<Dialog> dotNetRef);
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
    /// Programmatically hides the global drawer.
    /// </summary>
    Task HideAsync();
    /// <summary>
    /// Shows a typed Blazor component as a dialog with the specified title and parameters.
    /// </summary>
    /// <typeparam name="TDialog">The Blazor component type to render inside the dialog.</typeparam>
    /// <param name="title">The title to display in the dialog header.</param>
    /// <param name="parameters">A dictionary of parameters to pass to the dialog component.</param>
    /// <param name="options">Optional dialog display options such as backdrop, etc.</param>
    /// <returns>A task that returns a reference to the shown dialog, allowing further control.</returns>
    Task<DialogReference> ShowAsync<TDialog>(
        string title,
        Dictionary<string, object?> parameters,
        DialogOptions? options = null
    ) where TDialog : ComponentBase;
}