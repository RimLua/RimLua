using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;
using RimWorld;

namespace RimLua.Windows
{
	// Token: 0x02000F17 RID: 3863
	public class Page_Addons : Page
	{
		// Token: 0x06005EE6 RID: 24294 RVA: 0x00205264 File Offset: 0x00203464
		public Page_Addons()
		{
			this.doCloseButton = true;
			this.closeOnCancel = true;
		}

		// Token: 0x06005EE7 RID: 24295 RVA: 0x002052D8 File Offset: 0x002034D8
		public override void PreOpen()
		{
			base.PreOpen();
			ModLister.RebuildModList();
			this.selectedMod = AddonManager.InstalledAddons.First();
			this.activeModsWhenOpenedHash = ModLister.InstalledModsListHash(true);
		}

		// Token: 0x06005EE8 RID: 24296 RVA: 0x0020542C File Offset: 0x0020362C
		private List<AddonInfo> ModsInListOrder()
		{
			Page_Addons.modsInListOrderCached.Clear();
			Page_Addons.modsInListOrderCached.AddRange(AddonManager.InstalledAddons);

			return Page_Addons.modsInListOrderCached;
		}

		// Token: 0x06005EE9 RID: 24297 RVA: 0x002054C0 File Offset: 0x002036C0
		public override void DoWindowContents(Rect rect)
		{
			Rect mainRect = base.GetMainRect(rect, 0f, true);
			GUI.BeginGroup(mainRect);
			Text.Font = GameFont.Small;
			float num = 0f;
			if (Widgets.ButtonText(new Rect(17f, num, 316f, 30f), "OpenSteamWorkshop".Translate(), true, true, true))
			{
				SteamUtility.OpenSteamWorkshopPage();
			}
			num += 30f;
			if (Widgets.ButtonText(new Rect(17f, num, 316f, 30f), "GetModsFromForum".Translate(), true, true, true))
			{
				Application.OpenURL("http://rimworldgame.com/getmods");
			}
			num += 30f;
			num += 17f;
			this.filter = Widgets.TextField(new Rect(0f, num, 350f, 30f), this.filter);
			num += 30f;
			num += 10f;
			float num2 = 47f;
			Rect rect2 = new Rect(0f, num, 350f, mainRect.height - num - num2);
			Widgets.DrawMenuSection(rect2);
			float height = (float)ModLister.AllInstalledMods.Count<ModMetaData>() * 26f + 8f;
			Rect rect3 = new Rect(0f, 0f, rect2.width - 16f, height);
			Widgets.BeginScrollView(rect2, ref this.modListScrollPosition, rect3, true);
			Rect rect4 = rect3.ContractedBy(4f);
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = rect4.width;
			float num3 = this.modListScrollPosition.y - 26f;
			float num4 = this.modListScrollPosition.y + rect2.height;
			listing_Standard.Begin(rect4);
			int num5 = ReorderableWidget.NewGroup(delegate(int from, int to)
			{
				ModsConfig.Reorder(from, to);
				this.modsInListOrderDirty = true;
			}, ReorderableDirection.Vertical, -1f, null);
			int num6 = 0;
			foreach (AddonInfo addonInfo in this.ModsInListOrder())
			{
				float num7 = (float)num6 * 26f;
				bool active = addonInfo.Active;
				Rect rect5 = new Rect(0f, (float)num6 * 26f, listing_Standard.ColumnWidth, 26f);
				if (active)
				{
					ReorderableWidget.Reorderable(num5, rect5, false);
				}
				if (num7 >= num3 && num7 <= num4)
				{
					this.DoModRow(rect5, addonInfo, num6, num5);
				}
				num6++;
			}
			int downloadingItemsCount = WorkshopItems.DownloadingItemsCount;
			for (int i = 0; i < downloadingItemsCount; i++)
			{
				this.DoModRowDownloading(listing_Standard, num6);
				num6++;
			}
			listing_Standard.End();
			Widgets.EndScrollView();
			num += rect2.height;
			num += 10f;
			if (Widgets.ButtonText(new Rect(17f, num, 316f, 30f), "ResolveModOrder".Translate(), true, true, true))
			{
				ModsConfig.TrySortMods();
				this.modsInListOrderDirty = true;
			}
			Rect position = new Rect(rect2.xMax + 17f, 0f, mainRect.width - rect2.width - 17f, mainRect.height);
			GUI.BeginGroup(position);
			if (this.selectedMod != null)
			{
				Text.Font = GameFont.Medium;
				Rect rect6 = new Rect(0f, 0f, position.width, 40f);
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(rect6, this.selectedMod.Name.Truncate(rect6.width, null));
				Text.Anchor = TextAnchor.UpperLeft;
				Rect position2 = new Rect(0f, rect6.yMax, 0f, 20f);
				//if (this.selectedMod.PreviewImage != null)
				//{
				//	position2.width = Mathf.Min((float)this.selectedMod.PreviewImage.width, position.width);
				//	position2.height = (float)this.selectedMod.PreviewImage.height * (position2.width / (float)this.selectedMod.PreviewImage.width);
				//	float num8 = Mathf.Ceil(position.height * 0.37f);
				//	if (position2.height > num8)
				//	{
				//		float height2 = position2.height;
				//		position2.height = num8;
				//		position2.width *= position2.height / height2;
				//	}
				//	if (position2.height > 300f)
				//	{
				//		position2.width *= 300f / position2.height;
				//		position2.height = 300f;
				//	}
				//	position2.x = position.width / 2f - position2.width / 2f;
				//	GUI.DrawTexture(position2, this.selectedMod.PreviewImage, ScaleMode.ScaleToFit);
				//}
				float num9 = position2.yMax + 10f;
				Text.Font = GameFont.Small;
				float num10 = num9;
				//if (!this.selectedMod.Author.NullOrEmpty())
				//{
				//	Widgets.Label(new Rect(0f, num10, position.width / 2f, Text.LineHeight), "Author".Translate() + ": " + this.selectedMod.Author);
				//	num10 += Text.LineHeight;
				//}
				//if (!this.selectedMod.PackageId.NullOrEmpty())
				//{
				//	GUI.color = Color.gray;
				//	Widgets.Label(new Rect(0f, num10, position.width / 2f, Text.LineHeight), "ModPackageId".Translate() + ": " + this.selectedMod.PackageIdPlayerFacing);
				//	num10 += Text.LineHeight;
				//	GUI.color = Color.white;
				//}
				float num11 = num9;
				WidgetRow widgetRow = new WidgetRow(position.width, num11, UIDirection.LeftThenUp, 99999f, 4f);
				//if (SteamManager.Initialized && this.selectedMod.OnSteamWorkshop)
				//{
				//	if (widgetRow.ButtonText("Unsubscribe".Translate(), null, true, true))
				//	{
				//		Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmUnsubscribe".Translate(this.selectedMod.Name), delegate
				//		{
				//			this.selectedMod.enabled = false;
				//			this.Notify_SteamItemUnsubscribed(this.selectedMod.GetPublishedFileId());
				//		}, true, null));
				//	}
				//	if (widgetRow.ButtonText("WorkshopPage".Translate(), null, true, true))
				//	{
				//		SteamUtility.OpenWorkshopPage(this.selectedMod.GetPublishedFileId());
				//	}
				//	num11 += 25f;
				//}
				//if (!this.selectedMod.IsCoreMod)
				//{
				//	Text.Anchor = TextAnchor.UpperRight;
					Rect rect7 = new Rect(position.width - 300f, num11, 300f, Text.LineHeight);
				//	if (!this.selectedMod.VersionCompatible)
				//	{
				//		GUI.color = Color.red;
				//	}
				//	Widgets.Label(rect7, "ModTargetVersion".Translate() + ": " + this.selectedMod.SupportedVersionsReadOnly.Select(delegate(System.Version v)
				//	{
				//		string text = VersionControl.IsCompatible(v) ? "<color=green>" : "<color=red>";
				//		string text2 = "</color>";
				//		if (v.Build > 0)
				//		{
				//			return string.Format("{0}{1}.{2}.{3}{4}", new object[]
				//			{
				//				text,
				//				v.Major.ToString(),
				//				v.Minor.ToString(),
				//				v.Build.ToString(),
				//				text2
				//			});
				//		}
				//		return string.Format("{0}{1}.{2}{3}", new object[]
				//		{
				//			text,
				//			v.Major.ToString(),
				//			v.Minor.ToString(),
				//			text2
				//		});
				//	}).ToCommaList(false));
				//	GUI.color = Color.white;
				//	num11 += Text.LineHeight;
				//}
				Text.Anchor = TextAnchor.UpperLeft;
				float num13 = Mathf.Max(num10, num11) + (this.anyReqsCached ? 10f : 17f);
				Rect outRect = new Rect(0f, num13, position.width, position.height - num13 - 40f);
				float width = outRect.width - 16f;
				float num14 = Text.CalcHeight("Здесь описание", width);
				num14 = Mathf.Min(num14 * 1.25f, num14 + 200f);
				Rect viewRect = new Rect(0f, 0f, width, num14 + this.modRequirementsHeightCached + (this.anyReqsInfoToShowCached ? 10f : 0f));
				float num15 = (viewRect.height > outRect.height) ? 16f : 0f;
				Widgets.BeginScrollView(outRect, ref this.modDescriptionScrollPosition, viewRect, true);
				float num16 = 0f;
				Widgets.Label(new Rect(0f, num16, viewRect.width - num15, viewRect.height - num16), "Здесь описание");
				Widgets.EndScrollView();
				//if (!this.selectedMod.Url.NullOrEmpty())
				//{
				//	Text.Anchor = TextAnchor.MiddleLeft;
				//	float num17 = Mathf.Min(position.width / 2f, Text.CalcSize(this.selectedMod.Url).x);
				//	if (Widgets.ButtonText(new Rect(position.width - num17, outRect.yMax, num17, position.yMax - outRect.yMax), this.selectedMod.Url.Truncate(num17, null), false, true, true))
				//	{
				//		Application.OpenURL(this.selectedMod.Url);
				//	}
				//	Text.Anchor = TextAnchor.UpperLeft;
				//}
			}
			GUI.EndGroup();
			GUI.EndGroup();
			Text.Font = GameFont.Tiny;
			TaggedString taggedString2 = "GameVersionIndicator".Translate() + ": " + VersionControl.CurrentVersionString;
			float x = Text.CalcSize(taggedString2).x;
			Widgets.Label(new Rect(0f, rect.height - 15f, x, Text.LineHeight), taggedString2);
			Text.Font = GameFont.Small;
			int num18 = ModLister.InstalledModsListHash(true);
			if (this.activeModsHash == -1 || this.activeModsHash != num18)
			{
				this.activeModsHash = num18;
			}
		}

