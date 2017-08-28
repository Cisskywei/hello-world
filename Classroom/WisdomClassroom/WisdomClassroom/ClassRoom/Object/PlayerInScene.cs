using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class PlayerInScene
    {
        public int selfid = -1;
        public int roomid = -1;
        public int teamid = -1;

        public string uuid = string.Empty;

        public Enums.PermissionEnum permission = Enums.PermissionEnum.Student;
        public Enums.CharacterStatus status = Enums.CharacterStatus.Ordinary;

        // 三维场景数据
        public Hashtable handhead;
        public bool _ischange = false;

        // 初始化自己
        public void Init(UserInfor playerinfor, int classid)
        {
            if (playerinfor == null)
            {
                return;
            }

            // 根据平台数据初始化自己
            this.uuid = playerinfor.uuid;
            this.selfid = playerinfor.selfid;
            this.teamid = playerinfor.groupid;
            this.roomid = classid;

            if (playerinfor.identity == "teacher")
            {
                this.permission = Enums.PermissionEnum.Teacher;
            }
            else
            {
                this.permission = Enums.PermissionEnum.Student;
            }

        }

        //三维场景数据解析
        public void Change3DInfor(Hashtable infor)
        {
            handhead = infor;

            _ischange = true;
        }

        public Hashtable Get3DInfor()
        {
            if(!_ischange)
            {
                return null;
            }

            _ischange = false;

            return handhead;
        }
    }
}
