using System;
using System.IO;

namespace BpmTagger;

/// <summary>
/// Reads and writes ID3v2 BPM tags using TagLibSharp in an ID3v2.3 format compatible with Windows Explorer.
/// </summary>
public static class Mp3Tagger
{
    static Mp3Tagger()
    {
        // Enforce ID3v2.3 so Windows Explorer natively displays "Beats-per-minute" in file properties
        TagLib.Id3v2.Tag.DefaultVersion = 3;
        TagLib.Id3v2.Tag.ForceDefaultVersion = true;
    }

    /// <summary>
    /// Checks if an MP3 file already has a non-zero BPM tag.
    /// </summary>
    public static uint GetExistingBpm(string filePath)
    {
        try
        {
            using var file = TagLib.File.Create(filePath);
            return file.Tag.BeatsPerMinute;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Writes the BPM tag to the MP3 file.
    /// </summary>
    public static bool WriteBpm(string filePath, int bpm)
    {
        try
        {
            // Remove read-only attribute if set
            var attributes = File.GetAttributes(filePath);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
            }

            using (var file = TagLib.File.Create(filePath))
            {
                file.Tag.BeatsPerMinute = (uint)bpm;
                file.Save();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
