namespace Services.Controllers;

using Contracts;
using Contracts.Dto;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoanController : ControllerBase
{
    public ILoanService _loanService;
    
    public LoanController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    [Route("CreateLoan")]
    public CreateLoanResponse CreateLoan(CreateLoanRequest createLoanRequest)
    {
        return _loanService.CreateLoanAsync(createLoanRequest);
    }
}