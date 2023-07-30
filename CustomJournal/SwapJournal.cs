using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CustomKnight;
using System.IO;
using Satchel;
namespace CustomJournal
{
    public class SwapJournal
    {
        private static string SWAP_DIR = Path.Combine(SkinManager.DATA_DIR, "Swap");
        private static string journalPath = Path.Combine(SWAP_DIR, "Journal");
        private static List<GameObject> chidrenlist = new();
        private static void SwapSkinForGo(string objectpath, GameObject GO,ISelectableSkin skin = null)
        {
            string SWAP_DIR_SKIN = skin == null ? Path.Combine(SkinManager.GetDefaultSkin().getSwapperPath(), "Swap", "Journal"): Path.Combine(skin.getSwapperPath(), "Swap", "Journal"); ;
            var je = GO.GetComponent<JournalEntryStats>();
            SpriteRenderer sr = GO.GetComponent<SpriteRenderer>();
            string mainpath = File.Exists(Path.Combine(SWAP_DIR_SKIN, objectpath) + ".png") ? Path.Combine(SWAP_DIR_SKIN, objectpath) + ".png" : Path.Combine(journalPath, objectpath) + ".png";
            Modding.Logger.Log($"{mainpath}");
            if (je != null)
            {
                if (File.Exists(mainpath))
                {
                    DumpJournal.SaveTextureByPath(objectpath, Util.ExtractSprite(je.sprite), SkinManager.GetDefaultSkin());
                    Texture2D texture = new(2, 2);
                    texture.LoadImage(File.ReadAllBytes(mainpath));
                    GO.GetComponent<JournalEntryStats>().sprite= Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f), je.sprite.pixelsPerUnit);
                }
                else
                {
                    if(File.Exists(Path.Combine(SkinManager.GetDefaultSkin().getSwapperPath(), "Swap", "Journal",objectpath)+".png"))
                    {
                        mainpath = Path.Combine(SkinManager.GetDefaultSkin().getSwapperPath(), "Swap", "Journal", objectpath) + ".png";
                        Texture2D texture = new(2, 2);
                        texture.LoadImage(File.ReadAllBytes(mainpath));
                        je.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), je.sprite.pixelsPerUnit);
                    }
                }
            }
            else if(sr != null)
            {
                if (File.Exists(mainpath))
                {
                    DumpJournal.SaveTextureByPath(objectpath, Util.ExtractSprite(sr.sprite), SkinManager.GetDefaultSkin());
                    Texture2D texture = new(2, 2);
                    texture.LoadImage(File.ReadAllBytes(mainpath));
                    sr.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), sr.sprite.pixelsPerUnit);
                }
                else
                {
                    if (File.Exists(Path.Combine(SkinManager.GetDefaultSkin().getSwapperPath(), "Swap", "Journal", objectpath) + ".png"))
                    {
                        mainpath = Path.Combine(SkinManager.GetDefaultSkin().getSwapperPath(), "Swap", "Journal", objectpath) + ".png";
                        Texture2D texture = new(2, 2);
                        texture.LoadImage(File.ReadAllBytes(mainpath));
                        sr.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), sr.sprite.pixelsPerUnit);
                    }
                }
            }
        }
        public static IEnumerator SwapJour()
        {
            chidrenlist.Clear();
            yield return new WaitForSeconds(1);
            GameObject jounallist = GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").FindGameObjectInChildren("Journal").FindGameObjectInChildren("Enemy List");

            if (jounallist != null)
            {
#if DEBUG
                Modding.Logger.Log("Start SWap");
                Modding.Logger.Log($"{SkinManager.GetCurrentSkin().GetName()}");
#endif
                jounallist.FindAllChildren(chidrenlist);
                foreach (GameObject go in chidrenlist)
                {
                    Modding.Logger.Log(go.name);
                    SwapSkinForGo(go.GetPath(true).Replace(jounallist.GetPath(true)+"/", ""), go,SkinManager.GetCurrentSkin());
                    GameObject icon = go.FindGameObjectInChildren("Portrait");
                    if (icon != null)
                    {
                        SwapSkinForGo(icon.GetPath(true).Replace(jounallist.GetPath(true) + "/", ""), icon, SkinManager.GetCurrentSkin());
                    }
                }
            }
        }


    }
}
