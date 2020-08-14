using System;

namespace HotelGlob.RMS.Data.Models
{
    public enum ReservationStatus
    {
        CheckIn,
        IsCancelled,
        NoShow
    }
    public enum ReservationType
    {
        Usual,
        Group,
        RackRate,
        Event
    }
    public class Reservation : Entity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public int DaysCount { get; set; }
        public double RoomPrice { get; set; }
        public double MealPrice { get; set; }
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
        public int PeopleNum { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        public int MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }
        public ReservationType ReservationType { get; set; }
        public int? GroupId { get; set; }
        public int? EventId { get; set; }
        public int HotelId { get; set; }
        public virtual HotelSettings Hotel { get; set; }
    }
}
