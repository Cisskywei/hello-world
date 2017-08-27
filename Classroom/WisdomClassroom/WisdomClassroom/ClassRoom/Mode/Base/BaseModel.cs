using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class BaseModel
    {
        public virtual void InitModel(params Object[] args)
        {
        }

        // ****************************************   对model 所属者提供的调用接口   ********************************************//
        public virtual void CheckOperationHold<T>(Int64 userid, Int64 objecid, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckOperationRelease<T>(Int64 userid, Int64 objecid, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckSyncCommond<T>(Int64 userid, string typ, string commond, T extrainfor, Object[] param = null)
        {

        }

        public virtual void CheckChangeObjectAllOnce<T>(Int64 userid, Hashtable clientallonce, T extrainfor, Object[] param)
        {

        }

        public virtual void CheckSyncClient<T>(T extrainfor, Object[] param)
        {

        }

        // 玩家离开
        public virtual void PlayerLeave(int userid)
        {

        }

        // ****************************************   对model 控制相关   ********************************************//
        public virtual void StartSynclient()
        {

        }

        public virtual void StopSynclient()
        {

        }

        public virtual void ClearData()
        {

        }

    }
}
