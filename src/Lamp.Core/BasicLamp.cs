using System.Text.RegularExpressions;

namespace Lamp.Core
{
    public class BasicLamp
    {
        public string Name { get; } = Environment.GetEnvironmentVariable("NAME") ?? "Lamp-01";
        public string Id { get; } = "421723782384";
        public bool IsOn { get; private set; } = false;
        public int Brightness { get; private set; } = 100;
        public ColorType Color { get; private set; } = new ColorType(255, 255, 255);

        public void TurnOn()
        {
            IsOn = true;
        }

        public void TurnOff()
        {
            IsOn = false;
        }

        public void Switch()
        {
            IsOn = !IsOn;
        }

        public void SetBrightness(int value)
        {
            if (value < 1) value = 1;
            if (value > 100) value = 100;
            Brightness = value;
        }

        public void SetColor(int r, int g, int b)
        {
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            {
                throw new ArgumentOutOfRangeException("RGB values must be between 0 and 255.");
            }
            Color = new ColorType(r, g, b);
        }
        public string GetStatus()
        {
            return $"Name: {Name}, Status: {(IsOn ? "On" : "Off")}, Brightness: {Brightness}, Color: {Color}";
        }
    }

    public record ColorType(int R, int G, int B);
}
