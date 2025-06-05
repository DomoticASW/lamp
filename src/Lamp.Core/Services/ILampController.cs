using Lamp.Core.Models;

namespace Lamp.Core.Services
{
    public interface ILampController
    {
        LampStatus ToggleLamp();
        LampStatus SetLightState(bool isOn);
        LampStatus AdjustBrightness(int brightness);
        LampStatus ConfirmBrightnessChange();
        LampStatus CancelBrightnessChange();
        LampStatus SetLampColor(string hexColor);
        LampStatus SetLampColor(int red, int green, int blue);
        LampStatus GetCurrentStatus();
        Models.Lamp GetLamp();
    }
}
