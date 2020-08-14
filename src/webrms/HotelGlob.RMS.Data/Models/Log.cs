using System;

namespace HotelGlob.RMS.Data.Models
{
    public enum LogType
    {
        Error,
        Info
    }

    public class Log
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Body { get; set; }
        public LogType LogType { get; set; }

        public int HotelId { get; set; }
        public HotelSettings Hotel { get; set; }
    }
}
