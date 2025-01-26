using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartE_commerce.Data
{
    
    [Table("Item_images")]
    [PrimaryKey(nameof(Item_ID), nameof(Item_Image))]
    public class ItemImage
    {
        [Key]
        public string Item_ID { get; set; }
        [Key]
        public string Item_Image { get; set; }
    }
}
