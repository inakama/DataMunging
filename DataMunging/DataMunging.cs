using System;
using System.Collections.Generic;
using System.Configuration;
using Dtos;
using IServices;
using Services;

namespace DataMunging.ConsoleApp
{
    public class DataMunging
    {
        private readonly CSVManager csvManager;
        private readonly IDataMungingService dataMungingService;

        public DataMunging()
        {
            this.csvManager = new CSVManager();
            this.dataMungingService = new DataMungingService();
        }
       
        public void Start()
        {
            var categories = this.csvManager.GetCategories(ConfigurationManager.AppSettings["CategoriesCSVPath"]);
            var expenses = this.csvManager.GetExpenses(ConfigurationManager.AppSettings["ExpensesCSVPath"]);

            var dataMunging = this.dataMungingService.GetDataMunging(categories, expenses);

            DisplayDataMunging(dataMunging);
        }

        private void DisplayDataMunging(List<DataMungingOutputDto> dataMunging)
        {
            foreach (var data in dataMunging)
            {
                Console.Write(data.Date.ToString("dd/MM/yyyy") + " : ");
                Console.Write(data.Location + " - ");
                Console.Write("$" + data.TotalAmount);

                Console.WriteLine();
            }
        }
    }
}
