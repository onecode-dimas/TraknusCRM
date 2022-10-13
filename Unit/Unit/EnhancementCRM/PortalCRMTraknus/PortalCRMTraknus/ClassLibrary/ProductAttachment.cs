using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class ProductAttachment : IComparable<ProductAttachment>
    {
        #region Member Variable

        private bool cecking;
        private int id;
        private string productNumber;
        private string productName;
        private string unitProduct;
        private decimal productPrice;
        private string productDesc;

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
        public string UnitProduct
        {
            get { return unitProduct; }
            set { unitProduct = value; }
        }
        public decimal ProductPrice
        {
            get { return productPrice; }
            set { productPrice = value; }
        }
        public string ProductDesc
        {
            get { return productDesc; }
            set { productDesc = value; }
        }

        #endregion

        public int CompareTo(ProductAttachment other)
        {
            if (other.ProductNumber.CompareTo(this.ProductNumber) == -1)
            {
                return 1;
            }
            else if (other.ProductNumber.CompareTo(this.ProductNumber) == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private ProductAttachment()
        {
        }
        public ProductAttachment(int id,string productNumber, string productName,string unitProduct, decimal price, string desc)
        {
            this.id = id;
            this.productNumber = productNumber;
            this.productName = productName;
            this.unitProduct = unitProduct;
            this.productPrice = price;
            this.productDesc = desc;
        }
    }