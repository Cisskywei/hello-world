using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 负责处理与后台服务器的交互
    /// </summary>
    class BackDataService
    {
        public static BackDataService getInstance()
        {
            return Singleton<BackDataService>.getInstance();
        }

        public UserInfor CheckUser(string token, params Object[] p)
        {
            // 只为测试
            UserInfor ret = new UserInfor();
            return ret;
        }

        public Hashtable GetUserList(string token, params Object[] p)
        {
            return null;
        }

        public Hashtable GetUserTokenNameList(string token)
        {
            Hashtable h = new Hashtable();
            string t = "token";
            for (int i = 0; i < 10; i++)
            {
                h.Add(t+i,t+i);
            }

            return h;
        }
    }
}
