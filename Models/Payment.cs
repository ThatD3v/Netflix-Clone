namespace NetflixClone.Models
{
    public class Payment
    {
        public  int  Id { get; set; }
        public DateTime InitializeOn { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        public string?  PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string CheckoutUrl { get; set; }
        public string AccessCode { get; set; }

        public string? VerificationResponse { get; set; }


    }
}
