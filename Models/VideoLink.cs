namespace VideoLinkSaver.Models
{
    /// <summary>
    /// Represents a saved video link record stored in the Excel file.
    /// </summary>
    public class VideoLink
    {
        public int SerialNo { get; set; }
        public string LinkOfVideo { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string NameOfChannel { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Row timestamp: used to sort newest first (row number in file).
        /// </summary>
        public int RowIndex { get; set; }
    }
}
