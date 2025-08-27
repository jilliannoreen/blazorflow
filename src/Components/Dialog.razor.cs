using BlazorFlow.Enums;
using BlazorFlow.Services;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;
/// <summary>
/// A fully dynamic, lazily-rendered modal component with Flowbite interop, animations,
/// and support for callback-driven dialog results. 
/// </summary>
public partial class Dialog : ComponentBase, IAsyncDisposable 
{
    [Inject] private IDialogService DialogInterop { get; set; }
    
    #region Parameters

    /// <summary>Dialog title displayed in the header.</summary>
    [Parameter] public string Title { get; set; } = string.Empty;

    /// <summary>Enables or disables the modal backdrop (dimmed background).</summary>
    [Parameter] public bool Backdrop { get; set; } = false;

    /// <summary>Controls whether the modal can be closed via UI actions.</summary>
    [Parameter] public bool Closable { get; set; } = false;

    /// <summary>Size of the modal (e.g. Small, Large, ExtraLarge).</summary>
    [Parameter] public DialogSize Size { get; set; } = DialogSize.Default;

    /// <summary>Placement of the modal on screen (e.g. Center, TopLeft).</summary>
    [Parameter] public DialogPlacement DialogPlacement { get; set; } = DialogPlacement.CenterCenter;

    /// <summary>Content to render in the modal header.</summary>
    [Parameter] public RenderFragment? DialogHeader { get; set; }

    /// <summary>Content to render in the modal body.</summary>
    [Parameter] public RenderFragment? DialogBody { get; set; }

    /// <summary>Content to render in the modal footer (e.g. buttons).</summary>
    [Parameter] public RenderFragment? DialogFooter { get; set; }

    /// <summary>Callback triggered when modal is shown.</summary>
    [Parameter] public EventCallback OnShowCallback { get; set; }

    /// <summary>Callback triggered when modal is hidden.</summary>
    [Parameter] public EventCallback OnHideCallback { get; set; }

    /// <summary>Callback triggered when modal is toggled (shown/hidden).</summary>
    [Parameter] public EventCallback OnToggleCallback { get; set; }
    
    [Parameter] public string Class { get; set; }
    [Parameter] public Color BackgroundColor { get; set; } = Color.Light;

    #endregion

    #region JSInvokable (Interop Events)

    /// <summary>Invoked from JS when the modal is shown.</summary>
    [JSInvokable] public Task OnShow() => OnShowCallback.InvokeAsync();

    /// <summary>Invoked from JS when the modal is hidden.</summary>
    [JSInvokable]
    public async Task OnHide() => await OnHideCallback.InvokeAsync();
    

    /// <summary>Invoked from JS when the modal is toggled.</summary>
    [JSInvokable] public Task OnToggle() => OnToggleCallback.InvokeAsync();

    #endregion

    #region Private Fields

    /// <summary>Reference to this instance for JS interop callbacks.</summary>
    private DotNetObjectReference<Dialog>? _dotNetRef;

    /// <summary>Tracks whether the modal is currently in the DOM.</summary>
    private bool _isVisible;

    /// <summary>Controls CSS animation class (fade-in/fade-out).</summary>
    private bool _isShowingAnimation;

    /// <summary>Reference to allow returning a DialogResult.</summary>
    private DialogReference? _dialogReference;

    #endregion

    #region Lifecycle

    /// <summary>
    /// Sets up the .NET reference for JS interop on first render.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
        }
    }

    /// <summary>
    /// Properly disposes of resources used by the component.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        _dotNetRef = null;
    }

    #endregion
    
    #region Public Methods
    /// <summary>
    /// Initializes the Flowbite modal with updated options and binds .NET interop reference.
    /// </summary>
    public async Task ReInitAsync()
    {
        var options = new DialogOptions
        {
            Closable = Closable,
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
            }
        };

        await DialogInterop.InitAsync(Id, options, _dotNetRef);
        
    }
    
    /// <summary>
    /// Programmatically shows the drawer.
    /// </summary>
    public async Task ShowAsync()
    {
        _isVisible = true;             
        StateHasChanged();              

        await Task.Yield();     
        await ReInitAsync();            
        await DialogInterop.ShowAsync(Id);  
    }

    /// <summary>
    /// Hides the modal with fade-out animation and removes it from DOM.
    /// </summary>
    public async Task HideAsync()
    {
        await DialogInterop.HideAsync(Id);

        _isVisible = false;
        StateHasChanged();
    }
    
    #endregion

    #region Private Methods
    /// <summary>
    /// Returns the max-width class based on the selected size.
    /// </summary>
    private string GetSizeClass() => Size switch
    {
        DialogSize.Small => "max-w-md",
        DialogSize.Large => "max-w-4xl",
        DialogSize.ExtraLarge => "max-w-5xl",
        _ => "max-w-lg"
    };
    
    /// <summary>
    /// Unique ID assigned to the dialog.
    /// </summary>
    private string Id { get; } = $"dialog-{Guid.NewGuid()}";

    private string DialogClass => ClassBuilder
        .Default("overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 flex justify-center items-center w-full md:inset-0 h-full max-h-full")
        .AddClass(Class)
        .Build();
    
    private string DialogContainerClass => ClassBuilder
        .Default("relative p-10 rounded-2xl shadow-sm flex flex-col gap-3")
        .AddClass(BackgroundColor switch
        {
            Color.Secondary => "bg-(--secondary)",
            Color.Surface => "bg-(--surface)",
            _ => "bg-white"
        })
        .Build();

    #endregion
}

/// <summary>
/// Options to configure dialog behavior and appearance.
/// </summary>
public class DialogOptions
{
    /// <summary>Specifies the modal placement (e.g., center-center).</summary>
    public string Placement { get; set; } = "center-center";

    /// <summary>Determines if the modal is closable via UI controls.</summary>
    public bool Closable { get; set; } = false;
}

/// <summary>
/// Reference to a specific dialog instance used for returning results.
/// </summary>
public class DialogReference
{
    private readonly TaskCompletionSource<DialogResult> _tcs = new();

    /// <summary>Returns a Task that completes when the dialog is closed.</summary>
    public Task<DialogResult> Result => _tcs.Task;

    /// <summary>Completes the dialog with a result.</summary>
    public void Close(DialogResult result)
    {
        _tcs.TrySetResult(result);
    }
}

/// <summary>
/// Represents the result returned by a dialog.
/// </summary>
public class DialogResult
{
    /// <summary>Indicates whether the dialog was cancelled.</summary>
    public bool Cancelled { get; set; }

    /// <summary>Optional return data from the dialog.</summary>
    public object? Data { get; set; }

    /// <summary>Creates a successful dialog result with optional data.</summary>
    public static DialogResult Ok(object? data = null) => new() { Cancelled = false, Data = data };

    /// <summary>Creates a cancelled dialog result.</summary>
    public static DialogResult Cancel() => new() { Cancelled = true };
}


