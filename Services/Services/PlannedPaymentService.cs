namespace Services.Services;

using Contracts;
using Contracts.Dto;

public abstract class PlannedPaymentService : IPlannedPaymentService
{
    public virtual IList<IPlannedPayment> GetPlannedPayments(CreateLoanRequest createLoanRequest)
        => new List<IPlannedPayment>();

    protected List<DateTime> GetPlanDateTimes(DateTime startDate, DateTime endDate, int? payDay)
    {
        var lastDayOfPayment = payDay ?? startDate.Day;
        var endMonthDays = DateTime.DaysInMonth(endDate.Year, endDate.Month);
        var endDateTime = new DateTime(endDate.Year, endDate.Month, endMonthDays);

        var planDateTimes = Enumerable.Range(0, Int32.MaxValue)
            .Select(startDate.AddMonths)
            .TakeWhile(e => e <= endDateTime)
            .Select(e => new DateTime(e.Year, e.Month,
                DateTime.DaysInMonth(e.Year, e.Month) < lastDayOfPayment
                    ? DateTime.DaysInMonth(e.Year, e.Month)
                    : lastDayOfPayment))
            .ToList();

        if (planDateTimes.Count > 1)
        {
            planDateTimes.RemoveAt(0);
        }

        var lastPaymentPlanDate = planDateTimes.Last();
        planDateTimes.Remove(lastPaymentPlanDate);

        var lastPaymentExactDate = GetLastPaymentPlanDate(endDate, lastDayOfPayment);
        if (lastPaymentExactDate != null)
        {
            planDateTimes.Add((DateTime)lastPaymentExactDate);
        }

        return planDateTimes;
    }

    protected double CalculateLoanInterest(int amountOfDays, double mainDebt, double interestRate)
        => Enumerable.Range(0, amountOfDays)
            .Select(_ => ((mainDebt * interestRate) / 365) / 100)
            .Sum();

    private DateTime? GetLastPaymentPlanDate(DateTime endDate, int payDay)
    {
        var day = endDate.Day;

        if (day > payDay || day == payDay)
        {
            return new DateTime(endDate.Year, endDate.Month, payDay);
        }

        return null;
    }
}