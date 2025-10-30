using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ControlEntry
{
    public string action;
    public string key;
}

public class ControlsUI : MonoBehaviour
{
    public Transform gridParent; //The transform for texts below
    public TMP_FontAsset font;
    public int fontSize = 7;

    public ControlEntry[] entries = new ControlEntry[] //Array to hold all the control texts
    {   new ControlEntry { action = "Aksi", key = "Tombol" },
        new ControlEntry { action = "Bergerak", key = "WASD / Mouse" },
        new ControlEntry { action = "Interaksi", key = "E" },
        new ControlEntry { action = "Interaksi Tanaman", key = "F" },
        new ControlEntry { action = "Tas", key = "B" },
        new ControlEntry { action = "Info Kontrol", key = "H" },
        new ControlEntry { action = "Lewati Waktu", key = "E dekat Rumah" },
    };

    void Start()
    {
        foreach (var entry in entries)
        {
            GameObject row = new GameObject(entry.action, typeof(RectTransform));
            row.transform.SetParent(gridParent, false);

            HorizontalLayoutGroup hLayout = row.AddComponent<HorizontalLayoutGroup>();
            hLayout.childAlignment = TextAnchor.MiddleLeft;
            hLayout.spacing = 20;

            AddText(row.transform, entry.action, 300);
            AddText(row.transform, entry.key, 250);
        }
    }

    void AddText(Transform parent, string content, float width)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Left;

        if (font != null)
            text.font = font;

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
    }

}
