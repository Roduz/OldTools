using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameViewer
{
    public class Sprite
    {
        //List<string> fileName = new List<string>();
        public List<Frame> frame = new List<Frame>();
        public int layer = 0;
        public string startN= "none";
        public int startF = 0;
        public Sprite(List<Frame> frame, int layer, string startN, int startF)
        {
            this.frame = frame;
            this.layer = layer;
            this.startN = startN;
            this.startF = startF;
        }
        public Sprite(int layer, string startN, int startF)
        {
            this.layer = layer;
            this.startN = startN;
            this.startF = startF;
        }
        public Sprite()
        {
            //Possible state machine
        }
    }
}
