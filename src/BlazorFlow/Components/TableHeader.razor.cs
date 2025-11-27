using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components
{
    public partial class TableHeader
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private string TableHeaderClass => ClassBuilder
            .Default("font-semibold text-xs text-center px-6 py-4 first:rounded-s-lg last:rounded-e-lg")
            .AddClass(Class)
            .Build();
    }
}
