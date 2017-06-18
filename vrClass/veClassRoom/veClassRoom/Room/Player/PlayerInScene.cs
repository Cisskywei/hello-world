using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class PlayerInScene : BaseScenePlayer
    {
        public Enums.PermissionEnum permission = Enums.PermissionEnum.Student;
        public Enums.TeachingMode model = Enums.TeachingMode.WatchLearnModel_Sync;
        public GroupInRoom group;
        public int roomid;
        public int groupid = -1;
        public string groupname;

        public bool isleader;  //如果为true 则相应等级需要提升
        private bool _isbechoosed; // 指导模式下标记是否被选中的学生
        public bool isbechoosed
        {
            get
            {
                return _isbechoosed;
            }

            set
            {
                if(value)
                {
                    isCanOperate = true;
                    isCanSend = true;
                    isCanReceive = true;
                }

                _isbechoosed = value;
            }
        }
        public int selfid;

        // 根据模式控制收发状态
        public bool isCanReceive = false;
        public bool isCanSend = false;
        public bool isCanOperate = false;   //是否可操作物体  用于请求释放操作权限


        // 玩家模型同步数据
        public Structs.sTransform lefthand;
        public Structs.sTransform righthand;
        public Structs.sTransform head;
        private Hashtable handhead = null;// new Hashtable();

        public PlayerInScene(UserInfor playerinfor)
        {
            if(playerinfor == null)
            {
                return;
            }

            // 根据平台数据初始化自己
            this.token = playerinfor.access_token;
            this.name = playerinfor.user_name;
            this.uuid = playerinfor.uuid;
            this.selfid = playerinfor.selfid;
            this.groupid = playerinfor.groupid;
            this.groupname = playerinfor.groupname;

            this.isleader = playerinfor.identity == "teacher";

            if(this.isleader)
            {
                this.permission = Enums.PermissionEnum.Teacher;
            }
            else
            {
                this.permission = Enums.PermissionEnum.Student;
            }

        }

        public PlayerInScene(UserInfor playerinfor, int roomid)
        {
            if (playerinfor == null)
            {
                return;
            }

            // 根据平台数据初始化自己
            this.token = playerinfor.access_token;
            this.name = playerinfor.user_name;
            this.uuid = playerinfor.uuid;
            this.selfid = playerinfor.selfid;
            this.groupid = playerinfor.groupid;
            this.groupname = playerinfor.groupname;

            this.isleader = playerinfor.identity == "teacher";

            if (this.isleader)
            {
                this.permission = Enums.PermissionEnum.Teacher;
            }
            else
            {
                this.permission = Enums.PermissionEnum.Student;
            }

            this.roomid = roomid;

        }

        // 接收自身模式改变  默认情况的收、发、操作权限
        public void ChangePlayerModel(Enums.TeachingMode tomodel)
        {
            switch(tomodel)
            {
                case Enums.TeachingMode.WatchLearnModel_Async:
                case Enums.TeachingMode.WatchLearnModel_Sync:
                    isCanReceive = true;
                    if (isleader)
                    {
                        // 权限大于等于老师
                        if (this.permission >= Enums.PermissionEnum.Teacher)
                        {
                            isCanOperate = true;
                            isCanSend = true;
                        }
                    }
                    break;
                case Enums.TeachingMode.GuidanceMode_Personal:
                    isCanReceive = true;
                    isCanSend = false;
                    isCanOperate = false;
                    break;
                case Enums.TeachingMode.GuidanceMode_Group:
                    isCanReceive = true;
                    isCanSend = false;
                    isCanOperate = false;
                    if (isleader)
                    {
                        if(group != null && group.leader != null)
                        {
                            if(token == group.leader.token)
                            {
                                //isCanSend = true;
                                //isCanOperate = true;
                            }
                        }

                        // 权限大于等于老师
                        if (this.permission >= Enums.PermissionEnum.Teacher)
                        {
                            isCanOperate = true;
                            isCanSend = true;
                        }
                    }
                    break;
                case Enums.TeachingMode.SelfTrain_Personal:
                    isCanReceive = false;
                    isCanSend = false;
                    isCanOperate = true;
                    break;
                case Enums.TeachingMode.SelfTrain_Group:
                case Enums.TeachingMode.SelfTrain_All:
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


        // 玩家3维数据初始化 头 左手 右手
        public void InitPlayerHeadHand(Hashtable data)
        {
            if(data == null)
            {
                return;
            }

            this.handhead = data;

            changeorno = true;
        }

        public override Hashtable Serialize()
        {
            // 序列化头部手的数据
            //           this.serializeHandHead();

            changeorno = false;

            return this.handhead;
        }

        public override void Conversion(Hashtable t)
        {
            //           this.conversionHandHead(t);
            this.handhead = t;

            changeorno = true;
        }

        private void initHandHead()
        {
            handhead.Add("headposx", this.head.pos.x);
            handhead.Add("headposy", this.head.pos.y);
            handhead.Add("headposz", this.head.pos.z);
            handhead.Add("headrotx", this.head.rot.x);
            handhead.Add("headroty", this.head.rot.y);
            handhead.Add("headrotz", this.head.rot.z);
            handhead.Add("headrotw", this.head.rot.w);
            handhead.Add("headscalx", this.head.scal.x);
            handhead.Add("headscaly", this.head.scal.y);
            handhead.Add("headscalz", this.head.scal.z);

            handhead.Add("lhandposx", this.lefthand.pos.x);
            handhead.Add("lhandposy", this.lefthand.pos.y);
            handhead.Add("lhandposz", this.lefthand.pos.z);
            handhead.Add("lhandrotx", this.lefthand.rot.x);
            handhead.Add("lhandroty", this.lefthand.rot.y);
            handhead.Add("lhandrotz", this.lefthand.rot.z);
            handhead.Add("lhandrotw", this.lefthand.rot.w);
            handhead.Add("lhandscalx", this.lefthand.scal.x);
            handhead.Add("lhandscaly", this.lefthand.scal.y);
            handhead.Add("lhandscalz", this.lefthand.scal.z);

            handhead.Add("rhandposx", this.righthand.pos.x);
            handhead.Add("rhandposy", this.righthand.pos.y);
            handhead.Add("rhandposz", this.righthand.pos.z);
            handhead.Add("rhandrotx", this.righthand.rot.x);
            handhead.Add("rhandroty", this.righthand.rot.y);
            handhead.Add("rhandrotz", this.righthand.rot.z);
            handhead.Add("rhandrotw", this.righthand.rot.w);
            handhead.Add("rhandscalx", this.righthand.scal.x);
            handhead.Add("rhandscaly", this.righthand.scal.y);
            handhead.Add("rhandscalz", this.righthand.scal.z);
        }

        private void conversionHandHead(Hashtable t)
        {
            if(t["headposx"]!=null)
            {
                this.head.pos.x = (float)Convert.ToDouble(t["headposx"]);
            }
            if(t["headposy"] !=null)
            {
                this.head.pos.y = (float)Convert.ToDouble(t["headposy"]);
            }
            if (t["headposz"] != null)
            {
                this.head.pos.z = (float)Convert.ToDouble(t["headposz"]);
            }
            if (t["headrotx"] != null)
            {
                this.head.rot.x = (float)Convert.ToDouble(t["headrotx"]);
            }
            if (t["headroty"] != null)
            {
                this.head.rot.y = (float)Convert.ToDouble(t["headroty"]);
            }
            if (t["headrotz"] != null)
            {
                this.head.rot.z = (float)Convert.ToDouble(t["headrotz"]);
            }
            if (t["headrotw"] != null)
            {
                this.head.rot.w = (float)Convert.ToDouble(t["headrotw"]);
            }
            if (t["headscalx"] != null)
            {
                this.head.scal.x = (float)Convert.ToDouble(t["headscalx"]);
            }
            if (t["headscaly"] != null)
            {
                this.head.scal.y = (float)Convert.ToDouble(t["headscaly"]);
            }
            if (t["headscalz"] != null)
            {
                this.head.scal.z = (float)Convert.ToDouble(t["headscalz"]);
            }

            if (t["lhandposx"] != null)
            {
                this.lefthand.pos.x = (float)Convert.ToDouble(t["lhandposx"]);
            }
            if (t["lhandposy"] != null)
            {
                this.lefthand.pos.y = (float)Convert.ToDouble(t["lhandposy"]);
            }
            if (t["lhandposz"] != null)
            {
                this.lefthand.pos.z = (float)Convert.ToDouble(t["lhandposz"]);
            }
            if (t["lhandrotx"] != null)
            {
                this.lefthand.rot.x = (float)Convert.ToDouble(t["lhandrotx"]);
            }
            if (t["lhandroty"] != null)
            {
                this.lefthand.rot.y = (float)Convert.ToDouble(t["lhandroty"]);
            }
            if (t["lhandrotz"] != null)
            {
                this.lefthand.rot.z = (float)Convert.ToDouble(t["lhandrotz"]);
            }
            if (t["lhandrotw"] != null)
            {
                this.lefthand.rot.w = (float)Convert.ToDouble(t["lhandrotw"]);
            }
            if (t["lhandscalx"] != null)
            {
                this.lefthand.scal.x = (float)Convert.ToDouble(t["lhandscalx"]);
            }
            if (t["lhandscaly"] != null)
            {
                this.lefthand.scal.y = (float)Convert.ToDouble(t["lhandscaly"]);
            }
            if (t["lhandscalz"] != null)
            {
                this.lefthand.scal.z = (float)Convert.ToDouble(t["lhandscalz"]);
            }

            if (t["rhandposx"] != null)
            {
                this.righthand.pos.x = (float)Convert.ToDouble(t["rhandposx"]);
            }
            if (t["rhandposy"] != null)
            {
                this.righthand.pos.y = (float)Convert.ToDouble(t["rhandposy"]);
            }
            if (t["rhandposz"] != null)
            {
                this.righthand.pos.z = (float)Convert.ToDouble(t["rhandposz"]);
            }
            if (t["rhandrotx"] != null)
            {
                this.righthand.rot.x = (float)Convert.ToDouble(t["rhandrotx"]);
            }
            if (t["rhandroty"] != null)
            {
                this.righthand.rot.y = (float)Convert.ToDouble(t["rhandroty"]);
            }
            if (t["rhandrotz"] != null)
            {
                this.righthand.rot.z = (float)Convert.ToDouble(t["rhandrotz"]);
            }
            if (t["rhandrotw"] != null)
            {
                this.righthand.rot.w = (float)Convert.ToDouble(t["rhandrotw"]);
            }
            if (t["rhandscalx"] != null)
            {
                this.righthand.scal.x = (float)Convert.ToDouble(t["rhandscalx"]);
            }
            if (t["rhandscaly"] != null)
            {
                this.righthand.scal.y = (float)Convert.ToDouble(t["rhandscaly"]);
            }
            if (t["rhandscalz"] != null)
            {
                this.righthand.scal.z = (float)Convert.ToDouble(t["rhandscalz"]);
            }
        }

        private void serializeHandHead()
        {
            if(handhead == null)
            {
                handhead = new Hashtable();
            }

            handhead["headposx"] = this.head.pos.x;
            handhead["headposy"] = this.head.pos.y;
            handhead["headposz"] = this.head.pos.z;
            handhead["headrotx"] = this.head.rot.x;
            handhead["headroty"] = this.head.rot.y;
            handhead["headrotz"] = this.head.rot.z;
            handhead["headrotw"] = this.head.rot.w;
            handhead["headscalx"] = this.head.scal.x;
            handhead["headscaly"] = this.head.scal.y;
            handhead["headscalz"] = this.head.scal.z;

            handhead["lhandposx"] = this.lefthand.pos.x;
            handhead["lhandposy"] = this.lefthand.pos.y;
            handhead["lhandposz"] = this.lefthand.pos.z;
            handhead["lhandrotx"] = this.lefthand.rot.x;
            handhead["lhandroty"] = this.lefthand.rot.y;
            handhead["lhandrotz"] = this.lefthand.rot.z;
            handhead["lhandrotw"] = this.lefthand.rot.w;
            handhead["lhandscalx"] = this.lefthand.scal.x;
            handhead["lhandscaly"] = this.lefthand.scal.y;
            handhead["lhandscalz"] = this.lefthand.scal.z;

            handhead["rhandposx"] = this.righthand.pos.x;
            handhead["rhandposy"] = this.righthand.pos.y;
            handhead["rhandposz"] = this.righthand.pos.z;
            handhead["rhandrotx"] = this.righthand.rot.x;
            handhead["rhandroty"] = this.righthand.rot.y;
            handhead["rhandrotz"] = this.righthand.rot.z;
            handhead["rhandrotw"] = this.righthand.rot.w;
            handhead["rhandscalx"] = this.righthand.scal.x;
            handhead["rhandscaly"] = this.righthand.scal.y;
            handhead["rhandscalz"] = this.righthand.scal.z;
        }

    }
}
