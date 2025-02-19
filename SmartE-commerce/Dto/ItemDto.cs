namespace SmartE_commerce.Dto
{
    public class ItemDto
    {
        public string Item_ID { get; set; }
        public string Item_Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price_in { get; set; }
        public decimal Price_out { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Rate { get; set; }
        public int Category_ID { get; set; }
        public int Seller_ID { get; set; }
        public int Sub_Category_ID { get; set; }
        public IFormFile? Image { get; set; }
        public int? View_Count { get; set; }
        public int? Sold_Count { get; set; }
        public DateTime? Crate_Date { get; set; }

    }
}
