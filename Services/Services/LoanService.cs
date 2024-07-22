namespace Services.Services;

using System.Text;
using Contracts;
using Contracts.Constants;
using Contracts.Dto;
using Newtonsoft.Json;

public class LoanService : ILoanService
{
    public ILoan _loan;
    public PlannedPaymentResolver _plannedPaymentResolver;

    public LoanService(
        ILoan loan,
        PlannedPaymentResolver plannedPaymentResolver)
    {
        _loan = loan;
        _plannedPaymentResolver = plannedPaymentResolver;
    }

    public CreateLoanResponse CreateLoanAsync(CreateLoanRequest createLoanRequest)
    {
        try
        {
            var errorMessage = ValidateRequestData(createLoanRequest);
            if (errorMessage.Length != 0)
            {
                return new CreateLoanResponse()
                {
                    Error = $"{errorMessage}",
                    Success = false,
                };
            }

            var loan = _loan.CreateLoan(createLoanRequest); // Логика создания кредита (по тестовому сказали пропустить)

            var plannedPaymentService = GetPlannedPaymentService(createLoanRequest.ChartType);
            var plannedPayments = plannedPaymentService!.GetPlannedPayments(createLoanRequest);
            var paymentShedules = plannedPayments
                .Select(payment => new PaymentShedule()
                {
                    PaymentDate = payment.PaymentDate,
                    InterestPaymentAmount = Math.Round(payment.Interest, 2),
                    AmoutOfPrincipalPayment = Math.Round(payment.BaseDebt, 2),
                    AmountRemainingAfterPayment = Math.Round(payment.RemainingBaseDebt, 2),
                })
                .OrderBy(paymentShedule => paymentShedule.PaymentDate)
                .ToList();

            return new CreateLoanResponse()
            {
                Success = true,
                PaymentShedules = paymentShedules,
            };
        }
        catch (Exception e)
        {
            return new CreateLoanResponse()
            {
                Error = e.Message,
                Success = false,
            };
        }
    }

    private StringBuilder ValidateRequestData(CreateLoanRequest createLoanRequest)
    {
        var errorMessage = new StringBuilder();
        var countErrors = 0;

        if (createLoanRequest.PaymentDay != null)
        {
            if (createLoanRequest.PaymentDay < 1 || createLoanRequest.PaymentDay > 31)
            {
                errorMessage.Append($"Неверно указан день платежа.{Environment.NewLine}");
                countErrors++;
            }
            else
            {
                var incorrectPayDay = CheckDatesIfLoanOnOneMonth(createLoanRequest.DateOfIssueLoan,
                    createLoanRequest.ClosingDateOfLoad,
                    (int)createLoanRequest.PaymentDay);

                if (incorrectPayDay)
                {
                    errorMessage.Append($"Неверно указан день платежа.{Environment.NewLine}");
                    countErrors++;
                }
            }
        }

        if (createLoanRequest.CreditAmount <= 0)
        {
            errorMessage.Append($"Неверно указана сумма кредита.{Environment.NewLine}");
            countErrors++;
        }

        if (createLoanRequest.InterestRate < 0)
        {
            errorMessage.Append($"Неверно указана процентная ставка кредита.{Environment.NewLine}");
            countErrors++;
        }

        if (createLoanRequest.DateOfIssueLoan < DateTime.Now.Date)
        {
            errorMessage.Append($"Кредит не может выдаваться задним числом.{Environment.NewLine}");
        }

        if (createLoanRequest.ClosingDateOfLoad < createLoanRequest.DateOfIssueLoan)
        {
            errorMessage.Append($"Дата закрытия кредита меньше, чем дата открытия кредита.{Environment.NewLine}");
            countErrors++;
        }

        if (createLoanRequest.ClosingDateOfLoad == createLoanRequest.DateOfIssueLoan)
        {
            errorMessage.Append(
                $"Дата закрытия кредита не может быть равна дате открытия кредита.{Environment.NewLine}");
            countErrors++;
        }

        if (Enum.IsDefined(typeof(LoanType), createLoanRequest.ChartType) == false)
        {
            errorMessage.Append($"Неверно указан тип кредита.{Environment.NewLine}");
            countErrors++;
        }

        if (countErrors > 0)
        {
            errorMessage.Insert(0,
                countErrors > 1
                    ? $"Ошибки в данных для создания кредита:{Environment.NewLine}"
                    : $"Ошибка в данных для создания кредита:{Environment.NewLine}");
        }

        return errorMessage;
    }

    private IPlannedPaymentService? GetPlannedPaymentService(int chartType)
    {
        var type = (LoanType)chartType;
        return _plannedPaymentResolver(type);
    }

    private bool CheckDatesIfLoanOnOneMonth(DateTime startDate, DateTime endDate, int payDay)
    {
        var startDay = startDate.Day;
        var endDay = endDate.Day;

        var startMonth = startDate.Month;
        var endMonth = endDate.Month;

        var startYear = startDate.Year;
        var endYear = endDate.Year;

        if (startYear == endYear)
        {
            if (startMonth == endMonth)
            {
                if (startDay < payDay && payDay < endDay)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }
}