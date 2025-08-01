using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components
{
    public partial class BFlowTableData
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private string TableDataClass => ClassBuilder
            .Default("px-6 py-3 text-center first:rounded-s-lg last:rounded-e-lg")
            .AddClass(Class)
            .Build();
    }
}
