using ClosedXML.Excel;
using System.IO;
using VideoLinkSaver.Models;

namespace VideoLinkSaver.Services
{
    /// <summary>
    /// Manages all Excel file operations: create, read, write, and validate.
    /// The file is stored at: %LOCALAPPDATA%\VideoLinkSaver\the saves links.xlsx
    /// </summary>
    public class ExcelService
    {
        // ---------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------
        private const string AppFolderName = "VideoLinkSaver";
        private const string FileName = "the saves links.xlsx";
        private const string SheetName = "Links";

        // Column indices (1-based)
        private const int ColSerial = 1;
        private const int ColLink = 2;
        private const int ColPlatform = 3;
        private const int ColChannel = 4;
        private const int ColPurpose = 5;
        private const int ColCategory = 6;

        // ---------------------------------------------------------------
        // Public properties
        // ---------------------------------------------------------------
        public string FilePath { get; private set; }
        public string FolderPath { get; private set; }

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------
        public ExcelService()
        {
            FolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppFolderName);
            FilePath = Path.Combine(FolderPath, FileName);
        }

        // ---------------------------------------------------------------
        // Ensure file exists — called at startup and before every operation
        // ---------------------------------------------------------------
        public void EnsureFileExists()
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                if (!File.Exists(FilePath) || !IsFileValid())
                {
                    CreateFreshFile();
                }
            }
            catch (Exception ex)
            {
                // Last resort: log and recreate
                System.Diagnostics.Debug.WriteLine($"[ExcelService] EnsureFileExists failed: {ex.Message}");
                CreateFreshFile();
            }
        }

        // ---------------------------------------------------------------
        // Check if the existing file has the expected structure
        // ---------------------------------------------------------------
        private bool IsFileValid()
        {
            try
            {
                using var wb = new XLWorkbook(FilePath);
                var ws = wb.Worksheet(SheetName);
                // Check header row exists
                return ws.Cell(1, ColSerial).GetString().Trim() == "Serial No.";
            }
            catch
            {
                return false;
            }
        }

        // ---------------------------------------------------------------
        // Create a brand-new Excel file with header row
        // ---------------------------------------------------------------
        private void CreateFreshFile()
        {
            Directory.CreateDirectory(FolderPath);
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet(SheetName);

            // Style the header row
            ws.Cell(1, ColSerial).Value = "Serial No.";
            ws.Cell(1, ColLink).Value = "Link of the Video";
            ws.Cell(1, ColPlatform).Value = "Platform";
            ws.Cell(1, ColChannel).Value = "Name of the Channel";
            ws.Cell(1, ColPurpose).Value = "Purpose";
            ws.Cell(1, ColCategory).Value = "Category";

            var headerRange = ws.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#6C63FF");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Auto-fit columns
            ws.Columns().AdjustToContents();

            wb.SaveAs(FilePath);
        }

        // ---------------------------------------------------------------
        // Save a new VideoLink record
        // ---------------------------------------------------------------
        public async Task SaveLinkAsync(VideoLink link)
        {
            await Task.Run(() =>
            {
                EnsureFileExists();

                using var wb = new XLWorkbook(FilePath);
                var ws = wb.Worksheet(SheetName);

                // Find the next empty row (after header)
                int nextRow = ws.LastRowUsed()?.RowNumber() + 1 ?? 2;
                if (nextRow < 2) nextRow = 2;

                // Auto-increment serial number
                int serial = nextRow - 1;

                ws.Cell(nextRow, ColSerial).Value = serial;
                ws.Cell(nextRow, ColLink).Value = link.LinkOfVideo;
                ws.Cell(nextRow, ColPlatform).Value = link.Platform;
                ws.Cell(nextRow, ColChannel).Value = link.NameOfChannel;
                ws.Cell(nextRow, ColPurpose).Value = link.Purpose;
                ws.Cell(nextRow, ColCategory).Value = link.Category;

                // Style data rows with alternating colors
                var rowStyle = ws.Row(nextRow).Style;
                rowStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Columns().AdjustToContents();
                wb.Save();
            });
        }

        // ---------------------------------------------------------------
        // Load all VideoLink records from the file
        // ---------------------------------------------------------------
        public async Task<List<VideoLink>> LoadAllLinksAsync()
        {
            return await Task.Run(() =>
            {
                EnsureFileExists();
                var results = new List<VideoLink>();

                using var wb = new XLWorkbook(FilePath);
                var ws = wb.Worksheet(SheetName);

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

                // Start from row 2 (skip header)
                for (int row = 2; row <= lastRow; row++)
                {
                    var serial = ws.Cell(row, ColSerial).GetString().Trim();
                    var link = ws.Cell(row, ColLink).GetString().Trim();
                    var platform = ws.Cell(row, ColPlatform).GetString().Trim();
                    var channel = ws.Cell(row, ColChannel).GetString().Trim();
                    var purpose = ws.Cell(row, ColPurpose).GetString().Trim();
                    var category = ws.Cell(row, ColCategory).GetString().Trim();

                    // Skip completely empty rows
                    if (string.IsNullOrWhiteSpace(link)) continue;

                    results.Add(new VideoLink
                    {
                        SerialNo = int.TryParse(serial, out int s) ? s : row - 1,
                        LinkOfVideo = link,
                        Platform = platform,
                        NameOfChannel = channel,
                        Purpose = purpose,
                        Category = category,
                        RowIndex = row
                    });
                }

                // Newest first (highest row index = newest)
                results.Sort((a, b) => b.RowIndex.CompareTo(a.RowIndex));
                return results;
            });
        }

        // ---------------------------------------------------------------
        // Open the folder in Windows File Explorer
        // ---------------------------------------------------------------
        public void OpenFolderInExplorer()
        {
            EnsureFileExists();
            System.Diagnostics.Process.Start("explorer.exe", $"\"{FolderPath}\"");
        }
    }
}
