using System;


namespace HotelGlob.RMS.Data.Models
{
    public class Parser_RoomData : Entity
    {
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public int Parser_RoomInfoId { get; set; }
        public virtual Parser_RoomInfo Parser_RoomInfo { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public int AverageOccupancy { get; set; }
        public int StayLength { get; set; }
    }
}
