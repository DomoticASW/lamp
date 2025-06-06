using Lamp.Core.Models;
using Lamp.Core.Services;
using Xunit;

namespace Lamp.Core.Tests
{
    public class LampControllerTests
    {
        private readonly LampController _controller;
        private readonly Models.Lamp _mockLamp;

        public LampControllerTests()
        {
            _mockLamp = new Models.Lamp();
            _controller = new LampController(_mockLamp);
        }

        [Fact]
        public void ToggleLamp_ShouldChangePowerState()
        {
            Assert.False(_mockLamp.IsOn);

            var response1 = _controller.ToggleLamp();
            Assert.True(response1.IsOn);
            Assert.True(_mockLamp.IsOn);

            var response2 = _controller.ToggleLamp();
            Assert.False(response2.IsOn);
            Assert.False(_mockLamp.IsOn);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetLightState_ShouldSetCorrectState(bool desiredState)
        {
            var response = _controller.SetLightState(desiredState);
            Console.WriteLine(response);
            
            Assert.Equal(desiredState, response.IsOn);
            Assert.Equal(desiredState, _mockLamp.IsOn);
            
            if (desiredState)
            {
                Assert.True(response.IsOn);
            }
        }

        [Fact]
        public void AdjustBrightness_ShouldSetPendingBrightness()
        {
            _controller.SetLightState(true);
            
            const int testBrightness = 5;
            var response = _controller.AdjustBrightness(testBrightness);
            
            Assert.Equal(testBrightness, response.PendingBrightness);
            Assert.Equal(testBrightness, _mockLamp.PendingBrightness);
            Assert.NotEqual(testBrightness, _mockLamp.Brightness);
        }

        [Fact]
        public void ConfirmBrightnessChange_ShouldApplyPendingBrightness()
        {
            _controller.SetLightState(true);
            const int testBrightness = 7;
            _controller.AdjustBrightness(testBrightness);
            
            var response = _controller.ConfirmBrightnessChange();
            
            Assert.Equal(testBrightness, response.CurrentBrightness);
            Assert.Equal(testBrightness, _mockLamp.Brightness);
        }

        [Fact]
        public void CancelBrightnessChange_ShouldNotChangeBrightness()
        {
            _controller.SetLightState(true);
            const int initialBrightness = 3;
            _controller.AdjustBrightness(initialBrightness);
            _controller.ConfirmBrightnessChange();
            
            const int newBrightness = 8;
            _controller.AdjustBrightness(newBrightness);
            
            var response = _controller.CancelBrightnessChange();
            
            Assert.Equal(initialBrightness, response.CurrentBrightness);
            Assert.Equal(initialBrightness, _mockLamp.Brightness);
        }

        [Theory]
        [InlineData("#FF0000")]
        [InlineData("#00FF00")]
        [InlineData("#0000FF")]
        public void SetLampColor_WithHex_ShouldChangeColor(string hexColor)
        {
            var response = _controller.SetLampColor(hexColor);
            
            Assert.Equal(hexColor, response.HexColor);
            Assert.Equal(hexColor, _mockLamp.Color);
        }

        [Fact]
        public void SetLampColor_WithRGB_ShouldChangeColor()
        {
            const int red = 255;
            const int green = 128;
            const int blue = 0;
            
            var response = _controller.SetLampColor(red, green, blue);
            
            Assert.Equal("#FF8000", response.HexColor);
            Assert.Equal(red, response.Red);
            Assert.Equal(green, response.Green);
            Assert.Equal(blue, response.Blue);
            Assert.Equal("#FF8000", _mockLamp.Color);
        }

        [Fact]
        public void GetCurrentStatus_ShouldReturnCompleteState()
        {
            _controller.SetLightState(true);
            _controller.AdjustBrightness(5);
            _controller.ConfirmBrightnessChange();
            _controller.SetLampColor("#123456");
            
            var status = _controller.GetCurrentStatus();
            
            Assert.True(status.IsOn);
            Assert.Equal(5, status.Brightness);
            Assert.Equal("#123456", status.Color);
        }

        [Fact]
        public void GetLamp_ShouldReturnTheLampInstance()
        {
            var lamp = _controller.GetLamp();
            
            Assert.Same(_mockLamp, lamp);
        }
    }
}