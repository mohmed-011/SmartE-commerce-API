namespace SmartE_commerce.Data
{
    public class Seller
    {
        public int Seller_ID { get; set; }
        public string seller_Name { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string package { get; set; }
        public decimal Rate { get; set; }
        public int Code_Yet { get; set; }
        public string password { get; set; }
        public string Location { get; set; }
        public string Seller_Image { get; set; }
    }

    public class SellerPost
    {
        public string seller_Name { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string package { get; set; }
        public string password { get; set; }
        public string Location { get; set; }
    }
}
