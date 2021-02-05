using System.Collections.Generic;
using System.Linq;
using Dtos;
using IServices;

namespace Services
{
    public class DataMungingService : IDataMungingService
    {
        public List<DataMungingOutputDto> GetDataMunging(List<CategoryInputDto> categories, List<ExpenseInputDto> expenses)
        {
            var expensableExpenses = categories.GroupJoin(expenses,  
                                            cat => cat.CategoryID, 
                                            exp => exp.CategoryCode, 
                                            (cat, exp) => new 
                                            {
                                                IsExpensible = cat.IsExpensible,
                                                Expenses = exp
                                            })
                                            .Where(x => x.IsExpensible)
                                            .SelectMany(e => e.Expenses);

            return expensableExpenses
                    .GroupBy(x => new { x.Date, x.Location })
                    .OrderBy(x => x.Key.Date)
                    .ThenBy(x => x.Key.Location)
                    .Select(exp => new DataMungingOutputDto()
                    {
                        Date = exp.Key.Date,
                        Location = exp.Key.Location,
                        TotalAmount = exp.Sum(x => x.Cost)
                    }).ToList();
        }
    }
}
