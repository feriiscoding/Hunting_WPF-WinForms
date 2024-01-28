using System.Text;
using System.Collections.Generic;

namespace Hunting.Persistence
{
    public class Player
    {
        private List<int[]> hunters;
        private List<int[]> prey;
        private List<int[]> empty;

        public List<int[]> Hunters { get { return hunters; }set { hunters = value; } }
        public List<int[]> Prey { get { return prey; }set { prey = value; } }
        public List<int[]> Empty { get { return empty; }set { empty = value; } }
        public Player()
        {
            hunters = new List<int[]>();
            prey = new List<int[]>();
            empty = new List<int[]>();
        }
    }
}