		// Token: 0x06005EEA RID: 24298 RVA: 0x0020606C File Offset: 0x0020426C
		private void DoModRow(Rect r, AddonInfo mod, int index, int reorderableGroup)
		{
			bool active = mod.Active;
			Action clickAction = null;
			ContentSourceUtility.DrawContentSource(r, ContentSource.ModsFolder, clickAction);
			r.xMin += 28f;
			bool flag = mod == this.selectedMod;
			Rect rect = r;
			
			if (mod.Active)
			{
				string text = "";
				if (active)
				{
					text += "DragToReorder".Translate() + ".\n";
				}
				//if (!mod.VersionCompatible)
				//{
				//	GUI.color = Color.yellow;
				//	if (!text.NullOrEmpty())
				//	{
				//		text += "\n";
				//	}
				//	if (mod.MadeForNewerVersion)
				//	{
				//		text += "ModNotMadeForThisVersion_Newer".Translate();
				//	}
				//	else
				//	{
				//		text += "ModNotMadeForThisVersion".Translate();
				//	}
				//}

				//if (active && !Page_ModsConfig.modWarningsCached.NullOrEmpty<string>() && !Page_ModsConfig.modWarningsCached[index].NullOrEmpty())
				//{
				//	GUI.color = Color.red;
				//	if (!text.NullOrEmpty())
				//	{
				//		text += "\n";
				//	}
					//text += Page_ModsConfig.modWarningsCached[index];
				//}
				GUI.color = this.FilteredColor(GUI.color, mod.Name);
				if (!text.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, new TipSignal(text, mod.GetHashCode() * 3311));
				}
				float num = rect.width - 24f;
				if (active)
				{
					GUI.DrawTexture(new Rect(rect.xMax - 48f + 2f, rect.y, 24f, 24f), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true));
					num -= 24f;
				}
				Text.Font = GameFont.Small;
				string label = mod.Name.Truncate(num, this.truncatedModNamesCache);
				bool flag2 = active;
				if (Widgets.CheckboxLabeledSelectable(rect, label, ref flag, ref flag2))
				{
					this.selectedMod = mod;
					//this.RecacheSelectedModRequirements();
				}
				//if (active && !flag2 && mod.IsCoreMod)
				//{
				//	ModMetaData coreMod = mod;
				//	Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDisableCoreMod".Translate(), delegate
				//	{
				//		coreMod.Active = false;
				//		this.truncatedModNamesCache.Clear();
				//	}, false, null));
				//}
				//else
				//{
				//	if (!active && flag2)
				//	{
				//		foreach (ModMetaData modMetaData in ModsConfig.ActiveModsInLoadOrder)
				//		{
				//			if (modMetaData.PackageIdNonUnique.Equals(mod.PackageIdNonUnique, StringComparison.InvariantCultureIgnoreCase))
							{
				//				Find.WindowStack.Add(new Dialog_MessageBox("MessageModWithPackageIdAlreadyEnabled".Translate(mod.PackageIdPlayerFacing, modMetaData.Name), null, null, null, null, null, false, null, null));
				//				return;
				//			}
				//		}
				//	}
				//	if (flag2 != active)
				//	{
				//		mod.Active = flag2;
				//	}
				//	this.truncatedModNamesCache.Clear();
				}
			}
			else
			{
				GUI.color = this.FilteredColor(Color.gray, mod.Name);
				Widgets.Label(rect, mod.Name);
			}

