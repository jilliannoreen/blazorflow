using System.Collections.Generic;

namespace BlazorFlow.Utilities
{
    public class ClassBuilder
    {
        private readonly List<string> _classes = new();

        public static ClassBuilder Default(string baseClass)
        {
            var builder = new ClassBuilder();
            if (!string.IsNullOrWhiteSpace(baseClass))
                builder._classes.Add(baseClass);
            return builder;
        }

        public ClassBuilder AddClass(string? cssClass, bool condition = true)
        {
            if (condition && !string.IsNullOrWhiteSpace(cssClass))
                _classes.Add(cssClass);
            return this;
        }

        public override string ToString() => string.Join(" ", _classes);

        public string Build() => ToString();
    }
}