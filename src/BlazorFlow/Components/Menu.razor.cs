using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Menu : ComponentBase
{
    [Inject] 
    public IPopoverService? PopoverService { get; set; }
    private ElementReference _triggerRef;
    private bool _isOpen;
    private Guid _popoverId;

    [Parameter] public string? Text { get; set; }
    [Parameter] public string? Icon { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    private void Open()
    {
        _popoverId = PopoverService.Show(builder =>
        {
            builder.OpenComponent(0, typeof(Popover));
            builder.AddAttribute(1, "ChildContent", ChildContent);
            builder.CloseComponent();
        }, _triggerRef);

        _isOpen = true;
    }

    private void Close()
    {
        PopoverService.Close(_popoverId);
        _isOpen = false;
    }
}