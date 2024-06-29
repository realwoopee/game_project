using UnityEngine;

    public class InventoryState : ScriptableObject
    {
        public bool shotgunEquipped;
        
        public int pistolAmmoAmount;

        public int shotgunAmmoAmount;

        public int AmmoAmount
        {
            get => shotgunEquipped ? shotgunAmmoAmount : pistolAmmoAmount;
            set
            {
                if (shotgunEquipped)
                    shotgunAmmoAmount = value;
                else
                    pistolAmmoAmount = value;
            }
        }

        public int aptechasAmount;

        public int componentAAmount;

        public int componentBAmount;

        public int componentCAmount;

        public int fuelAmount;
    }
