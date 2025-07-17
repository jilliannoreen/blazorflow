using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Services;

public class ToastService
{
    [Inject] public IJSRuntime JSRuntime { get; set; }
    
}