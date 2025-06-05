namespace Lamp.Core.Models
{
    public class LampStatus
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
