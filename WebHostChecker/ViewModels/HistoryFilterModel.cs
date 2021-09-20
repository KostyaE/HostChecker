using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.ViewModels
{
    public class HistoryFilterModel
    {
        [DataType(DataType.Url)]
        public string WebAddress { get; set; }

        [DataType(DataType.DateTime)]
        public DataType StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DataType EndDate { get; set; }

        public bool Availability { get; set; }
    }
}
