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
        Text = "BPM Tagger - Windows Music Scanner";
        ClientSize = new Size(640, 500);
        MinimumSize = new Size(540, 400);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(248, 249, 250);

        // Header
        var lblHeader = new Label
        {
            Text = "MP3 Beats-Per-Minute (BPM) Tagger",
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(20, 15),
            AutoSize = true
        };
        Controls.Add(lblHeader);

        var lblSubheader = new Label
        {
            Text = "Calculates tempo and saves it into Windows Explorer file properties.",
            ForeColor = Color.FromArgb(108, 117, 125),
            Location = new Point(21, 42),
            AutoSize = true
        };
        Controls.Add(lblSubheader);

        // Path Selection Group
        var lblPath = new Label
        {
            Text = "Music Folder:",
            Location = new Point(20, 75),
            AutoSize = true
        };
        Controls.Add(lblPath);

        txtPath = new TextBox
        {
            Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            Location = new Point(20, 97),
            Size = new Size(490, 26),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        Controls.Add(txtPath);

        btnBrowse = new Button
        {
            Text = "Browse...",
            Location = new Point(520, 95),
            Size = new Size(95, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Cursor = Cursors.Hand
        };
        btnBrowse.Click += (s, e) =>
        {
            using var dlg = new FolderBrowserDialog();
            dlg.SelectedPath = txtPath.Text;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtPath.Text = dlg.SelectedPath;
            }
        };
        Controls.Add(btnBrowse);

        // Options
        chkFastPreview = new CheckBox
        {
            Text = "Fast analysis (samples 60s preview)",
            Checked = true,
            Location = new Point(20, 135),
            AutoSize = true
        };
        Controls.Add(chkFastPreview);

        chkOverwrite = new CheckBox
        {
            Text = "Overwrite existing BPM tags",
            Checked = false,
            Location = new Point(280, 135),
            AutoSize = true
        };
        Controls.Add(chkOverwrite);

        // Actions
        btnScan = new Button
        {
            Text = "Scan & Tag MP3s",
            Location = new Point(20, 170),
            Size = new Size(180, 38),
            BackColor = Color.FromArgb(13, 110, 253),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnScan.FlatAppearance.BorderSize = 0;
        btnScan.Click += BtnScan_Click;
        Controls.Add(btnScan);

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(210, 170),
            Size = new Size(90, 38),
            Enabled = false,
            Cursor = Cursors.Hand
        };
        btnCancel.Click += (s, e) => cts?.Cancel();
        Controls.Add(btnCancel);

        btnAddToPath = new Button
        {
            Text = PathHelper.IsInUserPath() ? "✓ Added to Terminal PATH" : "Add to Terminal PATH",
            Location = new Point(430, 170),
            Size = new Size(185, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Enabled = !PathHelper.IsInUserPath(),
            Cursor = Cursors.Hand
        };
        btnAddToPath.Click += (s, e) =>
        {
            if (PathHelper.AddToUserPath(out string msg))
            {
                MessageBox.Show(this, msg, "Terminal PATH", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnAddToPath.Text = "✓ Added to Terminal PATH";
                btnAddToPath.Enabled = false;
            }
            else
            {
                MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        Controls.Add(btnAddToPath);

        // Progress
        progressBar = new ProgressBar
        {
            Location = new Point(20, 220),
            Size = new Size(595, 18),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        Controls.Add(progressBar);

        lblStatus = new Label
        {
            Text = "Ready to scan.",
            Location = new Point(20, 245),
            Size = new Size(595, 20),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            ForeColor = Color.FromArgb(73, 80, 87)
        };
        Controls.Add(lblStatus);

        // Results List
        lstLog = new ListBox
        {
            Location = new Point(20, 275),
            Size = new Size(595, 195),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Consolas", 9f)
        };
        Controls.Add(lstLog);
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
