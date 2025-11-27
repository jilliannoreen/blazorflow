using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class MenuItem : ComponentBase
{
    [Parameter] public string? Text { get; set; }
    [Parameter] public string? Icon { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool _isSubOpen;
    private bool HasChildren => ChildContent is not null;

    private async Task HandleClick()
    {
        if (!HasChildren && OnClick.HasDelegate)
            await OnClick.InvokeAsync();
    }

    private void OpenSubMenu() => _isSubOpen = true;
    private void CloseSubMenu() => _isSubOpen = false;
}