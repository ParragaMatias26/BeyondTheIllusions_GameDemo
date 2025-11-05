using TMPro;
using UnityEngine;

public class Develop_TextBox : DevelopInfo
{
    [SerializeField] TextMeshProUGUI text;
    public void UpdateText(string info) => text.text = info;
}
