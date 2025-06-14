using System.Text.RegularExpressions;

public class Lamp
{
    public string Name { get; set; } = "Lamp";
    public bool IsOn { get; set; } = false;
    public int Brightness { get; set; } = 100;
    public string ColorHex { get; set; } = "#FFFFFF";

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

    public void SetColor(string hex)
    {
        if (Regex.IsMatch(hex, "^#([0-9A-Fa-f]{6})$"))
        {
            ColorHex = hex;
        }
        else
        {
            throw new ArgumentException("Invalid hex color format. Use format: #RRGGBB");
        }
    }

    public string GetStatus()
    {
        return $"Name: {Name}, Status: {(IsOn ? "On" : "Off")}, Brightness: {Brightness}, Color: {ColorHex}";
    }
}
