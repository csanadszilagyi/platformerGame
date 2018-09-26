using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using platformerGame.GameObjects;
namespace platformerGame.GameCommands
{
    class comPlacePickup : cBaseGameCommand
    {
        cPickupAble item;
        public comPlacePickup(cGameScene scene, cPickupAble item) : base(scene)
        {
            this.item = item;
        }

        public override void Execute()
        {
            //scene.EntityPool.AddPickup(item);
        }
    }
}
