namespace CinemaRazor.Models
{
    public class PromoDto
    {
        public int PromoID { get; set; }
        public string PromoCode { get; set; } = "";
        public string PromoName { get; set; } = "";
        public int DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}
