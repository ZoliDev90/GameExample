using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject paramPanel;      
    [SerializeField] private GameObject startPanel;      
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputField widthInput;    // Input field for grid width
    [SerializeField] private InputField heightInput;   // Input field for grid height



    [SerializeField] private GridHandler gridHandler; 


    public void OnOkButtonClick()
    {
        // Read values from the input fields
        int gridWidth = int.Parse(widthInput.text);
        int gridHeight = int.Parse(heightInput.text);

        // Hide the UI panel
        if (paramPanel != null)
        {
            paramPanel.SetActive(false);
        }

        // Call the SetupGrid function on GridHandler
        if (gridHandler != null)
        {
            gridHandler.setupGrid(gridWidth, gridHeight);
        }
        else
        {
            Debug.LogWarning("GridHandler is not assigned!");
        }
    }
}
