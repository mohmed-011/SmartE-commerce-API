namespace SmartE_commerce.Classes
{
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public int UserId { get; set; }
        public int AddressID { get; set; }


        // Optional: لو عايز تخزن الريسبونس الخام
        public string RawResponse { get; set; }
    }
}
