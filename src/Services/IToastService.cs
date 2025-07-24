using BlazorFlow.Components;
using BlazorFlow.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Services;

/// <summary>
/// Provides a contract for showing and managing toast notifications in a Blazor application.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Event triggered when a toast is added. Carries the toast instance to be rendered.
    /// </summary>
    event Action<ToastInstance>? OnToastAdded;

    /// <summary>
    /// Event triggered when a toast is removed. Carries the toast ID to be removed from the view.
    /// </summary>
    event Action<string>? OnToastRemoved;

    /// <summary>
    /// Shows a toast with the specified title, message, type, and optional options.
    /// </summary>
    /// <param name="title">Title of the toast.</param>
    /// <param name="message">Optional message body.</param>
    /// <param name="type">Type of the toast (e.g., success, error, info).</param>
    /// <param name="options">Optional additional options for customization.</param>
    /// <returns>The ID of the shown toast.</returns>
    string Show(string title, string? message, ToastType type, ToastOptions? options = null);

    /// <summary>
    /// Convenience method to show a success toast.
    /// </summary>
    string ShowSuccess(string title, string? message);

    /// <summary>
    /// Convenience method to show an error toast.
    /// </summary>
    string ShowError(string title, string? message);

    /// <summary>
    /// Convenience method to show a warning toast.
    /// </summary>
    string ShowWarning(string title, string? message);

    /// <summary>
    /// Convenience method to show an informational toast.
    /// </summary>
    string ShowInfo(string title, string? message);

    /// <summary>
    /// Removes a toast by its ID.
    /// </summary>
    void Remove(string toastId);

    /// <summary>
    /// Clears all active toasts.
    /// </summary>
    void Clear();
}   