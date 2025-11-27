using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class RadioGroup<T> : ComponentBase
{
    [Parameter] public T Value { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public EventCallback<T> ValueChanged { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    internal async Task SelectAsync(T newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
        StateHasChanged();
    }
}