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

        public LampStatus ToggleLamp()
        {
            _lamp.ToggleState();
            return GetStatus();
        }

        public LampStatus SetLightState(bool isOn)
        {
            if (isOn)
                _lamp.TurnOn();
            else
                _lamp.TurnOff();

            return GetStatus();
        }

        public LampStatus AdjustBrightness(int brightness)
        {
            _lamp.SetPendingBrightness(brightness);
            return GetStatus();
        }

        public LampStatus ConfirmBrightnessChange()
        {
            _lamp.ConfirmBrightness();
            return GetStatus();
        }

        public LampStatus CancelBrightnessChange()
        {
            _lamp.CancelBrightnessChange();
            return GetStatus();
        }

        public LampStatus SetLampColor(string hexColor)
        {
            _lamp.SetColor(hexColor);
            return GetStatus();
        }

        public LampStatus SetLampColor(int red, int green, int blue)
        {
            _lamp.SetColor(red, green, blue);
            return GetStatus();
        }

        public LampStatus GetCurrentStatus()
        {
            return GetStatus();
        }

        public Models.Lamp GetLamp()
        {
            return _lamp;
        }

        private LampStatus GetStatus()
        {
            return new LampStatus
            {
                IsOn = _lamp.IsOn,
                Brightness = _lamp.Brightness,
                PendingBrightness = _lamp.PendingBrightness,
                Color = _lamp.Color,
                Red = _lamp.Red,
                Green = _lamp.Green,
                Blue = _lamp.Blue,
                LastUpdated = _lamp.LastUpdated
            };
        }
    }
}
