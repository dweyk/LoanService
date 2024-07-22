namespace Services.Entites;

using Contracts;
using Contracts.Constants;
using Contracts.Dto;

public class Loan : ILoan
{
    public Loan(){}

    private Loan(double amount, 
        double interestRate,
        DateTime startDate, 
        DateTime endDate, 
        DateTime? closeDate,
        LoanStatus status)
    {
        Amount = amount;
        InterestRate = interestRate;
        StartDate = startDate;
        EndDate = endDate;
        CloseDate = closeDate;
        Status = status;
    }

    public double Amount { get; }
    public double InterestRate { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public DateTime? CloseDate { get; }
    public LoanStatus Status { get; }

    public List<IPlannedPayment> GetPaymentSchedule()
    {
        throw new NotImplementedException();
    }

    public List<IOperation> GetOperations()
    {
        throw new NotImplementedException();
    }

    public ILoan CreateLoan(CreateLoanRequest createLoanRequest)
    {
        return new Loan(createLoanRequest.CreditAmount,
            createLoanRequest.InterestRate,
            createLoanRequest.DateOfIssueLoan,
            createLoanRequest.ClosingDateOfLoad,
            null,
            LoanStatus.NEW);
    }
}