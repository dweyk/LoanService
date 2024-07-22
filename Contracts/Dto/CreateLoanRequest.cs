namespace Contracts.Dto;

public class CreateLoanRequest
{
    public double CreditAmount { get; set; }
    public DateTime DateOfIssueLoan { get; set; }
    public DateTime ClosingDateOfLoad { get; set; }
    public double InterestRate { get; set; }
    public int ChartType { get; set; }
    public int? PaymentDay { get; set; }
}