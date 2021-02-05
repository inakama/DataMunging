using System;

namespace Dtos
{
    public class ExpenseInputDto
    {
        public string Location { get; set; }

        public DateTime Date { get; set; }

        public string ItemDescription { get; set; }

        public double Cost { get; set; }

        public string CategoryCode { get; set; }
    }
}
