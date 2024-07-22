namespace Contracts;

using Constants;

public interface IOperation
{
    int Id { get; }
    double Amount { get; } // can be only positive or negative
    DateTime Date { get; }
    AccountType AccountType { get; }
}