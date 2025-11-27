using BlazorFlow.Enums;

namespace BlazorFlow.Services;

/// <summary>
/// Service class used to manage color context mappings for dynamic styling in a Blazor application.
/// </summary>
public class ColorService
{
    // Dictionary for storing mappings of Color -> Context -> Tailwind Class
    private readonly Dictionary<Color, Dictionary<ColorContext, string>> _colorMappings = new()
    {
        { Color.Primary, new Dictionary<ColorContext, string>
            {
                { ColorContext.Background, "bg-(--primary)" },
                { ColorContext.Text, "text-(--primary)" },
                { ColorContext.Border, "border-(--primary)" },
                { ColorContext.Ring, "ring-(--primary)" },
                { ColorContext.Placeholder, "placeholder-(--primary)" },
                { ColorContext.Divide, "divide-(--primary)" },
                { ColorContext.GradientFrom, "from-(--primary)" },
                { ColorContext.GradientTo, "to-(--primary)" },
                { ColorContext.Decoration, "decoration-(--primary)" },
                { ColorContext.Caret, "caret-(--primary)" }
            }
        },
        { Color.Secondary, new Dictionary<ColorContext, string>
            {
                { ColorContext.Background, "bg-(--secondary)" },
                { ColorContext.Text, "text-(--secondary)" },
                { ColorContext.Border, "border-(--secondary)" },
                { ColorContext.Ring, "ring-(--secondary)" },
                { ColorContext.Placeholder, "placeholder-(--secondary)" },
                { ColorContext.Divide, "divide-(--secondary)" },
                { ColorContext.GradientFrom, "from-(--secondary)" },
                { ColorContext.GradientTo, "to-(--secondary)" },
                { ColorContext.Decoration, "decoration-(--secondary)" },
                { ColorContext.Caret, "caret-(--secondary)" }
            }
        },
        { Color.Success, new Dictionary<ColorContext, string>
            {
                { ColorContext.Background, "bg-(--success)" },
                { ColorContext.Text, "text-(--success)" },
                { ColorContext.Border, "border-(--success)" },
                { ColorContext.Ring, "ring-(--success)" },
                { ColorContext.Placeholder, "placeholder-(--success)" },
                { ColorContext.Divide, "divide-(--success)" },
                { ColorContext.GradientFrom, "from-(--success)" },
                { ColorContext.GradientTo, "to-(--success)" },
                { ColorContext.Decoration, "decoration-(--success)" },
                { ColorContext.Caret, "caret-(--success)" }
            }
        },
        { Color.Error, new Dictionary<ColorContext, string>
            {
                { ColorContext.Background, "bg-(--error)" },
                { ColorContext.Text, "text-(--error)" },
                { ColorContext.Border, "border-(--error)" },
                { ColorContext.Ring, "ring-(--error)" },
                { ColorContext.Placeholder, "placeholder-(--error)" },
                { ColorContext.Divide, "divide-(--error)" },
                { ColorContext.GradientFrom, "from-(--error)" },
                { ColorContext.GradientTo, "to-(--error)" },
                { ColorContext.Decoration, "decoration-(--error)" },
                { ColorContext.Caret, "caret-(--error)" }
            }
        }
    };

    
    /// <summary>
    /// Method to get Tailwind class for a specific color and context
    /// </summary>
    /// <param name="color"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string GetTailwindClass(Color color, ColorContext context)
    {
        if (!_colorMappings.TryGetValue(color, out var contextMappings) ||
            !contextMappings.TryGetValue(context, out var tailwindClass))
        {
            throw new ArgumentException($"No mapping found for Color '{color}' and Context '{context}'.");
        }

        return tailwindClass;
    }
    
    /// <summary>
    /// Allow users to dynamically add new mappings
    /// </summary>
    /// <param name="color"></param>
    /// <param name="context"></param>
    /// <param name="tailwindClass"></param>
    public void AddColorMapping(Color color, ColorContext context, string tailwindClass)
    {
        if (!_colorMappings.ContainsKey(color))
        {
            _colorMappings[color] = new Dictionary<ColorContext, string>();
        }

        _colorMappings[color][context] = tailwindClass;
    }
}
