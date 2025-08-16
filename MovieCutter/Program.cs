using System.Diagnostics;
using System.Reflection;

string videoPath = args[0];
string fileName = Path.GetFileName(videoPath);
string outputFolder = args.Length > 1 ? args[1] : "C:\\Users\\User\\Wideo\\" + fileName;

Directory.CreateDirectory(outputFolder);

string arguments = $"-i \"{videoPath}\" -fps_mode passthrough \"{Path.Combine(outputFolder, "frame_%06d.png")}\"";

var startInfo = new ProcessStartInfo
{
    FileName = ExtractFFmpeg(),
    Arguments = arguments,
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true,
};

using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
{
    if (process != null)
    {
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        Console.WriteLine("Rozpoczynam cięcie filmu...");
        process.Start();

        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }
    else
    {
        Console.WriteLine("Nie udało się uruchomić procesu ffmpeg.");
        Environment.Exit(1);
    }
}

string ExtractFFmpeg()
{
    string tempPath = Path.Combine(Path.GetTempPath(), "ffmpeg_embedded.exe");

    if (!File.Exists(tempPath))
    {
        using Stream stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MovieCutter.Resources.ffmpeg.exe")
            ?? throw new Exception("Nie znaleziono zasobu ffmpeg.exe");

        using FileStream fileStream = new(tempPath, FileMode.Create, FileAccess.Write);
        stream.CopyTo(fileStream);
    }

    return tempPath;
}
