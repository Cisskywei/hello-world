using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    public class Structs
    {
        public struct sVector3
        {
            public float x;
            public float y;
            public float z;
        }

        public struct sVector4
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        public struct sPhysicalProperty
        {
            public bool useGravity;
            public bool isKinematic;
        }

        public struct sTransform
        {
            public sVector3 pos;
            public sVector4 rot;
            public sVector3 scal;
        }
    }
}
