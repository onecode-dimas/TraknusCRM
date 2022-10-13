using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.WorkflowActivity.Helper
{
    public  enum TssType
    {
        SalesMarketSize = 865920000,
        SalesNonMarketSize = 865920001,
        SalesAll = 865920002,
        KAContributionAmount = 865920003,
        KAContributionPercentage = 865920004,
        MarketShareAmount = 865920005,
        MarketSharePercentage = 865920006

    }

    public enum CalculateMethodType
    {
        Method1=0,
        Method2=1,
        Method3=2,
        Method4=3
    }
}
