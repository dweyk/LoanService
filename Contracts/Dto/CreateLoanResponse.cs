namespace Contracts.Dto;

public class CreateLoanResponse
{
    public string? Error { get; set; }
    public bool Success { get; set; }
    public List<PaymentShedule>? PaymentShedules { get; set; }
}