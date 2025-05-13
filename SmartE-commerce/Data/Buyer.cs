using System.Numerics;
using System.Security.Principal;

namespace SmartE_commerce.Data
{
    public class Buyer
    {
        public int Buyer_ID { get; set; }
        public string Buyer_Name { get; set; }
        public string Email   { get; set; }
        public string phone   { get; set; }
        public string password  { get; set; } 
	    public string Buyer_Image   { get; set; }
    }

    public class BuyerPost
    {
        public string Buyer_Name { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
    }
}
