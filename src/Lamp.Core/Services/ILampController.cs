using Lamp.Core.Models;

namespace Lamp.Core.Services
{
    public interface ILampController
    {
        PowerStateResponse ToggleLamp();
        PowerStateResponse SetLightState(bool isOn);
        BrightnessResponse AdjustBrightness(int brightness);
        BrightnessResponse ConfirmBrightnessChange();
        BrightnessResponse CancelBrightnessChange();
        ColorResponse SetLampColor(string hexColor);
        ColorResponse SetLampColor(int red, int green, int blue);
        FullStatusResponse GetCurrentStatus();
        Models.Lamp GetLamp();
    }
}