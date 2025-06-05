using System.ComponentModel.DataAnnotations;

namespace Lamp.Core.Models
{
    public class Lamp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "Lamp";

        public bool IsOn { get; set; } = false;

        [Range(0, 100)]
        public int Brightness { get; set; } = 50;

        [Range(0, 100)]
        public int PendingBrightness { get; set; } = 50;

        [StringLength(7)]
        public string Color { get; set; } = "#FFFFFF";

        [Range(0, 255)]
        public int Red { get; set; } = 255;

        [Range(0, 255)]
        public int Green { get; set; } = 255;

        [Range(0, 255)]
        public int Blue { get; set; } = 255;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public void TurnOn()
        {
            IsOn = true;
            LastUpdated = DateTime.UtcNow;
        }

        public void TurnOff()
        {
            IsOn = false;
            LastUpdated = DateTime.UtcNow;
        }

        public void ToggleState()
        {
            IsOn = !IsOn;
            LastUpdated = DateTime.UtcNow;
        }

        public void SetPendingBrightness(int brightness)
        {
            if (brightness >= 0 && brightness <= 100)
            {
                PendingBrightness = brightness;
            }
        }

        public void ConfirmBrightness()
        {
            Brightness = PendingBrightness;
            LastUpdated = DateTime.UtcNow;
        }

        public void CancelBrightnessChange()
        {
            PendingBrightness = Brightness;
        }

        public void SetColor(string hexColor)
        {
            if (IsValidHexColor(hexColor))
            {
                Color = hexColor;
                ConvertHexToRgb(hexColor);
                LastUpdated = DateTime.UtcNow;
            }
        }

        public void SetColor(int red, int green, int blue)
        {
            if (red >= 0 && red <= 255 && green >= 0 && green <= 255 && blue >= 0 && blue <= 255)
            {
                Red = red;
                Green = green;
                Blue = blue;
                Color = $"#{red:X2}{green:X2}{blue:X2}";
                LastUpdated = DateTime.UtcNow;
            }
        }

        private bool IsValidHexColor(string hexColor)
        {
            return !string.IsNullOrEmpty(hexColor) &&
                   hexColor.StartsWith("#") &&
                   hexColor.Length == 7 &&
                   System.Text.RegularExpressions.Regex.IsMatch(hexColor, @"^#[0-9A-Fa-f]{6}$");
        }

        private void ConvertHexToRgb(string hexColor)
        {
            if (IsValidHexColor(hexColor))
            {
                Red = Convert.ToInt32(hexColor.Substring(1, 2), 16);
                Green = Convert.ToInt32(hexColor.Substring(3, 2), 16);
                Blue = Convert.ToInt32(hexColor.Substring(5, 2), 16);
            }
        }
    }
}
