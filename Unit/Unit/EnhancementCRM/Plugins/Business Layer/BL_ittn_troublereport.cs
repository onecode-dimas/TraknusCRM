using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_ittn_troublereport
    {
        #region CONSTANT
        private const string _classname = "BL_ittn_troublereport";
        private const string _entityname_troublereport = "ittn_troublereport";
        #endregion

        #region GENERATE RUNNING NUMBER
        public void GenerateName(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                if (_entity.LogicalName == _entityname_troublereport)
                {
                    DateTime _createdon = DateTime.Now.ToLocalTime();

                    BL_runningnumber _runningnumber = new BL_runningnumber();
                    string _newrunningnumber = _runningnumber.GenerateNewRunningNumber(_organizationservice, _context, _entityname_troublereport, _createdon);

                    _entity.Attributes["ittn_trnumber"] = _newrunningnumber;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GenerateName : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
