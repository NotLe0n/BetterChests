using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests.src
{
    public class BetterChests : Mod
    {
        internal UserInterface SortUserInterface;
        internal UserInterface ConfirmationUserInterface;
        public static BetterChests instance;

        public override void Load()
        {
            instance = this;

            if (!Main.dedServ)
            {
                SortUserInterface = new UserInterface();
                SortUserInterface.SetState(new SortOptionsUI());

                ConfirmationUserInterface = new UserInterface();
                ConfirmationUserInterface.SetState(new ConfirmationUI());
            }

            ILEdits.Load();
        }

        public override void Unload()
        {
            instance = null;

            SortUserInterface = null;
            ConfirmationUserInterface = null;

            base.Unload();
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (Main.LocalPlayer.chest != -1)
            {
                if (SortOptionsUI.visible)
                {
                    SortUserInterface.Update(gameTime);
                }

                if (ConfirmationUI.visible)
                {
                    ConfirmationUserInterface.Update(gameTime);
                }
            }
            else
            {
                ConfirmationUI.visible = false;
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
                        if (Main.LocalPlayer.chest != -1)
                        {
                            if (SortOptionsUI.visible)
                            {
                                SortUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                            }
                            if (ConfirmationUI.visible)
                            {
                                ConfirmationUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                            }
                        }
                        return true;
                    }, InterfaceScaleType.UI));
            }
        }
    }
}