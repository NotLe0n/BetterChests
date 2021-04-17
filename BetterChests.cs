using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests
{
	public class BetterChests : Mod
	{
        internal UserInterface UserInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                UserInterface = new UserInterface();
                UserInterface.SetState(new BetterChestsUI());
            }

            ILEdits.Load();
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (Main.LocalPlayer.chest != -1 && BetterChestsUI.visible)
            {
                UserInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "BetterChests: UI",
                    delegate
                    {
                        if (Main.LocalPlayer.chest != -1 && BetterChestsUI.visible)
                        {
                            UserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    }, InterfaceScaleType.UI));
            }
        }
    }
}