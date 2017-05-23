using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class BaseModelClass : BaseModelInterface
    {
        public Enums.ModelEnums model = Enums.ModelEnums.None;
        public bool IsUsed = false;

        public ArrayList limiteduuid = new ArrayList();
        // 
        public void InitModel(ArrayList uuids)
        {
            try
            {
                this.limiteduuid.Clear();
                this.limiteduuid = uuids.Clone() as ArrayList;
            }
            catch
            {

            }

        }

        public void AddUuid(string uuid)
        {
            try
            {
                if (!this.limiteduuid.Contains(uuid))
                {
                    this.limiteduuid.Add(uuid);
                }
            }
            catch
            {

            }
        }

        public void RemoveUuid(string uuid)
        {
            try
            {
                this.limiteduuid.Remove(uuid);
            }
            catch
            {

            }
        }

        // ****************************************   对model 所属者提供的调用接口   ********************************************//
        public virtual void CheckOperationHold<T>(string token, string objectname, string uuid, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckOperationRelease<T>(string token, string objectname, string uuid, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckSyncCommond<T>(string typ, string commond, string name, string objectname, string uuid, T extrainfor, Object[] param = null)
        {

        }

        public virtual void CheckChangeObjectAllOnce<T>(string token, Hashtable clientallonce, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckSyncClient<T>(T extrainfor, Object[] param)
        {

        }

        // ***************************************    以上与BaseRoomInterface对应    ***********************************************//

        //------------------------------------------  model 本身选择性实现的接口  ---------------------------------------------------//

        public virtual void CollaborationMult<T>(string fromuuid, ArrayList touuids, T msg)
        {
        }

        public virtual void CollaborationOne2One<T>(string fromuuid, string touuid, T msg)
        {
        }

        public virtual void SyncMult3DInfor<T>(string fromuuid, ArrayList touuids, T msg)
        {
        }

        public virtual void SyncMultCommond<T>(string fromuuid, ArrayList touuids, T msg)
        {
        }

        public virtual void SyncOne3DInfor<T>(string fromuuid, string touuid, T msg)
        {
        }

        public virtual void SyncOneCommond<T>(string fromuuid, string touuid, T msg)
        {
        }
    }
}
