using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace CustomJournal
{
    public static class Util
    {
        static tk2dSprite cacheTk2d = null!;
        static SpriteRenderer cacheSpriteR = null!;
        static Camera cacheCamera = null!;
        public delegate void WithDelegate<T>(ref T self);

        public static T With<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
        public static T With<T>(this T self, WithDelegate<T> action)
        {
            action(ref self);
            return self;
        }
        private static void PrepareCamera()
        {
            var camGO = new GameObject("HKTool Cache Camera");
            cacheCamera = camGO.AddComponent<Camera>();
            cacheCamera.orthographic = true;
            cacheCamera.clearFlags = CameraClearFlags.Nothing;
            UObject.DontDestroyOnLoad(camGO);
        }
        public static void BuildCamera(UnityEngine.Bounds bounds)
        {
            if (cacheCamera == null) PrepareCamera();

            cacheCamera!.orthographicSize = bounds.size.y / 2;
            cacheCamera!.aspect = bounds.size.x / bounds.size.y;
            cacheCamera!.transform.position = bounds.center.With((ref Vector3 x) => x.z = bounds.min.z - 1);
        }
        public static Texture2D? Render(this GameObject go, int width, int height)
        {
            return Render(go, width, height, false);
        }
        public static Texture2D? Render(this GameObject go, int width, int height, bool includeChildren)
        {
            var render = go.GetComponent<Renderer>();
            if (render == null) return null;
            var origPos = go.transform.position;
            var origE = render.enabled;
            Renderer[] enabled = go.GetComponentsInChildren<Renderer>(false).Where(x => x.enabled && x.gameObject.activeInHierarchy).ToArray();
            if (!includeChildren)
            {
                go.transform.position = go.transform.position.With((ref Vector3 x) => x.z = -999999);
                foreach (var v in enabled) v.enabled = false;
            }
            render.enabled = true;
            UnityEngine.Bounds bounds = new();
            if (!includeChildren)
            {
                bounds = render.bounds;
            }
            else
            {
                bounds.min = new(enabled.Select(x => x.bounds.min.x).Min(), enabled.Select(x => x.bounds.min.y).Min(), render.bounds.min.z);
                bounds.max = new(enabled.Select(x => x.bounds.max.x).Max(), enabled.Select(x => x.bounds.max.y).Max(), render.bounds.max.z);
            }
            BuildCamera(bounds);

            var rtex = new RenderTexture(width, height, 0);
            rtex.Create();
            cacheCamera.targetTexture = rtex;
            cacheCamera.Render();
            go.transform.position = origPos;
            render.enabled = origE;
            if (!includeChildren)
            {
                foreach (var v in enabled) v.enabled = true;
            }
            var tex2d = new Texture2D(width, height);
            var prev = RenderTexture.active;
            RenderTexture.active = rtex;
            tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex2d.Apply();
            RenderTexture.active = prev;

            rtex.Release();
            return tex2d;
        }
        public static Texture2D ExtractSprite(Sprite sprite)
        {
            if (cacheSpriteR == null)
            {
                var renderGO = new GameObject("Sprite Renderer");
                renderGO.transform.position = new(0, 0, 55555);
                UObject.DontDestroyOnLoad(renderGO);
                cacheSpriteR = renderGO.AddComponent<SpriteRenderer>();
            }
            cacheSpriteR.sprite = sprite;

            var width = (int)(sprite.bounds.size.x * sprite.pixelsPerUnit);
            var height = (int)(sprite.bounds.size.y * sprite.pixelsPerUnit);

            cacheSpriteR.gameObject.SetActive(true);
            var tex2d = cacheSpriteR.gameObject.Render(width, height)!;
            cacheSpriteR.gameObject.SetActive(false);
            return tex2d;
        }
        public static Texture2D ExtractTk2dSprite(tk2dSpriteCollectionData def, int id)
        {
            var sdef = def.spriteDefinitions[id];

            if (cacheTk2d == null)
            {
                var renderGO = new GameObject("Tk2d Renderer", typeof(MeshRenderer), typeof(MeshFilter));
                renderGO.transform.position = new(0, 0, 55555);
                UObject.DontDestroyOnLoad(renderGO);
                cacheTk2d = tk2dSprite.AddComponent(renderGO, def, id);
            }
            cacheTk2d.SetSprite(def, id);
            var width = (int)((sdef.uvs.Max(x => x.x) - sdef.uvs.Min(x => x.x)) * sdef.material.mainTexture.width) + 1;
            var height = (int)((sdef.uvs.Max(x => x.y) - sdef.uvs.Min(x => x.y)) * sdef.material.mainTexture.height) + 1;
            if (sdef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
            {
                var o = width;
                width = height;
                height = o;
            }

            cacheTk2d.gameObject.SetActive(true);
            var tex2d = cacheTk2d.gameObject.Render(width, height)!;
            cacheTk2d.gameObject.SetActive(false);

            return tex2d;
        }
    }
}
