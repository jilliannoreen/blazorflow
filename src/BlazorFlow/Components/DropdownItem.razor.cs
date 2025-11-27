using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorFlow.Components;

public partial class DropdownItem : ComponentBase
{
    [CascadingParameter]
    private Dropdown ParentDropdown { get; set; } = default!;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public bool Disabled { get; set; }
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetDropdownItemClass() => ClassBuilder
        .Default("flex w-full items-center px-4 py-2 text-sm font-normal text-center")
        .AddClass(Disabled ? "cursor-not-allowed text-gray-400" :
            "cursor-pointer text-(--gray-900) hover:bg-(--primary) focus:outline-none")
        .Build();
    
    private async Task HandleClick(MouseEventArgs args)
    {
        if (Disabled) 
            return;

        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(args);
        
        if (ParentDropdown.DismissOnClick)
            await ParentDropdown.CloseDropdown();
        
    }
}