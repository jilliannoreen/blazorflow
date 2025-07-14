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
    public async Task InitAsync(string id, Dialog.DialogOptions options)
    {
        await _js.InvokeVoidAsync("dialogInterop.init", id, options);
    }
    
    /// <inheritdoc />
    public async Task ShowAsync(string id)
    {
        await _js.InvokeVoidAsync("dialogInterop.showDialog", id);
    }
    /// <inheritdoc />
    public async Task HideAsync(string id)
    {
        await _js.InvokeVoidAsync("dialogInterop.hideDialog", id);
    }
    /// <inheritdoc />
    public async Task ToggleAsync(string id)
    {
        await _js.InvokeVoidAsync("dialogInterop.toggleDialog", id);
    }
    /// <inheritdoc />
    public async Task<bool> IsVisibleAsync(string id)
    {
        return await _js.InvokeAsync<bool>("dialogInterop.isDialogVisible", id);
    }
    /// <inheritdoc/>
    public Task HideAsync()
    {
        return OnHideRequested?.Invoke() ?? Task.CompletedTask;
    }
}