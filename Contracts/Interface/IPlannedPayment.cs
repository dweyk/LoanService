namespace Contracts;

public interface IPlannedPayment
{
    DateTime PaymentDate { get; init; }
    double BaseDebt { get; init; }
    double Interest { get; init; }
    double RemainingBaseDebt { get; init; }
}