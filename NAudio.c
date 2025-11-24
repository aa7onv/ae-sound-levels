// DO THIS
// Install-Package NAudio

using System;
using System.Diagnostics;
using System.Linq;
using NAudio.CoreAudioApi;
using System.Threading;

class Program
{
    static void Main()
    {
        // Get default output device
        var deviceEnum = new MMDeviceEnumerator();
        var device = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var sessionManager = device.AudioSessionManager;
        var sessions = sessionManager.Sessions;

        // Find After Effects session
        AudioSessionControl aeSession = null;

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
                    break;
                }
            }
            catch { }
        }

        if (aeSession == null)
        {
            Console.WriteLine("After Effects session not found. Is audio playing?");
            return;
        }

        Console.WriteLine("Found AfterFX audio session. Reading levels...");

        while (true)
        {
            float peak = aeSession.AudioMeterInformation.MasterPeakValue; // 0.0 – 1.0
            Console.WriteLine($"Peak: {peak:0.000}");
            Thread.Sleep(10);     // 100 times per second
        }
    }
}
