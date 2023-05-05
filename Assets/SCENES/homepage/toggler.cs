using UnityEngine;
using UnityEngine.UI;

public class toggler : MonoBehaviour
{
    public TextChanger textChanger;

    private Text buttonText;
    private Color originalColor;
    private FontStyle originalFontStyle;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);

        // Get the Text component of the button
        buttonText = GetComponentInChildren<Text>();

        // Save the original color and font style of the text
        originalColor = buttonText.color;
        originalFontStyle = buttonText.fontStyle;
    }

    private void OnClick()
    {
        // Get the TextChanger component from the scene
        textChanger = GameObject.FindObjectOfType<TextChanger>();

        // Change the text in the thirdText component of the TextChanger
        

        // Set the color of the text to yellow
        buttonText.color = Color.yellow;

        // Make the text bold
        buttonText.fontStyle = FontStyle.Bold;
    }

    public void OnPlayButtonClick()
    {
        // Get the TextChanger component from the scene
        textChanger = GameObject.FindObjectOfType<TextChanger>();

        if (buttonText.color == originalColor && buttonText.fontStyle == originalFontStyle)
        {
            // Change the text in the thirdText component of the TextChanger
            textChanger.ChangeText("Serious Game");

            // Set the color of the text to yellow
            buttonText.color = Color.yellow;

            // Make the text bold
            buttonText.fontStyle = FontStyle.Bold;

            // Reset the visualize button's text style and color
            GameObject visualizeButton = GameObject.Find("VisualizeButton");
            Text visualizeButtonText = visualizeButton.GetComponentInChildren<Text>();
            visualizeButtonText.color = originalColor;
            visualizeButtonText.fontStyle = originalFontStyle;
        }
        else
        {
            // Revert the color and font style of the text
            ResetText();
        }
    }

    public void OnVisualizeButtonClick()
    {
        // Get the TextChanger component from the scene
        textChanger = GameObject.FindObjectOfType<TextChanger>();

        if (buttonText.color == originalColor && buttonText.fontStyle == originalFontStyle)
        {
            // Change the text in the thirdText component of the TextChanger
            textChanger.ChangeText("Atlas");

            // Set the color of the text to yellow
            buttonText.color = Color.yellow;

            // Make the text bold
            buttonText.fontStyle = FontStyle.Bold;

            // Reset the play button's text style and color
            GameObject playButton = GameObject.Find("PlayButton");
            Text playButtonText = playButton.GetComponentInChildren<Text>();
            playButtonText.color = originalColor;
            playButtonText.fontStyle = originalFontStyle;
        }
        else
        {
            // Revert the color and font style of the text
            ResetText();
        }
    }

    public void ResetText()
    {
        // Revert the color and font style of the text
        buttonText.color = originalColor;
        buttonText.fontStyle = originalFontStyle;
    }
}
