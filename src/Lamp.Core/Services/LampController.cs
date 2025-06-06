using Lamp.Core.Models;

namespace Lamp.Core.Services
{
    public class LampController : ILampController
    {
        private Models.Lamp _lamp;

        public LampController()
        {
            _lamp = new Models.Lamp();
        }

        public LampController(Models.Lamp lamp)
        {
            _lamp = lamp ?? new Models.Lamp();
        }

        public PowerStateResponse ToggleLamp()
        {
            return _lamp.ToggleState();
        }

        public PowerStateResponse SetLightState(bool isOn)
        {
            return isOn ? _lamp.TurnOn() : _lamp.TurnOff();
        }

        public BrightnessResponse AdjustBrightness(int brightness)
        {
            return _lamp.SetPendingBrightness(brightness);
        }

        public BrightnessResponse ConfirmBrightnessChange()
        {
            return _lamp.ConfirmBrightness();
        }

        public BrightnessResponse CancelBrightnessChange()
        {
            return _lamp.CancelBrightnessChange();
        }

        public ColorResponse SetLampColor(string hexColor)
        {
            return _lamp.SetColor(hexColor);
        }

        public ColorResponse SetLampColor(int red, int green, int blue)
        {
            return _lamp.SetColor(red, green, blue);
        }

        public FullStatusResponse GetCurrentStatus()
        {
            return _lamp.GetFullStatus();
        }

        public Models.Lamp GetLamp()
        {
            return _lamp;
        }
    }
}
