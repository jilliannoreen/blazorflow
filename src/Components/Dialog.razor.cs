using BlazorFlow.Enums;
using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;

public partial class Dialog : ComponentBase, IAsyncDisposable 
{
    [Inject] private IDialogService DialogInterop { get; set; }
    
    [Parameter] public DialogPlacement DialogPlacement { get; set; } = DialogPlacement.CenterCenter;
    [Parameter] public RenderFragment? DialogHeader { get; set; }
    [Parameter] public RenderFragment? DialogContent { get; set; }
    [Parameter] public RenderFragment? DialogFooter { get; set; }
    [Parameter] public bool Backdrop { get; set; } = false;
    [Parameter] public bool Closable { get; set; } = false;
    /// <summary>
    /// The heading/title of the dialog.
    /// </summary>
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public DialogSize Size { get; set; } = DialogSize.Default;
    [Parameter] public EventCallback OnShowCallback { get; set; }
    [Parameter] public EventCallback OnHideCallback { get; set; }
    [Parameter] public EventCallback OnToggleCallback { get; set; }
    [JSInvokable] public Task OnShow() => OnShowCallback.InvokeAsync();
    [JSInvokable] public Task OnHide() => OnHideCallback.InvokeAsync();
    [JSInvokable] public Task OnToggle() => OnToggleCallback.InvokeAsync();

    
    private DotNetObjectReference<Dialog>? _dotNetRef;

    
    
    /// <summary>
    /// Initializes the Flowbite dialog via JavaScript interop on first render.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await ReInitAsync();
        }
    }

    public async Task ReInitAsync()
    {
        var options = new DialogOptions
        {
            Closable = Closable,
            Backdrop = Backdrop ? "dynamic" : "static",
            Placement = DialogPlacement switch
            {
                DialogPlacement.TopCenter => "top-center",
                DialogPlacement.TopLeft => "top-left",
                DialogPlacement.TopRight => "top-right",
                DialogPlacement.CenterCenter => "center-center",
                DialogPlacement.CenterLeft => "center-left",
                DialogPlacement.CenterRight => "center-right",
                DialogPlacement.BottomCenter => "bottom-center",
                DialogPlacement.BottomLeft => "bottom-left",
                DialogPlacement.BottomRight => "bottom-right",
                _ => "center-center"
            },
        };

        await DialogInterop.InitAsync(Id, options, _dotNetRef);
    }

    /// <summary>
    /// Programmatically shows the drawer.
    /// </summary>
    public async Task Show() => await DialogInterop.ShowAsync(Id);

    /// <summary>
    /// Programmatically hides the drawer.
    /// </summary>
    public async Task Hide() => await DialogInterop.HideAsync(Id);
    
    /// <summary>
    /// A unique DOM ID assigned to this dialog instance.
    /// </summary>
    private string Id { get; } = $"dialog-{Guid.NewGuid()}";
    /// <summary>
    /// The ID used for the drawer's ARIA label.
    /// </summary>
    private string LabelId => $"{Id}-label";
    
    
    private string GetSizeClass()
    {
        return Size switch
        {
            DialogSize.Small => "max-w-md",
            DialogSize.Large => "max-w-4xl",
            DialogSize.ExtraLarge => "max-w-5xl",
            _ => "max-w-lg"
        };
    }
    
    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
    
    
    public class DialogOptions
    {
        public string Placement { get; set; } = "center-center";

        /// <summary>
        /// Indicates whether a backdrop overlay should be displayed behind the drawer.
        /// Default is true.
        /// </summary>
        public string Backdrop { get; set; } = "static";

        public bool Closable { get; set; } = false;

        /// <summary>
        /// Custom Tailwind CSS utility classes applied to the backdrop element.
        /// Allows full control over the backdrop styling.
        /// </summary>
        public string BackdropClasses { get; set; } = "bg-gray-900/50 dark:bg-gray-900/80 fixed inset-0 z-40";
    }
}

