using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_annotation
    {
        #region Constants
        private const string _classname = "BL_annotation";
        private const int _depth = 1;
        private const int tss_sopartheaderobjecttypecode = 10203;
        private const string excelLoc = @"D:\Temp\";
        #endregion

        #region Depedencies
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        #endregion

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            Application excel = null;
            Workbooks wbs = null;
            Workbook wb = null;
            Worksheet excelSheet = null;
            string filename = string.Empty;
            string trace = string.Empty;

            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == "annotation")
                {
                    var context = new OrganizationServiceContext(organizationService);
                    var annotation = (from c in context.CreateQuery("annotation")
                                      where c.GetAttributeValue<Guid>("annotationid") == entity.Id
                                      where c.GetAttributeValue<int>("objecttypecode") == tss_sopartheaderobjecttypecode
                                      select c).ToList();

                    if (annotation.Count > 0)
                    {
                        if (annotation[0].GetAttributeValue<string>("filename").Contains("xls"))
                        {
                            filename = annotation[0].GetAttributeValue<string>("filename");
                            byte[] fileContent = Convert.FromBase64String(annotation[0].Attributes["documentbody"].ToString());
                            File.WriteAllBytes(excelLoc + filename, fileContent);

                            trace += excelLoc + filename + " created" + Environment.NewLine;
                            excel = new Application();
                            wbs = excel.Workbooks;
                            wb = wbs.Open(excelLoc + filename);
                            excelSheet = wb.ActiveSheet;
                            int maxRow = excelSheet.UsedRange.Rows.Count;

                            trace += "Read " + maxRow.ToString() + " rows"+ Environment.NewLine;
                            for (int rCnt = 2; rCnt <= maxRow; rCnt++)
                            {
                                if (excelSheet.Cells[rCnt, 1].Value != null)
                                {
                                    string masterpartname = excelSheet.Cells[rCnt, 1].Value.toString();
                                    var masterpart = (from c in context.CreateQuery("trs_masterpart")
                                                      where c.GetAttributeValue<string>("trs_name") == masterpartname
                                                      select c).ToList();

                                    if (masterpart.Count > 0)
                                    {
                                        Entity ent = new Entity("tss_sopartlines");
                                        ent.Attributes["tss_partnumber"] = new EntityReference("trs_masterpart", masterpart[0].GetAttributeValue<Guid>("trs_masterpartid"));

                                        if (excelSheet.Cells[rCnt, 2].Value != null)
                                        {
                                            if (excelSheet.Cells[rCnt, 2].Value.toString().ToLower() == "yes")
                                            {
                                                ent.Attributes["tss_isinterchange"] = true;

                                                if (excelSheet.Cells[rCnt, 3].Value != null)
                                                {
                                                    string partinterchangename = excelSheet.Cells[rCnt, 3].Value.toString();
                                                    var partinterchange = (from c in context.CreateQuery("tss_partmasterlinesinterchange")
                                                                           where c.GetAttributeValue<string>("tss_partnumber") == partinterchangename
                                                                           where c.GetAttributeValue<EntityReference>("tss_partmasterid").Id == masterpart[0].GetAttributeValue<Guid>("trs_masterpartid")
                                                                           select c).ToList();

                                                    if (partinterchange.Count > 0)
                                                    {
                                                        ent.Attributes["tss_partnumberinterchange"] = new EntityReference("tss_partmasterlinesinterchange", partinterchange[0].GetAttributeValue<Guid>("tss_partmasterlinesinterchangeid"));
                                                    }
                                                }
                                            }
                                            else if (excelSheet.Cells[rCnt, 2].Value.toString().ToLower() == "no") ent.Attributes["tss_isinterchange"] = false;
                                        }
                                        else ent.Attributes["tss_isinterchange"] = false;

                                        if (excelSheet.Cells[rCnt, 4].Value != null)
                                        {
                                            ent.Attributes["tss_qtyrequest"] = Convert.ToInt32(excelSheet.Cells[rCnt, 4].Value);
                                        }

                                        organizationService.Create(ent);
                                    }
                                }
                            }

                            if (wb != null)
                            {
                                wb.Close();
                                Marshal.ReleaseComObject(wb);
                            }
                            if (wbs != null)
                            {
                                wbs.Close();
                                Marshal.ReleaseComObject(wbs);
                            }
                            if (excel != null)
                            {
                                excel.Quit();
                                Marshal.ReleaseComObject(excel);
                            }

                            //File.Delete(excelLoc + filename);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (wb != null)
                {
                    wb.Close();
                    Marshal.ReleaseComObject(wb);
                }
                if (wbs != null)
                {
                    wbs.Close();
                    Marshal.ReleaseComObject(wbs);
                }
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }

                //if (filename != string.Empty)
                //{
                //    File.Delete(excelLoc + filename);
                //}

                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation : " + ex.Message.ToString() + trace);
            }
        }
    }
}