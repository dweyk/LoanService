namespace Contracts.Dto;

public class PaymentShedule
{
    public DateTime PaymentDate { get; set; }
    public double AmoutOfPrincipalPayment { get; set; }
    public double InterestPaymentAmount { get; set; }
    public double AmountRemainingAfterPayment { get; set; }
}