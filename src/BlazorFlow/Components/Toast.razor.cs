using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using System.Timers;

namespace BlazorFlow.Components;

/// <summary>
/// A reusable toast notification component that shows transient messages.
/// </summary>
public partial class Toast : ComponentBase
{
    /// <summary>
    /// The title or main message of the toast.
    /// </summary>
    [Parameter] public required string Title { get; set; }
    /// <summary>
    /// Optional subtitle or description text.
    /// </summary>
    [Parameter] public string? Subtitle { get; set; }
    /// <summary>
    /// The type of the toast, which determines its color and icon.
    /// </summary>
    [Parameter] public ToastType Type { get; set; }
    /// <summary>
    /// Whether the toast can be manually dismissed.
    /// </summary>
    [Parameter] public bool Dismissible { get; set; } = true;
    /// <summary>
    /// Optional icon to display in the toast.
    /// </summary>
    [Parameter] public string? Icon { get; set; }
    /// <summary>
    /// Callback invoked when the toast is closed.
    /// </summary>
    [Parameter] public EventCallback OnClose { get; set; }
    /// <summary>
    /// Duration (in milliseconds) before the toast automatically disappears. 
    /// Set to 0 to disable auto-dismiss.
    /// </summary>
    [Parameter] public int Duration { get; set; }

    /// <summary>
    /// Timer used to automatically dismiss the toast after a specified duration.
    /// </summary>
    private System.Timers.Timer? _timer;

    /// <summary>
    /// Indicates whether the toast is currently visible on the screen.
    /// </summary>
    private bool _isVisible = true;

    /// <summary>
    /// Initializes the toast component and starts the auto-dismiss timer if specified.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Duration > 0)
            StartTimer(Duration);
        
    }

    /// <summary>
    /// Dynamically builds the CSS class for the toast wrapper based on state and type.
    /// </summary>
    private string ToastClass => ClassBuilder
        .Default("flex items-center w-full max-w-md bg-white p-4 gap-4 rounded-lg shadow-sm")
        .AddClass(Type switch
            {
                ToastType.Success => "text-(--success)",
                ToastType.Info => "text-(--info)",
                ToastType.Warning => "text-(--warning)",
                ToastType.Error => "text-(--error)",
                _ => "text-black",
            })
        .AddClass(_isVisible ? "toast-fade-in" : "toast-fade-out")
        .Build();

    /// <summary>
    /// Gets the CSS class for the icon based on the toast type.
    /// </summary>
    private string IconClass => ClassBuilder
        .Default("inline-flex items-center justify-center shrink-0 w-8 h-8 rounded-full")
        .AddClass(Type switch
            {
                ToastType.Success => "bg-(--success)/25 text-(--success)",
                ToastType.Info => "bg-(--info)/25 text-(--info)",
                ToastType.Warning => "bg-(--warning)/25 text-(--warning)",
                ToastType.Error => "bg-(--error)/25 text-(--error)",
                _ => "bg-black/25 text-black",
            })
        .Build();

    /// <summary>
    /// Starts the auto-dismiss timer with the specified duration.
    /// </summary>
    /// <param name="duration">The duration in milliseconds.</param>
    private void StartTimer(int duration)
    {
        _timer = new System.Timers.Timer(duration); 
        _timer.Elapsed += HandleTimerElapsed;
        _timer.AutoReset = false;
        _timer.Start();
    }

    /// <summary>
    /// Handles the timer expiration by triggering toast close on the UI thread.
    /// </summary>
    private void HandleTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        InvokeAsync(CloseToast);
    }

    /// <summary>
    /// Closes the toast with fade-out animation and invokes the OnClose callback.
    /// </summary>
    private async Task CloseToast()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _isVisible = false;
        StateHasChanged();

        await Task.Delay(300); 
        await OnClose.InvokeAsync(); 
    }

}

/// <summary>
/// Configuration options for displaying a toast.
/// </summary>
public class ToastOptions
{
    /// <summary>
    /// Duration before toast auto-dismisses (default: 4000 ms).
    /// </summary>
    public int Duration { get; set; } = 4000;

    /// <summary>
    /// Whether the toast is manually dismissible.
    /// </summary>
    public bool Dismissible { get; set; } = true;

    /// <summary>
    /// Optional icon to show in the toast.
    /// </summary>
    public string? Icon { get; set; } = Icons.Icons.Outlined.Alert;

    /// <summary>
    /// Optional ID used to associate the toast with a provider or container.
    /// </summary>
    public string? ProviderId { get; set; } = null;

    /// <summary>
    /// Position on screen where the toast should appear.
    /// </summary>
    public ToastPosition Position { get; set; } = ToastPosition.BottomRight;
}

/// <summary>
/// Represents a toast instance that can be queued and rendered.
/// </summary>
public class ToastInstance
{
    /// <summary>
    /// Unique identifier for this toast instance.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The main message or title of the toast.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional additional message or subtitle.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Type of the toast (success, error, info, etc.).
    /// </summary>
    public ToastType Type { get; set; }

    /// <summary>
    /// Configuration options for the toast.
    /// </summary>
    public ToastOptions Options { get; set; } = new();

    /// <summary>
    /// Callback that is invoked when the toast is closed.
    /// </summary>
    public EventCallback OnClose { get; set; }
}