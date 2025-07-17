using BlazorFlow.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

public class DialogService : IDialogService
{
    private readonly IJSRuntime _js;
    /// <inheritdoc/>
    public event Func<string, RenderFragment, Dialog.DialogOptions?, Task>? OnShowRequested;
    /// <inheritdoc/>
    public event Func<Task>? OnHideRequested;


    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="js">The JavaScript runtime used for interop.</param>
    public DialogService(IJSRuntime js)
    {
        _js = js;
    }
    
    /// <inheritdoc />
    public async Task InitAsync(string id, Dialog.DialogOptions options, DotNetObjectReference<Dialog> dotNetRef)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.dialog.init", id, options, dotNetRef);
    }
    
    /// <inheritdoc />
    public async Task ShowAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.dialog.show", id);
    }
    /// <inheritdoc />
    public async Task HideAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.dialog.hide", id);
    }
    /// <inheritdoc />
    public async Task ToggleAsync(string id)
    {
        await _js.InvokeVoidAsync("flowbiteBlazorInterop.dialog.toggle", id);
    }
    /// <inheritdoc />
    public async Task<bool> IsVisibleAsync(string id)
    {
        return await _js.InvokeAsync<bool>("flowbiteBlazorInterop.dialog.isVisible", id);
    }
    /// <inheritdoc/>
    public Task HideAsync()
    {
        return OnHideRequested?.Invoke() ?? Task.CompletedTask;
    }
}