using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimInputsUI : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private InputField gridRows;
    [SerializeField] private InputField gridCols;
    [SerializeField] private InputField numberOfRovers;

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

            if (settings.gridMapRows < 1)
            {
                settings.gridMapRows = 1;
            }
        });
    }

    private void OnApply()
    {
        
    }
}
