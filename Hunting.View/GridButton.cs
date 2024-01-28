using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hunting.View
{
    class GridButton : Button
    {
        public Int32 _x;
        public Int32 _y;

        public Int32 GridX 
        { 
            get { return _x; } 
            set { _x = value; } 
        }
        public Int32 GridY 
        { 
            get { return _y; }
            set { _y = value; }
        }
        public GridButton(Int32 x, Int32 y) { _x = x; _y = y; }
    }
}
