using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_partactivityheader
    {
        #region Constants
        private const string _classname = "BL_tss_partactivityheader";
        private const int _depth = 1;
        #endregion

        public void setPlanDate(IOrganizationService organizationService, Entity now)
        {
            try
            {
                if (now.Attributes.Contains("tss_appointment") && now.Attributes["tss_appointment"] != null)
                {
                    var context = new OrganizationServiceContext(organizationService);
                    var appointment = (from c in context.CreateQuery("appointment")
                                       where c.GetAttributeValue<Guid>("activityid") == now.GetAttributeValue<EntityReference>("tss_appointment").Id
                                       select c).ToList();

                    if (appointment.Count > 0)
                    {
                        if (appointment[0].Attributes.Contains("scheduledstart") && appointment[0].Attributes["scheduledstart"] != null)
                        {
                            Entity ent = new Entity("tss_partactivityheader");
                            ent.Id = now.Id;
                            ent.Attributes["tss_plandate"] = appointment[0].GetAttributeValue<DateTime>("scheduledstart").ToLocalTime();
                            organizationService.Update(ent);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void checkBackDate(IOrganizationService organizationService, Entity now, bool onCreate)
        {
            try
            {
                if (now.Attributes.Contains("tss_actualdate") && now.Attributes["tss_actualdate"] != null && now.Attributes.Contains("tss_pss") && now.Attributes["tss_pss"] != null)
                {
                    DateTime dateActual = now.GetAttributeValue<DateTime>("tss_actualdate").ToLocalTime();

                    if (dateActual.Date > DateTime.Now.ToLocalTime().Date)
                    {
                        throw new InvalidPluginExecutionException("Actual Date must be less than today");
                    }
                    else
                    {
                        var context = new OrganizationServiceContext(organizationService);
                        var setup = (from c in context.CreateQuery("tss_sparepartsetup")
                                     select c).ToList();
                        if (setup.Count > 0)
                        {
                            //select tss_maxbackdated,tss_maxvisitperday,* from tss_sparepartsetup
                            if (setup[0].GetAttributeValue<string>("tss_maxbackdated") != null)
                            {
                                int maxBackDate = Convert.ToInt32(setup[0].GetAttributeValue<string>("tss_maxbackdated"));
                                DateTime dateNow = DateTime.Now.ToLocalTime();
                                if ((dateNow.Day > maxBackDate && dateNow.Month == dateActual.Month && dateNow.Day > maxBackDate) || ((dateNow.Day <= maxBackDate) && ((dateNow.Month == 1 && dateActual.Month == 12) || (dateNow.Month - dateActual.Month <= 1))))
                                {
                                }
                                else
                                {
                                    throw new InvalidPluginExecutionException("Can't insert back date!");
                                }
                            }

                            if (setup[0].GetAttributeValue<string>("tss_maxvisitperday") != null && now.GetAttributeValue<EntityReference>("tss_pss") != null)
                            {
                                int maxvisit = Convert.ToInt32(setup[0].GetAttributeValue<string>("tss_maxvisitperday"));
                                var partactivity = (from c in context.CreateQuery("tss_partactivityheader")
                                                    where c.GetAttributeValue<EntityReference>("tss_pss").Id == now.GetAttributeValue<EntityReference>("tss_pss").Id
                                                    select c).ToList();

                                int ct = 0;
                                for (int i = 0; i < partactivity.Count; i++)
                                {
                                    if (partactivity[i].GetAttributeValue<DateTime>("tss_actualdate").ToLocalTime().Date == dateActual.Date) ct++;
                                }
                                if (ct > maxvisit)
                                {
                                    throw new InvalidPluginExecutionException("Already reach max visit per day!");
                                }

                                //if (onCreate)
                                //{
                                //    int ct = 0;
                                //    for (int i = 0; i < partactivity.Count; i++)
                                //    {
                                //        if (partactivity[i].GetAttributeValue<DateTime>("tss_actualdate").ToLocalTime().Date == dateActual.Date) ct++;
                                //    }
                                //    if (ct > maxvisit)
                                //    {
                                //        throw new InvalidPluginExecutionException("Already reach max visit per day!");
                                //    }
                                //}

                                //if (!onCreate)
                                //{
                                //    int ct = 0;
                                //    for (int i = 0; i < partactivity.Count; i++)
                                //    {
                                //        if (partactivity[i].GetAttributeValue<DateTime>("tss_actualdate").ToLocalTime().Date == dateActual.Date) ct++;
                                //    }

                                //    if (ct >= maxvisit)
                                //    {
                                //        throw new InvalidPluginExecutionException("Already reach max visit per day!");
                                //    }
                                //}

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void onComplete(IOrganizationService organizationService, Entity now)
        {
            try
            {
                if (now.Attributes.Contains("tss_actualdate") && now.Attributes["tss_actualdate"] != null && now.Attributes.Contains("tss_status") && now.Attributes["tss_status"] != null)
                {
                    //status complete
                    if (now.GetAttributeValue<OptionSetValue>("tss_status").Value == 865920001)
                    {
                        DateTime dateActual = now.GetAttributeValue<DateTime>("tss_actualdate").ToLocalTime();

                        var context = new OrganizationServiceContext(organizationService);
                        var lines = (from c in context.CreateQuery("tss_partactivitylines")
                                     where c.GetAttributeValue<EntityReference>("tss_partactivityheader").Id == now.Id
                                     select c).ToList();

                        foreach (var x in lines)
                        {
                            if (x.Attributes.Contains("tss_activitiestype") && x.Attributes["tss_activitiestype"] != null && x.Attributes.Contains("tss_activities") && x.Attributes["tss_activities"] != null)
                            {
                                int type = x.GetAttributeValue<OptionSetValue>("tss_activitiestype").Value;
                                string activity = x.GetAttributeValue<string>("tss_activities");

                                //type invoice
                                if (type == 865920008)
                                {
                                    var sosublines = (from c in context.CreateQuery("tss_salesorderpartsublines")
                                                      where c.GetAttributeValue<string>("tss_invoiceno") == activity
                                                      select c).ToList();
                                    foreach (var y in sosublines)
                                    {
                                        Entity ent = new Entity("tss_salesorderpartsublines");
                                        ent.Id = y.Id;
                                        ent.Attributes["tss_invoicereceiptdate"] = dateActual;
                                        organizationService.Update(ent);
                                    }
                                }
                                //type delivery
                                else if (type == 865920009)
                                {
                                    var sosublines = (from c in context.CreateQuery("tss_salesorderpartsublines")
                                                      where c.GetAttributeValue<string>("tss_deliveryno") == activity
                                                      select c).ToList();
                                    foreach (var y in sosublines)
                                    {
                                        Entity ent = new Entity("tss_salesorderpartsublines");
                                        ent.Id = y.Id;
                                        ent.Attributes["tss_customerreceiptdate"] = dateActual;
                                        organizationService.Update(ent);
                                    }
                                }
                            }
                        }

                        //update appointment
                        if (now.Attributes.Contains("tss_appointment") && now.Attributes["tss_appointment"] != null)
                        {
                            var appointment = (from c in context.CreateQuery("appointment")
                                               where c.GetAttributeValue<Guid>("activityid") == now.GetAttributeValue<EntityReference>("tss_appointment").Id
                                               select c).ToList();

                            if (appointment.Count > 0)
                            {
                                Entity ent = new Entity("appointment");
                                ent.Id = appointment[0].GetAttributeValue<Guid>("activityid");
                                ent.Attributes["tss_actualdate"] = dateActual;
                                ent.Attributes["tss_visitpart"] = new EntityReference("tss_partactivityheader", now.Id);
                                ent.Attributes["statecode"] = new OptionSetValue(1);
                                organizationService.Update(ent);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext, Entity postImage)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == "tss_partactivityheader")
                {
                    checkBackDate(organizationService, postImage, true);
                    setPlanDate(organizationService, postImage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext, Entity postImage)
        {
            try
            {
                Entity entity = (Entity)pluginExecutionContext.InputParameters["Target"];
                if (entity.LogicalName == "tss_partactivityheader")
                {
                    checkBackDate(organizationService, postImage, false);
                    onComplete(organizationService, postImage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation : " + ex.Message.ToString());
            }
        }
    }
}
