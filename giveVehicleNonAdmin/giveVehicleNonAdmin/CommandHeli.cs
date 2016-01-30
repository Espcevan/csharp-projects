﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using SDG.Unturned;

namespace giveVehicleNonAdmin
{
    class CommandHeli : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Player; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Ucaller = (UnturnedPlayer)caller;
            DateTime remainingCooldownDatetime;
            ushort id = (ushort)giveVehicle.instance.Configuration.Instance.HelicopterId;
            double maxCooldown = (double)giveVehicle.instance.Configuration.Instance.SpawnCooldown;

            if (!(giveVehicle.IndividualCooldowns.ContainsKey(caller.DisplayName)))
            {
                giveVehicle.IndividualCooldowns.Add(caller.DisplayName, DateTime.Now);
            }

            if (giveVehicle.IndividualCooldowns.TryGetValue(caller.DisplayName, out remainingCooldownDatetime))
            {
                if ((DateTime.Now - remainingCooldownDatetime).TotalSeconds >= maxCooldown || giveVehicle.FirstCommandExecution[caller.DisplayName] == true)
                {
                    if (VehicleTool.giveVehicle(Ucaller.Player, id))
                    {
                        UnturnedChat.Say(Ucaller, "giving you a Helicopter", UnityEngine.Color.yellow);
                        giveVehicle.IndividualCooldowns[caller.DisplayName] = DateTime.Now;
                        if (giveVehicle.FirstCommandExecution[caller.DisplayName])
                        {
                            giveVehicle.FirstCommandExecution[caller.DisplayName] = false;
                        }
                    }
                }
                else
                {
                    double cooldown = maxCooldown - (DateTime.Now - remainingCooldownDatetime).TotalSeconds;
                    UnturnedChat.Say(Ucaller, "you have to wait " + (int)cooldown+ " seconds to use this command again", UnityEngine.Color.yellow);
                }
            }
        }

        public string Help
        {
            get { return "gives the caller a helicopter"; }
        }

        public string Name
        {
            get { return "heli"; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "heli" }; }
        }

        public string Syntax
        {
            get { return ""; }
        }
    }
}
