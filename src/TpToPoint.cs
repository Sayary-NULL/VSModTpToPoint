using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace VSModTpToPoint
{
    public class TpToPoint : ModSystem
    {
        ICoreServerAPI SAPI; 
        private WorldMapManager _mapManager;
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            SAPI = api;
            api.RegisterCommand("tpp", "tp to point", "tpp <id lable>", StartCommand);
        }

        private void StartCommand(IServerPlayer player, int groupId, CmdArgs args)
        {
            _mapManager = SAPI.ModLoader.GetModSystem<WorldMapManager>();
            var map = _mapManager.MapLayers.OfType<WaypointMapLayer>().FirstOrDefault();
            int index;
            if(int.TryParse(args.PeekWord(), out index))
            {
                if (index < map.Waypoints.Count)
                    player.Entity.TeleportTo(map.Waypoints[index].Position);
            }
            else if(args.PeekWord() == "list")
            {
                string outstr = "";
                for (int i = 0; i < map.Waypoints.Count; i++)
                {
                    var way = map.Waypoints[i];
                    outstr += i + ": " + way.Title + "\r\n";
                }
                player.SendMessage(groupId, outstr, EnumChatType.Notification);
            }
            else
            {
                string namelable = args.PopAll();
                foreach (var way in map.Waypoints)
                {
                    if(way.Title == namelable)
                    {
                        player.Entity.TeleportTo(way.Position);
                        return;
                    }
                }
            }
        }
    }
}