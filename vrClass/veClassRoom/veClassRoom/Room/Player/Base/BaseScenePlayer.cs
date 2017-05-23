using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class BaseScenePlayer : BaseSceneObject
    {
        public string token;
        public string uuid;

        public AuthorityEnum authority = AuthorityEnum.None;

        public BaseScenePlayer():base()
        { }

        public BaseScenePlayer(string token, string name, string uuid) : base()
        {
            this.token = token;
            this.name = name;
            this.uuid = uuid;
        }
    }
}
