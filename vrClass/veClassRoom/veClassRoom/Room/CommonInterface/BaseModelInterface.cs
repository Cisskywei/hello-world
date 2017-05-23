using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 基础的模式接口
    /// </summary>
    interface BaseModelInterface
    {
        //void SeparateCommond();

        /// <summary>
        /// 一对一同步指令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuid"></param>
        /// <param name="touuid"></param>
        /// <param name="msg"></param>
        void SyncOneCommond<T>(string fromuuid, string touuid, T msg);

        /// <summary>
        /// 一对一同步3d位置角度等信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuid"></param>
        /// <param name="touuid"></param>
        /// <param name="msg"></param>
        void SyncOne3DInfor<T>(string fromuuid, string touuid, T msg);

        /// <summary>
        /// 一对多指令同步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuid"></param>
        /// <param name="touuids"></param>
        /// <param name="msg"></param>
        void SyncMultCommond<T>(string fromuuid, ArrayList touuids, T msg);

        /// <summary>
        /// 一对多3维位置角度数据同步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuid"></param>
        /// <param name="touuids"></param>
        /// <param name="msg"></param>
        void SyncMult3DInfor<T>(string fromuuid, ArrayList touuids, T msg);

        /// <summary>
        /// 一对一协同操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuid"></param>
        /// <param name="touuids"></param>
        /// <param name="msg"></param>
        void CollaborationOne2One<T>(string fromuuid, string touuid, T msg);

        /// <summary>
        /// 多人协同操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromuuids"></param>
        /// <param name="touuids"></param>
        /// <param name="msg"></param>
        void CollaborationMult<T>(string fromuuid, ArrayList touuids, T msg);

    }
}
