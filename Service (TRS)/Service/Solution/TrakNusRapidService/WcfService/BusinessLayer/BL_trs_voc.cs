using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;

using TrakNusRapidService.DataLayer;

namespace TrakNusRapidServiceWcfService.BusinessLayer
{
    public class BL_trs_voc
    {
        #region Constants
        private const string _classname = "BL_trs_workorderpartrecommendation";
        #endregion

        #region Dependencies
        private DL_trs_voc _DL_trs_voc = new DL_trs_voc();
        #endregion

        #region Privates
        #endregion

        #region Publics
        public void RecordVOC(IOrganizationService organizationService, Guid id
            , string contact, string topic, string customerName, string vocType, string comment
            , DateTime modifiedOn, DateTime syncTime, out Guid newId)
        {
            try
            {
                newId = Guid.Empty;
                if (id == Guid.Empty)
                {
                    _DL_trs_voc = new DL_trs_voc();
                    _DL_trs_voc.trs_contact = contact;
                    _DL_trs_voc.trs_topic = topic;
                    _DL_trs_voc.trs_customername = customerName;
                    _DL_trs_voc.trs_voctype = vocType;
                    _DL_trs_voc.trs_comment = comment;
                    newId = _DL_trs_voc.Insert(organizationService);
                }
                else
                {
                    Entity entity = _DL_trs_voc.Select(organizationService, id);
                    if ((DateTime)entity.Attributes["modifiedon"] < modifiedOn)
                    {
                        _DL_trs_voc = new DL_trs_voc();
                        _DL_trs_voc.trs_contact = contact;
                        _DL_trs_voc.trs_topic = topic;
                        _DL_trs_voc.trs_customername = customerName;
                        _DL_trs_voc.trs_voctype = vocType;
                        _DL_trs_voc.trs_comment = comment;
                        _DL_trs_voc.Update(organizationService, id);
                    }
                    else
                    {
                        throw new Exception("CRM more update.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".RecordVOC : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}