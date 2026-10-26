using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BpmTagger;

public class MainForm : Form
{
    private TextBox txtPath = null!;
    private Button btnBrowse = null!;
    private Button btnScan = null!;
    private Button btnCancel = null!;
    private Button btnAddToPath = null!;
    private CheckBox chkFastPreview = null!;
    private CheckBox chkOverwrite = null!;
    private ProgressBar progressBar = null!;
    private Label lblStatus = null!;
    private ListBox lstLog = null!;

    private CancellationTokenSource? cts;
    private readonly LibraryScanner scanner = new();

    public MainForm()
    {
        InitializeUi();
        scanner.ProgressChanged += OnProgressChanged;
    }

    private void InitializeUi()
    {
        SuspendLayout();

        Text = "BPM Tagger - Windows Music Scanner";
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(248, 249, 250);
        Padding = new Padding(20, 16, 20, 16);

        // ── Header Title ──────────────────────────────────────────────
        var lblHeader = new Label
        {
            Text = "BPM Tagger",
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(20, 16),
            AutoSize = true
        };
        Controls.Add(lblHeader);

        // ── Subheader Description ─────────────────────────────────────
        var lblSubheader = new Label
        {
            Text = "Scans MP3 files, detects tempo, and writes BPM to file properties.",
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            ForeColor = Color.FromArgb(108, 117, 125),
            Location = new Point(22, 50),
            AutoSize = true
        };
        Controls.Add(lblSubheader);

        // ── Music Folder Label ────────────────────────────────────────
        var lblPath = new Label
        {
            Text = "Music Folder:",
            Location = new Point(20, 82),
            AutoSize = true
        };
        Controls.Add(lblPath);

        // ── Path Text Box ─────────────────────────────────────────────
        txtPath = new TextBox
        {
            Text = PathHelper.GetDefaultMusicPath(),
            Location = new Point(20, 104),
            Size = new Size(440, 25),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        Controls.Add(txtPath);

        // ── Browse Button ─────────────────────────────────────────────
        btnBrowse = new Button
        {
            Text = "Browse",
            Location = new Point(470, 102),
            Size = new Size(80, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Cursor = Cursors.Hand
        };
        btnBrowse.Click += BtnBrowse_Click;
        Controls.Add(btnBrowse);

        // ── Options ───────────────────────────────────────────────────
        chkFastPreview = new CheckBox
        {
            Text = "Fast analysis (60s sample)",
            Checked = true,
            Location = new Point(20, 140),
            AutoSize = true
        };
        Controls.Add(chkFastPreview);

        chkOverwrite = new CheckBox
        {
            Text = "Overwrite existing BPM tags",
            Checked = false,
            Location = new Point(20, 164),
            AutoSize = true
        };
        Controls.Add(chkOverwrite);

        // ── Action Buttons ────────────────────────────────────────────
        int btnY = 198;

        btnScan = new Button
        {
            Text = "Scan && Tag",
            Location = new Point(20, btnY),
            Size = new Size(110, 34),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnScan.FlatAppearance.BorderSize = 0;
        btnScan.Click += BtnScan_Click;
        Controls.Add(btnScan);

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(140, btnY),
            Size = new Size(75, 34),
            Enabled = false,
            Cursor = Cursors.Hand
        };
        btnCancel.Click += (s, e) => cts?.Cancel();
        Controls.Add(btnCancel);

        btnAddToPath = new Button
        {
            Text = PathHelper.IsInUserPath() ? "✓ In PATH" : "Add to PATH",
            Location = new Point(420, btnY),
            Size = new Size(130, 34),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Enabled = !PathHelper.IsInUserPath(),
            Cursor = Cursors.Hand
        };
        btnAddToPath.Click += (s, e) =>
        {
            if (PathHelper.AddToUserPath(out string msg))
            {
                MessageBox.Show(this, msg, "Terminal PATH", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnAddToPath.Text = "✓ In PATH";
                btnAddToPath.Enabled = false;
            }
            else
            {
                MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        Controls.Add(btnAddToPath);

        // ── Progress ──────────────────────────────────────────────────
        progressBar = new ProgressBar
        {
            Location = new Point(20, 246),
            Size = new Size(530, 16),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        Controls.Add(progressBar);

        lblStatus = new Label
        {
            Text = "Ready to scan.",
            Location = new Point(20, 270),
            Size = new Size(530, 18),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            ForeColor = Color.FromArgb(73, 80, 87)
        };
        Controls.Add(lblStatus);

        // ── Results Log ───────────────────────────────────────────────
        lstLog = new ListBox
        {
            Location = new Point(20, 296),
            Size = new Size(530, 180),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Consolas", 9f)
        };
        Controls.Add(lstLog);

        // ── DPI Scaling ───────────────────────────────────────────────
        // This is the critical piece: tell WinForms these coordinates were
        // designed at 96 DPI (100% scaling).  On a 150% display WinForms
        // will multiply every coordinate and size by 1.5, keeping the
        // layout proportional.  Without this, fonts scale but controls
        // do not, causing clipping and overlap.
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        ClientSize = new Size(570, 500);
        MinimumSize = new Size(500, 460);

        ResumeLayout(false);
        PerformLayout();
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        try
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select Folder with MP3 Files",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                AutoUpgradeEnabled = true
            };

            string current = txtPath.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(current) && Directory.Exists(current))
            {
                dlg.InitialDirectory = current;
                dlg.SelectedPath = current;
            }
            else
            {
                string fallback = PathHelper.GetDefaultMusicPath();
                dlg.InitialDirectory = fallback;
                dlg.SelectedPath = fallback;
            }

            if (dlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.SelectedPath))
            {
                txtPath.Text = dlg.SelectedPath;
            }
        }
        catch (Exception ex)
        {
            // Fallback 1: Try classic folder browser dialog
            try
            {
                using var fallbackDlg = new FolderBrowserDialog
                {
                    AutoUpgradeEnabled = false,
                    Description = "Select Folder with MP3 Files"
                };
                if (fallbackDlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(fallbackDlg.SelectedPath))
                {
                    txtPath.Text = fallbackDlg.SelectedPath;
                    return;
                }
            }
            catch
            {
                // Fallback 2: OpenFileDialog folder selector
                using var ofd = new OpenFileDialog
                {
                    ValidateNames = false,
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "Select Folder",
                    Title = "Select Folder with MP3 Files"
                };
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string? folder = Path.GetDirectoryName(ofd.FileName);
                    if (!string.IsNullOrEmpty(folder))
                    {
                        txtPath.Text = folder;
                        return;
                    }
                }
            }

            MessageBox.Show(this, $"Could not open folder selector: {ex.Message}", "Browse Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void BtnScan_Click(object? sender, EventArgs e)
    {
        string targetDir = txtPath.Text.Trim();
        if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir))
        {
            MessageBox.Show(this, "Please select a valid folder path.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnScan.Enabled = false;
        btnBrowse.Enabled = false;
        btnCancel.Enabled = true;
        lstLog.Items.Clear();
        progressBar.Value = 0;
        lblStatus.Text = "Finding MP3 files...";

        cts = new CancellationTokenSource();

        try
        {
            int updated = await scanner.ScanAndTagAsync(
                targetDir,
                chkOverwrite.Checked,
                chkFastPreview.Checked,
                cts.Token);

            lblStatus.Text = cts.IsCancellationRequested
                ? "Scan cancelled."
                : $"Complete! Successfully updated {updated} files.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error during scan.";
            MessageBox.Show(this, ex.Message, "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnScan.Enabled = true;
            btnBrowse.Enabled = true;
            btnCancel.Enabled = false;
            cts.Dispose();
            cts = null;
        }
    }

    private void OnProgressChanged(ScanProgress p)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnProgressChanged(p)));
            return;
        }

        progressBar.Maximum = p.Total;
        progressBar.Value = Math.Min(p.Current, p.Total);
        lblStatus.Text = $"Processing {p.Current} of {p.Total} files: {p.FileName}";

        string bpmStr = p.Bpm.HasValue ? $"{p.Bpm.Value,3} BPM" : "--- BPM";
        lstLog.Items.Add($"[{bpmStr}] {p.FileName} -> {p.Status}");
        lstLog.TopIndex = lstLog.Items.Count - 1;
    }
}
