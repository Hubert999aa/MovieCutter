using Serilog;
using System.Diagnostics;
using System.Threading.Channels;

namespace Application.Helpers
{
    public class ProcessRunner()
    {
        public async static Task RunProcess(ProcessStartInfo processInfo, ChannelWriter<string> writer, CancellationToken cancellationToken)
        {
            using (var process = new Process { StartInfo = processInfo, EnableRaisingEvents = true })
            {
                if (process == null)
                {
                    Log.Fatal($"Process '{processInfo.FileName}' couldn't be started.");
                    Environment.Exit(1);
                }

                process.ErrorDataReceived += async (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        await writer.WriteAsync($"[Error] {e.Data}", cancellationToken);
                    }
                };

                process.OutputDataReceived += async (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        await writer.WriteAsync($"[Message] {e.Data}", cancellationToken);
                    }
                };

                process.Start();

                using var registration = cancellationToken.Register(() =>
                {
                    if (!process.HasExited)
                    {
                        Log.Warning("Process killed by cancellationToken");
                        process.Kill(entireProcessTree: true);
                    }
                });

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                await process.WaitForExitAsync(cancellationToken);

                writer.Complete();
            }
        }
    }
}
