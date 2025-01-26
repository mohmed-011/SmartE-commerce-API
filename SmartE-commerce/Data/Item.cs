using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartE_commerce.Data
{
    [Table("Item")]
    public class Item
    {
        [Key]
        public string Item_ID { get; set; }
        public string Image_Cover { get; set; } = "Image_Cover.jpg";
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
    }
}
