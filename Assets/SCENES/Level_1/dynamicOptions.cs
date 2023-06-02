using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;
public class dynamicOptions : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject buttonParent;
    [SerializeField]
    private TextAsset csvFile;

    public void Start()
    {
   
         string[] data = csvFile.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);

         for (int j = 0; j < data.Length; j++)
         {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = data[j];
         } 
            
    }
}
