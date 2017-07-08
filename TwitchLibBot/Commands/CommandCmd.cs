﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Models.Client;
using TwitchLibBot.Core.Helpers;
using TwitchLibBot.Interfaces;
using TwitchLibBot.Core.Database;
using TwitchLibBot.Core.Handlers;

namespace TwitchLibBot.Commands
{
    class CommandCmd : IChatCommand
    {
        public string Command => "!cmd";
        string usage = "Usage: !cmd <add | del | edit> <command name> (command response)";

        public void Execute(ChatMessage msg, string[] parameters)
        {
            if (parameters.Length == 0)
            {
                Chat.Send(usage);
                return;
            }

            if (parameters.Length == 2 && parameters[0].ToLower() == "del")
            {
                string CommandName = parameters[1].ToLower();
                var command = Database.GetCustomCommand(CommandName);

                if (command == null)
                {
                    Chat.Send("Command not found");
                    return;
                }
                Database.DeleteCustomCommand(command);
                Chat.Send($"Deleted Command {command.CmdName}");
                return;
            }

            if (!(parameters.Length >= 3))
            {
                Chat.Send(usage);
                return;
            }

            string CName = parameters[1].ToLower();
            string resp = string.Join(" ", parameters.Skip(2));

            if (parameters[0].ToLower() == "add")
            {
                if (CommandHandler.Commands.FirstOrDefault(x => x.Command.ToLower() == CName) != null)
                {
                    Chat.Send($"Command {CName} already exist");
                    return;
                }
                Database.AddOrUpdateCustomCommand(new Data.CustomCommand { CmdName = CName, Reponse = resp });
                Chat.Send($"Added Command {CName}");
            }
            else if (parameters[0].ToLower() == "edit")
            {
                var cmd = Database.GetCustomCommand(CName);
                cmd.Reponse = resp;
                Database.AddOrUpdateCustomCommand(cmd);
                Chat.Send($"Editted Command {CName}");
            }
        }
    }
}