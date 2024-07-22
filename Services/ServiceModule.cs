namespace Services;

using back_end_test.BaseModule;
using Contracts;
using Contracts.Constants;
using Entites;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;

public class ServiceModule : BaseModule
{
    public override void Setup(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddMvc().AddApplicationPart(GetType().Assembly).AddControllersAsServices();
        services.AddTransient<ILoan, Loan>();
        services.AddTransient<ILoanService, LoanService>();
        services.AddScoped<PlannedPaymentDifferentiatedService>();
        services.AddScoped<PlannedPaymentAnnuityService>();
        services.AddScoped<PlannedPaymentResolver>(serviceProvider => loanType =>
            loanType switch
            {
                LoanType.Annuity => serviceProvider.GetService<PlannedPaymentAnnuityService>(),
                LoanType.Differentiated => serviceProvider.GetService<PlannedPaymentDifferentiatedService>(),
                _ => throw new NotImplementedException(),
            }
        );
    }
}