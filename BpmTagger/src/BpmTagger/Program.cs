using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BpmTagger;

public static class Program
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        // If launched with command-line arguments, run purely as a terminal tool
        if (args.Length > 0)
        {
            return await RunCliAsync(args);
        }

        // When launched without arguments (e.g. from Start Menu or desktop shortcut),
        // hide the background console window and launch the clean graphical UI.
        IntPtr consoleHandle = GetConsoleWindow();
        if (consoleHandle != IntPtr.Zero)
        {
            ShowWindow(consoleHandle, SW_HIDE);
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        return 0;
    }

    private static async Task<int> RunCliAsync(string[] args)
    {
        bool showHelp = false;
        bool overwrite = false;
        bool fastPreview = true;
        bool addToPath = false;
        string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLowerInvariant();
            switch (arg)
            {
                case "--help":
                case "-h":
                case "/?":
                    showHelp = true;
                    break;
                case "--overwrite":
                    overwrite = true;
                    break;
                case "--full-scan":
                    fastPreview = false;
                    break;
                case "--add-to-path":
                    addToPath = true;
                    break;
                case "--path":
                case "-p":
                    if (i + 1 < args.Length)
                    {
                        targetPath = args[++i];
                    }
                    break;
                default:
                    if (!arg.StartsWith("-") && Directory.Exists(args[i]))
                    {
                        targetPath = args[i];
                    }
                    break;
            }
        }

        if (showHelp)
        {
            Console.WriteLine();
            Console.WriteLine("==========================================================");
            Console.WriteLine(" BPM Tagger - MP3 Beats-Per-Minute Scanner for Windows");
            Console.WriteLine("==========================================================");
            Console.WriteLine("Usage:");
            Console.WriteLine("  bpmscan [path] [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  [path]               Target directory (defaults to Windows Music folder)");
            Console.WriteLine("  --path, -p <dir>     Explicit target directory path");
            Console.WriteLine("  --overwrite          Recalculate BPM even if already tagged");
            Console.WriteLine("  --full-scan          Analyze entire audio file instead of 60s sample");
            Console.WriteLine("  --add-to-path        Add bpmscan folder to User PATH environment variable");
            Console.WriteLine("  --help, -h           Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  bpmscan");
            Console.WriteLine("  bpmscan \"D:\\MyMusic\" --overwrite");
            Console.WriteLine("  bpmscan --add-to-path");
            Console.WriteLine();
            return 0;
        }

        if (addToPath)
        {
            Console.WriteLine();
            if (PathHelper.AddToUserPath(out string msg))
            {
                Console.WriteLine($"[SUCCESS] {msg}");
                return 0;
            }
            else
            {
                Console.Error.WriteLine($"[ERROR] {msg}");
                return 1;
            }
        }

        if (!Directory.Exists(targetPath))
        {
            Console.Error.WriteLine($"\n[ERROR] Directory not found: {targetPath}");
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine($"Target folder : {targetPath}");
        Console.WriteLine($"Options       : Overwrite={overwrite}, FastPreview={fastPreview}");
        Console.WriteLine("----------------------------------------------------------");

        var scanner = new LibraryScanner();
        scanner.ProgressChanged += p =>
        {
            string bpm = p.Bpm.HasValue ? $"{p.Bpm.Value,3} BPM" : "--- BPM";
            Console.WriteLine($"[{p.Current}/{p.Total}] [{bpm}] {p.FileName} -> {p.Status}");
        };

        try
        {
            int updated = await scanner.ScanAndTagAsync(targetPath, overwrite, fastPreview);
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"[COMPLETE] Finished scanning. Successfully tagged {updated} file(s).");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"\n[FATAL] {ex.Message}");
            return 1;
        }
    }
}
