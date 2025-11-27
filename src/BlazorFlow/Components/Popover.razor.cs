using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Popover : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

}