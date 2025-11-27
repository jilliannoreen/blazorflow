using BlazorFlow.Enums;
using BlazorFlow.Services;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class Drawer : ComponentBase 
{
    [Inject] private IDrawerService DrawerInterop { get; set; } 
    /// <summary>
    /// Specifies the size of the drawer (width for left/right, height for top/bottom).
    /// </summary>
    [Parameter] public DrawerSize Size { get; set; } = DrawerSize.Medium;
    /// <summary>
    /// Content to be rendered inside the drawer.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>
    /// The heading/title of the drawer.
    /// </summary>
    [Parameter] public string Title { get; set; } = "Drawer";
    /// <summary>
    /// Defines where the drawer appears on the screen: left, right, top, or bottom.
    /// </summary>
    [Parameter] public DrawerPosition Position { get; set; } = DrawerPosition.Right;

    /// <summary>
    /// Enables edge mode for the drawer, allowing a portion of it to remain visible
    /// when hidden (useful for indicating the drawer's presence).
    /// </summary>
    [Parameter] public bool Edge { get; set; } = false;

    /// <summary>
    /// Determines whether a backdrop overlay is displayed behind the drawer when it is open.
    /// Helps focus the user’s attention on the drawer content. Default is <c>true</c>.
    /// </summary>
    [Parameter] public bool Backdrop { get; set; } = false;
    /// <summary>
    /// Specifies whether the main page content can still be scrolled while the drawer is open.
    /// Set to <c>true</c> to allow scrolling; <c>false</c> to lock scroll (recommended for modal-like drawers).
    /// </summary>
    [Parameter] public bool BodyScrolling { get; set; } = false;
    [Parameter] public string Class { get; set; } 
    
    
    /// <summary>
    /// A unique DOM ID assigned to this drawer instance.
    /// </summary>
    private string Id { get; } = $"drawer-{Guid.NewGuid()}";
    /// <summary>
    /// The ID used for the drawer's ARIA label.
    /// </summary>
    private string LabelId => $"{Id}-label";
    
    [Parameter] public bool Rounded { get; set; } 
    /// <summary>
    /// Returns the appropriate Tailwind CSS class that sets the drawer's size
    /// based on the selected <see cref="DrawerSize"/> and <see cref="DrawerPosition"/>.
    /// Width classes are applied for left/right positions, while height classes
    /// are applied for top/bottom positions. If <see cref="DrawerSize.Custom"/> is used,
    /// the <see cref="CustomSize"/> value will be returned instead.
    /// </summary>
    /// <returns>A string representing the Tailwind CSS size class.</returns>
    private string GetSizeClass()
    {
        return Position switch
        {
            DrawerPosition.Left or DrawerPosition.Right => Size switch
            {
                DrawerSize.Half => "w-full md:w-1/2",
                DrawerSize.Small => "w-full sm:w-sm",
                DrawerSize.Medium => "w-full sm:w-md",
                DrawerSize.Large => "w-full sm:w-lg",
                DrawerSize.ExtraLarge => "w-full sm:w-xl",
                DrawerSize.Full => "w-full",
                _ => "w-full sm:w-md"
            },

            DrawerPosition.Top or DrawerPosition.Bottom => Size switch
            {
                DrawerSize.Half => "h-1/2",
                DrawerSize.Small => "h-full sm:h-1/3",
                DrawerSize.Medium => "h-full sm:h-1/2",
                DrawerSize.Large => "h-full sm:h-3/4",
                DrawerSize.ExtraLarge => "h-full sm:h-9/10",
                DrawerSize.Full => "h-full",
                _ => "h-full sm:h-64"
            },

            _ => string.Empty
        };
    }
    
    /// <summary>
    /// Returns the Tailwind CSS classes responsible for drawer placement and transitions based on the <see cref="DrawerPosition"/>.
    /// </summary>
    private string GetPositionClass()
    {
        return Position switch
        {
            DrawerPosition.Left => "top-0 left-0 z-40 h-screen -translate-x-full",
            DrawerPosition.Right => "top-0 right-0 z-40 h-screen translate-x-full",
            DrawerPosition.Top => "top-0 left-0 z-40 w-full -translate-y-full",
            DrawerPosition.Bottom => "bottom-0 left-0 z-40 w-full translate-y-full",
            _ => string.Empty
        };
    }
    
    private string GetRoundedClass()
    {
        if (!Rounded)
            return string.Empty;
        
        return Position switch
        {
            DrawerPosition.Left => "rounded-r-2xl",
            DrawerPosition.Right => "rounded-l-2xl",
            DrawerPosition.Top => "rounded-b-2xl",
            DrawerPosition.Bottom => "rounded-t-2xl",
            _ => string.Empty
        };
    }

    /// <summary>
    /// Builds the full Tailwind class string for the drawer element.
    /// </summary>
    private string DrawerClass => ClassBuilder
        .Default("fixed z-40 overflow-y-auto transition-transform bg-(--foreground)")
        .AddClass(GetPositionClass())
        .AddClass(GetRoundedClass())
        .AddClass(GetSizeClass())
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Initializes the Flowbite drawer via JavaScript interop on first render.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ReInitAsync();
        }
    }

    /// <summary>
    /// Reinitializes the drawer component on the client side by invoking the JavaScript interop.
    /// This is useful for reapplying options after dynamic changes to placement, backdrop behavior,
    /// body scrolling, or edge snapping.
    /// This method should be called after the drawer is rendered or after any configuration changes.
    /// </summary>
    public async Task ReInitAsync()
    {
        var options = new DrawerOptions
        {
            Placement = Position.ToString().ToLower(),
            Backdrop = Backdrop,
            BodyScrolling = BodyScrolling,
            Edge = Edge
        };
        
        await DrawerInterop.InitAsync(Id, options);
    }

    /// <summary>
    /// Programmatically shows the drawer.
    /// </summary>
    public async Task Show() => await DrawerInterop.ShowAsync(Id);

    /// <summary>
    /// Programmatically hides the drawer.
    /// </summary>
    public async Task Hide() => await DrawerInterop.HideAsync(Id);
    
    /// <summary>
    /// Represents the configuration options for a Flowbite drawer component.
    /// These options control the drawer's placement, behavior, and appearance.
    /// </summary>
    public class DrawerOptions
    {
        /// <summary>
        /// Specifies the placement of the drawer relative to the screen.
        /// Valid values include: "left", "right", "top", or "bottom".
        /// Default is "right".
        /// </summary>
        public string Placement { get; set; } = "right";

        /// <summary>
        /// Indicates whether a backdrop overlay should be displayed behind the drawer.
        /// Default is true.
        /// </summary>
        public bool Backdrop { get; set; } = true;

        /// <summary>
        /// Specifies whether body scrolling should be allowed while the drawer is open.
        /// Default is false to prevent background scrolling.
        /// </summary>
        public bool BodyScrolling { get; set; } = false;

        /// <summary>
        /// Enables edge mode, which keeps a small portion of the drawer visible when inactive.
        /// Default is false.
        /// </summary>
        public bool Edge { get; set; } = false;

        /// <summary>
        /// Sets the offset height (e.g., "bottom-[60px]") used when edge mode is enabled.
        /// This determines how much of the drawer remains visible.
        /// </summary>
        public string? EdgeOffset { get; set; }

        /// <summary>
        /// Custom Tailwind CSS utility classes applied to the backdrop element.
        /// Allows full control over the backdrop styling.
        /// </summary>
        public string BackdropClasses { get; set; } = "bg-gray-900/50 dark:bg-gray-900/80 fixed inset-0 z-30";
    }
}