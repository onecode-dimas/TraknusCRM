using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_workorderpartssummary
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartssummary";
        #endregion

        #region Depedencies
        DL_trs_workorderpartssummary _DL_trs_workorderpartssummary = new DL_trs_workorderpartssummary();
        DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        DL_trs_masterpart _DL_trs_masterpart = new DL_trs_masterpart();

        private FSAP _fSAP = new FSAP();
        #endregion

        #region Privates
        #endregion

        #region Events
        #region Forms
        public void Form_OnUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                Guid id = entity.Id;
                if (entity.LogicalName == _DL_trs_workorderpartssummary.EntityName)
                {
                    if (_fSAP.SynchronizetoSAP(organizationService))
                    {
                        #region content
                        Entity enWorkOrderPartsSummary = _DL_trs_workorderpartssummary.Select(organizationService, id);

                        string trType = "A";
                        string trSta = string.Empty;
                        string trPart = "D";
                        string woNumber = string.Empty;
                        string materialNumber = string.Empty;
                        string quantity = string.Empty;

                        if (enWorkOrderPartsSummary.Contains("trs_workorder"))
                        {
                            Entity workOrder = _DL_serviceappointment.Select(organizationService, ((EntityReference)enWorkOrderPartsSummary["trs_workorder"]).Id);
                            woNumber = workOrder["subject"].ToString();
                        }
                        if (enWorkOrderPartsSummary.Contains("trs_returnedquantity"))
                            quantity = ((int)enWorkOrderPartsSummary["trs_returnedquantity"]).ToString();
                        if (enWorkOrderPartsSummary.Contains("trs_partnumber"))
                        {
                            Entity enMasterPart = _DL_trs_masterpart.Select(organizationService, ((EntityReference)enWorkOrderPartsSummary["trs_partnumber"]).Id);
                            materialNumber = enMasterPart["trs_name"].ToString();
                        }

                        string content = trType + "|" + trSta + "|" + trPart + "|" + woNumber + "|" + materialNumber + "|" + quantity;
                        #endregion

                        string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                        //string path = @"D:\Shared Folder\";
                        if (System.IO.Directory.Exists(path))
                        {
                            string filename = "WO_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + woNumber + ".txt";
                            System.IO.File.WriteAllText(path + filename, content);
                        }

                        else
                        {
                            throw new Exception("Directory not found: " + path);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Fields
        #endregion
        #endregion
    }
}
