using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class BaseSceneObject
    {
        public Structs.sVector3 position;
        public Structs.sVector4 rotation;
        public Structs.sVector3 scale;

        public Structs.sPhysicalProperty physical;

        public int state;  // 具体数值定义有客户端统一  服务器只是保存 转发  不具有实际解析意义
        public string locker;
        public bool locked;
        public Enums.PermissionEnum lockpermission = Enums.PermissionEnum.None;
        public bool changeorno;

        public float frametime;

        public string name;

        // 序列化的信息
        public Hashtable serializedata = new Hashtable();

        public BaseSceneObject()
        {
            position = new Structs.sVector3();
            rotation = new Structs.sVector4();
            scale = new Structs.sVector3();
            physical = new Structs.sPhysicalProperty();

            locker = null;
            locked = false;
            changeorno = false;

            serializedata.Add("posx", position.x);
            serializedata.Add("posy", position.y);
            serializedata.Add("posz", position.z);

            serializedata.Add("rotx", rotation.x);
            serializedata.Add("roty", rotation.y);
            serializedata.Add("rotz", rotation.z);
            serializedata.Add("rotw", rotation.w);

            serializedata.Add("scalx", scale.x);
            serializedata.Add("scaly", scale.y);
            serializedata.Add("scalz", scale.z);

            serializedata.Add("phycg", physical.useGravity);
            serializedata.Add("phyck", physical.isKinematic);

            serializedata.Add("state", state);

            serializedata.Add("frametime", frametime);
        }

        public void changeposition(float x, float y, float z)
        {
            position.x = x;
            position.y = y;
            position.z = z;

            changeorno = true;
        }

        public void changerotation(float x, float y, float z, float w)
        {
            rotation.x = x;
            rotation.y = y;
            rotation.z = z;
            rotation.w = w;

            changeorno = true;
        }

        public void changescale(float x, float y, float z)
        {
            scale.x = x;
            scale.y = y;
            scale.z = z;

            changeorno = true;
        }

        public void changephysical(bool g, bool k)
        {
            physical.useGravity = g;
            physical.isKinematic = k;

            changeorno = true;
        }

        public void changestate(int state)
        {
            this.state = state;

            changeorno = true;
        }

        // 序列化自己
        public virtual Hashtable Serialize()
        {
            serializedata["posx"] = position.x;
            serializedata["posy"] = position.y;
            serializedata["posz"] = position.z;

            serializedata["rotx"] = rotation.x;
            serializedata["roty"] = rotation.y;
            serializedata["rotz"] = rotation.z;
            serializedata["rotw"] = rotation.w;

            serializedata["scalx"] = scale.x;
            serializedata["scaly"] = scale.y;
            serializedata["scalz"] = scale.z;

            serializedata["phycg"] = physical.useGravity;
            serializedata["phyck"] = physical.isKinematic;

            serializedata["state"] = state;

            serializedata["frametime"] = frametime;

            changeorno = false;

            return serializedata;
        }

        //反序列化自己
        public virtual void Deserialization(Hashtable t)
        {
            if (t == null || t.Count <= 0)
            {
                return;
            }

            position.x = (float)Convert.ToDouble(t["posx"]);
            position.y = (float)Convert.ToDouble(t["posy"]);
            position.z = (float)Convert.ToDouble(t["posz"]);

            rotation.x = (float)Convert.ToDouble(t["rotx"]);
            rotation.y = (float)Convert.ToDouble(t["roty"]);
            rotation.z = (float)Convert.ToDouble(t["rotz"]);
            rotation.w = (float)Convert.ToDouble(t["rotw"]);

            scale.x = (float)Convert.ToDouble(t["scalx"]);
            scale.y = (float)Convert.ToDouble(t["scaly"]);
            scale.z = (float)Convert.ToDouble(t["scalz"]);

            physical.useGravity = (bool)Convert.ToBoolean(t["phycg"]);
            physical.useGravity = (bool)Convert.ToBoolean(t["phyck"]);

            state = (int)Convert.ToInt64(t["state"]);

            frametime = (float)Convert.ToDouble(t["frametime"]);
        }

        public virtual void Conversion(Hashtable t)
        {
            Console.WriteLine("改变物体: " + t.Count);

            if (t == null || t.Count <= 0)
            {
                return;
            }

            if (t["posx"] != null)
            {
                position.x = (float)Convert.ToDouble(t["posx"]);
            }
            if (t["posy"] != null)
            {
                position.y = (float)Convert.ToDouble(t["posy"]);
            }
            if (t["posz"] != null)
            {
                position.z = (float)Convert.ToDouble(t["posz"]);
            }

            if (t["rotx"] != null)
            {
                rotation.x = (float)Convert.ToDouble(t["rotx"]);
            }
            if (t["roty"] != null)
            {
                rotation.y = (float)Convert.ToDouble(t["roty"]);
            }
            if (t["rotz"] != null)
            {
                rotation.z = (float)Convert.ToDouble(t["rotz"]);
            }
            if (t["rotw"] != null)
            {
                rotation.w = (float)Convert.ToDouble(t["rotw"]);
            }

            if (t["scalx"] != null)
            {
                scale.x = (float)Convert.ToDouble(t["scalx"]);
            }
            if (t["scaly"] != null)
            {
                scale.y = (float)Convert.ToDouble(t["scaly"]);
            }
            if (t["scalz"] != null)
            {
                scale.z = (float)Convert.ToDouble(t["scalz"]);
            }

            if (t["phycg"] != null)
            {
                physical.useGravity = (bool)Convert.ToBoolean(t["phycg"]);
            }
            if (t["phyck"] != null)
            {
                physical.useGravity = (bool)Convert.ToBoolean(t["phyck"]);
            }
            if (t["state"] != null)
            {
                state = (int)Convert.ToInt64(t["state"]);
            }

            if(t["frametime"] != null)
            {
                frametime = (float)Convert.ToDouble(t["frametime"]);
            }

            changeorno = true;
        }
    }
}
