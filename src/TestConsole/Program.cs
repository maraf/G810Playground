using LedCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        private static CancellationToken cancellationToken;

        static void Main(string[] args)
        {
            if (LogitechGSDK.LogiLedInit())
            {
                LogitechGSDK.LogiLedSaveCurrentLighting();

                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken cancellationToken = cts.Token;
                Task.Run(
                    async () =>
                    {
                        //LogitechGSDK.LogiLedSetLighting(0, 0, 0);
                        //LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(KeyboardNames.LEFT_WINDOWS, 50, 0, 100);

                        Console.WriteLine("Blinking...");
                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            int stepDelay = 20;
                            int delay = 300;

                            int steps = 10;

                            await LightAsync(KeyboardNames.G_LOGO, LightDirection.Up, steps, stepDelay);
                            await Task.Delay(delay, cancellationToken);

                            await LightAsync(KeyboardNames.G_LOGO, LightDirection.Down, steps, stepDelay);
                            await Task.Delay(delay, cancellationToken);
                        }
                    },
                    cancellationToken
                );

                Console.ReadKey(true);
                cts.Cancel();

                LogitechGSDK.LogiLedRestoreLighting();
                LogitechGSDK.LogiLedShutdown();
            }
            else
            {
                Console.WriteLine("Failed to initialize.");
                Console.ReadKey(true);
            }
        }

        private static async Task LightAsync(KeyboardNames key, LightDirection direction, int steps, int stepDelay = 50)
        {
            int value = 0;
            for (int i = 0; i < steps; i++)
            {
                int step = direction == LightDirection.Up ? i : steps - i;
                value = (int)((double)step / steps * 100);

                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, value, 0, 0);
                await Task.Delay(stepDelay, cancellationToken);
            }

            int? last = null;
            if (direction == LightDirection.Up)
            {
                if (value != 100)
                    last = 100;
            }
            else
            {
                if (value != 0)
                    last = 0;
            }

            if (last != null)
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(key, last.Value, 0, 0);
        }

        private enum LightDirection
        {
            Up,
            Down
        }
    }
}
