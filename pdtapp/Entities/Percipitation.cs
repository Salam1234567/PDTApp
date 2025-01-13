using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdtapp.Entities
{
    /// <summary>
    /// Prcipitation Entity 
    /// Represent a row of the Prcipitation database table
    /// </summary>
    public class Precipitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long Xref { get; set; } = 0;
        public long Yref { get; set; } = 0;
        public DateTime Date { get; set; }
        public long Value { get; set; }
    }
}
