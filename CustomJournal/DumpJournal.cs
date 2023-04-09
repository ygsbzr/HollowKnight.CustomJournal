using UnityEngine;
using Satchel;
using CustomKnight;
using System.IO;
using System.Collections.Generic;
using static Satchel.IoUtils;
namespace CustomJournal
{
    public class DumpJournal
    {
        private static List<GameObject> childrenlist = new();
        private static Dictionary<string, bool> isTextureDumped = new();
        private static string DUMP_DIR = Path.Combine(SkinManager.DATA_DIR, "Dump");
        private static string journalPath = Path.Combine(DUMP_DIR, "Journal");
        private static GameObject jounallist = GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").FindGameObjectInChildren("Journal").FindGameObjectInChildren("Enemy List");
        public static void DumpJournalImages()
        {
           

            if (jounallist != null)
            {
                Modding.Logger.Log($"JournalList:{jounallist.GetPath(true)}");
                jounallist.FindAllChildren(childrenlist);
                foreach (GameObject go in childrenlist)
                {
                    Modding.Logger.Log($"go:{go.GetPath(true)}");
                    if(go.GetComponent<JournalEntryStats>() != null)
                    {
                        string name = go.GetPath(true).Replace(jounallist.GetPath(true) +"/", "");
                        Sprite mainsp = go.GetComponent<JournalEntryStats>().sprite;
                        SaveTextureByPath(name, Util.ExtractSprite(mainsp));
                    }
                    GameObject child = go.FindGameObjectInChildren("Portrait");
                    if(child != null)
                    {
                        Sprite icon = child.GetComponent<SpriteRenderer>().sprite;
                        SaveTextureByPath(child.GetPath(true).Replace(jounallist.GetPath(true) +"/", ""), Util.ExtractSprite(icon));
                    }
                }
            }
        }
        internal static void SaveTextureByPath(string objectName, Texture2D texture,ISelectableSkin skin=null)
        {
            
            if(skin != null)
            {
                DUMP_DIR = Path.Combine(skin.getSwapperPath(), "Swap");
                journalPath =Path.Combine(skin.getSwapperPath(),"Swap","Journal");
            }
            else
            {
                DUMP_DIR = Path.Combine(SkinManager.DATA_DIR, "Dump");
                journalPath = Path.Combine(DUMP_DIR, "Journal");
            }
            EnsureDirectory(DUMP_DIR);
            EnsureDirectory(journalPath);

            string outpath = Path.Combine(journalPath, objectName.Replace('/', Path.DirectorySeparatorChar) + ".png");
            try
            {
                EnsureDirectory(Path.GetDirectoryName(outpath));
            }
            catch (IOException e)
            {
                Modding.Logger.Log(e.ToString());
            }
            if (!isTextureDumped.TryGetValue(outpath, out bool path) && !File.Exists(outpath))
            {
                Texture2D dupe = texture.isReadable ? texture : TextureUtils.duplicateTexture(texture);
                byte[] texBytes = dupe.EncodeToPNG();
                if (dupe != texture)
                {
                    GameObject.Destroy(dupe);
                }
                try
                {
                    File.WriteAllBytes(outpath, texBytes);
                }
                catch (IOException e)
                {
                   Modding.Logger.Log(e.ToString());
                }
                isTextureDumped[outpath] = true;
            }
        }
    }
}
