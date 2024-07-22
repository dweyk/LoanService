namespace Services.Services;

using Contracts;
using Contracts.Dto;
using Entites;

public class PlannedPaymentDifferentiatedService : PlannedPaymentService
{
    public override IList<IPlannedPayment> GetPlannedPayments(CreateLoanRequest createLoanRequest)
    {
        var startDate = createLoanRequest.DateOfIssueLoan;
        var endDate = createLoanRequest.ClosingDateOfLoad;
        var paymentDay = createLoanRequest.PaymentDay;
        var creditAmount = createLoanRequest.CreditAmount;
        var interstRate = createLoanRequest.InterestRate;

        var planDateTimes = GetPlanDateTimes(startDate, endDate, paymentDay);

        var paymentPlansAnnuity = GetPaymentPlansAnnuity(planDateTimes, creditAmount, interstRate, startDate);

        return paymentPlansAnnuity;
    }

    private List<IPlannedPayment> GetPaymentPlansAnnuity(List<DateTime> planDateTimes,
        double creditAmount, double interestRate, DateTime startDate)
    {
        var paymentPlans = new List<IPlannedPayment>();
        var debt = creditAmount;
        var months = planDateTimes.Count;
        var monthlyPayment = creditAmount / months;
        var prevDate = startDate;
        var count = planDateTimes.Count;

        planDateTimes.ForEach(planDate =>
        {
            var days = (planDate - prevDate).TotalDays;
            days = Math.Round(days);
            var amountOfDays = (int)days;

            var interest = CalculateLoanInterest(amountOfDays, debt, interestRate);
            var remainingBaseDebt = debt - monthlyPayment;

            var paymentPlan = new PlannedPayment
            {
                PaymentDate = planDate,
                Interest = interest,
                BaseDebt = count == 1
                    ? monthlyPayment + remainingBaseDebt
                    : monthlyPayment,
                RemainingBaseDebt = count == 1
                    ? default(double)
                    : remainingBaseDebt,
            };

            paymentPlans.Add(paymentPlan);
            debt = remainingBaseDebt;
            prevDate = planDate;
            count--;
        });

        return paymentPlans;
    }
}