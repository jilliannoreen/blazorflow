namespace PassCredit.Portal.WEB.Constants;

/// <summary>
/// Defines size options for a Flowbite drawer based on Tailwind's container widths.
/// Applied as width for left/right drawers and height for top/bottom drawers.
/// </summary>
public enum DrawerSize
{
    /// <summary>Small (w-sm → 24rem / 384px)</summary>
    Small,
    /// <summary>Medium (w-md → 28rem / 448px)</summary>
    Medium,
    /// <summary>Large (w-lg → 32rem / 512px)</summary>
    Large,
    /// <summary>Extra Large (w-xl → 36rem / 576px)</summary>
    ExtraLarge,
    /// <summary>50% width or height depending on drawer position.</summary>
    Half,
    /// <summary>Full width or height (w-full / h-full)</summary>
    Full
}