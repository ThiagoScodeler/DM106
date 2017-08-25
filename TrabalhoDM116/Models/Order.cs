using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrabalhoDM116.Models
{
    public class Order
    {

        public int Id { get; set; }

        public string email { get; set; }

        public DateTime dataOrder { get; set; }

        public DateTime dataEntrega { get; set; }

        public string status { get; set; }

        public decimal precoTotal { get; set; }

        public decimal pesoTotal { get; set; }

        public decimal precoFrete { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}