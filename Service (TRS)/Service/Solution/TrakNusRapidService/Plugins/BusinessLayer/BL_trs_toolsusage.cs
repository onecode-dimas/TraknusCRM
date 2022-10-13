using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_toolsusage
    {
        #region Constants
        private const string _classname = "BL_trs_toolsusage";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_toolsusage _DL_trs_toolsusage = new DL_trs_toolsusage();
        private DL_trs_toolsmaster _DL_trs_toolsmaster = new DL_trs_toolsmaster();
        #endregion

        #region Events
        public void updateToolsUsage(IOrganizationService service, Entity preEntityImage, Entity postEntityImage)
        {
            try
            {                
                // Check whether tools in borrow and changed to return
                if (postEntityImage.LogicalName == _DL_trs_toolsusage.EntityName)
                {
                    // Get tools master status
                    // Borrowed = 0 (false), Available = 1 (true)   
                    Guid toolsMasterId = ((EntityReference)postEntityImage["trs_toolsmaster"]).Id;
                    //Guid toolsMasterId = postEntityImage.Id;
                    Entity entity = _DL_trs_toolsmaster.Select(service, toolsMasterId);
                    bool toolsMasterStatus = (bool)entity.Attributes["trs_status"];

                    if (toolsMasterStatus)
                    {
                        //throw new InvalidPluginExecutionException("Tool is available");     
                        return;
                    }
                    else
                    {
                        if (preEntityImage.Attributes["trs_dateborrowed"] != null && postEntityImage.Attributes["trs_dateborrowed"] == null)
                        {
                            throw new InvalidPluginExecutionException("Cannot empty borrow date");
                        }
                        else if ((DateTime)preEntityImage.Attributes["trs_dateborrowed"] != (DateTime)postEntityImage.Attributes["trs_dateborrowed"])
                        {
                            throw new InvalidPluginExecutionException("Cannot change borrow date");
                        }
                        else if ((DateTime)postEntityImage.Attributes["trs_datereturned"] < (DateTime)postEntityImage.Attributes["trs_dateborrowed"])
                        {
                            throw new InvalidPluginExecutionException("Return date cannot be earlier than borrow date");
                        }
                        else if (postEntityImage.Attributes["trs_dateborrowed"] != null && postEntityImage.Attributes["trs_datereturned"] != null)
                        {
                             //Update tools master status
                            _DL_trs_toolsmaster.trs_status = true;
                            _DL_trs_toolsmaster.Update(service, toolsMasterId);
                        }                         
                        else
                        {
                            throw new InvalidPluginExecutionException("Borrow date or return date should be filled");
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
                throw new InvalidPluginExecutionException(_classname + ".updateToolsUsage : " + ex.Message.ToString());
            }
        }

        public void createToolsUsage(IOrganizationService service, Entity postEntityImage)
        {
            try
            {                
                // Check whether tools in borrow
                if (postEntityImage.LogicalName == _DL_trs_toolsusage.EntityName)
				{
                    // Get tools master status
                    // Borrowed = 0 (false), Available = 1 (true)
                    Guid toolsMasterId = ((EntityReference)postEntityImage["trs_toolsmaster"]).Id;
                    //Guid toolsMasterId = postEntityImage.Id;
                    Entity entity = _DL_trs_toolsmaster.Select(service, toolsMasterId);
                    bool toolsMasterStatus = (bool)entity.Attributes["trs_status"];

                    if (toolsMasterStatus)
                    {
                        // Update tools master status
                        _DL_trs_toolsmaster.trs_status = false;
                        _DL_trs_toolsmaster.Update(service, toolsMasterId); 
                    }
                	else				
					{
                        throw new InvalidPluginExecutionException("Tools status is borrowed. Cannot borrow tool in used");
					}
				}
                else
                {
                    return;
                }
			}
			catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".createToolsUsage : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
