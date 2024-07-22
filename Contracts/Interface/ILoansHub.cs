namespace Contracts;

public interface ILoansHub
{
    ILoan GetLoanById(int loanId); //returns null if object was not found
}