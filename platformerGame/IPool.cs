using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    interface IPool
    {
        List<IDrawable> ListVisibles(cAABB visible_region);
    }
}
