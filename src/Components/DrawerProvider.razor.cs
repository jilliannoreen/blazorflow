using BlazorFlow.Enums;
using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

/// <summary>
/// DrawerProvider acts as a wrapper component that listens to show/hide events
/// from the <see cref="IDrawerService"/> and dynamically renders a Drawer component.
/// </summary>
public partial class DrawerProvider : ComponentBase, IDisposable
{
    /// <summary>
    /// Injected DrawerService used to subscribe to show/hide events.
    /// </summary>
    [Inject] IDrawerService DrawerService { get; set; } = null!;
    /// <summary>
    /// Drawer title to be displayed.
    /// </summary>
    private string? Title { get; set; }
    /// <summary>
    /// RenderFragment (UI content) to be shown inside the Drawer body.
    /// </summary>
    private RenderFragment? Body { get; set; }
    /// <summary>
    /// Position of the Drawer (e.g., left, right, top, bottom).
    /// </summary>
    private DrawerPosition DrawerPosition { get; set; } = DrawerPosition.Right;
    /// <summary>
    /// Size of the Drawer (e.g., small, medium, half, full).
    /// </summary>
    private DrawerSize DrawerSize { get; set; } = DrawerSize.Half;
    /// <summary>
    /// Whether a backdrop (overlay) should be shown behind the Drawer.
    /// </summary>
    private bool Backdrop { get; set; } = true;
    /// <summary>
    /// Whether the Drawer should appear on the edge of the screen.
    /// </summary>
    private bool Edge { get; set; } = false;
    /// <summary>
    /// Whether body scrolling should be enabled when the Drawer is open.
    /// </summary>
    private bool BodyScrolling { get; set; } = false;
    /// <summary>
    /// A unique key for the Drawer instance based on its position.
    /// </summary>
    private string DrawerKey => $"{DrawerPosition}-{Guid.NewGuid()}";
    /// <summary>
    /// Reference to the Drawer instance used to call show/hide methods.
    /// </summary>
    private Drawer? _drawerRef;

    /// <summary>
    /// Subscribes to show/hide events from the DrawerService during initialization.
    /// </summary>
    protected override void OnInitialized()
    {
        DrawerService.OnShowRequested += ShowDrawerAsync;
        DrawerService.OnHideRequested += HideDrawerAsync;
    }

    /// <summary>
    /// Handles showing the Drawer with a title, content, and optional configuration.
    /// </summary>
    private async Task ShowDrawerAsync(string title, RenderFragment content, Drawer.DrawerOptions? options)
    {
        Title = title;
        Body = content;
        
        if (options is not null)
        {
            Enum.TryParse<DrawerPosition>(options.Placement, true, out var position);
            DrawerPosition = position;
        }

        await InvokeAsync(StateHasChanged);
        await Task.Yield(); 
        if (_drawerRef is not null)
        {
            await _drawerRef.ReInitAsync();
            await _drawerRef.Show();
        }
    }

    /// <summary>
    /// Handles hiding the Drawer and clearing its content.
    /// </summary>
    private async Task HideDrawerAsync()
    {
        if (_drawerRef is not null)
            await _drawerRef.Hide();

        Title = string.Empty;
        Body = null;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Unsubscribes from events to prevent memory leaks.
    /// </summary
    public void Dispose()
    {
        DrawerService.OnShowRequested -= ShowDrawerAsync;
        DrawerService.OnHideRequested -= HideDrawerAsync;
    }
}