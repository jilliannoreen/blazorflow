using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Services;

public interface IPopoverService
{
    event Action? OnChange; 
    Guid Show(RenderFragment content, ElementReference anchor);
    void Close(Guid id);
    IReadOnlyList<PopoverInstance> Popovers { get; }
}