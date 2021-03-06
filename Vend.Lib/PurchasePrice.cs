using System;
using System.Collections.Generic;
using System.Text;

namespace Vend.App
{
    public class PurchasePrice
    {
        private int price;
        private decimal priceDecimal;

        public PurchasePrice()
        {            
        }
        public PurchasePrice(int initialPrice)
        {
            this.price = initialPrice;
        }
        public PurchasePrice(decimal initialPrice)
        {
            this.priceDecimal = initialPrice;
        }

        public int Price
        {
            get 
            {
                return Math.Max(this.price, Convert.ToInt32(this.priceDecimal));
            }
            set { price = value; }
        }

        public decimal PriceDecimal
        {
            get 
            { 
                return Math.Max(Convert.ToDecimal(this.price), this.priceDecimal);
            }
            set { priceDecimal = value; }
        }
    }
}
