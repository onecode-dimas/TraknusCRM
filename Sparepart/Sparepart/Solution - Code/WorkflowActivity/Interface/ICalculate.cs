using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.WorkflowActivity.Interface
{
   public  interface ICalculate
    {
        decimal[] TargetSales { get; }
        void Calculate();
    }
}
