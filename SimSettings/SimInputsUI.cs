using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimInputsUI : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private InputField gridRows;
    [SerializeField] private InputField gridCols;
    [SerializeField] private InputField numberOfRovers;
    [SerializeField] private InputField numTrashItems;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button startButton;

    [Header("Scene References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pathsPanel;
    [SerializeField] private GameObject simRoot;
    [SerializeField] private SimManager simManager;

    [Header("Dynamic Rows")]
    [SerializeField] Transform rowsParent;
    [SerializeField] GameObject rowPrefab; 

    private SimSettings settings = new SimSettings();
    readonly List<RoverPathRow> _rows = new List<RoverPathRow>();

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

        continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnContinueClicked()
    {
        settingsPanel.SetActive(false);
        pathsPanel.SetActive(true);

        BuildRows();
        startButton.onClick.AddListener(OnStartClicked);
    }

    private void BuildRows()
    {
        // Clear existing rows
        for (int i = rowsParent.childCount - 1; i >= 0; i--)
            Destroy(rowsParent.GetChild(i).gameObject);

        _rows.Clear();

        // Create new rows based on the number of rovers
        for (int i = 0; i < settings.numberOfRovers; i++)
        {
            var rowGO = Instantiate(rowPrefab, rowsParent);
            var row = rowGO.GetComponent<RoverPathRow>();

            // Set up row fields
            row.roverIdText.text = $"Rover {i}";
            row.startField.placeholder.GetComponent<TMP_Text>().text = $"start node";
            row.endField.placeholder.GetComponent<TMP_Text>().text = $"end node";

            row.startField.contentType = TMP_InputField.ContentType.IntegerNumber;
            row.endField.contentType = TMP_InputField.ContentType.IntegerNumber;

            _rows.Add(row);
        }
    }

    public List<(int start, int end)> ReadAssignments()
    {
        var result = new List<(int, int)>(_rows.Count);
        for (int i = 0; i < _rows.Count; i++)
        {
            print("start: " + _rows[i].startField.text + ", end: " + _rows[i].endField.text);
            int s = ParseSafe(_rows[i].startField.text);
            int e = ParseSafe(_rows[i].endField.text);
            result.Add((s, e));
        }

        return result;
    }

    int ParseSafe(string t) => int.TryParse(t, out var v) ? v : -1;

    private void OnStartClicked()
    {
        // update simSettings
        settings.assignments = ReadAssignments();
        if (settings.assignments.Count != settings.numberOfRovers)
        {
            Debug.LogWarning("Number of assignments does not match number of rovers.");
        }
        
        pathsPanel.SetActive(false);
        simManager.SetSettings(settings);
    }
}
