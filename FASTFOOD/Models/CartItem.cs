using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASTFOOD.Models
{
    public class CartItem
    {
        public MONAN foodOrder { get; set; }
        public int Quantity { get; set; }

        public double SumPrice()
        {
            double s = 0;
            s = (double)(Quantity * foodOrder.DONGIA);
            return s;
        }
    }
}