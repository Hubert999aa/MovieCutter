using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace Application.Helpers
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

        public async static Task<T> RunProcessWithCustomBehaviour<T>(ProcessStartInfo processInfo, CancellationToken cancellationToken, Func<Process, Task<T>> customBehaviour)
        {
            using (var process = new Process { StartInfo = processInfo, EnableRaisingEvents = true })
            {
                if (process != null)
                {
                    process.Start();

                    var customResult = await customBehaviour.Invoke(process);

                    await process.WaitForExitAsync();

                    process.Close();
                    process.Dispose();

                    return customResult;
                }
                else
                {
                    Log.Fatal($"Nie udało się uruchomić procesu: {processInfo.FileName}.");
                    Environment.Exit(1);

                    return default;
                }
            }
        }
    }
}
