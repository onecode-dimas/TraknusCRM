using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;

namespace SendEmail
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
            }
            catch (Exception ex)
            {
                log.Write(ex.Message);
                throw;
            }

            BusinessLayer.BL_new_population reminder = new BusinessLayer.BL_new_population();
            reminder.Execute(organizationService, userId);
        }
    }
}
