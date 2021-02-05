using System.Collections.Generic;
using Dtos;

namespace IServices
{
    public interface IDataMungingService
    {
        List<DataMungingOutputDto> GetDataMunging(List<CategoryInputDto> categories, List<ExpenseInputDto> expenses);
    }
}
