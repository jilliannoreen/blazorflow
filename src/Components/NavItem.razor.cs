using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
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
    // NavLink match behavior (e.g., prefix or full match)
    [Parameter] public NavLinkMatch Match { get; set; }
    // Optional child content, typically for sub-items or nested nav
    [Parameter] public RenderFragment? ChildContent { get; set; }
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
        .Default("flex justify-center items-center gap-2 px-2 py-2.5 rounded-lg text-white hover:text-white focus:text-white active:text-white hover:bg-neutral-900 cursor-pointer")
        .AddClass("lg:justify-start",!IsCollapsed)
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Inner content class builder to manage alignment and spacing
    /// </summary>
    private string NavItemContentClass => ClassBuilder
        .Default("flex justify-center align-center gap-2 flex-grow")
        .AddClass("lg:justify-start", !IsCollapsed)
        .Build();

    /// <summary>
    /// Class to control visibility of elements depending on collapsed state
    /// </summary>
    private string NavItemVisibilityClass => ClassBuilder
        .Default("hidden")
        .AddClass("lg:block", !IsCollapsed)
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
        {
            ToggleExpand();
        }
    }
}