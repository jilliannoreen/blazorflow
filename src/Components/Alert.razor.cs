using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorFlow.Components;

/// <summary>
/// Represents an alert component used in Blazor applications for displaying warning, informational, success, or error messages.
/// </summary>
/// <remarks>
/// The Alert component allows customizable behavior such as setting message text, title, type of alert, icon presence, and duration.
/// These parameters provide flexibility in styling and functionality, making it suitable for different UI requirements.
/// </remarks>
public partial class Alert : ComponentBase
{
    /// <summary>
    /// Gets or sets the message text to be displayed in the alert component.
    /// </summary>
    [Parameter] public string Message { get; set; }

    /// <summary>
    /// Determines the type of alert displayed. This property accepts values from the <see cref="AlertType"/> enumeration,
    /// such as Info, Success, Warning, or Danger. It allows customization of the visual appearance and significance of the alert.
    /// </summary>
    [Parameter] public AlertType Type { get; set; }

    /// <summary>
    /// Gets or sets the title of the alert.
    /// </summary>
    /// <remarks>
    /// The title provides a brief summary or headline for the alert.
    /// This property is optional and can be left unset if no title is required.
    /// </remarks>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an icon should be displayed in the alert component.
    /// </summary>
    /// <remarks>
    /// When set to true, the alert component will render an icon that corresponds
    /// to the alert's type. If set to false, no icon will be displayed.
    /// </remarks>
    [Parameter] public bool ShowIcon { get; set; }

    /// Gets or sets the icon to display in the alert component.
    /// This property allows you to define a custom icon, typically represented
    /// as a string, such as a CSS class or path to an SVG, to visually represent
    /// the alert more distinctly. If not provided, no custom icon will be rendered.
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// Indicates whether the alert can be dismissed by the user.
    /// When set to true, the alert will include functionality to hide or remove it from the UI.
    /// </summary>
    [Parameter] public bool Dismissible { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to the alert component.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Allows additional content to be rendered inside the component, providing flexibility to include custom elements or markup.
    /// </summary>
    [Parameter] public RenderFragment AdditionalContent { get; set; }

    /// <summary>
    /// Gets or sets the duration, in milliseconds, for which the alert is displayed before automatically dismissing.
    /// </summary>
    /// <remarks>
    /// When set to a value greater than zero, the alert will automatically hide after the specified duration.
    /// If set to zero or a negative value, the alert will not auto-dismiss.
    /// </remarks>
    [Parameter] public int Duration { get; set; }
    
    private string AlertClass => ClassBuilder
        .Default("rounded-lg p-4 text-gray-800")
        .AddClass(GetClassByAlertType())
        .AddClass(Class)
        .Build();

    private string GetClassByAlertType() => Type switch
    {
        AlertType.Info => "bg-(--info)",
        AlertType.Success => "bg-(--success)",
        AlertType.Warning => "bg-(--warning)",
        AlertType.Danger => "bg-(--danger)",
        _ => ""
    };

}