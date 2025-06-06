using System;
using System.ComponentModel.DataAnnotations;
using Lamp.Core.Models;

namespace Lamp.Core.Models
{
    public class Lamp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "Smart Lamp";

        private bool _isOn = false;
        private int _brightness = 50;
        private int _pendingBrightness = 50;
        private string _color = "#FFFFFF";
        private int _red = 255;
        private int _green = 255;
        private int _blue = 255;

        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;
        public bool IsOn
        {
            get => _isOn;
            private set
            {
                _isOn = value;
                LastUpdated = DateTime.UtcNow;
            }
        }

        [Range(0, 100)]
        public int Brightness
        {
            get => _brightness;
            private set
            {
                if (value >= 0 && value <= 100)
                {
                    _brightness = value;
                    LastUpdated = DateTime.UtcNow;
                }
            }
        }

        [Range(0, 100)]
        public int PendingBrightness
        {
            get => _pendingBrightness;
            private set
            {
                if (value >= 0 && value <= 100)
                {
                    _pendingBrightness = value;
                }
            }
        }

        [StringLength(7)]
        public string Color
        {
            get => _color;
            private set
            {
                if (IsValidHexColor(value))
                {
                    _color = value;
                    LastUpdated = DateTime.UtcNow;
                }
            }
        }

        [Range(0, 255)]
        public int Red
        {
            get => _red;
            private set
            {
                if (value >= 0 && value <= 255)
                {
                    _red = value;
                }
            }
        }

        [Range(0, 255)]
        public int Green
        {
            get => _green;
            private set
            {
                if (value >= 0 && value <= 255)
                {
                    _green = value;
                }
            }
        }

        [Range(0, 255)]
        public int Blue
        {
            get => _blue;
            private set
            {
                if (value >= 0 && value <= 255)
                {
                    _blue = value;
                }
            }
        }


        public PowerStateResponse TurnOn()
        {
            IsOn = true;
            return new PowerStateResponse { IsOn = IsOn };
        }

        public PowerStateResponse TurnOff()
        {
            IsOn = false;
            return new PowerStateResponse { IsOn = IsOn };
        }

        public PowerStateResponse ToggleState()
        {
            IsOn = !IsOn;
            return new PowerStateResponse { IsOn = IsOn };
        }

        public BrightnessResponse SetPendingBrightness(int brightness)
        {
            PendingBrightness = brightness;
            return new BrightnessResponse
            {
                CurrentBrightness = Brightness,
                PendingBrightness = PendingBrightness
            };
        }

        public BrightnessResponse ConfirmBrightness()
        {
            Brightness = PendingBrightness;
            return new BrightnessResponse
            {
                CurrentBrightness = Brightness,
                PendingBrightness = PendingBrightness
            };
        }

        public BrightnessResponse CancelBrightnessChange()
        {
            PendingBrightness = Brightness;
            return new BrightnessResponse
            {
                CurrentBrightness = Brightness,
                PendingBrightness = PendingBrightness
            };
        }

        public ColorResponse SetColor(string hexColor)
        {
            if (IsValidHexColor(hexColor))
            {
                Color = hexColor;
                ConvertHexToRgb(hexColor);
                return new ColorResponse
                {
                    HexColor = Color,
                    Red = Red,
                    Green = Green,
                    Blue = Blue
                };
            }

            return new ColorResponse
            {
                Success = false,
                ErrorMessage = "Invalid hex color format",
                HexColor = Color,
                Red = Red,
                Green = Green,
                Blue = Blue
            };
        }

        public ColorResponse SetColor(int red, int green, int blue)
        {
            if (red >= 0 && red <= 255 && green >= 0 && green <= 255 && blue >= 0 && blue <= 255)
            {
                Red = red;
                Green = green;
                Blue = blue;
                Color = $"#{red:X2}{green:X2}{blue:X2}";

                return new ColorResponse
                {
                    HexColor = Color,
                    Red = Red,
                    Green = Green,
                    Blue = Blue
                };
            }

            return new ColorResponse
            {
                Success = false,
                ErrorMessage = "RGB values must be between 0-255",
                HexColor = Color,
                Red = Red,
                Green = Green,
                Blue = Blue
            };
        }

        public FullStatusResponse GetFullStatus()
        {
            return new FullStatusResponse
            {
                IsOn = IsOn,
                Brightness = Brightness,
                PendingBrightness = PendingBrightness,
                Color = Color,
                Red = Red,
                Green = Green,
                Blue = Blue,
                LastUpdated = LastUpdated
            };
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
