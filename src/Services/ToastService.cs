using BlazorFlow.Components;
using BlazorFlow.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

/// <inheritdoc />
public class ToastService : IToastService
{
    /// <summary>
    /// JavaScript runtime used for interop operations, if needed.
    /// </summary>
    private readonly IJSRuntime _jsRuntime;
    /// <summary>
    /// Stores the currently active toast instances by their ID.
    /// </summary>
    private readonly Dictionary<string, ToastInstance> _activeToasts = new();

    /// <inheritdoc />
    public event Action<ToastInstance>? OnToastAdded;
    /// <inheritdoc />
    public event Action<string>? OnToastRemoved;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    /// <inheritdoc />
    public string Show(string title, string? message, ToastType type, ToastOptions? options = null)
    {
        var toast = new ToastInstance
        {
            Title = title,
            Subtitle = message,
            Type = type,
            Options = options ??= new ToastOptions(),
        };

        toast.OnClose = EventCallback.Factory.Create(this, 
            () => Remove(toast.Id));

        _activeToasts[toast.Id] = toast;
        OnToastAdded?.Invoke(toast);
        return toast.Id;
    }

    /// <inheritdoc />
    public string ShowSuccess(string title, string? message)
    {
        return Show(title, message, ToastType.Success, new ToastOptions { Icon = Icons.Icons.Outlined.VerifiedCheck});
    }
    /// <inheritdoc />
    public string ShowError(string title, string? message)
    {
        return Show(title, message, ToastType.Error, new ToastOptions());
    }
    /// <inheritdoc />
    public string ShowWarning(string title, string? message)
    {
        return Show(title, message, ToastType.Warning, new ToastOptions());
    }
    /// <inheritdoc />
    public string ShowInfo(string title, string? message)
    {
        return Show(title, message, ToastType.Info, new ToastOptions());
    }
    /// <inheritdoc />
    public void Remove(string toastId)
    {
        if (_activeToasts.ContainsKey(toastId))
        {
            _activeToasts.Remove(toastId);
            OnToastRemoved?.Invoke(toastId);
        }
    }
    /// <inheritdoc />
    public void Clear()
    {
        var toastIds = new List<string>(_activeToasts.Keys);

        foreach (var id in toastIds)
        {
            Remove(id);
        }
    }

}