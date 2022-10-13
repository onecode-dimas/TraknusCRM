using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Check_Escalation_Approval_for_Market_Size_Result.Business_Layer;

namespace Check_Escalation_Approval_for_Market_Size_Result
{
    class Program
    {
       
        static void Main(string[] args)
        {
            #region Dependencies
            BL_MarketSizeResultPSS _BL_MarketSizeResultPSS = new BL_MarketSizeResultPSS();
            #endregion

            try
            {
                _BL_MarketSizeResultPSS.checkEscalationApprovalforMarketSizeResultPSS();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unexpected error while executing. Details : " + ex.Message.ToString());
            }
        }
    }
}
