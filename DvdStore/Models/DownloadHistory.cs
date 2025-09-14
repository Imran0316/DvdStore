﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class DownloadHistory
    {
        [Key]
        public int DownloadID { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public Users User { get; set; }

        public int? SongID { get; set; }
        [ForeignKey("SongID")]
        public Songs Song { get; set; }

        public int? ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products Product { get; set; }

        public DateTime DownloadDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Type { get; set; } // "Song", "Trailer", "Sample"
    }
}