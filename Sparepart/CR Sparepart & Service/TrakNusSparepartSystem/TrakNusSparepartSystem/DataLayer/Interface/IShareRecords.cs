using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer.Interface
{
    public interface IShareRecords
    {
        void ShareRecord(Entity TargetEntity, Entity TargetShare);
        void UnShareRecord(Entity TargetEntity, Entity TargetShare);
        void ShareRecordReadOnly(Entity TargetEntity, Entity TargetShare);
        void UnShareAllRecords(Entity TargetEntity, Guid ownerid);
    }
}
