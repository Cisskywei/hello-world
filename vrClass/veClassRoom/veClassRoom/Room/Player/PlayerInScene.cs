using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class PlayerInScene : BaseScenePlayer
    {
        public Enums.PermissionEnum permission = Enums.PermissionEnum.Student;
        public Enums.ModelEnums model = Enums.ModelEnums.Separate;
        public GroupInRoom group;

        public bool isleader;  //如果为true 则相应等级需要提升

        // 根据模式控制收发状态
        public bool isCanReceive = false;
        public bool isCanSend = false;
        public bool isCanOperate = false;   //是否可操作物体  用于请求释放操作权限

        public PlayerInScene(UserInfor playerinfor)
        {
            // 根据平台数据初始化自己

        }

        public PlayerInScene(UserInfor playerinfor, string token, string name, string uuid):base(token,name,uuid)
        {
            // 根据平台数据初始化自己

            this.isleader = playerinfor.isleader;

            if(playerinfor.isleader)
            {
                this.permission = Enums.PermissionEnum.Teacher;
            }
            else
            {
                this.permission = Enums.PermissionEnum.Student;
            }

        }

        // 接收自身模式改变  默认情况的收、发、操作权限
        public void ChangePlayerModel(Enums.ModelEnums tomodel)
        {
            switch(tomodel)
            {
                case Enums.ModelEnums.None:
                    isCanReceive = false;
                    isCanSend = false;
                    isCanOperate = false;
                    break;
                case Enums.ModelEnums.Separate:
                    isCanReceive = false;
                    isCanSend = false;
                    isCanOperate = true;
                    break;
                case Enums.ModelEnums.SynchronousOne:
                    isCanReceive = true;
                    isCanSend = false;
                    isCanOperate = false;
                    if (isleader)
                    {
                        if(group != null && group.leader != null)
                        {
                            if(token == group.leader.token)
                            {
                                isCanSend = true;
                                isCanOperate = true;
                            }
                        }
                    }
                    break;
                case Enums.ModelEnums.SynchronousMultiple:
                    isCanReceive = true;
                    isCanSend = false;
                    isCanOperate = false;
                    if (isleader)
                    {
                        if (group != null && group.leader != null)
                        {
                            if (token == group.leader.token)
                            {
                                isCanSend = true;
                                isCanOperate = true;
                            }
                        }
                        else   // 暂时没有小组  只为测试
                        {
                            isCanSend = true;
                            isCanOperate = true;
                        }
                    }
                    break;
                case Enums.ModelEnums.Collaboration:
                    isCanReceive = true;
                    isCanSend = true;
                    isCanOperate = true;
                    break;
                default:
                    break;
            }

            this.model = tomodel;
        }

        // 动态提升学生收、发、操作权限
        public void ChangePlayerCanReceive(Enums.PermissionEnum who, bool change)
        {
            if(who < this.permission)
            {
                return;
            }

            if(who == this.permission)
            {
                if(!this.isleader)
                {
                    return;
                }
                else
                {
                    if (group != null)
                    {
                        if (group.leader.token == this.token)
                        {
                            isCanReceive = change;
                        }
                    }
                    else
                    {
                        isCanReceive = change;
                    }
                }
            }
            else
            {
                isCanReceive = change;
            }
        }

        public void ChangePlayerCanSend(Enums.PermissionEnum who, bool change)
        {
            if (who < this.permission)
            {
                return;
            }

            if (who == this.permission)
            {
                if (!this.isleader)
                {
                    return;
                }
                else
                {
                    if (group != null)
                    {
                        if (group.leader.token == this.token)
                        {
                            isCanSend = change;
                        }
                    }
                    else
                    {
                        isCanSend = change;
                    }
                }
            }
            else
            {
                isCanSend = change;
            }
        }

        public void ChangePlayerCanOperate(Enums.PermissionEnum who, bool change)
        {
            if (who < this.permission)
            {
                return;
            }

            if (who == this.permission)
            {
                if (!this.isleader)
                {
                    return;
                }
                else
                {
                    if (group != null)
                    {
                        if (group.leader.token == this.token)
                        {
                            isCanOperate = change;
                        }
                    }
                    else
                    {
                        isCanOperate = change;
                    }
                }
            }
            else
            {
                isCanOperate = change;
            }
        }

        // 接收自身权限改变
        public void ChangePlayerPermission(Enums.PermissionEnum who, Enums.PermissionEnum permission)
        {
            if(who < this.permission)
            {
                // 操作者 如果没有被操作者权限大 则无法操作    关于同权限级别的判定在上一层 判定， 同小组之间不可相互操作，  等于则可能是此小组leader
                return;
            }

            // 如果是提升权限至房间leader 则自动拥有操作权限
            if(group != null)
            {
                if(permission >= group.permission)
                {
                    isCanOperate = true;
                }
            }

            this.permission = permission;
        }
    }
}
