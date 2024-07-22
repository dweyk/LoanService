namespace Services.Services;

using Contracts;
using Contracts.Dto;
using Entites;

public class PlannedPaymentAnnuityService : PlannedPaymentService
{
    public override IList<IPlannedPayment> GetPlannedPayments(CreateLoanRequest createLoanRequest)
    {
        var startDate = createLoanRequest.DateOfIssueLoan;
        var endDate = createLoanRequest.ClosingDateOfLoad;
        var paymentDay = createLoanRequest.PaymentDay;
        var creditAmount = createLoanRequest.CreditAmount;
        var interstRate = createLoanRequest.InterestRate;

        var planDateTimes = GetPlanDateTimes(startDate, endDate, paymentDay);

        var paymentPlansAnnuity = GetPaymentPlansAnnuity(planDateTimes,
            creditAmount,
            interstRate,
            startDate);

        return paymentPlansAnnuity;
    }

    private List<IPlannedPayment> GetPaymentPlansAnnuity(IList<DateTime> planDateTimes,
        double creditAmount, double interestRate, DateTime startDate)
    {
        var paymentPlans = new List<IPlannedPayment>();
        var debt = creditAmount;
        var months = planDateTimes.Count;
        var monthlyPayment = CalculateBaseDebt(creditAmount, interestRate, months);
        var prevDate = startDate;
        var count = planDateTimes.Count;

        foreach (var planDate in planDateTimes)
        {
            var days = (planDate - prevDate).TotalDays;
            days = Math.Round(days);
            var amountOfDays = (int)days;

            var interest = CalculateLoanInterest(amountOfDays, debt, interestRate);
            var baseDebt = monthlyPayment - interest;
            var remainingBaseDebt = debt - baseDebt;

            var paymentPlan = new PlannedPayment
            {
                PaymentDate = planDate,
                BaseDebt = count == 1
                    ? baseDebt + remainingBaseDebt
                    : baseDebt,
                Interest = interest,
                RemainingBaseDebt = count == 1
                    ? default(double)
                    : remainingBaseDebt,
            };

            paymentPlans.Add(paymentPlan);
            debt = remainingBaseDebt;
            prevDate = planDate;
            count--;
        }

        return paymentPlans;
    }

    private double CalculateBaseDebt(double mainDebt, double interest, int months)
    {
        var interestRate = interest / (100 * 12);
        return mainDebt * (interestRate / (1 - Math.Pow(1 + interestRate, -months)));
    }
}