using CheckEscalationApproval.Business_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckEscalationApproval
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Dependencies
            BL_cposaleseffort _BL_cposaleseffort = new BL_cposaleseffort();
            BL_incentive _BL_incentive = new BL_incentive();
            BL_quote _BL_quote = new BL_quote();
            BL_quoteconditiontype _BL_quoteconditiontype = new BL_quoteconditiontype();
            #endregion

            try
            {
                _BL_cposaleseffort.CheckEscalationApproval();
                _BL_quote.CheckEscalationApproval();
                _BL_quoteconditiontype.CheckEscalationApproval();
                _BL_incentive.CheckEscalationApproval();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error while executing. Details : " + ex.Message.ToString());
            }
        }
    }
}
