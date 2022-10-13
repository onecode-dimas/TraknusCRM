using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CPOProduct : IComparable<CPOProduct>
{
    #region Member Variable

        private bool cecking;
        private int id;
        private decimal itemNumber;
        private string productNumber;
        private string productName;
        private decimal pricePerUnit;
        private decimal discount;
        private decimal retention;
        private decimal totalPrice;
        private string descProd;

        #endregion

        #region Public Property

        public bool Cecking
        {
            get { return cecking; }
            set { cecking = value; }
        }
        public int ID
        {
            get{return id;}
            set
            {
                if (id < 0)
                {
                    throw new ArgumentException(@"ID must be greater than or equal to zero.");
                }
                else
                {
                    id = value;
                }
            }
        }
        public decimal ItemNumber
        {
            get { return decimal.Round(itemNumber,0); }
            set { itemNumber = value; }
        }
        public string ProductNumber
        {
            get { return productNumber; }
            set { productNumber = value; }
        }
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }
        public decimal PricePerUnit
        {
            get { return decimal.Round(pricePerUnit, 2); }
            set { pricePerUnit = value; }
        }
        public decimal Discount
        {
            get { return decimal.Round(discount, 2); }
            set { discount = value; }
        }
        public decimal Retention
        {
            get { return decimal.Round(retention, 2); }
            set { retention = value; }
        }
        
        public decimal TotalPrice
        {
            get { return decimal.Round(totalPrice, 2); }
            set { totalPrice = value; }
        }
        public string ProductDesc
        {
            get { return descProd; }
            set { descProd = value; }
        }

        #endregion

        public int CompareTo(CPOProduct other)
        {
            if (other.ItemNumber.CompareTo(this.ItemNumber) == -1)
            {
                return 1;
            }
            else if (other.ItemNumber.CompareTo(this.ItemNumber) == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private CPOProduct()
        {
        }
        public CPOProduct(int id, decimal itemNumber, string productNumber, string productName, decimal pricePerUnit, decimal discount, decimal retention, decimal totalPrice, string desc)
        {
            //this.id = id;
            //this.productNumber = productNumber;
            //this.productName = productName;
            //this.unitProduct = unitProduct;
            //this.productPrice = price;
            //this.productDesc = desc;
            this.id = id;
            this.itemNumber = itemNumber;
            this.productNumber = productNumber;
            this.productName = productName;
            this.pricePerUnit = pricePerUnit;
            this.discount = discount;
            this.retention = retention;
            this.totalPrice = totalPrice;
            this.descProd = desc;
        }
}