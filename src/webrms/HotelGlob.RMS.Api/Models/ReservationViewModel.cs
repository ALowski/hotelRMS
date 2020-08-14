using HotelGlob.RMS.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelGlob.RMS.Api.Models
{
    /// <summary>
    /// Reservation Model
    /// </summary>
    public class ReservationViewModel
    {
        /// <summary>
        /// Order date
        /// </summary>
        [Required]
        public string OrderDate { get; set; }

        /// <summary>
        /// Check in date
        /// </summary>
        [Required]
        public string CheckInDate { get; set; }

        /// <summary>
        /// Days count
        /// </summary>
        [Required]
        public short DaysCount { get; set; }

        /// <summary>
        /// Price per night
        /// </summary>
        [Required]
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// Room type
        /// </summary>
        [Required]
        public byte RoomType { get; set; }

        /// <summary>
        /// Reservation status
        /// </summary>
        [Required]
        public ReservationStatus ReservationStatus { get; set; }

        /// <summary>
        /// Meal type
        /// </summary>
        [Required]
        public byte MealType { get; set; }
    }
}