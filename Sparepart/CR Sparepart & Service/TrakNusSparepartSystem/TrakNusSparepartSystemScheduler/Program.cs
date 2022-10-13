using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.Helper;
using TrakNusSparepartSystemScheduler.Business_Layer;
using TrakNusSparepartSystemScheduler.Helper;
using Microsoft.SqlServer.Management.IntegrationServices;
using System.Data.SqlClient;
using Microsoft.SqlServer.Dts.Runtime;

namespace TrakNusSparepartSystemScheduler
{
    public interface iCalculate
    {
        int[] Ref { get; }
        void Cal();
    }
    public class Calculate : iCalculate
    {
        private int[] _ref;
        public int[] Ref
        {
            get
            {
                return _ref;

            }
        }

        public void Cal()
        {
            _ref = new int[12];
            for (int i = 0; i < 12; i++)
            {
                _ref[i] = i;
            }
        }
    }

    public interface IOperation
    {
        void Send();
    }

    public class LowLevelModule : IOperation
    {
        public LowLevelModule()
        {
            Initiate();
        }

        private void Initiate()
        {
            //do initiation before sending
        }

        public void Send()
        {
            //perform sending operation
        }
    }

    public class HighLevelModule
    {
        private readonly IOperation _operation;

        public HighLevelModule(IOperation operation)
        {
            _operation = operation;
        }

        public void Call()
        {
            _operation.Send();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string[] argsparam = { "/PE=6" };
            args = argsparam;
            DoJob(args);

            
        }

        private static void DoJob(string[] param)
        {
            BL_RecordHelper _BL_RecordHelper = new BL_RecordHelper();
            BL_SendNotification BL_SendNotification = new BL_SendNotification();
            if (param.Count() > 0)
            {

                foreach (var val in param)
                {
                    string[] paramsplit = val.Split('/');
                    foreach (var o in paramsplit)
                    {
                        string[] result = o.Split('=');
                        if (result.Count() > 1)
                        {

                            switch (result[1].Trim())
                            {
                                case "1":
                                    Console.WriteLine(string.Format("Execute Programm: {0}={1} [Send Notif Before Start Period Marketsize]", result[0], result[0]));
                                    BL_SendNotification.SendNotifBeforeStardPeriod(Helper.SendEmailNotif.BeforeStartMSPeriod);
                                    break;
                                case "2":
                                    Console.WriteLine(string.Format("Execute Programm: {0}={1} [Send Notif Before End Period Marketsize]", result[0], result[0]));
                                    BL_SendNotification.SendNotifBeforeStardPeriod(Helper.SendEmailNotif.BeforeEndMSPeriod);
                                    break;
                                case "3":
                                    Console.WriteLine(string.Format("Execute Programm: {0}={1} [Enable Revise Key Account]", result[0], result[0]));
                                    BL_SendNotification.SendNotifEnableButtonRevise();
                                    break;
                                case "4":
                                    Console.WriteLine(string.Format("Execute Programm: {0}={1} [Calculate Sales Actual]", result[0], result[0]));
                                    BL_CalculateSalesActual _BL_CalculateSalesActual = new BL_CalculateSalesActual();
                                    _BL_CalculateSalesActual.CalcualteSalesTarget();
                                    break;
                                case "5":
                                    Console.WriteLine(string.Format("Execute Programm: {0}={1} [Delete All Records]", result[0], result[0]));
                                    _BL_RecordHelper.PerformBulkDelete();
                                    break;
                                case "6":
                                    BL_IncentiveCalculation BL_IncentiveCalculation = new BL_IncentiveCalculation();
                                    BL_IncentiveCalculation.IncetiveCalculation();
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }

            }



        }
    }
}
