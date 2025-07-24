using BlazorFlow.Enums;
using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

/// <summary>
/// Provides a container for displaying toast notifications in a Blazor application.
/// Handles toast positioning, adding, removing, and rendering.
/// </summary>
public partial class ToastProvider : ComponentBase, IDisposable
{
    /// <summary>
    /// Injected service used to manage toast events.
    /// </summary>
    [Inject] private IToastService ToastService { get; set; } = default!;
    /// <summary>
    /// Optional identifier used to scope this provider's toasts.
    /// Only toasts with a matching <c>ProviderId</c> will be shown.
    /// </summary>
    [Parameter] public string? ProviderId { get; set; }

    /// <summary>
    /// The current toast position on screen. Defaults to BottomRight.
    /// </summary>
    private ToastPosition? Position { get; set; } = ToastPosition.BottomRight;
    /// <summary>
    /// List of currently active toast instances.
    /// </summary>

    private readonly List<ToastInstance> _toasts = new();
    /// <summary>
    /// CSS classes corresponding to the toast position.
    /// </summary>
    private string PositionClasses => GetPositionClasses();

    /// <summary>
    /// Subscribes to toast events when the component is initialized.
    /// </summary>
    protected override void OnInitialized()
    {
        ToastService.OnToastAdded += HandleToastAdded;
        ToastService.OnToastRemoved += HandleToastRemoved;
    }
    /// <summary>
    /// Handles adding a new toast instance to the display if it matches this provider's ID.
    /// </summary>
    /// <param name="toast">The toast instance to add.</param>
    private void HandleToastAdded(ToastInstance toast)
    {
        if (toast.Options.ProviderId != ProviderId) 
            return;

        InvokeAsync(() =>
        {
            Position = toast.Options.Position;
            _toasts.Add(toast);
            StateHasChanged();
        });
    }
    /// <summary>
    /// Handles removing a toast by its ID.
    /// </summary>
    /// <param name="toastId">The ID of the toast to remove.</param>
    private void HandleToastRemoved(string toastId)
    {

        InvokeAsync(() =>
        { 
            var item = _toasts.FirstOrDefault(t => t.Id == toastId);
            if (item != null)
            {
                _toasts.Remove(item);
                StateHasChanged();
            }
        });
    }
    /// <summary>
    /// Handles removing a toast by its ID.
    /// </summary>
    /// <param name="toastId">The ID of the toast to remove.</param>
    private async Task HandleToastClose(string toastId)
    {
        await InvokeAsync(() => ToastService.Remove(toastId));
    }

    /// <summary>
    /// Maps the toast position enum to corresponding Tailwind CSS classes.
    /// </summary>
    /// <returns>CSS classes for positioning the toast container.</returns>
    private string GetPositionClasses()
    {
        return Position switch
        {
            ToastPosition.TopLeft => "fixed top-5 left-5 z-[80] space-y-4",
            ToastPosition.TopCenter => "fixed top-5 left-1/2 -translate-x-1/2 z-[80] space-y-4",
            ToastPosition.TopRight => "fixed top-5 right-5 z-[80] space-y-4",
            ToastPosition.BottomLeft => "fixed bottom-5 left-5 z-[80] space-y-4",
            ToastPosition.BottomCenter => "fixed bottom-5 left-1/2 -translate-x-1/2 z-[80] space-y-4",
            ToastPosition.BottomRight => "fixed bottom-5 right-5 z-[80] space-y-4",
            _ => "fixed top-5 right-5 z-[80] space-y-4" 
        };
    }
    /// <summary>
    /// Unsubscribes from toast events and performs cleanup when the component is disposed.
    /// </summary>
    public void Dispose()
    {
        ToastService.OnToastAdded -= HandleToastAdded;
        ToastService.OnToastRemoved -= HandleToastRemoved;
        GC.SuppressFinalize(this);
    }
}