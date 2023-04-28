using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour
{
    public Text thirdText;

    public void ChangeText(string newText)
    {
        thirdText.text = newText;
    }
}
