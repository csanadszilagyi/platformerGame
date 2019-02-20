using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using platformerGame.GameObjects;

namespace platformerGame.Inventory
{
    class cInventory
    {
        Dictionary<string, cItem> items;

        cGameObject owner;
        cItem currentItem = null;

        public cInventory()
        {
            this.items = new Dictionary<string, cItem>();
        }


        public void AddItem(string name, cItem item)
        {
            if(this.items.ContainsKey(name))
            {
                this.items[name].HandleAlreadyHave(item);
                return;
            }

            this.items.Add(name, item);
        }

        public void Delete(string name)
        {
            this.items.Remove(name);
        }

        public bool Switch(string to_name)
        {
            if (this.items.ContainsKey(to_name))
            {
                currentItem = items[to_name];
                return true;
            }

            return false;
        }

        public void Use()
        {
            currentItem.Use();
        }
    }
}
