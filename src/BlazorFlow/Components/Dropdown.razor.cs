using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorFlow.Components;

public partial class Dropdown : IDisposable
{
    [Parameter]
    public string Label { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public string Icon { get; set; }
    [Parameter]
    public Color TriggerButtonColor { get; set; }
    [Parameter] 
    public Variant TriggerButtonVariant { get; set; } = Variant.Filled;
    [Parameter]
    public VisualPlacement IconPlacement { get; set; } = VisualPlacement.End;
    [Parameter]
    public bool Inline { get; set; }
    [Parameter]
    public DropdownPlacement Placement { get; set; } = DropdownPlacement.BottomStart;
    [Parameter]
    public bool DismissOnClick { get; set; } = true;
    [Parameter]
    public EventCallback<MouseEventArgs> OnTriggerClick { get; set; }
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    private bool _isOpen = false;
    private string _id = $"dropdown-{Guid.NewGuid()}";
    
    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape")
        {
            await CloseDropdown();
        }
    }
    
    public async Task CloseDropdown()
    {
        if (_isOpen)
        {
            _isOpen = false;
            StateHasChanged();
        }
    }
    
    private async Task ToggleDropdown(MouseEventArgs args)
    {
        if (OnTriggerClick.HasDelegate)
        {
            await OnTriggerClick.InvokeAsync(args);
        }

        _isOpen = !_isOpen;
        await IsOpenChanged.InvokeAsync(_isOpen);
    }

    private string MenuClass => ClassBuilder
        .Default("absolute w-fit rounded border border-gray-200 bg-white text-gray-900")
        .AddClass(Placement switch
        {
            DropdownPlacement.Bottom or DropdownPlacement.TopStart or DropdownPlacement.TopEnd => "bottom-full mb-2",
            DropdownPlacement.Right or DropdownPlacement.RightStart or DropdownPlacement.RightEnd => "left-full ml-2",
            DropdownPlacement.Left or DropdownPlacement.LeftStart or DropdownPlacement.LeftEnd => "right-full mr-2",
            _ => "top-full mt-2" 
        })
        .AddClass(Placement switch
        {
            DropdownPlacement.TopStart or DropdownPlacement.BottomStart => "left-0",
            DropdownPlacement.TopEnd or DropdownPlacement.BottomEnd => "right-0",
            DropdownPlacement.TopCenter or DropdownPlacement.BottomCenter => "right-0 left-0",
            _ => "" 
        })
        .Build();
    
    public void Dispose()
    {
        
    }
}