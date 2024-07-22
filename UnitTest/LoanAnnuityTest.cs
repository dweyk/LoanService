namespace UnitTest;

using back_end_test.BaseModule;
using Contracts;
using Contracts.Dto;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services;
using Xunit;

public class LoanAnnuityTest
{
    [Fact]
    public void GetPaymentPlansAnnuityTest()
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
            ChartType = 1,
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
        00:00"",""AmoutOfPrincipalPayment"":7942.27,""InterestPaymentAmount"":849.32,""AmountRemainingAfterPayment"":92057.73},{""PaymentDate"":""2023 -
            07 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8034.95,""InterestPaymentAmount"":756.64,""AmountRemainingAfterPayment"":84022.78},{""PaymentDate"":""2023 -
            08 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8077.97,""InterestPaymentAmount"":713.62,""AmountRemainingAfterPayment"":75944.81},{""PaymentDate"":""2023 -
            09 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8146.58,""InterestPaymentAmount"":645.01,""AmountRemainingAfterPayment"":67798.23},{""PaymentDate"":""2023 -
            10 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8234.34,""InterestPaymentAmount"":557.25,""AmountRemainingAfterPayment"":59563.88},{""PaymentDate"":""2023 -
            11 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8285.7,""InterestPaymentAmount"":505.89,""AmountRemainingAfterPayment"":51278.18},{""PaymentDate"":""2023 -
            12 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8370.12,""InterestPaymentAmount"":421.46,""AmountRemainingAfterPayment"":42908.06},{""PaymentDate"":""2024 -
            01 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8427.16,""InterestPaymentAmount"":364.42,""AmountRemainingAfterPayment"":34480.89},{""PaymentDate"":""2024 -
            02 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8498.74,""InterestPaymentAmount"":292.85,""AmountRemainingAfterPayment"":25982.16},{""PaymentDate"":""2024 -
            03 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8592.27,""InterestPaymentAmount"":199.32,""AmountRemainingAfterPayment"":17389.88},{""PaymentDate"":""2024 -
            04 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8643.89,""InterestPaymentAmount"":147.69,""AmountRemainingAfterPayment"":8745.99},{""PaymentDate"":""2024 -
            05 - 01T00:
        00:00"",""AmoutOfPrincipalPayment"":8745.99,""InterestPaymentAmount"":71.88,""AmountRemainingAfterPayment"":0.0}]
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