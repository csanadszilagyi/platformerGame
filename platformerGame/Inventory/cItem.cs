using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Inventory
{
    abstract class cItem
    {
        protected cInventory inventory;

        public cItem(cInventory invetory)
        {
            this.inventory = inventory;
        }

        public abstract void Use();
        public abstract void HandleAlreadyHave(cItem item);

        public cInventory Inventory
        {
            get { return this.inventory; }
        }
    }
}
