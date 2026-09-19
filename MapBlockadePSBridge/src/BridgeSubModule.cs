using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MapBlockadePSBridge
{
    public class BridgeSubModule : MBSubModuleBase
    {
        private static readonly object SubscriberAnchor = new object();
        private bool _subscribed;
        private bool _onLoadLogged;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            if (_onLoadLogged) return;
            _onLoadLogged = true;
            try
            {
                BridgeLog.WriteFile("=== OnSubModuleLoad ===  MapBlockadePSBridge v0.3.0 DLL loaded.");
            }
            catch (Exception ex)
            {
                // If even the log path resolution fails, there's nothing we can do
                try { BridgeLog.WriteFile("OnSubModuleLoad log error: " + ex.Message); } catch { }
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            try
            {
                var gt = game != null && game.GameType != null ? game.GameType.GetType().Name : "null";
                BridgeLog.WriteFile("OnGameInitializationFinished; GameType=" + gt);

                if (game == null || !(game.GameType is Campaign))
                {
                    BridgeLog.WriteFile("skip subscribe: not a campaign game");
                    return;
                }
                if (_subscribed)
                {
                    BridgeLog.WriteFile("skip subscribe: already subscribed this session");
                    return;
                }

                var status = BlockadeInjector.TrySubscribe(SubscriberAnchor);
                BridgeLog.Both("subscribe result: " + status);
                _subscribed = status.StartsWith("subscribed");

                if (_subscribed)
                {
                    try
                    {
                        var n = BlockadeInjector.InjectExistingPlayerSettlements();
                        BridgeLog.Both("retro scan: injected " + n + " existing player-built cities");
                    }
                    catch (Exception ex)
                    {
                        BridgeLog.Both("retro scan error: " + ex.GetType().Name + " " + ex.Message);
                        BridgeLog.WriteFile("stack: " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                BridgeLog.Both("OnGameInitializationFinished error: " + ex.GetType().Name + " " + ex.Message);
                BridgeLog.WriteFile("stack: " + ex.StackTrace);
            }
        }
    }
}
