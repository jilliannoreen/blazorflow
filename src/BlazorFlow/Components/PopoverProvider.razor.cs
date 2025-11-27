using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class PopoverProvider : ComponentBase
{
    [Inject] private IPopoverService PopoverService { get; set; }
    
    protected override void OnInitialized()
    {
        PopoverService.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        PopoverService.OnChange -= StateHasChanged;
    }

    private string GetStyle(ElementReference anchor)
    {
        // TODO: Use JS interop to calculate real position
        return "position: absolute; top:100px; left:100px;";
    }
    
}

