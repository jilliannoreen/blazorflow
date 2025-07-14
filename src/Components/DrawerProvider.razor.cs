using BlazorFlow.Enums;
using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class DrawerProvider : ComponentBase
{
    [Inject] IDrawerService DrawerService { get; set; } = null!;
    private string? Title { get; set; }
    private RenderFragment? Body { get; set; }
    private DrawerPosition DrawerPosition { get; set; } = DrawerPosition.Right;
    private DrawerSize DrawerSize { get; set; } = DrawerSize.Half;
    private bool Backdrop { get; set; } = true;
    private bool Edge { get; set; } = false;
    private bool BodyScrolling { get; set; } = false;
    private string DrawerKey => $"{DrawerPosition}-{Guid.NewGuid()}";
    private Drawer? _drawerRef;
    


    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("DrawerProvider Initialized");
        DrawerService.OnShowRequested += ShowDrawerAsync;
        DrawerService.OnHideRequested += HideDrawerAsync;
    }

    private async Task ShowDrawerAsync(string title, RenderFragment content, Drawer.DrawerOptions? options)
    {
        Title = title;
        Body = content;
        
        if (options is not null)
        {
            // Optional: Map string values to enum if needed
            Enum.TryParse<DrawerPosition>(options.Placement, true, out var position);
            DrawerPosition = position;

            // Optionally pass size via Tag/EdgeOffset/etc.
            // (You could extend DrawerOptions to also contain size)
        }

        await InvokeAsync(StateHasChanged);
        await Task.Yield(); 
        if (_drawerRef is not null)
        {
            await _drawerRef.ReInitAsync();
            await _drawerRef.Show();
        }
    }

    private async Task HideDrawerAsync()
    {
        if (_drawerRef is not null)
        {
            await _drawerRef.Hide();
        }

        Title = string.Empty;
        Body = null;
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DrawerService.OnShowRequested -= ShowDrawerAsync;
        DrawerService.OnHideRequested -= HideDrawerAsync;
    }
}