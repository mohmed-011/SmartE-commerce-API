namespace SmartE_commerce.Data
{
    public class Product
    {
        public required int Item_ID { get; set; }
        public required string Item_Name { get; set; }
        public string Description { get; set; }
        public required int Category_ID { get; set; }
        public required int Sub_Category_ID { get; set; }
        public required int Quantity { get; set; }
        public required decimal Price_in { get; set; }
        public required decimal Price_out { get; set; }
        public decimal Discount { get; set; }

        public decimal Rate { get; set; }
        public required int Seller_ID { get; set; }






    }
}