			GUI.color = Color.white;
		}

		private void DoModRowDownloading(Listing_Standard listing, int index)
		{
			Rect rect = new Rect(0f, (float)index * 26f, listing.ColumnWidth, 26f);
			ContentSourceUtility.DrawContentSource(rect, ContentSource.SteamWorkshop, null);
			rect.xMin += 28f;
			Widgets.Label(rect, "Downloading".Translate() + GenText.MarchingEllipsis(0f));
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x002068C4 File Offset: 0x00204AC4
		public void Notify_ModsListChanged()
		{
			string selModId = this.selectedMod.Name;
			this.selectedMod = AddonManager.InstalledAddons.Where(i => i.Name == selModId).FirstOrDefault();       
			this.modsInListOrderDirty = true;
		}

		// Token: 0x06005EF0 RID: 24304 RVA: 0x00206911 File Offset: 0x00204B11
		internal void Notify_SteamItemUnsubscribed(PublishedFileId_t pfid)
		{
			if (this.selectedMod != null && this.selectedMod.RootDir == pfid.ToString())
			{
				this.selectedMod = null;
			}
			this.modsInListOrderDirty = true;
		}

		// Token: 0x06005EF1 RID: 24305 RVA: 0x0020694E File Offset: 0x00204B4E
		public void SelectMod(AddonInfo addon)
		{
			this.selectedMod = addon;
		}

		// Token: 0x06005EF2 RID: 24306 RVA: 0x00206960 File Offset: 0x00204B60
		public override void PostClose()
		{
			ModsConfig.Save();
			foreach (ModMetaData modMetaData in ModsConfig.ActiveModsInLoadOrder)
			{
				modMetaData.UnsetPreviewImage();
			}
			Resources.UnloadUnusedAssets();
			if (this.activeModsWhenOpenedHash != ModLister.InstalledModsListHash(true))
			{
				ModsConfig.RestartFromChangedMods();
			}
		}

		// Token: 0x06005EF3 RID: 24307 RVA: 0x002069C8 File Offset: 0x00204BC8
		private Color FilteredColor(Color color, string label)
		{
			if (this.filter.NullOrEmpty())
			{
				return color;
			}
			if (label.IndexOf(this.filter, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return color;
			}
			return color * new Color(1f, 1f, 1f, 0.3f);
		}

		// Token: 0x04003406 RID: 13318
		public AddonInfo selectedMod;

		// Token: 0x04003407 RID: 13319
		private Vector2 modListScrollPosition = Vector2.zero;

		// Token: 0x04003408 RID: 13320
		private Vector2 modDescriptionScrollPosition = Vector2.zero;

		// Token: 0x04003409 RID: 13321
		private int activeModsWhenOpenedHash = -1;

		// Token: 0x0400340A RID: 13322
		private int activeModsHash = -1;

		// Token: 0x0400340B RID: 13323
		private bool displayFullfilledRequirements;

		// Token: 0x0400340C RID: 13324
		protected string filter = "";

		// Token: 0x0400340D RID: 13325
		private Dictionary<string, string> truncatedModNamesCache = new Dictionary<string, string>();

		// Token: 0x0400340E RID: 13326
		private static List<string> modWarningsCached = new List<string>();

		// Token: 0x0400340F RID: 13327
		private List<ModRequirement> visibleReqsCached = new List<ModRequirement>();

		// Token: 0x04003410 RID: 13328
		private bool anyReqsCached;

		// Token: 0x04003411 RID: 13329
		private bool anyReqsInfoToShowCached;

		// Token: 0x04003412 RID: 13330
		private bool anyUnfulfilledReqsCached;

		// Token: 0x04003413 RID: 13331
		private bool anyOrderingIssuesCached;

		// Token: 0x04003414 RID: 13332
		private float modRequirementsHeightCached;

		// Token: 0x04003415 RID: 13333
		private bool modsInListOrderDirty;

		// Token: 0x04003416 RID: 13334
		private static List<AddonInfo> modsInListOrderCached = new List<AddonInfo>();

		// Token: 0x04003417 RID: 13335
		private const float ModListAreaWidth = 350f;

		// Token: 0x04003418 RID: 13336
		private const float ModsListButtonHeight = 30f;

		// Token: 0x04003419 RID: 13337
		private const float ModsFolderButHeight = 30f;

		// Token: 0x0400341A RID: 13338
		private const float ButtonsGap = 4f;

		// Token: 0x0400341B RID: 13339
		private const float UploadRowHeight = 40f;

		// Token: 0x0400341C RID: 13340
		private const float PreviewMaxHeight = 300f;

		// Token: 0x0400341D RID: 13341
		private const float VersionWidth = 30f;

		// Token: 0x0400341E RID: 13342
		private const float ModRowHeight = 26f;

		// Token: 0x0400341F RID: 13343
		private const float RequirementBoxInnerOffset = 10f;

		// Token: 0x04003420 RID: 13344
		private static readonly Color RequirementBoxOutlineColor = new Color(0.25f, 0.25f, 0.25f);

		// Token: 0x04003421 RID: 13345
		private static readonly Color UnmetRequirementBoxOutlineColor = new Color(0.62f, 0.18f, 0.18f);

		// Token: 0x04003422 RID: 13346
		private static readonly Color UnmetRequirementBoxBGColor = new Color(0.1f, 0.065f, 0.072f);

		// Token: 0x04003423 RID: 13347
		private static readonly Color RequirementRowColor = new Color(0.13f, 0.13f, 0.13f);

		// Token: 0x04003424 RID: 13348
		private static readonly Color UnmetRequirementRowColor = new Color(0.23f, 0.15f, 0.15f);

		// Token: 0x04003425 RID: 13349
		private static readonly Color UnmetRequirementRowColorHighlighted = new Color(0.27f, 0.18f, 0.18f);

		// Token: 0x04003426 RID: 13350
		private static readonly List<List<string>> translationOnlyModFolders = new List<List<string>>
		{
			new List<string>
			{
				"Languages",
				"About"
			},
			new List<string>
			{
				"Languages",
				"About",
				".git"
			}
		};

		// Token: 0x04003427 RID: 13351
		private Dictionary<string, string> truncatedStringCache = new Dictionary<string, string>();
	}
}
