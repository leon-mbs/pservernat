using System.Collections;
using System;
using System.IO;
using ESC_POS_USB_NET.Interfaces.Command;
using System.Drawing;

namespace ESC_POS_USB_NET.EpsonCommands
{
  
    public class BitmapData
    {
        public BitArray Dots { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}

