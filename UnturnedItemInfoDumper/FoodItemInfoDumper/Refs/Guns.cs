﻿using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Items;
using SDG.Unturned;
using System.IO;
using Logger = Rocket.Core.Logging.Logger;

namespace AdminToolz
{
    public class Guns : RocketPlugin<Config>
    {
        public static Guns Instance;

        protected override void Load()
        {
            Instance = this;
            Logger.Log("Writing all Gun items to file...");

            Asset[] items = Assets.find(EAssetType.ITEM);
            using (var w = new StreamWriter("GunItems.txt"))
            {
                foreach (var i in items)
                {
                    if (i is ItemGunAsset f)
                    {
                        w.WriteLine($"{f.itemName}\t\t{f.id}\t\t\t\tPlayer_Damage:{f.playerDamageMultiplier.damage}\t\tAnimal_Damage:{f.animalDamageMultiplier.damage}\t\tZombie_Damage:{f.zombieDamageMultiplier.damage}\t\tBarricade_Damage:{f.barricadeDamage}\t\tStructure_Damage:{f.structureDamage}\t\tVehicle_Damage:{f.vehicleDamage}\t\tRange:{f.range}\t\tFirerate:{f.firerate}\t\tAmmo_Max:{f.ammoMax}");
                    }
                }
            }

            UnloadPlugin();
        }

        protected override void Unload()
        {
            Logger.Log("Done!");
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {

        };
    }
}
