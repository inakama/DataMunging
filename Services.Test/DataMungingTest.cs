using System;
using System.Collections.Generic;
using System.Linq;
using Dtos;
using NUnit.Framework;

namespace Services.Test
{
    public class DataMungingTest
    {
        private DataMungingService dataMungingService;

        [SetUp]
        public void Setup()
        {
            this.dataMungingService = new DataMungingService();
        }

        [Test]
        public void When_ThereAreNoExpenses_Expect_NoResults()
        {

            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE2", IsExpensible = true },
                new CategoryInputDto() { CategoryID = "PRS2", IsExpensible = false }
            };

            var expenses = new List<ExpenseInputDto>();

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 0);
        }

        [Test]
        public void When_AnExpenseHasACategoryThatIsNotDefined_Expect_ToBeExcluded()
        {

            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "PRS", IsExpensible = false }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() { Date = DateTime.Today, Location = "High Point Market",  CategoryCode = "CFE", Cost = 40.00 }
            };

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 0);
        }

        [Test]
        public void When_TheCategoryOfAnExpenseIsNotExpensable_Expect_ToBeExcluded()
        {
            var categories = new List<CategoryInputDto>()
            {
                   new CategoryInputDto() { CategoryID = "CFE", IsExpensible = false }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                   new ExpenseInputDto() {  Date = DateTime.Now, Location = "Starbucks",  CategoryCode = "CFE" }
            };

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 0);
        }

        [Test]
        public void When_TheCategoryOfAnExpenseIsExpensable_Expect_ToBeIncluded()
        {

            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE", IsExpensible = true }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() {  Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 10.01 }
            };


            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 1);
            Assert.AreEqual(dataMunging.First().Date, DateTime.Today);
            Assert.AreEqual(dataMunging.First().Location, "Starbucks");
            Assert.AreEqual(dataMunging.First().TotalAmount, 10.01);
        }

        [Test]
        public void When_ExistTwoExpensesWithTheSameDateAndLocation_Expect_TheirCostsAreAdded()
        {
            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE", IsExpensible = true }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 10.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 20.00 }
            };


            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 1);

            Assert.AreEqual(dataMunging.First().Date, DateTime.Today);
            Assert.AreEqual(dataMunging.First().Location, "Starbucks");
            Assert.AreEqual(dataMunging.First().TotalAmount, 30.00);
        }


        [Test]
        public void When_SomeExpensesWithDifferentDateButTheSameLocation_Expect_ToBeSeparatedAndOrderedByDate()
        {

            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE", IsExpensible = true }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() { Date = DateTime.Today.AddDays(1), Location = "Starbucks",  CategoryCode = "CFE", Cost = 40.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 10.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 20.00 }
            };

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 2);

            Assert.AreEqual(dataMunging[0].Date, DateTime.Today);
            Assert.AreEqual(dataMunging[0].Location, "Starbucks");
            Assert.AreEqual(dataMunging[0].TotalAmount, 30.00);

            Assert.AreEqual(dataMunging[1].Date, DateTime.Today.AddDays(1));
            Assert.AreEqual(dataMunging[1].Location, "Starbucks");
            Assert.AreEqual(dataMunging[1].TotalAmount, 40.00);
        }

        [Test]
        public void When_SomeExpensesWithDifferentLocationButTheSameDate_Expect_ToBeSeparatedAndOrderedByDateAndThenByLocation()
        {

            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE", IsExpensible = true }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 10.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "High Point Market",  CategoryCode = "CFE", Cost = 40.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 20.00 }
            };

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 2);

            Assert.AreEqual(dataMunging[0].Date, DateTime.Today);
            Assert.AreEqual(dataMunging[0].Location, "High Point Market");
            Assert.AreEqual(dataMunging[0].TotalAmount, 40.00);

            Assert.AreEqual(dataMunging[1].Date, DateTime.Today);
            Assert.AreEqual(dataMunging[1].Location, "Starbucks");
            Assert.AreEqual(dataMunging[1].TotalAmount, 30.00);
        }

        [Test]
        public void When_SomeExpensesAreNotExpensabled_Expect_ThoseOnesBeExcluded()
        {
            var categories = new List<CategoryInputDto>()
            {
                new CategoryInputDto() { CategoryID = "CFE", IsExpensible = true },
                new CategoryInputDto() { CategoryID = "PRS", IsExpensible = false }
            };

            var expenses = new List<ExpenseInputDto>()
            {
                new ExpenseInputDto() { Date = DateTime.Today, Location = "High Point Market",  CategoryCode = "CFE", Cost = 40.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 10.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "CFE", Cost = 20.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "Starbucks",  CategoryCode = "PRS", Cost = 100.00 },
                new ExpenseInputDto() { Date = DateTime.Today, Location = "BurguerKing",  CategoryCode = "PRS", Cost = 200.00 }
            };

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            Assert.AreEqual(dataMunging.Count, 2);

            Assert.AreEqual(dataMunging[0].Date, DateTime.Today);
            Assert.AreEqual(dataMunging[0].Location, "High Point Market");
            Assert.AreEqual(dataMunging[0].TotalAmount, 40.00);

            Assert.AreEqual(dataMunging[1].Date, DateTime.Today);
            Assert.AreEqual(dataMunging[1].Location, "Starbucks");
            Assert.AreEqual(dataMunging[1].TotalAmount, 30.00);
        }
    }
}