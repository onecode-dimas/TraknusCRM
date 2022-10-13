using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_new_population
    {
        #region Constants
        private const string _classname = "BL_new_population";
        #endregion

        #region Depedencies
        private DL_new_population _DL_new_population = new DL_new_population();
        private FSAP _fSAP = new FSAP();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            try
            {
                FMobile _fmobile = new FMobile(organizationService);
                _fmobile.SendPopulation(organizationService, id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".SendtoMobile : " + ex.Message);
            }
        }
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreatePreValidate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    if (entity.Attributes.Contains("trs_bastsigndate"))
                    {
                        if (entity.Attributes["trs_bastsigndate"] != null)
                        {
                            DateTime bastsigndate = ((DateTime)entity.Attributes["trs_bastsigndate"]).ToLocalTime();
                            entity.Attributes["trs_warrantystartdate"] = bastsigndate;
                            entity.Attributes["trs_warrantyenddate"] = bastsigndate.AddYears(1);
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
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreatePreValidate : " + ex.Message.ToString());
            }
        }
        
        public void Form_OnUpdatePreValidate_trs_bastsigndate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity pre = (Entity)pluginExcecutionContext.PreEntityImages["Target"];
                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    if (entity.Attributes.Contains("trs_bastsigndate"))
                    {
                        if (entity.Attributes["trs_bastsigndate"] != null)
                        {
                            DateTime bastsigndate = ((DateTime)entity.Attributes["trs_bastsigndate"]).ToLocalTime();
                            entity.Attributes["trs_warrantystartdate"] = bastsigndate;
                            entity.Attributes["trs_warrantyenddate"] = bastsigndate.AddYears(1);
                            if (entity.Attributes.Contains("new_deliverydate"))
                                entity.Attributes["new_deliverydate"] = bastsigndate;
                            else
                                entity.Attributes.Add("new_deliverydate", bastsigndate);
                        }
                        else
                        {
                            entity.Attributes["trs_warrantystartdate"] = null;
                            entity.Attributes["trs_warrantyenddate"] = null;
                        }

                        if (entity.Attributes.Contains("trs_unitstatus"))
                            entity.Attributes["trs_unitstatus"] = true;
                        else
                            entity.Attributes.Add("trs_unitstatus", true);
                    }

                    if (entity.Attributes.Contains("trs_hourmeteronvisit") || entity.Attributes.Contains("trs_datevisit"))
                    {
                        if (entity.Attributes.Contains("trs_uiostatus"))
                            entity.Attributes["trs_uiostatus"] = 167630000;
                        else
                            entity.Attributes.Add("trs_uiostatus", 167630000);
                    }

                    #region Hour Meter on Visit
                    if (entity.Attributes.Contains("trs_hourmeteronvisit"))
                    {
                        if (pre.Attributes.Contains("trs_hourmeteronvisit") && pre.GetAttributeValue<int>("trs_hourmeteronvisit") != null)
                        {
                            if (entity.GetAttributeValue<int>("trs_hourmeteronvisit") > pre.GetAttributeValue<int>("trs_hourmeteronvisit"))
                            {
                                if (entity.Attributes.Contains("new_latesthourmeter"))
                                    entity.Attributes["new_latesthourmeter"] = pre.GetAttributeValue<int>("trs_hourmeteronvisit");
                                else
                                    entity.Attributes.Add("new_latesthourmeter", pre.GetAttributeValue<int>("trs_hourmeteronvisit"));
                            }
                            else if (entity.GetAttributeValue<int>("trs_hourmeteronvisit") < pre.GetAttributeValue<int>("trs_hourmeteronvisit"))
                                throw new InvalidPluginExecutionException("Hour Meter on Visit can't less than '" + pre.GetAttributeValue<int>("trs_hourmeteronvisit").ToString() + "'.");
                        }
                    }
                    #endregion

                    //Update by : [Santony] on [8/4/2015]
                    //Remark : Salah casting tipe data untuk field "trs_datevisit", seharusnya datetime bukannya decimal.
                    #region Date Visit
                    if (entity.Attributes.Contains("trs_datevisit"))
                    {
                        if (pre.Attributes.Contains("trs_datevisit") && pre.GetAttributeValue<DateTime>("trs_datevisit") != null)
                        {
                            if (entity.GetAttributeValue<DateTime>("trs_datevisit") > pre.GetAttributeValue<DateTime>("trs_datevisit"))
                            {
                                if (entity.Attributes.Contains("trs_datelatesthourmeter"))
                                    entity.Attributes["trs_datelatesthourmeter"] = pre.GetAttributeValue<DateTime>("trs_datevisit");
                                else
                                    entity.Attributes.Add("trs_datelatesthourmeter", pre.GetAttributeValue<DateTime>("trs_datevisit"));
                            }
                            else if (entity.GetAttributeValue<DateTime>("trs_datevisit") < pre.GetAttributeValue<DateTime>("trs_datevisit"))
                                throw new InvalidPluginExecutionException("Hour Meter on Visit (Date) can't less than '" + pre.GetAttributeValue<DateTime>("trs_datevisit").ToLocalTime().ToString("yyyy/MM/dd") + "'.");
                        }
                    }
                    #endregion

                    #region New Latest Hour Meter
                    if (entity.Attributes.Contains("new_latesthourmeter"))
                    {
                        if (pre.Attributes.Contains("new_latesthourmeter") && pre.GetAttributeValue<decimal>("new_latesthourmeter") != null)
                        {
                            if (entity.GetAttributeValue<int>("new_latesthourmeter") > pre.GetAttributeValue<decimal>("new_latesthourmeter"))
                            {
                                if (entity.Attributes.Contains("trs_hourmeter"))
                                    entity.Attributes["trs_hourmeter"] = pre.GetAttributeValue<decimal>("new_latesthourmeter");
                                else
                                    entity.Attributes.Add("trs_hourmeter", pre.GetAttributeValue<decimal>("new_latesthourmeter"));
                            }
                            else if (entity.GetAttributeValue<int>("new_latesthourmeter") < pre.GetAttributeValue<decimal>("new_latesthourmeter"))
                                throw new InvalidPluginExecutionException("Latest Hour Meter can't less than '" + pre.GetAttributeValue<decimal>("new_latesthourmeter").ToString() + "'.");
                        }
                    }
                    #endregion

                    //Update by : [Santony] on [8/4/2015]
                    //Remark : Salah casting tipe data untuk field "trs_datelatesthourmeter", seharusnya datetime bukannya decimal.
                    #region Date Latest Hour Meter
                    if (entity.Attributes.Contains("trs_datelatesthourmeter"))
                    {
                        if (pre.Attributes.Contains("trs_datelatesthourmeter") && pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter") != null)
                        {
                            if (entity.GetAttributeValue<DateTime>("trs_datelatesthourmeter") > pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter"))
                            {
                                if (entity.Attributes.Contains("trs_datehourmeter"))
                                    entity.Attributes["trs_datehourmeter"] = pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter");
                                else
                                    entity.Attributes.Add("trs_datehourmeter", pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter"));
                            }
                            else if (entity.GetAttributeValue<DateTime>("trs_datelatesthourmeter") < pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter"))
                                throw new InvalidPluginExecutionException("Latest Hour Meter (Date) can't less than '" + pre.GetAttributeValue<DateTime>("trs_datelatesthourmeter").ToLocalTime().ToString("yyyy/MM/dd") + "'.");
                        }
                    }
                    #endregion
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdatePreValidate_trs_bastsigndate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_UpdateSAP(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity previous = (Entity)pluginExcecutionContext.PreEntityImages["PreImage"];
                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    if (_fSAP.SynchronizetoSAP(organizationService))
                    {
                        if ((entity.Attributes.Contains("trs_warrantystartdate") && entity.Attributes.Contains("trs_warrantyenddate")) && entity.Attributes.Contains("trs_functionallocation") && entity.Attributes.Contains("trs_bastsigndate"))
                        {
                            string serialNumber = string.Empty;
                            string warrantystart = string.Empty;
                            string warrantyend = string.Empty;
                            string bastsigndate = string.Empty;
                            string functionallocation = string.Empty;

                            serialNumber = previous.Attributes["new_serialnumber"].ToString();
                            //start date
                            if (entity.Attributes.Contains("trs_warrantystartdate") && entity.Attributes["trs_warrantystartdate"] != null)
                            {
                                warrantystart = ((DateTime)entity.Attributes["trs_warrantystartdate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            else if (previous.Attributes.Contains("trs_warrantystartdate") && previous.Attributes["trs_warrantystartdate"] != null)
                            {
                                warrantystart = ((DateTime)previous.Attributes["trs_warrantystartdate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            //end date
                            if (entity.Attributes.Contains("trs_warrantyenddate") && entity.Attributes["trs_warrantyenddate"] != null)
                            {
                                warrantyend = ((DateTime)entity.Attributes["trs_warrantyenddate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            else if (previous.Attributes.Contains("trs_warrantyenddate") && previous.Attributes["trs_warrantyenddate"] != null)
                            {
                                warrantyend = ((DateTime)previous.Attributes["trs_warrantyenddate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            //BAST sign date
                            if (entity.Attributes.Contains("trs_bastsigndate") && entity.Attributes["trs_bastsigndate"] != null)
                            {
                                bastsigndate = ((DateTime)entity.Attributes["trs_bastsigndate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            else if (previous.Attributes.Contains("trs_bastsigndate") && previous.Attributes["trs_bastsigndate"] != null)
                            {
                                bastsigndate = ((DateTime)previous.Attributes["trs_bastsigndate"]).ToLocalTime().ToString("yyyyMMdd");
                            }
                            if (entity.Attributes.Contains("trs_functionallocation"))
                            {
                                if (entity.Attributes["trs_functionallocation"] != null)
                                {
                                    EntityReference erFunctionalAllocation = (EntityReference)entity.Attributes["trs_functionallocation"];
                                    Entity eFunctionalAllocation = organizationService.Retrieve(erFunctionalAllocation.LogicalName, erFunctionalAllocation.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                                    functionallocation = eFunctionalAllocation.Attributes["trs_name"].ToString();
                                }
                            }
                            else if (previous.Attributes.Contains("trs_functionallocation") && previous.Attributes["trs_functionallocation"] != null)
                            {
                                EntityReference erFunctionalAllocation = (EntityReference)previous.Attributes["trs_functionallocation"];
                                Entity eFunctionalAllocation = organizationService.Retrieve(erFunctionalAllocation.LogicalName, erFunctionalAllocation.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                                functionallocation = eFunctionalAllocation.Attributes["trs_name"].ToString();
                            }

                            string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                            //string path2 = @"D:\Shared Folder\";
                            if (System.IO.Directory.Exists(path))
                            {
                                string text = "2||H|" + serialNumber + "|" + functionallocation + "|" + warrantystart + "|" + warrantyend + "|" + bastsigndate;
                                string filename = "EQ_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + serialNumber + ".txt";
                                System.IO.File.WriteAllText(path + filename, text);
                            }
                            else
                            {
                                throw new Exception("Directory not found: " + path);
                                //+ System.IO.Directory.Exists(@"\\192.168.0.165\d$\").ToString()
                                //+ "remote " + System.IO.Directory.Exists(@"\\192.168.0.165\d$\REPOSITORY\Traknus-trs\").ToString()
                                //+ " shared " + System.IO.Directory.Exists(@"D:\Shared Folder\").ToString()
                                //+ " shared2 " + System.IO.Directory.Exists(@"\\192.168.0.70\d$\Shared Folder\").ToString());
                            }
                        }

                        if (entity.Attributes.Contains("trs_lock"))
                        {
                            string serialNumber = string.Empty;

                            serialNumber = previous.Attributes["new_serialnumber"].ToString();

                            if (entity.GetAttributeValue<bool>("trs_lock")) //ACTIVE POPULATION
                            {
                                string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                                //string path2 = @"D:\Shared Folder\";
                                if (System.IO.Directory.Exists(path))
                                {
                                    string text = "4||H|" + serialNumber;
                                    string filename = "EQ_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + serialNumber + ".txt";
                                    System.IO.File.WriteAllText(path + filename, text);
                                }
                                else
                                {
                                    throw new Exception("Directory not found: " + path);
                                    //+ System.IO.Directory.Exists(@"\\192.168.0.165\d$\").ToString()
                                    //+ "remote " + System.IO.Directory.Exists(@"\\192.168.0.165\d$\REPOSITORY\Traknus-trs\").ToString()
                                    //+ " shared " + System.IO.Directory.Exists(@"D:\Shared Folder\").ToString()
                                    //+ " shared2 " + System.IO.Directory.Exists(@"\\192.168.0.70\d$\Shared Folder\").ToString());
                                }
                            }
                            else
                            {
                                string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                                //string path2 = @"D:\Shared Folder\";
                                if (System.IO.Directory.Exists(path))
                                {
                                    string text = "5||H|" + serialNumber;
                                    string filename = "EQ_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + serialNumber + ".txt";
                                    System.IO.File.WriteAllText(path + filename, text);
                                }
                                else
                                {
                                    throw new Exception("Directory not found: " + path);
                                    //+ System.IO.Directory.Exists(@"\\192.168.0.165\d$\").ToString()
                                    //+ "remote " + System.IO.Directory.Exists(@"\\192.168.0.165\d$\REPOSITORY\Traknus-trs\").ToString()
                                    //+ " shared " + System.IO.Directory.Exists(@"D:\Shared Folder\").ToString()
                                    //+ " shared2 " + System.IO.Directory.Exists(@"\\192.168.0.70\d$\Shared Folder\").ToString());
                                }
                            }
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
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_UpdateSAP : " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PostPopulationCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    SendtoMobile(organizationService, entity.Id);
                }
            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostPopulationCreate : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostPopulationUpdate(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_new_population.EntityName)
                {
                    SendtoMobile(organizationService, entity.Id);
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
