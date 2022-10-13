using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CPOProductList : List<CPOProduct>
{
    public static CPOProductList GetProducts()
    {
        CPOProductList ls = new CPOProductList();
        return ls;
    }
    public void AddList(CPOProduct otherItm)
    {
        if (this.Count > 0)
        {
            bool cek = false;
            for (int i = 0; i < this.Count; i++)
            {
                if (otherItm.ItemNumber.Equals(this[i].ItemNumber))
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