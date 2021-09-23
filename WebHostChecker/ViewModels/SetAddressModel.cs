using System;
using System.ComponentModel.DataAnnotations;


namespace WebHostChecker.ViewModels
{
    public class SetAddressModel
    {

        [Required(ErrorMessage = "Не указан URL-адрес")]
        [DataType(DataType.Url)]
        public string WebAddress { get; set; }

        [Required(ErrorMessage = "Не указан период проверки")]
        public DateTime TimePeriod { get; set; }
    }
}
