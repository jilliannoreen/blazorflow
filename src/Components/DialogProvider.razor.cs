using System.Collections.Concurrent;
using BlazorFlow.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

/// <summary>
/// Provides a centralized service for showing and managing dialogs in the Blazor application.
/// </summary>
public partial class DialogProvider : ComponentBase
{
    [Inject] public IDialogService DialogService { get; set; } = null!;

    private readonly List<DialogRenderItem> _dialogs = new();

    private readonly object _lock = new();

    /// <summary>
    /// On first render, register this provider with the dialog service.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DialogService.RegisterProvider(this);
        }
    }

    /// <summary>
    /// Shows a dialog of the specified type with title, parameters, and options.
    /// </summary>
    public async Task<DialogReference> ShowAsync<TDialog>(
        string title,
        Dictionary<string, object?> parameters,
        DialogOptions? options = null
    ) where TDialog : ComponentBase
    {
        var reference = new DialogReference();
        var dialogFragment = BuildRenderFragment<TDialog>(title, parameters, options, reference);

        lock (_lock)
        {
            _dialogs.Add(new DialogRenderItem
            {
                RenderFragment = dialogFragment,
                Reference = reference
            });
        }
        

        StateHasChanged();
        return reference;
    }

    /// <summary>
    /// Closes a dialog and removes it from the render list.
    /// </summary>
    public void Close(DialogReference reference)
    {
        lock (_lock)
        {
            var item = _dialogs.FirstOrDefault(x => x.Reference == reference);
            if (item is not null)
            {
                _dialogs.Remove(item);
                InvokeAsync(StateHasChanged);
            }
        }
    }

    /// <summary>
    /// Constructs the RenderFragment to be rendered in the UI.
    /// </summary>

    private RenderFragment BuildRenderFragment<TDialog>(
        string title,
        Dictionary<string, object?> parameters,
        DialogOptions? options,
        DialogReference reference
    ) where TDialog : ComponentBase => builder =>
    {
        builder.OpenComponent<CascadingValue<DialogReference>>(0);
        builder.AddAttribute(1, "Value", reference);
        builder.AddAttribute(2, "IsFixed", true);
        builder.AddAttribute(3, "ChildContent", (RenderFragment)((childBuilder) =>
        {
            childBuilder.OpenComponent<TDialog>(0);
            foreach (var param in parameters)
            {
                childBuilder.AddAttribute(1, param.Key, param.Value);
            }
            childBuilder.CloseComponent();
        }));
        builder.CloseComponent(); 
    };

    /// <summary>
    /// Internal container for tracking individual dialogs.
    /// </summary>
    private class DialogRenderItem
    {
        public RenderFragment RenderFragment { get; set; } = default!;
        public DialogReference Reference { get; set; } = default!;
    }
}
