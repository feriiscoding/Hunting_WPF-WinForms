using Hunting.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.Model
{
    public class GameWonEventArgs : EventArgs
    {
        public List<int[]> player { get; private set; }
        public GameWonEventArgs(List<int[]> player)
        {
            this.player = player;
        }   
    }
}
