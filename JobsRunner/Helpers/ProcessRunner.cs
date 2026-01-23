using Serilog;
using System.Diagnostics;

namespace JobsRunner.Helpers
{
    public class ProcessRunner(ILogger<ProcessRunner> logger)
    {
        public async static Task RunProcess(ProcessStartInfo processInfo, CancellationToken cancellationToken, bool showConsoleLogs = false)
        {
            using (var process = new Process { StartInfo = processInfo, EnableRaisingEvents = true })
            {
                if (process != null)
                {
                    if (showConsoleLogs)
                    {
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                Log.Debug(e.Data);
                        };

                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                Log.Debug(e.Data);
                        };
                    }

                    process.Start();

                    using var registration = cancellationToken.Register(() =>
                    {
                        if (!process.HasExited)
                        {
                            process.Kill(entireProcessTree: true);
                        }
                    });

                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    await process.WaitForExitAsync();

                    process.Close();
                    process.Dispose();
                }
                else
                {
                    Log.Fatal($"Nie udało się uruchomić procesu: {processInfo.FileName}.");
                    Environment.Exit(1);
                }
            }
        }
    }
}
