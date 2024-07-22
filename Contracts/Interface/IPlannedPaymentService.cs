namespace Contracts;

using Dto;

public interface IPlannedPaymentService
{
    IList<IPlannedPayment> GetPlannedPayments(CreateLoanRequest createLoanRequest);
}