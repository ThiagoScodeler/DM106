using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoDM116.Models
{
    public class Product
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name field is mandatory")]
        public string nome { get; set; }

        public string descricao { get; set; }

        public string cor { get; set; }

        [Required(ErrorMessage = "Model field is mandatory")]
        public string modelo { get; set; }

        [Required(ErrorMessage = "Code field is mandatory")]
        [StringLength(6, ErrorMessage = "Maximium Length is 6 characters")]
        public string codigo { get; set; }

        [Range(1, 10000, ErrorMessage = "Price value must be higher than 1")]
        public decimal preco { get; set; }

        public decimal peso { get; set; }

        public decimal altura { get; set; }

        public decimal largura { get; set; }

        public decimal comprimento { get; set; }

        public decimal diametro { get; set; }

        public string url { get; set; }
    }
}
