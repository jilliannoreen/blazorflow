using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Services;

public interface IDialogService
{
    /// <summary>
    /// Raised when a global drawer show request is triggered.
    /// </summary>
    event Func<string, RenderFragment, Dialog.DialogOptions?, Task>? OnShowRequested;

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
    Task InitAsync(string id, Dialog.DialogOptions options);
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
}