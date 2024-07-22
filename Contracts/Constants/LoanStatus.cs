namespace Contracts.Constants;

public enum LoanStatus
{
    NEW, //just created
    NORMAL, //without overdue
    IN_OVERDUE, //with overdue
    CLOSED //repaid and closed
}