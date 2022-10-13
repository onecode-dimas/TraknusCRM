using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;


namespace CalculateRTG
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper.LogCreator log = new Helper.LogCreator();
            OrganizationService organizationService;
            Guid userId;

            try
            {
                Helper.CRMConnector crmConnector = new Helper.CRMConnector();
                organizationService = crmConnector.Connect();
                userId = crmConnector.GetLoggedUserId(organizationService);
                log.UserID = userId;

                BusinessLayer.BL_serviceappointment sp = new BusinessLayer.BL_serviceappointment();
                sp.Execute(organizationService, userId);
            }
            catch (Exception ex)
            {
                log.Write(ex.Message);
                throw;
            }
        }
    }
}
