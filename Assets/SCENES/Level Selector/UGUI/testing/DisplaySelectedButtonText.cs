using UnityEngine;
using UnityEngine.UI;

public class DisplaySelectedButtonText : MonoBehaviour
{
    public Text selectedButtonText;

    void Start()
    {
        // Retrieve the selected button's text from PlayerPrefs and display it in the text field
        if (PlayerPrefs.HasKey("SelectedButton"))
        {
            string selectedButton = PlayerPrefs.GetString("SelectedButton");
            selectedButtonText.text = selectedButton;
        }
    }
}
