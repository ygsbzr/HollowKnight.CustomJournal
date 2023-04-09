using System;
using Modding;
using CustomKnight;
namespace CustomJournal
{
    public class CustomJournal:Mod
    {
        public override string GetVersion()
        {
            return "1.0.0.0";
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
            }
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
            if (!dumped)
            {
                DumpJournal.DumpJournalImages();
                dumped = true;
            }
        }

        private static bool swaped = false;
        public static bool dumped = false;
    }
}
