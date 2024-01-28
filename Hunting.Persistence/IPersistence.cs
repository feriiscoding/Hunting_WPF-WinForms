using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.Persistence
{
    public interface IPersistence
    {
        Player Load(String path, out int turns, out string who);

        void Save(String path, Player fields, int turns, string who);
    }
}
