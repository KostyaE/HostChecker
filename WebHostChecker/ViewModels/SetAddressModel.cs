using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.ViewModels
{
    public class SetAddressModel
    {
        [Required(ErrorMessage = "Не указан URL-адрес")]
        [DataType(DataType.Url)]
        public string WebAddress { get; set; }

        [Required(ErrorMessage = "Не указан период проверки")]
        public ushort Period { get; set; }
    }
}
