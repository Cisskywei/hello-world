using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 辅助函数工具类
    /// </summary>
    class Utilities
    {
        public static Utilities getInstance()
        {
            return Singleton<Utilities>.getInstance();
        }

        // 枚举转换函数
        public Enums.DivideGroupRules convertRuleToEnum(string rule)
        {
            Enums.DivideGroupRules ret = Enums.DivideGroupRules.Grade;

            switch (rule)
            {
                case "Grade":
                    ret = Enums.DivideGroupRules.Grade;
                    break;
                case "Random":
                    ret = Enums.DivideGroupRules.Random;
                    break;
                default:
                    break;
            }

            return ret;
        }

        public Enums.TeachingMode convertModelToEnum(string modelname)
        {
            Enums.TeachingMode m = Enums.TeachingMode.None;
            switch (modelname)
            {
                //case "Separate":
                //    m = Enums.TeachingMode.Separate;
                //    break;
                //case "SynchronousOne":
                //    m = Enums.ModelEnums.SynchronousOne;
                //    break;
                //case "SynchronousOne_Fixed":
                //    m = Enums.ModelEnums.SynchronousOne_Fixed;
                //    break;
                //case "SynchronousMultiple":
                //    m = Enums.ModelEnums.SynchronousMultiple;
                //    break;
                //case "Collaboration":
                //    m = Enums.ModelEnums.Collaboration;
                //    break;
                default:
                    break;
            }

            return m;
        }

        public string convertEnumToModel(Enums.ModelEnums mudelenum)
        {
            string m = null;
            switch (mudelenum)
            {
                case Enums.ModelEnums.Separate:
                    m = "Separate";
                    break;
                case Enums.ModelEnums.SynchronousOne:
                    m = "SynchronousOne";
                    break;
                case Enums.ModelEnums.SynchronousOne_Fixed:
                    m = "SynchronousOne_Fixed";
                    break;
                case Enums.ModelEnums.SynchronousMultiple:
                    m = "SynchronousMultiple";
                    break;
                case Enums.ModelEnums.Collaboration:
                    m = "Collaboration";
                    break;
                default:
                    break;
            }

            return m;
        }
    }
}
