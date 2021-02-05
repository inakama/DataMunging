using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Dtos;
using LumenWorks.Framework.IO.Csv;

namespace DataMunging.ConsoleApp
{
    public class CSVManager
    {
        public List<ExpenseInputDto> GetExpenses(string csvPath)
        {
            var csvTable = new DataTable();
            List<ExpenseInputDto> expensesInputDto = new List<ExpenseInputDto>();

            using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(csvPath)), true))
            {
                csvTable.Load(csvReader);

                for (int i = 0; i < csvTable.Rows.Count; i++)
                {
                    expensesInputDto.Add(new ExpenseInputDto
                    {
                        Location = csvTable.Rows[i][0].ToString(),
                        Date = Convert.ToDateTime(csvTable.Rows[i][1].ToString()),
                        ItemDescription = csvTable.Rows[i][2].ToString(),
                        Cost = Convert.ToDouble(csvTable.Rows[i][3].ToString()),
                        CategoryCode = csvTable.Rows[i][4].ToString()
                    });
                }
            }

            return expensesInputDto;
        }


        public List<CategoryInputDto> GetCategories(string csvPath)
        {
            var csvTable = new DataTable();
            List<CategoryInputDto> categoriesInputDto = new List<CategoryInputDto>();

            using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(csvPath)), true))
            {
                csvTable.Load(csvReader);

                for (int i = 0; i < csvTable.Rows.Count; i++)
                {
                    categoriesInputDto.Add(new CategoryInputDto
                    {
                        CategoryID = csvTable.Rows[i][0].ToString(),
                        CategoryName = csvTable.Rows[i][1].ToString(),
                        IsExpensible = csvTable.Rows[i][2].ToString() == "Y" ? true : false
                    }); ;
                }
            }

            return categoriesInputDto;
        }
    }
}
