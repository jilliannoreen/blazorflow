using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Services;

public class PopoverService : IPopoverService
{
    private readonly List<PopoverInstance> _popovers = new();
    public event Action? OnChange;

    public IReadOnlyList<PopoverInstance> Popovers => _popovers;

    public Guid Show(RenderFragment content, ElementReference anchor)
    {
        var id = Guid.NewGuid();
        _popovers.Add(new PopoverInstance(id, content, anchor));
        OnChange?.Invoke();
        return id;
    }

    public void Close(Guid id)
    {
        var popover = _popovers.FirstOrDefault(p => p.Id == id);
        if (popover != null)
        {
            _popovers.Remove(popover);
            OnChange?.Invoke();
        }
    }


}

public record PopoverInstance(Guid Id, RenderFragment Content, ElementReference Anchor);
