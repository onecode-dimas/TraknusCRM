using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidService.Helper
{
    public class MTARGenerator
    {
        #region Constants
        private const string _classname = "MTARGenerator";
        #endregion

        #region Depedencies
        DL_activityparty _DL_activityparty = new DL_activityparty();
        DL_trs_mtar _DL_trs_mtar = new DL_trs_mtar();
        #endregion

        #region Properties
        private EntityCollection _mechanicList;
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void GenerateMTARforAllMechanic(IOrganizationService organizationService
            , Guid woId
            , int mtarStatus
            , bool workshop
            , bool sdh
            , decimal longitude = Configuration.TNGPSLongitude
            , decimal latitude = Configuration.TNGPSLatitude
        )
        {
            try
            {
                FMobile _fMobile = new FMobile(organizationService);

                QueryExpression queryExpression = new QueryExpression(_DL_activityparty.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                FilterExpression filterExpression = queryExpression.Criteria.AddFilter(LogicalOperator.And);
                filterExpression.AddCondition("activityid", ConditionOperator.Equal, woId);
                filterExpression.AddCondition("participationtypemask", ConditionOperator.Equal, 10);
                filterExpression.AddCondition("partyobjecttypecode", ConditionOperator.Equal, 4000);

                EntityCollection entityCollection = _DL_activityparty.Select(organizationService, queryExpression);
                foreach (Entity entity in entityCollection.Entities)
                {
                    _DL_trs_mtar = new DL_trs_mtar();
                    _DL_trs_mtar.trs_name = _fMobile.ConvertMTARtoWords(mtarStatus);
                    _DL_trs_mtar.trs_workorder = woId;
                    _DL_trs_mtar.trs_mechanic = entity.GetAttributeValue<EntityReference>("partyid").Id;
                    _DL_trs_mtar.trs_mtarstatus = mtarStatus;
                    switch (mtarStatus)
                    {
                        case Configuration.MTAR_Hold:
                            if (sdh)
                                _DL_trs_mtar.trs_statusremarks = "Hold by SDH";
                            else
                                _DL_trs_mtar.trs_statusremarks = "Hold by Mechanic Leader";
                            break;
                        case Configuration.MTAR_Resume:
                            if (sdh)
                                _DL_trs_mtar.trs_statusremarks = "Unhold by SDH";
                            else
                                _DL_trs_mtar.trs_statusremarks = "Unhold by Mechanic Leader";
                            break;
                    }
                    _DL_trs_mtar.trs_longitude = longitude;
                    _DL_trs_mtar.trs_latitude = latitude;
                    _DL_trs_mtar.trs_automatictime = DateTime.Now;
                    _DL_trs_mtar.trs_frommobile = false;
                    _DL_trs_mtar.trs_workshop = workshop;
                    _DL_trs_mtar.trs_updatewostatus = true;
                    _DL_trs_mtar.Insert(organizationService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CreateMTAR : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
