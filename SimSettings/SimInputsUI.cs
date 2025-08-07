using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimInputsUI : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Text gridRows;
    [SerializeField] private Text gridCols;
    [SerializeField] private Text numberOfRovers;

    [Header("Scene References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject simRoot;
    [SerializeField] private SimManager simManager;

    private SimSettings settings = new SimSettings();
    void Awake()
    {
        
    }

    private void OnApply()
    {
        
    }
}
