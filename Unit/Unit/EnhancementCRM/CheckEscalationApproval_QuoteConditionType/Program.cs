using CheckEscalationApproval_QuoteConditionType.Business_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckEscalationApproval_QuoteConditionType
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Dependencies
            BL_quoteconditiontype _BL_quoteconditiontype = new BL_quoteconditiontype();
            #endregion

            try
            {
                _BL_quoteconditiontype.CheckEscalationApproval();
                _BL_quoteconditiontype.CheckEscalationApproval();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error while executing. Details : " + ex.Message.ToString());
            }
        }
    }
}
