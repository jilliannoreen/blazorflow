using System.Linq.Expressions;
using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFlow.Components;

public partial class Switch<TValue>
{
    [CascadingParameter] private EditContext? EditContext { get; set; }
    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    [Parameter] public Color CheckedColor { get; set; } = Color.Success;
    
    [Parameter] public Color UncheckedColor { get; set; } = Color.Error;
    [Parameter] public string Id { get; set; } = $"switch-{Guid.NewGuid()}";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string Label { get; set; }
    
    /// <summary>
    /// Expression used for validation with EditForm (e.g. @bind-Value="Model.Name").
    /// </summary>
    [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; } = default!;

    
    
    private bool IsChecked => Value;
    private string SwitchClass => ClassBuilder
        .Default("relative w-11 h-6 rounded-full peer-focus:outline-none transition-colors duration-300 after:content-[''] " +
                 "after:absolute after:top-0.5 after:start-[2px] after:bg-white after:rounded-full " +
                 "after:h-5 after:w-5 after:transition-all rtl:peer-checked:after:-translate-x-full")
        .AddClass($"bg-(--error)", !IsChecked)
        .AddClass($"bg-(--success)", IsChecked)
        .AddClass("peer-checked:after:translate-x-full")
        .Build();

    private async Task OnChangeHandler(ChangeEventArgs e)
    {
        if (bool.TryParse(e.Value?.ToString(), out var newValue))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }
    }

    private void NotifyFieldChanged()
    {
        if (EditContext != null && ValueExpression != null)
        {
            var fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            EditContext.NotifyFieldChanged(fieldIdentifier);
        }
    }

    private bool HasValidationError
        => EditContext?.GetValidationMessages(FieldIdentifier.Create(ValueExpression)).Any() == true;

    private string ValidationMessage
        => EditContext?.GetValidationMessages(FieldIdentifier.Create(ValueExpression)).FirstOrDefault();
}