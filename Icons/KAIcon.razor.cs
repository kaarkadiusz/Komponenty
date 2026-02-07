using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Icons
{
    public partial class KAIcon : KAComponentBase
    {
        [Parameter]
        public string? IconSrc
        {
            get => field;
            set
            {
                if (field == value)
                {
                    return;
                }

                field = value;
                IsIconValid = IsValidSvg(field);
            }
        }
        [Parameter]
        public IconSize Size { get; set; } = IconSize.Medium;
        [Parameter]
        public Color Color { get; set; } = Color.Default;

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"size{Size}");
            sb.AppendLine($"color{Color}");
            if (!IsIconValid)
            {
                sb.AppendLine(IconSrc);
            }
            sb.AppendLine(base.GetCssClass());
            return sb.ToString();
        }

        private bool IsIconValid = true;

        private bool IsValidSvg(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input = input.Trim();

            return 
                input.StartsWith("<svg", StringComparison.OrdinalIgnoreCase) &&
                input.Contains("</svg>", StringComparison.OrdinalIgnoreCase);
        }
    }

    public enum IconSize
    {
        /// <summary>
        /// Sets icon's --fontSize variable to 1.25rem (by default, for most browsers, this equals to 20px). Default value.
        /// </summary>
        Medium,
        /// <summary>
        /// Sets icon's --fontSize variable to inherit.
        /// </summary>
        Inherit,
        /// <summary>
        /// Unsets icon's size making it fill the container.
        /// </summary>
        Unset,
        /// <summary>
        /// Sets icon's --fontSize variable to 0.75rem (by default, for most browsers, this equals to 12px).
        /// </summary>
        ExtraSmall,
        /// <summary>
        /// Sets icon's --fontSize variable to 1rem (by default, for most browsers, this equals to 16px).
        /// </summary>
        Small,
        /// <summary>
        /// Sets icon's --fontSize variable to 1.5rem (by default, for most browsers, this equals to 24px).
        /// </summary>
        Large,
        /// <summary>
        /// Sets icon's --fontSize variable to 1.75rem (by default, for most browsers, this equals to 28px).
        /// </summary>
        ExtraLarge
    }
}
