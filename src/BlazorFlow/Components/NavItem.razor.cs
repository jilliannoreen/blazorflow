using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorFlow.Components;

public partial class NavItem
{
    // Custom CSS class to be applied to the nav item
    [Parameter] public string Class { get; set; }
    // Navigation target URL
    [Parameter] public string Href { get; set; }
    // Display label for the navigation item
    [Parameter] public string Label { get; set; }
    // Optional icon class or identifier (e.g., for a font icon)
    [Parameter] public string Icon { get; set; }
    // Whether the sidebar/navigation is in a collapsed state
    [Parameter] public bool IsCollapsed { get; set; }
    [Parameter] public bool IsCollapsable { get; set; } = false;
    // NavLink match behavior (e.g., prefix or full match)
    [Parameter] public NavLinkMatch Match { get; set; }
    // Optional child content, typically for sub-items or nested nav
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? Indicator { get; set; }
    [Parameter] public EventCallback<string> OnClick { get; set; }   
    [Parameter] public string ActiveClass { get; set; } = "!text-gray-800 !bg-(--secondary) font-medium";
    [Parameter] public bool IsActive { get; set; } = false;
    // Whether the nested content is currently expanded (e.g., sub-items)
    private bool IsExpanded { get; set; } = false;


    /// <summary>
    /// Toggles the expansion state of the nav item
    /// </summary>
    private void ToggleExpand()
    {
        IsExpanded = !IsExpanded;
    }

    /// <summary>
    /// Main navigation item class builder with responsive behavior
    /// </summary>
    private string NavItemClass => ClassBuilder
        .Default("flex justify-between items-center gap-2 px-2 py-2.5 rounded-lg " +
                 "focus:outline-none cursor-pointer")
        .AddClass("justify-center", IsCollapsable)
        .AddClass("justify-start", !IsCollapsable)
        .AddClass("lg:justify-start",!IsCollapsed)
        .AddClass(ActiveClass, IsActive)
        .AddClass(Class)
        .Build();
    

    /// <summary>
    /// Inner content class builder to manage alignment and spacing
    /// </summary>
    private string NavItemContentClass => ClassBuilder
        .Default("flex align-center gap-2 flex-grow")
        .AddClass("justify-center", IsCollapsable)
        .AddClass("justify-start", !IsCollapsable)
        .AddClass("lg:justify-start", !IsCollapsed && IsCollapsable)
        .Build();

    /// <summary>
    /// Class to control visibility of elements depending on collapsed state
    /// </summary>
    private string NavItemVisibilityClass => ClassBuilder
        .Default(string.Empty)
        .AddClass("hidden", IsCollapsable)
        .AddClass("block", !IsCollapsable)
        .AddClass("lg:block", !IsCollapsed && IsCollapsable)
        .Build();
    /// <summary>
    /// Rotation class for expand/collapse icon (e.g., an arrow)
    /// </summary>
    private string ArrowRotation => IsExpanded ? "rotate-90" : "rotate-0";
    /// <summary>
    /// Handles keyboard interaction for accessibility.
    /// Triggers expansion or collapse of the menu item when the Enter or Space key is pressed.
    /// </summary>
    /// <param name="e">The keyboard event arguments.</param>
    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == " ")
            ToggleExpand();
    }

    /// <summary>
    /// Resolves the navigation target URL for the navigation item.
    /// If no URL is provided, a default "javascript:void(0);" value is assigned.
    /// </summary>
    private string ResolvedHref => string.IsNullOrWhiteSpace(Href) ? "javascript:void(0);" : Href;

    /// <summary>
    /// Determines the resolved match behavior for the navigation item based on the provided target URL and match criteria.
    /// </summary>
    private NavLinkMatch ResolvedMatch => string.IsNullOrWhiteSpace(Href) ? NavLinkMatch.All : Match;    
    private async Task HandleClick(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(Label);
    }
    
    private bool HasChildContent()
    {
        if (ChildContent is null) return false;

        var builder = new RenderTreeBuilder();
        ChildContent(builder);

        // If the fragment produced at least one render entry, it’s not empty.
        return builder.GetFrames().Array.Any();
    }
}