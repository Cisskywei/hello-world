using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    public class DataConvertInterface
    {
        public interface HashTo3DInformation
        {
            void Deserialization(Hashtable t);
            Hashtable Serialize();
        }
    }
}
