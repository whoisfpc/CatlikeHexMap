using UnityEngine;
using UnityEngine.UI;

namespace HexMap.MapEditor
{
    public class SaveLoadItem : MonoBehaviour
    {
        public SaveLoadMenu menu;
        public Text nameText;

        public string MapName
        {
            get
            {
                return mapName;
            }
            set
            {
                mapName = value;
                nameText.text = value;
            }
        }

        string mapName;

        public void Select()
        {
            menu.SelectItem(mapName);
        }
    }
}
