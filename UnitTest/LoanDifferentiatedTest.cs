namespace UnitTest;

using back_end_test.BaseModule;
using Contracts;
using Contracts.Dto;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services;
using Xunit;

public class LoanDifferenceTest
{
    [Fact]
    public void GetPaymentPlansDifferentiatedTest()
    {
        var services = new ServiceCollection();
        services.InstallModule<ServiceModule>(null);
        var provider = services.BuildServiceProvider();
        var loanService = provider.GetRequiredService<ILoanService>();
        var date = DateTime.Now.AddYears(1);
        if (DateTime.IsLeapYear(date.Year))
        {
            date = date.AddYears(1);
        }

        var year = 2023;
        var countYearNeedToAdd = date.Year - year;
        var paymentShedulesTest = PrepareTestData(countYearNeedToAdd)
            .OrderBy(paymentShedule => paymentShedule.PaymentDate);

        var loanCreateData = new CreateLoanRequest
        {
            CreditAmount = 100000,
            DateOfIssueLoan = new DateTime(date.Year, 5, 1),
            ClosingDateOfLoad = new DateTime(date.AddYears(1).Year, 5, 1),
            InterestRate = 10,
            ChartType = 2,
            PaymentDay = 1,
        };

        var count = 0;
        var paymentShedules = loanService.CreateLoanAsync(loanCreateData);
        paymentShedules.PaymentShedules!.ForEach(paymentShedule =>
        {
            var sheduleTestForCompare = paymentShedulesTest.ElementAt(count);
            Assert.Equal(paymentShedule, sheduleTestForCompare, new PlannedPaymentComparer());
            count++;
        });
    }

    private List<PaymentShedule> PrepareTestData(int needAddYear)
    {
        var jsonData = @"
        {
          ""error"": null,
          ""success"": true,
          ""paymentShedules"": [{""PaymentDate"":""2023 - 06 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":849.32,""AmountRemainingAfterPayment"":91666.67},{""PaymentDate"":""2023 -
            07 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":753.42,""AmountRemainingAfterPayment"":83333.33},{""PaymentDate"":""2023 -
            08 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":707.76,""AmountRemainingAfterPayment"":75000.0},{""PaymentDate"":""2023 -
            09 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":636.99,""AmountRemainingAfterPayment"":66666.67},{""PaymentDate"":""2023 -
            10 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":547.95,""AmountRemainingAfterPayment"":58333.33},{""PaymentDate"":""2023 -
            11 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":495.43,""AmountRemainingAfterPayment"":50000.0},{""PaymentDate"":""2023 -
            12 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":410.96,""AmountRemainingAfterPayment"":41666.67},{""PaymentDate"":""2024 -
            01 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":353.88,""AmountRemainingAfterPayment"":33333.33},{""PaymentDate"":""2024 -
            02 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":283.11,""AmountRemainingAfterPayment"":25000.0},{""PaymentDate"":""2024 -
            03 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":191.78,""AmountRemainingAfterPayment"":16666.67},{""PaymentDate"":""2024 -
            04 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":141.55,""AmountRemainingAfterPayment"":8333.33},{""PaymentDate"":""2024 -
            05 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8333.33,""InterestPaymentAmount"":68.49,""AmountRemainingAfterPayment"":0.0}]
        }";

        var data = JsonConvert.DeserializeObject<CreateLoanResponse>(jsonData, new JsonSerializerSettings()
        {
            Culture = System.Globalization.CultureInfo.GetCultureInfo("es-ES")
        });
        var paymentShedules = data!.PaymentShedules;
        paymentShedules!.ForEach(paymentShedule =>
        {
            paymentShedule.PaymentDate = paymentShedule.PaymentDate.AddYears(needAddYear);
        });

        return paymentShedules;
    }

    private class PlannedPaymentComparer : IEqualityComparer<PaymentShedule>
    {
        public bool Equals(PaymentShedule? x, PaymentShedule? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.PaymentDate.Equals(y.PaymentDate) && x.AmoutOfPrincipalPayment.Equals(y.AmoutOfPrincipalPayment) &&
                   x.InterestPaymentAmount.Equals(y.InterestPaymentAmount) &&
                   x.AmountRemainingAfterPayment.Equals(y.AmountRemainingAfterPayment);
        }

        public int GetHashCode(PaymentShedule obj)
        {
            return HashCode.Combine(obj.PaymentDate, obj.AmoutOfPrincipalPayment, obj.InterestPaymentAmount,
                obj.AmountRemainingAfterPayment);
        }
    }
}