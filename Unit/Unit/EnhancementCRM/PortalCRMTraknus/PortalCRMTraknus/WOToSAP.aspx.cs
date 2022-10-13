using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xrm;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;

namespace PortalCRMTraknus
{
    public partial class WOToSAP : System.Web.UI.Page
    {
        private void validationSerialNumberInPopulation(XrmServiceContext crm,string sn)
        {
            crm.ClearChanges();

            Xrm.new_population pop = crm.new_populationSet.Where(p => p.new_SerialNumber == sn).FirstOrDefault();
            if (pop == null)
            {
                throw new Exception("Not Found Serial Number in Population");
            }
        }
        private void processWOToSAP(Guid id)
        {
            string strConnectionCRM = ConfigurationManager.AppSettings["ConnectionCRM"];
            string fileName = ConfigurationManager.AppSettings["fileNameWOToSAP"];
            string pathOnly = ConfigurationManager.AppSettings["pathFileWOToSAP"];
            string PathFile = pathOnly + fileName;
            Microsoft.Xrm.Client.CrmConnection connection = Microsoft.Xrm.Client.CrmConnection.Parse(strConnectionCRM);
            XrmServiceContext crm = new XrmServiceContext(connection);

            var serviceContainer = crm as Microsoft.Xrm.Client.IOrganizationServiceContainer;
            var cachedOrgService = serviceContainer.Service as Microsoft.Xrm.Client.Services.CachedOrganizationService;
            var orgServiceCache = cachedOrgService.Cache as Microsoft.Xrm.Client.Services.IOrganizationServiceCache;

            orgServiceCache.Mode = Microsoft.Xrm.Client.Services.OrganizationServiceCacheMode.Disabled;

            crm.ClearChanges();
            
            try
            {
                var etySA = crm.ServiceAppointmentSet.Where(a => a.Id.Equals(id)).FirstOrDefault();
                if (etySA != null)
                {
                    validationSerialNumberInPopulation(crm, etySA.new_SerialNumber);
                }
                if (etySA != null && etySA.new_SerialNumber != null && etySA.new_SerialNumber != "" && etySA.new_SerialNumber.Trim()!="")
                {
                    #region Create File

                    if (etySA.RegardingObjectId != null)
                    {
                        var etyVOC = crm.IncidentSet.Where(r => r.Id.Equals(etySA.RegardingObjectId.Id)).FirstOrDefault();
                        if (etyVOC != null && etyVOC.TicketNumber != null && etyVOC.TicketNumber != "" && etyVOC.TicketNumber.Trim() != "")
                        {
                            PathFile = PathFile.Replace("[custome]", etySA.new_SerialNumber.Trim());

                            if (File.Exists(PathFile))
                            {
                                File.Delete(PathFile);
                            }
                            System.IO.StreamWriter oWriter = File.CreateText(PathFile);
                            oWriter.Write
                            (
                                etyVOC.TicketNumber.Trim() + "|" +
                                etySA.new_SerialNumber.Trim() + "|" +
                                etySA.CreatedOn.Value.ToString("yyyyMMdd")
                            );
                            oWriter.Close();
                            oWriter.Dispose();
                        }
                        else
                        {
                            throw new Exception("Not found VOC Number!");
                        }

                        string connectionSQL = ConfigurationManager.AppSettings["connectionSQL"];
                        using (SqlConnection cn = new SqlConnection(connectionSQL))
                        {
                            cn.Open();
                            string chechingStatus = "INSERT";

                            #region Checking Existing Data

                            chechingStatus = "INSERT";
                            using (SqlCommand cmCecking = cn.CreateCommand())
                            {
                                cmCecking.CommandType = CommandType.Text;
                                cmCecking.CommandText = "SELECT * FROM WorkOrder WHERE VOCNumber='" + etySA.RegardingObjectId.Id.ToString() + "' AND SerialNumber='" + etySA.new_SerialNumber.ToString() + "'";
                                using (SqlDataReader dr = cmCecking.ExecuteReader())
                                {
                                    if (dr.Read()) { chechingStatus = "UPDATE"; }
                                }
                            }

                            #endregion

                            SqlTransaction tr = cn.BeginTransaction();
                            try
                            {
                                using (SqlCommand cm = tr.Connection.CreateCommand())
                                {
                                    cm.Transaction = tr;
                                    cm.CommandType = CommandType.Text;
                                    if (chechingStatus == "INSERT")
                                    {
                                        cm.CommandText = "INSERT INTO WorkOrder " +
                                            "(VOCNumber,SerialNumber,CreatedOn)" +
                                            "VALUES('" + etyVOC.TicketNumber.ToString() + "','" + etySA.new_SerialNumber + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                        cm.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                    }

                                };
                                tr.Commit();
                            }
                            catch (SqlException ex)
                            {
                                throw new Exception(ex.ErrorCode + " => " + ex.Message, ex);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message, ex);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Not found VOC!");
                    }

                    #endregion
                }
                else
                {
                    throw new Exception("Not found Service Activity / Serial Number!");
                }
                Response.Write("<script type='text/javascript'>alert('Service Activity has been send to SAP'); self.close();</script>");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            crm.ClearChanges();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string WO_Id = Request.QueryString["id"];
            //WO_Id = "{BD4C52E7-1A48-E211-90D5-005056924533}";
            try
            {
                if (IsPostBack == false)
                {
                    Guid id_req = new Guid(WO_Id);
                    this.processWOToSAP(id_req);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}