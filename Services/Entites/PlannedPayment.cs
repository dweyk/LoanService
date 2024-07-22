namespace Services.Entites;

using Contracts;

public class PlannedPayment : IPlannedPayment
{
    public DateTime PaymentDate { get; init; }
    public double BaseDebt { get; init; }
    public double Interest { get; init; }
    public double RemainingBaseDebt { get; init; }

    public PlannedPayment()
    {
    }

    private PlannedPayment(DateTime paymentDate, double baseDebt, double interest, double remainingBaseDebt)
    {
        PaymentDate = paymentDate;
        BaseDebt = baseDebt;
        Interest = interest;
        RemainingBaseDebt = remainingBaseDebt;
    }

    public IPlannedPayment CreatePlannedPayment()
    {
        return new PlannedPayment();
    }
}