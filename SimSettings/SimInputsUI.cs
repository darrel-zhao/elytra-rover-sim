using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimInputsUI : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button createPathsButton;
    [SerializeField] private InputField gridRows;
    [SerializeField] private InputField gridCols;
    [SerializeField] private InputField numberOfRovers;
    [SerializeField] private InputField numTrashItems;


    [Header("Scene References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject simRoot;
    [SerializeField] private SimManager simManager;

    private SimSettings settings = new SimSettings();
    void Awake()
    {
        gridRows.onEndEdit.AddListener(s =>
        {
            int.TryParse(s, out settings.gridMapRows);

            if (settings.gridMapRows < 2)
            {
                settings.gridMapRows = 2;
            }
        });

        gridCols.onEndEdit.AddListener(s =>
        {
            int.TryParse(s, out settings.gridMapCols);

            if (settings.gridMapCols < 2)
            {
                settings.gridMapCols = 2;
            }
        });

        numberOfRovers.onEndEdit.AddListener(s =>
        {
            // tries to convert s into integer, sets to 0 if fails
            int.TryParse(s, out settings.numberOfRovers);

            // ensures that number of rovers is at least 1
            if (settings.numberOfRovers < 1)
            {
                settings.numberOfRovers = 1;
            }
        });

        numTrashItems.onEndEdit.AddListener(s =>
        {
            // tries to convert s into integer, sets to 0 if fails
            int.TryParse(s, out settings.numTrashItems);

            // ensures that number of trash items is at least 1
            if (settings.numTrashItems < 1)
            {
                settings.numTrashItems = 1;
            }
        });

        startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        settingsPanel.SetActive(false);
        simManager.SetSettings(settings);
    }
}
