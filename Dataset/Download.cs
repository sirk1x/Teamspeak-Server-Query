using System;
using System.Collections.Generic;
using System.Text;

namespace MyLittleTeamspeakServerQuery.Dataset
{
    class Download
    {
        public ushort clientftfid { get; set; }
        public ushort serverftfid { get; set; }
        public string ftkey { get; set; }
        public ushort port { get; set; }
        public ulong size { get; set; }
        public ulong proto { get; set; }
    }
}
