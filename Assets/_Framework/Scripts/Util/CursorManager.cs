using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Util.Callbacks;

namespace _Framework.Scripts.Util
{
    public class CursorManager : Singleton<CursorManager>
    {
        private readonly Dictionary<GameObject, Tuple<int, bool>> registered = new Dictionary<GameObject, Tuple<int, bool>>();

        protected override void OnSingletonAwake()
        {
        }

        public void Add(GameObject go, int priority, bool cursorVisble)
        {
            registered[go] = Tuple.Create(priority, cursorVisble);
            Check();
        }
        public void AddAutoremove(GameObject go, int priority, bool cursorVisble)
        {
            go.GetOrAddComponent<OnDestroyCallback>().OnDestroyEvent += () => Remove(go);
            Add(go, priority, cursorVisble);
        }
        
        public void Remove(GameObject go)
        {
            if (registered.Remove(go))
            {
                Check();
            }
        }

        private void Check()
        {
           var showCursor = registered.Values
               .OrderByDescending(tuple => tuple.Item1)
               .FirstOrDefault()?.Item2 ?? true;
           
            if (showCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        public enum State
        {
            Default, ResizeHorizontal, ResizeVertical
        }
        
        [SerializeField] private Texture2D resizeHorizontal;
        [SerializeField] private Texture2D resizeVertical;

        public static void SetState(State state)
        {
            switch (state)
            {
                case State.Default:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case State.ResizeHorizontal:
                    Cursor.SetCursor(Instance.resizeHorizontal, GetTextureCenter(Instance.resizeHorizontal), CursorMode.Auto);
                    break;
                case State.ResizeVertical:
                    Cursor.SetCursor(Instance.resizeVertical, GetTextureCenter(Instance.resizeVertical), CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static Vector2 GetTextureCenter(Texture2D tex)
        {
            return new Vector2(tex.width, tex.height) / 2;
        }
    }
}