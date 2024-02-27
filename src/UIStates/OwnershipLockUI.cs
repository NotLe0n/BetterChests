using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterChests.UIStates;

public class OwnershipLockUI : UIState
{
	private readonly OwnershipSystem system = ModContent.GetInstance<OwnershipSystem>();
	private readonly Asset<Texture2D> locked = ModContent.Request<Texture2D>("Terraria/Images/Lock_0");
	private readonly Asset<Texture2D> unlocked = ModContent.Request<Texture2D>("Terraria/Images/Lock_1");
	private readonly UIImageFramed button;
	private readonly Rectangle frame = new(0, 0, 22, 22);
	private bool hovering;
	
	public OwnershipLockUI(int chestID)
	{
		button = new UIImageFramed(system.HasOwner(chestID) ? locked : unlocked, frame) {
			Top = new(Main.instance.invBottom+190, 0),
			Left = new(506+70, 0)
		};
		button.OnMouseOver += (_, _) => hovering = true;
		button.OnMouseOut += (_, _) => hovering = false;
		button.OnLeftClick += ToggleLockEvent;
		Append(button);
	}

	private void ToggleLockEvent(UIMouseEvent evt, UIElement listeningelement)
	{
		SoundEngine.PlaySound(SoundID.Unlock);

		if (system.HasOwner(Main.LocalPlayer.chest)) {
			button.SetImage(unlocked, frame);
			
			if (Main.netMode == NetmodeID.SinglePlayer) {
				system.RemoveOwner(Main.LocalPlayer.chest);
			}
			else {
				BetterChests.GetRemoveOwnerPacket(Main.LocalPlayer.chest).Send();
			}
		}
		else {
			button.SetImage(locked, frame);
			
			if (Main.netMode == NetmodeID.SinglePlayer) {
				system.SetOwner(Main.LocalPlayer.chest, Main.LocalPlayer.name);
			}
			else {
				BetterChests.GetAddOwnerPacket(Main.LocalPlayer.chest, Main.LocalPlayer.name).Send();
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		if (hovering) {
			Main.LocalPlayer.mouseInterface = true;
			// Draw border
			Main.spriteBatch.Draw((system.HasOwner(Main.LocalPlayer.chest) ? locked : unlocked).Value,
				button.GetDimensions().Position() - new Vector2(4, 0),
				new Rectangle(22, 0, 26, 24),
				Main.OurFavoriteColor
			);
		}
	}
}