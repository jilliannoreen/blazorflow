using System.ComponentModel;

namespace BlazorFlow.Enums;

public enum ButtonType
{
    [Description("button")]
    Button,
    [Description("submit")]
    Submit,
    [Description("reset")]
    Reset
}