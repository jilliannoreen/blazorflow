using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class NavMenu
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
}