﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApiEbtTests : BaseGpApiTests {
        EBTCardData ebtCardData;
        EBTTrackData ebtTrackData;

        private const string CURRENCY = "USD";
        private const decimal AMOUNT = 10m;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "Uyq6PzRbkorv2D4RQGlldEtunEeGNZll",
                AppKey = "QDsW1ETQKHX6Y4TA",
                Channel = Channel.CardPresent,
            });
        }

        [TestInitialize]
        public void TestInitialize() {
            ebtCardData = new EBTCardData {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                PinBlock = "32539F50C245A6A93D123412324000AA",
                CardHolderName = "Jane Doe",
                CardPresent = true
            };

            ebtTrackData = new EBTTrackData {
                Value =
                    "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                EntryMethod = EntryMethod.Swipe,
                PinBlock = "32539F50C245A6A93D123412324000AA",
                CardHolderName = "Jane Doe"
            };
        }

        //[TestMethod]
        //public void EbtBalanceInquiry() {
        //    var response = card.BalanceInquiry()
        //        .Execute();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual(SUCCESS, response?.ResponseCode);
        //}

        [TestMethod]
        public void EbtSale_CardData() {
            var response = ebtCardData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtSale_TrackData() {
            var response = ebtTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtSaleRefund_CardData() {
            var response = ebtCardData.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtSaleRefund_TrackData() {
            var response = ebtTrackData.Refund(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtTransaction_Refund_TrackData() {
            var transaction = ebtTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Refund()
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtTransaction_Refund_CreditData() {
            var transaction = ebtCardData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Refund()
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Captured);
        }

        [TestMethod]
        public void EbtTransaction_Reverse_TrackData() {
            var transaction = ebtTrackData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Reverse()
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Reversed);
        }

        [TestMethod]
        public void EbtTransaction_Reverse_CreditData() {
            var transaction = ebtCardData.Charge(AMOUNT)
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(transaction, TransactionStatus.Captured);

            var response = transaction.Reverse()
                .WithCurrency(CURRENCY)
                .Execute();

            assertEbtResponse(response, TransactionStatus.Reversed);
        }

        private void assertEbtResponse(Transaction response, TransactionStatus transactionStatus) {
            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(transactionStatus), response?.ResponseMessage);
        }
    }
}
