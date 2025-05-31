using CommandSystem;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace OverwatchUtil
{
	public class OverwatchPlayer
	{
		public static List<OverwatchPlayer> List = new List<OverwatchPlayer>();
		public static List<Player> OverwatchPlayers { get; set; } = new List<Player>();

		public Player Owner { get; set; }
		public List<Player> VoicePatches { get; set; } = new List<Player>();

		public OverwatchPlayer(Player player)
		{
			Owner = player;
			List.Add(this);
			OverwatchPlayers.Add(player);
		}

		public static OverwatchPlayer Get(Player player)
		{
			return List.FirstOrDefault(op => op.Owner == player);
		}

		public static OverwatchPlayer Get(ICommandSender sender)
		{
			return Get(Player.Get(sender));
		}
	}
}
