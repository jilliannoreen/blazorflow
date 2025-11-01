namespace BlazorFlow.Enums;

public enum Color
{
    Primary,
    Secondary,
    Tertiary,
    Success,
    Info,
    Warning,
    Error,
    Surface,
    Light,
    Dark,
    Inherit
}

public class Palette
{
    // Colors for utilities
    public string BackgroundColor { get; private set; }
    public string TextColor { get; private set; }
    public string BorderColor { get; private set; }
    public string RingColor { get; private set; }
    public string OutlineColor { get; private set; }
    public string ShadowColor { get; private set; }
    public string AccentColor { get; private set; }
    public string CaretColor { get; private set; }
    public string FillColor { get; private set; }
    public string StrokeColor { get; private set; }

    // Gradients for utilities
    public string GradientFrom { get; private set; }
    public string GradientVia { get; private set; }
    public string GradientTo { get; private set; }

    // Pseudo-element specific colors
    public string BeforeBackgroundColor { get; private set; }
    public string BeforeTextColor { get; private set; }
    public string AfterBackgroundColor { get; private set; }
    public string AfterTextColor { get; private set; }
    public string FirstLetterTextColor { get; private set; }
    public string FirstLineTextColor { get; private set; }

    // States
    public string HoverColor { get; private set; }
    public string FocusColor { get; private set; }
    public string ActiveColor { get; private set; }
    public string DisabledColor { get; private set; }
    public string HoverBeforeColor { get; private set; }
    public string HoverAfterColor { get; private set; }

    // Responsive breakpoints for colors
    public string LargeScreenColor { get; private set; }
    public string MediumScreenColor { get; private set; }
    public string SmallScreenColor { get; private set; }

    // Static predefined colors
    public static readonly Palette Gray = new Palette(
        backgroundColor: "bg-gray-500",
        textColor: "text-gray-500",
        borderColor: "border-gray-500",
        ringColor: "ring-gray-500",
        outlineColor: "outline-gray-500",
        shadowColor: "shadow-gray-500",
        accentColor: "accent-gray-500",
        caretColor: "caret-gray-500",
        fillColor: "fill-gray-500",
        strokeColor: "stroke-gray-500",
        gradientFrom: "from-gray-500",
        gradientVia: "via-gray-400",
        gradientTo: "to-gray-300",
        beforeBackgroundColor: "before:bg-gray-200",
        beforeTextColor: "before:text-gray-300",
        afterBackgroundColor: "after:bg-gray-200",
        afterTextColor: "after:text-gray-300",
        firstLetterTextColor: "first-letter:text-gray-400",
        firstLineTextColor: "first-line:text-gray-400",
        hoverColor: "hover:bg-gray-600",
        focusColor: "focus:bg-gray-700",
        activeColor: "active:bg-gray-800",
        disabledColor: "disabled:bg-gray-400",
        hoverBeforeColor: "hover:before:bg-gray-400",
        hoverAfterColor: "hover:after:bg-gray-500",
        largeScreen: "lg:bg-gray-500",
        mediumScreen: "md:bg-gray-600",
        smallScreen: "sm:bg-gray-400"
    );

    public static readonly Palette Red = new Palette(
        backgroundColor: "bg-red-500",
        textColor: "text-red-500",
        borderColor: "border-red-500",
        ringColor: "ring-red-500",
        outlineColor: "outline-red-500",
        shadowColor: "shadow-red-500",
        accentColor: "accent-red-500",
        caretColor: "caret-red-500",
        fillColor: "fill-red-500",
        strokeColor: "stroke-red-500",
        gradientFrom: "from-red-500",
        gradientVia: "via-red-400",
        gradientTo: "to-red-300",
        beforeBackgroundColor: "before:bg-red-200",
        beforeTextColor: "before:text-red-300",
        afterBackgroundColor: "after:bg-red-200",
        afterTextColor: "after:text-red-300",
        firstLetterTextColor: "first-letter:text-red-400",
        firstLineTextColor: "first-line:text-red-400",
        hoverColor: "hover:bg-red-600",
        focusColor: "focus:bg-red-700",
        activeColor: "active:bg-red-800",
        disabledColor: "disabled:bg-red-400",
        hoverBeforeColor: "hover:before:bg-red-400",
        hoverAfterColor: "hover:after:bg-red-500",
        largeScreen: "lg:bg-red-500",
        mediumScreen: "md:bg-red-600",
        smallScreen: "sm:bg-red-400"
    );

    // Dynamic custom colors
    public static Palette Custom(
        string backgroundColor,
        string textColor,
        string borderColor,
        string ringColor,
        string outlineColor,
        string shadowColor,
        string accentColor,
        string caretColor,
        string fillColor,
        string strokeColor,
        string gradientFrom,
        string gradientVia,
        string gradientTo,
        string beforeBackgroundColor,
        string beforeTextColor,
        string afterBackgroundColor,
        string afterTextColor,
        string firstLetterTextColor,
        string firstLineTextColor,
        string hoverColor,
        string focusColor,
        string activeColor,
        string disabledColor,
        string hoverBeforeColor,
        string hoverAfterColor,
        string largeScreen,
        string mediumScreen,
        string smallScreen
    )
    {
        return new Palette(
            backgroundColor,
            textColor,
            borderColor,
            ringColor,
            outlineColor,
            shadowColor,
            accentColor,
            caretColor,
            fillColor,
            strokeColor,
            gradientFrom,
            gradientVia,
            gradientTo,
            beforeBackgroundColor,
            beforeTextColor,
            afterBackgroundColor,
            afterTextColor,
            firstLetterTextColor,
            firstLineTextColor,
            hoverColor,
            focusColor,
            activeColor,
            disabledColor,
            hoverBeforeColor,
            hoverAfterColor,
            largeScreen,
            mediumScreen,
            smallScreen
        );
    }

    private Palette(
        string backgroundColor,
        string textColor,
        string borderColor,
        string ringColor,
        string outlineColor,
        string shadowColor,
        string accentColor,
        string caretColor,
        string fillColor,
        string strokeColor,
        string gradientFrom,
        string gradientVia,
        string gradientTo,
        string beforeBackgroundColor,
        string beforeTextColor,
        string afterBackgroundColor,
        string afterTextColor,
        string firstLetterTextColor,
        string firstLineTextColor,
        string hoverColor,
        string focusColor,
        string activeColor,
        string disabledColor,
        string hoverBeforeColor,
        string hoverAfterColor,
        string largeScreen,
        string mediumScreen,
        string smallScreen
    )
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        BorderColor = borderColor;
        RingColor = ringColor;
        OutlineColor = outlineColor;
        ShadowColor = shadowColor;
        AccentColor = accentColor;
        CaretColor = caretColor;
        FillColor = fillColor;
        StrokeColor = strokeColor;
        GradientFrom = gradientFrom;
        GradientVia = gradientVia;
        GradientTo = gradientTo;
        BeforeBackgroundColor = beforeBackgroundColor;
        BeforeTextColor = beforeTextColor;
        AfterBackgroundColor = afterBackgroundColor;
        AfterTextColor = afterTextColor;
        FirstLetterTextColor = firstLetterTextColor;
        FirstLineTextColor = firstLineTextColor;
        HoverColor = hoverColor;
        FocusColor = focusColor;
        ActiveColor = activeColor;
        DisabledColor = disabledColor;
        HoverBeforeColor = hoverBeforeColor;
        HoverAfterColor = hoverAfterColor;
        LargeScreenColor = largeScreen;
        MediumScreenColor = mediumScreen;
        SmallScreenColor = smallScreen;
    }

}
