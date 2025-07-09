using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorFlow.Components;

public partial class NavItem
{
    [Parameter] public string Class { get; set; }
    [Parameter] public string Href { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public bool IsCollapsed { get; set; }
    [Parameter] public NavLinkMatch Match { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool IsExpanded { get; set; } = false;

    private void ToggleExpand()
    {
        IsExpanded = !IsExpanded;
    }
    
    private string HideableClass => ClassBuilder
        .Default("hidden")
        .AddClass("sm:block", !IsCollapsed)
        .Build();

    private string NavItemClass => ClassBuilder
        .Default("flex justify-center items-center gap-2 px-4 py-3 rounded-lg text-white hover:text-white focus:text-white active:text-white hover:bg-neutral-900 cursor-pointer flex items-center gap-2 w-full")
        .AddClass("md:justify-start",!IsCollapsed)
        .Build();
    
    private string NavItemContentClass => ClassBuilder
        .Default("flex align-center gap-2 flex-grow")
        .AddClass("justify-center", IsCollapsed)
        .Build();
 
    private string NavItemVisibilityClass => ClassBuilder
        .Default("hidden")
        .AddClass("md:block", !IsCollapsed)
        .Build();

    private string ArrowRotation => IsExpanded ? "rotate-90" : "rotate-0";
}