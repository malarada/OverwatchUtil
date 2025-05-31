using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using System;

namespace OverwatchUtil.Commands
{
	//[CommandHandler(typeof(RemoteAdminCommandHandler))]
	internal class GetDisabledChannels : ICommand
	{
		public string Command => "getdisabledchannels";

		public string[] Aliases => new string[] { "gdc" };

		public string Description => "Gets the list of disabled voice channels for the player.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Disabled voice channels:";
			Player player = Player.Get(sender);
			if (player.Role is OverwatchRole role)
			{
				var owvm = role.VoiceModule as PlayerRoles.Spectating.OverwatchVoiceModule;
				foreach (var channel in owvm.DisabledChannels)
				{
					response += $"\n- {channel}";
				}
			}
			return true;
		}
	}
}
