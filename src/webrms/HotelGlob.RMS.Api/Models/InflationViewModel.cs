using HotelGlob.RMS.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelGlob.RMS.Api.Models
{
    /// <summary>
    /// Reservation Model
    /// </summary>
    public class InflationViewModel
    {
        /// <summary>
        /// Date
        /// </summary>
        [Required]
        public string Date { get; set; }

        /// <summary>
        /// Coefficient
        /// </summary>
        [Required]
        public decimal Coef { get; set; }

        /// <summary>
        /// Country Id
        /// </summary>
        [Required]
        public byte CountryId { get; set; }
        
    }
}