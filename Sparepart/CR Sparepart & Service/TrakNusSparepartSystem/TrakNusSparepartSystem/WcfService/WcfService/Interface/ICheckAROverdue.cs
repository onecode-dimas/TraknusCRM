using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystemWcfService
{
   public interface ICheckAROverdue
    {
        WS_Response_CheckAROverdue CheckAR(string transNumber,string customerCode,string businessTransType, IOrganizationService orgService); 
    }
}
