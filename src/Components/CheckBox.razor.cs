using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

public partial class CheckBox : ComponentBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public string LabelClass { get; set; }
    [Parameter] public Color Color { get; set; }
    
    private string Id = $"checkbox-{Guid.NewGuid().ToString()}";
    
    private string InputClass => ClassBuilder
        .Default("w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded-sm focus:ring-blue-500 dark:focus:ring-blue-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600")
        .AddClass("")
        .Build();
}