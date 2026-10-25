using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BpmTagger;

public record ScanProgress(int Current, int Total, string FileName, int? Bpm, string Status);

public class LibraryScanner
{
    public event Action<ScanProgress>? ProgressChanged;

    public async Task<int> ScanAndTagAsync(
        string targetDirectory, 
        bool overwriteExisting = false, 
        bool fastPreview = true,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(targetDirectory))
        {
            throw new DirectoryNotFoundException($"Directory not found: {targetDirectory}");
        }

        var files = await Task.Run(() => 
            Directory.GetFiles(targetDirectory, "*.mp3", SearchOption.AllDirectories),
            cancellationToken);

        int total = files.Length;
        int updatedCount = 0;

        for (int i = 0; i < total; i++)
        {
            if (cancellationToken.IsCancellationRequested) break;

            string filePath = files[i];
            string fileName = Path.GetFileName(filePath);

            uint existingBpm = Mp3Tagger.GetExistingBpm(filePath);
            if (existingBpm > 0 && !overwriteExisting)
            {
                ProgressChanged?.Invoke(new ScanProgress(i + 1, total, fileName, (int)existingBpm, "Skipped (Already tagged)"));
                continue;
            }

            // Detect BPM in background task
            int? bpm = await Task.Run(() => BpmDetector.DetectBpm(filePath, fastPreview ? 60 : 0), cancellationToken);

            if (bpm.HasValue)
            {
                bool saved = await Task.Run(() => Mp3Tagger.WriteBpm(filePath, bpm.Value), cancellationToken);
                if (saved)
                {
                    updatedCount++;
                    ProgressChanged?.Invoke(new ScanProgress(i + 1, total, fileName, bpm.Value, "Tagged & Saved"));
                }
                else
                {
                    ProgressChanged?.Invoke(new ScanProgress(i + 1, total, fileName, bpm.Value, "Failed to save (File locked/in use)"));
                }
            }
            else
            {
                ProgressChanged?.Invoke(new ScanProgress(i + 1, total, fileName, null, "Could not detect BPM"));
            }
        }

        return updatedCount;
    }
}
