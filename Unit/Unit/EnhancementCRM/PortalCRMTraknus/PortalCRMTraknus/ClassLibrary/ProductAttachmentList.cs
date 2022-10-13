using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class ProductAttachmentList : List<ProductAttachment>
    {
        //public ProductAttachmentList SortAsc()
        //{
        //    List<string> l=new List<string>() ;
        //    ProductAttachmentList temp = ProductAttachmentList.GetProducts();
        //    foreach (ProductAttachment itm in this)
        //    {
        //        if (temp.Count <= 0)
        //        {
        //            temp.Add(itm);
        //        }
        //        foreach (ProductAttachment itm2 in temp)
        //        {
        //            l.Add(itm.CompareTo(itm2).ToString() + " | " + itm.ProductNumber + " | " + itm2.ProductNumber);
        //            if (itm.CompareTo(itm2) == 0)
        //            {
                        
        //            }
        //            else if (itm.CompareTo(itm2) == -1)
        //            {
                        
        //            }
        //            else
        //            {

        //            }
        //        }
        //    }
        //    return temp;
        //}
        public static ProductAttachmentList GetProducts()
        {
            ProductAttachmentList ls = new ProductAttachmentList();            
            return ls;
        }
        public void AddList(ProductAttachment otherItm)
        {
            if (this.Count > 0)
            {
                bool cek = false;
                for(int i =0;i<this.Count;i++)
                {
                    if (otherItm.ProductNumber.Equals(this[i].ProductNumber))
                    {
                        cek = true;
                        break;
                    }
                    else
                    {
                        cek = false;
                    }
                }
                if (cek == false)
                {
                    this.Add(otherItm);
                }
            }
            else
            {
                this.Add(otherItm);
            }
        }
    }