
using System.Text.Json.Serialization;

namespace SmartE_commerce.Classes
{
    public class CartResponse
    {
        [JsonPropertyName("Item_ID")]
        public string ItemId { get; set; }

        [JsonPropertyName("Item_Name")]
        public string ItemName { get; set; }

        [JsonPropertyName("Image_Cover")]
        public string ImageCover { get; set; }

        [JsonPropertyName("Rate")]
        public double Rate { get; set; }

        [JsonPropertyName("Price_out")]
        public decimal PriceOut { get; set; }

        [JsonPropertyName("Quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("Category_Name")]
        public string CategoryName { get; set; }

        [JsonPropertyName("Sub_Category_Name")]
        public string SubCategoryName { get; set; }
    }
}
