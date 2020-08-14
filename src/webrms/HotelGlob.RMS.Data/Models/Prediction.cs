namespace HotelGlob.RMS.Data.Models
{
    public class Prediction : Entity
    {
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
        public int PeopleNum { get; set; }
        public int CategoryType { get; set; }
        public int MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }
        public double ExpectedLoad { get; set; }
        public double NoShows { get; set; }
        public double Price { get; set; }
        public double Cancelation { get; set; }
        public int CalculationId { get; set; }
        public virtual Calculation Calculation { get;set; }
    }
}
