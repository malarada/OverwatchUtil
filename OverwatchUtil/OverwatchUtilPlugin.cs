using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using VoiceChat;
using VoiceChat.Networking;
using Handlers = Exiled.Events.Handlers;

namespace OverwatchUtil
{
	public class OverwatchUtilPlugin : Plugin<Config>
	{
		public override void OnEnabled()
		{
			Handlers.Player.Spawned += TogglingOverwatch;
			Handlers.Player.VoiceChatting += VoiceChatting;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Handlers.Player.Spawned -= TogglingOverwatch;
			Handlers.Player.VoiceChatting -= VoiceChatting;

			base.OnDisabled();
		}

		private void TogglingOverwatch(SpawnedEventArgs ev)
		{
			if (ev.Player.IsOverwatchEnabled)
				_ = new OverwatchPlayer(ev.Player);
			else
			{
				OverwatchPlayer.OverwatchPlayers.Remove(ev.Player);
				OverwatchPlayer.List.Remove(OverwatchPlayer.Get(ev.Player));
			}
		}

		private void VoiceChatting(VoiceChattingEventArgs ev)
		{
			//if (ev.VoiceMessage.Channel == VoiceChatChannel.Scp1507)
			//	return;

			var message = new VoiceMessage(
				ev.VoiceMessage.Speaker,
				VoiceChatChannel.RoundSummary,
				ev.VoiceMessage.Data,
				ev.VoiceMessage.DataLength,
				ev.VoiceMessage.SpeakerNull
			);

			if (Round.IsLobby)
			{
				foreach (var p in OverwatchPlayer.OverwatchPlayers)
				{
					if (p != ev.Player)
						p.ReferenceHub.connectionToClient.Send(message);
				}
			}
			else if (!Round.IsEnded)
			{
				if (ev.Player.IsOverwatchEnabled)
				{
					foreach (var p in OverwatchPlayer.Get(ev.Player).VoicePatches)
						if (p.IsAlive)
							p.ReferenceHub.connectionToClient.Send(message);
				}
				else
				{
					var players = OverwatchPlayer.List.Where(t => t.VoicePatches.Any(p => p == ev.Player));
					foreach (var p in players)
					{
						var ow = p.Owner.Role as OverwatchRole;
						var owvm = ow.VoiceModule as PlayerRoles.Spectating.OverwatchVoiceModule;
						if (ev.Player.IsAlive &&
							!(!ev.Player.IsScp && ow.SpectatedPlayer == ev.Player) &&
							!(ev.Player.IsScp && !owvm.DisabledChannels.Contains(VoiceChatChannel.ScpChat))
							&& !owvm.DisabledChannels.Contains(VoiceChatChannel.Proximity))
							p.Owner.ReferenceHub.connectionToClient.Send(message);
					}
				}
			}
		}
	}

	public class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;
		public bool Debug { get; set; }
	}
}
