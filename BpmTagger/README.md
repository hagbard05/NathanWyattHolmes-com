# BPM Tagger for Windows (Store-Packaged & CLI)

A lightweight, standalone Windows utility that scans MP3 files in your Windows Music folder (or custom directory), calculates their Beats-Per-Minute (BPM), and saves the tempo into the MP3's ID3v2.3 metadata. Windows File Explorer natively reads this tag under **File Properties -> Details -> Beats-per-minute**.

---

## Features

- **One-Click Scan**: Defaults to `%USERPROFILE%\Music` with a single **"Scan & Tag MP3s"** button.
- **Custom Directory Support**: Easily enter or browse to any custom folder or external drive.
- **Fast Analysis**: Samples a representative 60-second preview window from each track (can be toggled for full-file scans).
- **Dual Mode (GUI + CLI)**:
  - Launch from Start Menu or desktop: Opens a clean, responsive graphical window.
  - Launch from Terminal (`PowerShell` or `cmd`): Runs directly as a high-performance command-line utility.
- **Microsoft Store Packaged**: Ready to build and package as an MSIX application.
- **Instant Terminal Access**:
  - **Store App**: Uses Windows `uap5:AppExecutionAlias` (`bpmscan.exe`) so it is instantly available in any terminal right after Store installation without modifying `PATH`.
  - **Standalone**: Provides an `--add-to-path` command line switch and UI button to add itself to the user's `PATH`.

---

## Project Structure

```
BpmTagger/
├── BpmTagger.sln                   # Visual Studio Solution linking both projects
└── src/
    ├── BpmTagger/                  # Main Application (.NET 10/Windows Desktop)
    │   ├── BpmTagger.csproj
    │   ├── Program.cs              # Hybrid CLI/GUI entry point
    │   ├── MainForm.cs             # Simple Windows UI
    │   ├── BpmDetector.cs          # Audio decoding & tempo detection
    │   ├── Mp3Tagger.cs            # ID3v2.3 tag writer (Windows Explorer compatible)
    │   ├── LibraryScanner.cs       # File crawler, cancellation & progress reporting
    │   └── PathHelper.cs           # User PATH environment variable helper
    └── BpmTagger.Package/          # Windows Application Packaging Project (MSIX)
        ├── BpmTagger.Package.wapproj
        ├── Package.appxmanifest    # App capabilities, Store assets, and AppExecutionAlias
        └── Images/                 # Store tiles, icons, and splash screen
```

---

## Command-Line Usage

```powershell
# Scan default Windows Music folder
bpmscan

# Scan a custom music directory
bpmscan "D:\Audio\DJ_Pool"

# Overwrite existing BPM tags and analyze full song length
bpmscan "D:\Audio\DJ_Pool" --overwrite --full-scan

# Add standalone executable directory to User PATH
bpmscan --add-to-path

# Display help and options
bpmscan --help
```

---

## Packaging for the Microsoft Store in Visual Studio

1. Open `BpmTagger.sln` in **Visual Studio 2022 / 2026**.
2. Set configuration to **Release | x64** (or **Any CPU**).
3. In Solution Explorer, right-click `BpmTagger.Package` -> **Publish** -> **Create App Packages...**.
4. Select:
   - **Microsoft Store using a new app name** (if uploading to Partner Center) OR
   - **Sideloading / Direct MSIX** (for testing locally or distributing standalone `.msixbundle`).
5. Visual Studio will produce the signed `.msixupload` package ready for submission to the Microsoft Partner Center.
