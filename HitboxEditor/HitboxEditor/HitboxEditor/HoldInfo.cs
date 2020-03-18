using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HitboxEditor
{
    class HoldInfo
    {
        private List<string> infoList = new List<string>();
        private int keyNum;

        public HoldInfo(List<string> info, int key)
        {
            this.infoList = info;
            this.keyNum = key;
        }

        /*public void Hold(List<string> info, int key)
        {
            this.info = info;
            this.key = key;
        }*/

        public List<string> InfoList
        { get { return infoList; } set { infoList = value; } }

        public int KeyFrame
        { get { return keyNum; } set { keyNum = value; } }
    }
}
