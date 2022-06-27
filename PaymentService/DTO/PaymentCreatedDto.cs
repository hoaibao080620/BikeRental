namespace PaymentService.DTO;

public class PaymentCreatedDto
{
    public string PaymentIntent { get; set; } = null!;
    public string EphemeralKey { get; set; } = null!;
    public string Customer { get; set; } = null!;
    public string PublishableKey { get; set; } = null!;
}
