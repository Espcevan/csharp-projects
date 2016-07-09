﻿using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace godmodeAnnouncer
{
    class CommandCV : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Both; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool playerFound = false;
            float foundDistance = 0;
            UnturnedPlayer Uplayer = null;
            if (!(caller is ConsolePlayer))
            {
                Uplayer = (UnturnedPlayer)caller;
            }
            int distance = -1;
            bool EnteredNumber = false;

            if (checkIfClearIsRunning(caller))
            {
                return;
            }

            if (command.Length == 1)
            {
                if (int.TryParse(command[0], out distance))
                {
                    EnteredNumber = true;
                }
            }

            new Thread(() =>
            {
                var toRemove = new List<InteractableVehicle>();

                /*  VehicleManager.Vehicles
                      //.Where( v => v.passengers.All( p => p?.player == null ) )
                            .Where(v => v.passengers.All(p => p.player == null))
                            .Where(v =>
                            {
                                if (distance == -1) return true;
                                return Vector3.Distance(v.transform.position, ((UnturnedPlayer)caller).Position) <= distance;
                            })
                            .Select(v => v.instanceID)
                            */
                foreach (var v in VehicleManager.vehicles)
                {
                    if (EnteredNumber && !(caller is ConsolePlayer))
                    {
                        foundDistance = Vector3.Distance(v.transform.position, Uplayer.Position);
                        if (foundDistance <= distance)
                        {
                            foreach (var passenger in v.passengers)
                            {
                                if (passenger.player == null) { }
                                else
                                    playerFound = true;
                            }
                        }

                        if (!playerFound && foundDistance <= distance)
                        {
                            toRemove.Add(v);
                        }
                    }
                    else
                    {
                        foreach (var passenger in v.passengers)
                        {
                            if (passenger.player == null) { }
                            else
                                playerFound = true;
                        }

                        if (!playerFound)
                        {
                            toRemove.Add(v);
                        }
                    }

                    playerFound = false;
                }

                DateTime start = DateTime.Now;

                LogInfo("Starting a vehicle clear! Estimated time: " + (toRemove.Count *
                    godmode.Instance.Configuration.Instance.DelayBetweenClears) / 1000 + " seconds.");
                sendMessage(caller, "Starting a vehicle clear! Estimated time: " + (toRemove.Count *
                    godmode.Instance.Configuration.Instance.DelayBetweenClears) / 1000 + " seconds.");

                godmode.ClearRunning = true;
                foreach (var v in toRemove)
                {
                    if (!checkForPassengers(v))
                    {
                        VehicleManager.Instance.SteamChannel.send("tellVehicleDestroy",
                                                ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, v.instanceID);
                        Thread.Sleep(godmode.Instance.Configuration.Instance.DelayBetweenClears);
                    }
                };

                sendMessage(caller, "Removed " + toRemove.Count + " vehicles in " + (DateTime.Now - start).Seconds +
                   " seconds!");

                Log("Removed " + toRemove.Count + " vehicles in " + (DateTime.Now - start).Seconds + " seconds!");
                Logger.Log("Removed " + toRemove.Count + " vehicles in " + (DateTime.Now - start).Seconds + " seconds!");
                godmode.ClearRunning = false; 
            }).Start();
        }

        public string Help
        {
            get { return "clears vehicles on the server"; }
        }

        public string Name
        {
            get { return "cv"; }
        }

        public List<string> Permissions
        {
            get { return new List<string> { "cv" }; }
        }

        public string Syntax
        {
            get { return "(radius)"; }
        }

        bool checkForPassengers(InteractableVehicle v)
        {
            foreach (var passenger in v.passengers)
            {
                if (passenger.player == null) { }
                else
                    return true;

            }

            return false;
        }

        void Log(string message)
        {
            var lastColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message + "\n");
            Console.ForegroundColor = lastColor;
        }

        void LogInfo(string message)
        {
            var lastColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message + "\n");
            Console.ForegroundColor = lastColor;
        }

        void sendMessage(IRocketPlayer caller, string message)
        {
            if (!(caller is ConsolePlayer))
            {
                UnturnedChat.Say(caller, message);
            }
        }

        bool checkIfClearIsRunning(IRocketPlayer caller)
        {
            if (godmode.ClearRunning)
            {
                if (!(caller is ConsolePlayer))
                {
                    sendMessage(caller, "A vehicle is already running!");
                }

                LogInfo("A vehicle clear is already running!");
                Logger.Log("A vehicle clear is already running!");

                return true;
            }
            return false;
        }
    }
}
