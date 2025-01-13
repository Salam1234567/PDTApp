namespace pdtapp.Models
{
    /// <summary>
    /// POC class serves as a DTO
    /// </summary>
    public class GridItem
    {
        public int Xref { get; set; }
        public int Yref { get; set; }
        public int Value { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; } = 1;
        public GridItem(int xref, int yref, int year, int month, int value)
        {
            Xref = xref;
            Yref = yref;
            Year = year;
            Month = month;
            Value = value;
        }
    }
}
