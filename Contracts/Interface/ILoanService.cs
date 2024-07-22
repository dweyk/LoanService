namespace Contracts;

using Dto;

public interface ILoanService
{
    CreateLoanResponse CreateLoanAsync(CreateLoanRequest createLoanRequest);
}