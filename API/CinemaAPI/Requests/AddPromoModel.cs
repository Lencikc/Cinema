namespace CinemaAPI.Requests
{
    public class AddPromoModel
    {
        public string PromoCode { get; set; }
        public string PromoName { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
