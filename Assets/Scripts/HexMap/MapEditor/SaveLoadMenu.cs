using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using HexMap.MapEditor;

namespace HexMap.MapEditor
{
    public class SaveLoadMenu : MonoBehaviour
    {
        public HexGrid hexGrid;
        public Text menuLabel, actionButtonLabel;
        public InputField nameInput;
        public RectTransform listContent;
        public SaveLoadItem itemPrefab;

        private bool saveMode;

        public void Open(bool saveMode)
        {
            this.saveMode = saveMode;
            if (saveMode)
            {
                menuLabel.text = "Save Map";
                actionButtonLabel.text = "Save";
            }
            else
            {
                menuLabel.text = "Load Map";
                actionButtonLabel.text = "Load";
            }
            FillList();
            gameObject.SetActive(true);
            HexMapCamera.Locked = true;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            HexMapCamera.Locked = false;
        }

        public void Action()
        {
            string path = GetSelectedPath();
            if (path == null)
            {
                return;
            }
            if (saveMode)
            {
                Save(path);
            }
            else
            {
                Load(path);
            }
            Close();
        }

        public void Delete()
        {
            string path = GetSelectedPath();
            if (path == null)
            {
                return;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            nameInput.text = "";
            FillList();
        }

        public void SelectItem(string name)
        {
            nameInput.text = name;
        }

        private string GetSelectedPath()
        {
            var mapName = nameInput.text;
            if (mapName.Length == 0)
            {
                return null;
            }
            return Path.Combine(Application.persistentDataPath, mapName + ".map");
        }

        private void FillList()
        {
            for (int i = 0; i < listContent.childCount; i++)
            {
                Destroy(listContent.GetChild(i).gameObject);
            }
            string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
            Array.Sort(paths);
            for (int i = 0; i < paths.Length; i++)
            {
                SaveLoadItem item = Instantiate(itemPrefab);
                item.menu = this;
                item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
                item.transform.SetParent(listContent, false);
            }
        }

        /// <summary>
        /// Save current hex map
        /// </summary>
        private void Save(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(2); // file head magic number
                hexGrid.Save(writer);
            }
        }

        /// <summary>
        /// Load hex map
        /// </summary>
        private void Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File does not exist " + path);
                return;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int header = reader.ReadInt32();
                if (header <= 2)
                {
                    hexGrid.Load(reader, header);
                    HexMapCamera.ValidatePosition();
                }
                else
                {
                    Debug.LogWarning("Unknown map format " + header);
                }
            }
        }
    }
}
