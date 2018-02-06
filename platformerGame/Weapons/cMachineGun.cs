using SFML.System;

using platformerGame.GameObjects;

namespace platformerGame.Weapons
{
    class cMachineGun : cWeapon
    {
        public cMachineGun(cGameObject owner, int firing_frequency) : base(owner, firing_frequency)
        {
            this.maxAmmo = 300;
            this.ammo = maxAmmo;
            this.magazineCapacity = 30;
            this.timeToReload = 1.5f;
        }

        public override void fire(Vector2f target)
        {
            base.fire(target);
        }
    }
}
