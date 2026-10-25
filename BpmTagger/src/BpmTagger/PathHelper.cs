using System;
using System.IO;

namespace BpmTagger;

public static class PathHelper
{
    /// <summary>
    /// Checks if the application directory is in the current User PATH environment variable.
    /// </summary>
    public static bool IsInUserPath()
    {
        try
        {
            string appDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string? userPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(userPath)) return false;

            string[] paths = userPath.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in paths)
            {
                if (string.Equals(p.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), appDir, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Appends the application directory to the User PATH environment variable.
    /// </summary>
    public static bool AddToUserPath(out string message)
    {
        try
        {
            string appDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string? userPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;

            if (IsInUserPath())
            {
                message = "Application directory is already in your User PATH.";
                return true;
            }

            string newPath = string.IsNullOrEmpty(userPath) ? appDir : $"{userPath};{appDir}";
            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
            message = $"Successfully added '{appDir}' to your User PATH. You can now run 'bpmscan' from any new terminal window!";
            return true;
        }
        catch (Exception ex)
        {
            message = $"Failed to update User PATH: {ex.Message}";
            return false;
        }
    }
}
