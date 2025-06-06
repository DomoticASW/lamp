using System;

namespace Lamp.Core.Models
{
    public abstract class LampResponseBase
    {
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PowerStateResponse : LampResponseBase
    {
        public bool IsOn { get; set; }
    }

    public class BrightnessResponse : LampResponseBase
    {
        public int CurrentBrightness { get; set; }
        public int PendingBrightness { get; set; }
        public bool HasPendingChanges => CurrentBrightness != PendingBrightness;
    }

    public class ColorResponse : LampResponseBase
    {
        public string HexColor { get; set; } = string.Empty;
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }

    public class FullStatusResponse : LampResponseBase
    {
        public bool IsOn { get; set; }
        public int Brightness { get; set; }
        public int PendingBrightness { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
