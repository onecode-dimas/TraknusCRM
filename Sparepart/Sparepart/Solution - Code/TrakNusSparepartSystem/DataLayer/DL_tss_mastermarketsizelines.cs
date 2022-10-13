using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_mastermarketsizelines
    {
        #region Dependencies
        DL_tss_mastermarketsize _DL_tss_mastermarketsize = new DL_tss_mastermarketsize();
        #endregion

        #region Properties
        private string _classname = "DL_tss_mastermarketsizelines";

        private string _entityname = "tss_mastermarketsizelines";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Master Market Size Lines";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_mastermarketsizelinesid = false;
        private Guid _tss_mastermarketsizelinesid_value;
        public Guid tss_mastermarketsizelinesid
        {
            get { return _tss_mastermarketsizelinesid ? _tss_mastermarketsizelinesid_value : Guid.Empty; }
            set { _tss_mastermarketsizelinesid = true; _tss_mastermarketsizelinesid_value = value; }
        }

        private bool _tss_mastermarketsizeref = false;
        private EntityReference _tss_mastermarketsizeref_value;
        public Guid tss_mastermarketsizeref
        {
            get { return _tss_mastermarketsizeref ? _tss_mastermarketsizeref_value.Id : Guid.Empty; }
            set { _tss_mastermarketsizeref = true; _tss_mastermarketsizeref_value = new EntityReference(_DL_tss_mastermarketsize.EntityName, value); }
        }

        private bool _tss_hmpm = false;
        private int _tss_hmpm_value;
        public int tss_hmpm
        {
            get { return _tss_hmpm ? _tss_hmpm_value : int.MinValue; }
            set { _tss_hmpm = true; _tss_hmpm_value = value; }
        }

        private bool _tss_hmconsumppm = false;
        private int _tss_hmconsumppm_value;
        public int tss_hmconsumppm
        {
            get { return _tss_hmconsumppm ? _tss_hmconsumppm_value : int.MinValue; }
            set { _tss_hmconsumppm = true; _tss_hmconsumppm_value = value; }
        }

        private bool _tss_forecastpmdate = false;
        private int _tss_forecastpmdate_value;
        public int tss_forecastpmdate
        {
            get { return _tss_forecastpmdate ? _tss_forecastpmdate_value : int.MinValue; }
            set { _tss_forecastpmdate = true; _tss_forecastpmdate_value = value; }
        }

        private bool _tss_estimatedpmdate = false;
        private DateTime _tss_estimatedpmdate_value;
        public DateTime tss_estimatedpmdate
        {
            get { return _tss_estimatedpmdate ? _tss_estimatedpmdate_value : DateTime.MinValue; }
            set { _tss_estimatedpmdate = true; _tss_estimatedpmdate_value = value.ToLocalTime(); }
        }

        private bool _tss_duedate = false;
        private DateTime _tss_duedate_value;
        public DateTime tss_duedate
        {
            get { return _tss_duedate ? _tss_duedate_value : DateTime.MinValue; }
            set { _tss_duedate = true; _tss_duedate_value = value.ToLocalTime(); }
        }

        private bool _tss_pm = false;
        private string _tss_pm_value;
        public string tss_pm
        {
            get { return _tss_pm ? _tss_pm_value : null; }
            set { _tss_pm = true; _tss_pm_value = value; }
        }

        private bool _tss_pmperiod = false;
        private int _tss_pmperiod_value;
        public int tss_pmperiod
        {
            get { return _tss_pmperiod ? _tss_pmperiod_value : int.MinValue; }
            set { _tss_pmperiod = true; _tss_pmperiod_value = value; }
        }

        private bool _tss_annualaging = false;
        private int _tss_annualaging_value;
        public int tss_annualaging
        {
            get { return _tss_annualaging ? _tss_annualaging_value : int.MinValue; }
            set { _tss_annualaging = true; _tss_annualaging_value = value; }
        }

        private bool _tss_aging = false;
        private int _tss_aging_value;
        public int tss_aging
        {
            get { return _tss_aging ? _tss_aging_value : int.MinValue; }
            set { _tss_aging = true; _tss_aging_value = value; }
        }

        //Option Set
        private bool _tss_methodcalculationused = false;
        private int _tss_methodcalculationused_value;
        public int tss_methodcalculationused
        {
            get { return _tss_methodcalculationused ? _tss_methodcalculationused_value : int.MinValue; }
            set { _tss_methodcalculationused = true; _tss_methodcalculationused_value = value; }
        }

        //Option Set
        private bool _tss_status = false;
        private int _tss_status_value;
        public int tss_status
        {
            get { return _tss_status ? _tss_status_value : int.MinValue; }
            set { _tss_status = true; _tss_status_value = value; }
        }

        //Option Set
        private bool _tss_statusreason = false;
        private int _tss_statusreason_value;
        public int tss_statusreason
        {
            get { return _tss_statusreason ? _tss_statusreason_value : int.MinValue; }
            set { _tss_statusreason = true; _tss_statusreason_value = value; }
        }
        #endregion
        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = organizationService.Retrieve(_entityname, id, new ColumnSet(true));
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public EntityCollection Select(IOrganizationService organizationService, QueryExpression queryExpression)
        {
            try
            {
                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                return entityCollection;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public Guid Insert(IOrganizationService organizationService)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                if (_tss_mastermarketsizeref) { entity.Attributes["tss_mastermarketsizeref"] = _tss_mastermarketsizeref_value; }
                if (_tss_hmpm) { entity.Attributes["tss_hmpm"] = _tss_hmpm_value; }
                if (_tss_hmconsumppm) { entity.Attributes["tss_hmconsumppm"] = _tss_hmconsumppm_value; }
                if (_tss_forecastpmdate) { entity.Attributes["tss_forecastpmdate"] = _tss_forecastpmdate_value; }
                if (_tss_estimatedpmdate) { entity.Attributes["tss_estimatedpmdate"] = _tss_estimatedpmdate_value; }
                if (_tss_duedate) { entity.Attributes["tss_duedate"] = _tss_duedate_value; }
                if (_tss_pm) { entity.Attributes["tss_pm"] = _tss_pm_value; }
                if (_tss_pmperiod) { entity.Attributes["tss_pmperiod"] = _tss_pmperiod_value; }
                if (_tss_annualaging) { entity.Attributes["tss_annualaging"] = _tss_annualaging_value; }
                if (_tss_aging) { entity.Attributes["tss_aging"] = _tss_aging_value; }
                if (_tss_methodcalculationused) { entity.Attributes["tss_methodcalculationused"] = new OptionSetValue(_tss_methodcalculationused_value); }
                if (_tss_status) { entity.Attributes["tss_status"] = new OptionSetValue(_tss_status_value); }
                if (_tss_statusreason) { entity.Attributes["tss_statusreason"] = new OptionSetValue(_tss_statusreason_value); }
                return tss_mastermarketsizelinesid = organizationService.Create(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }


        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_mastermarketsizeref) { entity.Attributes["tss_mastermarketsizeref"] = _tss_mastermarketsizeref_value; }
                if (_tss_hmpm) { entity.Attributes["tss_hmpm"] = _tss_hmpm_value; }
                if (_tss_hmconsumppm) { entity.Attributes["tss_hmconsumppm"] = _tss_hmconsumppm_value; }
                if (_tss_forecastpmdate) { entity.Attributes["tss_forecastpmdate"] = _tss_forecastpmdate_value; }
                if (_tss_estimatedpmdate) { entity.Attributes["tss_estimatedpmdate"] = _tss_estimatedpmdate_value; }
                if (_tss_duedate) { entity.Attributes["tss_duedate"] = _tss_duedate_value; }
                if (_tss_pm) { entity.Attributes["tss_pm"] = _tss_pm_value; }
                if (_tss_pmperiod) { entity.Attributes["tss_pmperiod"] = _tss_pmperiod_value; }
                if (_tss_annualaging) { entity.Attributes["tss_annualaging"] = _tss_annualaging_value; }
                if (_tss_aging) { entity.Attributes["tss_aging"] = _tss_aging_value; }
                if (_tss_methodcalculationused) { entity.Attributes["tss_methodcalculationused"] = new OptionSetValue(_tss_methodcalculationused_value); }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        public int GetNextPlanHM(int _nextplanhm)
        {
            string _parse = _nextplanhm.ToString();
            int _length = _parse.Length;

            int _getfrommatrix = 0;

            if (_length >= 3 && _parse.Substring(_length - 3, 3) == "250")
                _getfrommatrix = 250;
            else if (_length >= 3 && _parse.Substring(_length - 3, 3) == "500")
                _getfrommatrix = 250;
            else if (_length >= 3 && _parse.Substring(_length - 3, 3) == "750")
                _getfrommatrix = 250;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "1000")
                _getfrommatrix = 1000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "2000")
                _getfrommatrix = 2000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "3000")
                _getfrommatrix = 1000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "4000")
                _getfrommatrix = 2000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "5000")
                _getfrommatrix = 1000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "6000")
                _getfrommatrix = 2000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "7000")
                _getfrommatrix = 1000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "8000")
                _getfrommatrix = 2000;
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "9000")
                _getfrommatrix = 1000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "10000")
                _getfrommatrix = 2000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "20000")
                _getfrommatrix = 2000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "30000")
                _getfrommatrix = 2000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "40000")
                _getfrommatrix = 2000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "50000")
                _getfrommatrix = 2000;
            else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "60000")
                _getfrommatrix = 2000;

            return _getfrommatrix;
        }

        public int GetSubstring(int _nextplanhm)
        {
            string _parse = _nextplanhm.ToString();
            int _length = _parse.Length;

            if (_length >= 3 && _parse.Substring(_length - 3, 3) != "000")
                _parse = _parse.Substring(_length - 3, 3);
            else if (_length >= 4 && _parse.Substring(_length - 4, 4) != "0000")
                _parse = _parse.Substring(_length - 4, 4);
            else if (_length >= 5 && _parse.Substring(_length - 4, 4) != "00000")
                _parse = _parse.Substring(_length - 5, 5);
            else
                _parse = "0";

            return int.Parse(_parse);
        }
        public int GetPMType(IOrganizationService organizationService, Guid _productmaster, int _nextplanhm)
        {
            Entity _product = organizationService.Retrieve("product", _productmaster, new ColumnSet(true));
            int _getfrommatrix = GetSubstring(_nextplanhm);

            QueryExpression query = new QueryExpression("tss_matrixperiodmaintenancetype");

            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("tss_unitgroup", ConditionOperator.Equal, _product.GetAttributeValue<EntityReference>("defaultuomscheduleid").Id);
            query.Criteria.AddCondition("tss_nextplanhm", ConditionOperator.Equal, _getfrommatrix);

            EntityCollection _matrixperiodmaintenancetypecollection = organizationService.RetrieveMultiple(query);

            if (_matrixperiodmaintenancetypecollection.Entities.Count() > 0)
            {
                Entity _matrixperiodmaintenancetype = _matrixperiodmaintenancetypecollection.Entities[0];
                int _typepm = _matrixperiodmaintenancetype.GetAttributeValue<int>("tss_typepm");

                return _typepm;
            }
            else
            {
                return 0;
            }
        }

        //public int GetPMType(IOrganizationService organizationService, Guid _productmaster, int _nextplanhm)
        //{
        //    Entity _product = organizationService.Retrieve("product", _productmaster, new ColumnSet(true));
        //    string _parse = _nextplanhm.ToString();
        //    int _length = _parse.Length;
        //    int _getfrommatrix = 0;

        //    if (_length >= 3 && _parse.Substring(_length - 3, 3) == "250")
        //        _getfrommatrix = 250;
        //    else if (_length >= 3 && _parse.Substring(_length - 3, 3) == "500")
        //        _getfrommatrix = 250;
        //    else if (_length >= 3 && _parse.Substring(_length - 3, 3) == "750")
        //        _getfrommatrix = 250;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "1000")
        //        _getfrommatrix = 1000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "2000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "3000")
        //        _getfrommatrix = 1000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "4000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "5000")
        //        _getfrommatrix = 1000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "6000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "7000")
        //        _getfrommatrix = 1000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "8000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 4 && _parse.Substring(_length - 4, 4) == "9000")
        //        _getfrommatrix = 1000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "10000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "20000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "30000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "40000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "50000")
        //        _getfrommatrix = 2000;
        //    else if (_length >= 5 && _parse.Substring(_length - 5, 5) == "60000")
        //        _getfrommatrix = 2000;

        //    QueryExpression query = new QueryExpression("tss_matrixperiodmaintenancetype");

        //    query.ColumnSet = new ColumnSet(true);
        //    query.Criteria.AddCondition("tss_unitgroup", ConditionOperator.Equal, _product.GetAttributeValue<EntityReference>("defaultuomscheduleid").Id);
        //    query.Criteria.AddCondition("tss_nextplanhm", ConditionOperator.Equal, _getfrommatrix);
        //    //query.Orders.Add(new OrderExpression("rent_itemnumber", OrderType.Descending));

        //    EntityCollection _matrixperiodmaintenancetypecollection = organizationService.RetrieveMultiple(query);

        //    if (_matrixperiodmaintenancetypecollection.Entities.Count() > 0)
        //    {
        //        Entity _matrixperiodmaintenancetype = _matrixperiodmaintenancetypecollection.Entities[0];
        //        int _typepm = _matrixperiodmaintenancetype.GetAttributeValue<int>("tss_typepm");

        //        return _typepm;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //public int GetPMType(int HMPM)
        //{
        //    string parse = HMPM.ToString();
        //    int length = parse.Length;

        //    if (length >= 3 && parse.Substring(length - 3, 3) == "250")
        //        return 250;
        //    if (length >= 3 && parse.Substring(length - 3, 3) == "500")
        //        return 250;
        //    if (length >= 3 && parse.Substring(length - 3, 3) == "750")
        //        return 250;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "1000")
        //        return 1000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "2000")
        //        return 2000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "3000")
        //        return 1000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "4000")
        //        return 2000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "5000")
        //        return 1000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "6000")
        //        return 2000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "7000")
        //        return 1000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "8000")
        //        return 2000;
        //    if (length >= 4 && parse.Substring(length - 4, 4) == "9000")
        //        return 1000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "10000")
        //        return 2000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "20000")
        //        return 2000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "30000")
        //        return 2000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "40000")
        //        return 2000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "50000")
        //        return 2000;
        //    if (length >= 5 && parse.Substring(length - 5, 5) == "60000")
        //        return 2000;
        //    else
        //        return 0;
        //}
    }
}
