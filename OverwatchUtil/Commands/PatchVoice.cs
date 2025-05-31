using CommandSystem;
using Exiled.API.Features;
using System;

namespace OverwatchUtil.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	internal class PatchVoice : ICommand
	{
		public string Command => "patchvoice";

		public string[] Aliases => new string[] { "pv" };

		public string Description => "Listen and talk to a specific player from Overwatch.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 1)
			{
				response = "Usage: patchvoice <player id>";
				return false;
			}

			if (!int.TryParse(arguments.At(0), out int playerId))
			{
				response = "Invalid player ID.";
				return false;
			}

			var player = Player.Get(playerId);
			if (player == null)
			{
				response = $"Player with ID {playerId} not found.";
				return false;
			}
			else if (!Player.Get(sender).IsOverwatchEnabled)
			{
				response = "You must be Overwatch to use this command.";
				return false;
			}

			var owp = OverwatchPlayer.Get(sender);
			if (owp.VoicePatches.Contains(player))
			{
				owp.VoicePatches.Remove(player);
				response = $"Unpatched voice for player {player.Nickname} (ID: {playerId}).";
				return true;
			}
			owp.VoicePatches.Add(player);

			response = $"Patched voice for player {player.Nickname} (ID: {playerId}).";
			return true;
		}
	}
}
