using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
// Install-Package NAudio
using NAudio.CoreAudioApi;

class Program
{
    static void Main()
    {
        string outputPath = @"C:\temp\ae_audio.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        var deviceEnum = new MMDeviceEnumerator();
        var device = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        Console.WriteLine("Monitoring After Effects audio...");

        AudioSessionControl aeSession = null;

        while (aeSession == null)
        {
            var sessions = device.AudioSessionManager.Sessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                var s = sessions[i];

                try
                {
                    int pid = (int)s.GetProcessID;
                    var proc = Process.GetProcessById(pid);

                    if (proc.ProcessName.Equals("AfterFX", StringComparison.OrdinalIgnoreCase))
                    {
                        aeSession = s;
                        Console.WriteLine("AfterFX audio session found.");
                        break;
                    }
                }
                catch { }
            }

            if (aeSession == null)
            {
                File.WriteAllText(outputPath, "0");
                Thread.Sleep(500);
            }
        }

        // Main loop: read audio levels and write 1 or 0
        const float threshold = 0.02f; // Adjust sensitivity (0.0–1.0)

        while (true)
        {
            float peak = 0f;

            try
            {
                peak = aeSession.AudioMeterInformation.MasterPeakValue;
            }
            catch
            {
                // If AE quits, reset and look again
                aeSession = null;
                File.WriteAllText(outputPath, "0");
                Console.WriteLine("AfterFX session lost. Searching again...");
                Main();
                return;
            }

            // Write 1 if Above Threshold, else 0
            string state = peak > threshold ? "1" : "0";
            File.WriteAllText(outputPath, state);

            Thread.Sleep(50); // 20 updates per second
        }
    }
}
