using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace platformerGame
{
    public class cPlayerInfo
    {
        public float Time { get; set; }
        public string Level { get; set; }

        public cPlayerInfo()
        {
            Time = 0.0f;
            Level = "no-level";
        }

        
       
    }
}

