using Lamp.Core.Models;
using Lamp.Core.Services;
using System;

namespace Lamp.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the lamp controller
            var lampController = new LampController();

            Console.WriteLine("Lamp Control System");
            Console.WriteLine("-------------------");

            while (true)
            {
                Console.WriteLine("\nCurrent Status:");
                DisplayStatus(lampController.GetCurrentStatus());

                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Toggle Power");
                Console.WriteLine("2. Turn On");
                Console.WriteLine("3. Turn Off");
                Console.WriteLine("4. Adjust Brightness");
                Console.WriteLine("5. Confirm Brightness Change");
                Console.WriteLine("6. Cancel Brightness Change");
                Console.WriteLine("7. Set Color (Hex)");
                Console.WriteLine("8. Set Color (RGB)");
                Console.WriteLine("9. Exit");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out var choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            var toggleResponse = lampController.ToggleLamp();
                            Console.WriteLine($"Toggled lamp. New state: {toggleResponse.IsOn}");
                            break;

                        case 2:
                            var onResponse = lampController.SetLightState(true);
                            Console.WriteLine($"Turned lamp on. Brightness: {onResponse.IsOn}");
                            break;

                        case 3:
                            var offResponse = lampController.SetLightState(false);
                            Console.WriteLine("Turned lamp off.");
                            break;

                        case 4:
                            Console.Write("Enter brightness level (1-10): ");
                            if (int.TryParse(Console.ReadLine(), out var brightness))
                            {
                                var brightnessResponse = lampController.AdjustBrightness(brightness);
                                Console.WriteLine($"Pending brightness change to {brightnessResponse.CurrentBrightness}. Confirm or cancel.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid brightness value.");
                            }
                            break;

                        case 5:
                            var confirmResponse = lampController.ConfirmBrightnessChange();
                            Console.WriteLine($"Brightness confirmed: {confirmResponse.CurrentBrightness}");
                            break;

                        case 6:
                            var cancelResponse = lampController.CancelBrightnessChange();
                            Console.WriteLine($"Brightness change cancelled. Remains at {cancelResponse.CurrentBrightness}");
                            break;

                        case 7:
                            Console.Write("Enter hex color (e.g., #FF0000): ");
                            var hexColor = Console.ReadLine();
                            var hexColorResponse = lampController.SetLampColor(hexColor);
                            Console.WriteLine($"Color set to {hexColorResponse.HexColor}");
                            break;

                        case 8:
                            Console.Write("Enter Red (0-255): ");
                            var red = int.Parse(Console.ReadLine());
                            Console.Write("Enter Green (0-255): ");
                            var green = int.Parse(Console.ReadLine());
                            Console.Write("Enter Blue (0-255): ");
                            var blue = int.Parse(Console.ReadLine());
                            var rgbResponse = lampController.SetLampColor(red, green, blue);
                            Console.WriteLine($"Color set to RGB({rgbResponse.Red}, {rgbResponse.Green}, {rgbResponse.Blue})");
                            break;

                        case 9:
                            Console.WriteLine("Exiting lamp control system...");
                            return;

                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void DisplayStatus(FullStatusResponse status)
        {
            Console.WriteLine($"Power: {(status.IsOn ? "ON" : "OFF")}");
            Console.WriteLine($"Brightness: {status.Brightness}/10");
            Console.WriteLine($"Color: {status.Color} (RGB: {status.Red}, {status.Green}, {status.Blue})");
        }
    }
}
