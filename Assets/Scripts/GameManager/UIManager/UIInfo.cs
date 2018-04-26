using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameManager
{
    public class UIInfo
    {
        protected string name;
        public string Name { get { return name; } }
        public string Type { get; set; }

        protected GameObject templet;
        public GameObject Templet { get { return templet; } }

        protected GameObject gameObject;
        public GameObject GameObject { get { return gameObject; } }

        protected Transform parent;
        public Transform Parent { get { return parent; } }

        protected Camera renderCamera;

        protected List<UIInfo> relationalUIList;
        public List<UIInfo> RelationalUIList { get { return relationalUIList; } }

        public UIInfo(string name, GameObject templet, Camera renderCamera)
        {
            this.name = name;
            this.templet = templet;
            this.renderCamera = renderCamera;
            relationalUIList = new List<UIInfo>();
        }

        public bool Open()
        {
            if (!gameObject)
            {
                gameObject = Object.Instantiate(templet);
                gameObject.name = name;

                if (parent)
                {
                    gameObject.transform.SetParent(parent, false);
                }

                var canvas = gameObject.GetComponent<Canvas>();
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    if (renderCamera != null && renderCamera.gameObject.activeSelf)
                    {
                        canvas.worldCamera = renderCamera;
                    }
                    else
                    {
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    }
                }
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            for (int i = 0; i < relationalUIList.Count; i++)
            {
                relationalUIList[i].Open();
            }
            return true;
        }

        public bool Close(bool destroy = true, bool unload = false)
        {
            for (int i = 0; i < relationalUIList.Count; i++)
            {
                relationalUIList[i].Close(destroy, false);
            }

            if (destroy)
            {
                Object.DestroyImmediate(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }

            if (unload)
            {
                templet = null;
            }
            return true;
        }

        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }
        public void SetCamera(Camera camera)
        {
            this.renderCamera = camera;
        }
        public void AddRelationalUI(UIInfo uiInfo)
        {
            relationalUIList.Add(uiInfo);
        }

        public void TryAddRelationalUI(UIInfo uiInfo)
        {
            string noSuffixName = name;
            int index = noSuffixName.LastIndexOf('.');
            if (index != -1)
            {
                noSuffixName = noSuffixName.Substring(0, index);
            }

            if (uiInfo.name.Length > name.Length && uiInfo.name.LastIndexOf('_') == noSuffixName.Length && uiInfo.name.Substring(0, noSuffixName.Length) == noSuffixName)
            {
                AddRelationalUI(uiInfo);
            }
        }

        public void TryAddRelationalUI(List<UIInfo> uiInfoPool)
        {
            if (uiInfoPool == null) return;
            for (int i = 0; i < uiInfoPool.Count; i++)
            {
                TryAddRelationalUI(uiInfoPool[i]);
            }
        }
    }
}
