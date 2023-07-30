using System;
using Modding;
using CustomKnight;
using System.Collections.Generic;
namespace CustomJournal
{
    public class CustomJournal:Mod,IGlobalSettings<GlobalSetting>,IMenuMod
    {
        public bool ToggleButtonInsideMenu => false;

        public static GlobalSetting GS = new();
        public void OnLoadGlobal(GlobalSetting s) => GS = s;
        public GlobalSetting OnSaveGlobal() => GS;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? wrappedToggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> menuEntries = new List<IMenuMod.MenuEntry>();
            menuEntries.Add(
                new IMenuMod.MenuEntry {

                    Description="Enable Dump Journal",
                    Name="Dump Enable",
                    Values=new string[] {"Enable","Disable"},
                    Saver=(s)=>GS.dumpenabled=s==0,
                    Loader=()=>GS.dumpenabled?0:1
                }
                );
            return menuEntries;
        }
        public override string GetVersion()
        {
            return "1.0.0.1";
        }
        public bool isCustomKnightInstalled()
        {
            return ModHooks.GetMod("CustomKnight") is Mod;
        }
        public override void Initialize()
        {
           if (isCustomKnightInstalled())
            {
                SkinManager.OnSetSkin += Reset;
                DumpManager.BeforeDumpingGameObject += DumpJ;
                SwapManager.OnApplySkinUsingProxy += SwapJournalM;
                On.HeroController.Start += InitJournal;
                ModHooks.BeforeSavegameSaveHook += UnsetEnter;
            }
        }

        private void UnsetEnter(SaveGameData obj)
        {
            dumped = false;
            swaped = false;
        }


        private void InitJournal(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);
            GameManager.instance.StartCoroutine( SwapJournal.SwapJour());
        }

        private void Reset(object sender, EventArgs e)
        {
            dumped = false;
            GameManager.instance.StartCoroutine(SwapJournal.SwapJour());
            swaped = true;
        }

        private void SwapJournalM(object sender, SwapEvent e)
        {
            if (!swaped)
            {
                GameManager.instance.StartCoroutine(SwapJournal.SwapJour());
                swaped = true;
            }
        }

        private void DumpJ(object sender, DumpEvent e)
        {
            if (!dumped&&GS.dumpenabled)
            {
                DumpJournal.DumpJournalImages();
                dumped = true;
            }
        }

        private static bool swaped = false;
        public static bool dumped = false;
    }
}
