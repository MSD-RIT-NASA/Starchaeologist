using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugEntryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI entryPrefab = null;

    /// <summary>
    /// A dictionary of spawned entries for the debug canvas to show, and their given names.
    /// </summary>
    private Dictionary<string, TextMeshProUGUI> spawnedEntries = new Dictionary<string, TextMeshProUGUI>(StringComparer.Ordinal);

    /// <summary>
    /// Updates an entry with a given name in the debug canvas, or adds one if no entry with the given name exists.<br/><br/>
    /// <b>Arguments:</b><br/>
    /// - <see cref="string"/>: name/key of the entry<br/>
    /// - <see cref="string"/>: formatted value to display on the canvas (will be enclosed in quotation marks)
    /// </summary>
    public static Action<string, string> updateEntry;
    public static Action<string> removeEntry;

    private void OnEnable()
    {
        updateEntry += OnUpdateEntry;
        removeEntry += OnRemoveEntry;
    }
    private void OnDisable()
    {
        updateEntry -= OnUpdateEntry;
        removeEntry -= OnRemoveEntry;
    }

    private void OnUpdateEntry(string entryName, string formattedVal)
    {
        if (spawnedEntries.ContainsKey(entryName))
        {
            spawnedEntries[entryName].text = $"{entryName} = \"{formattedVal}\"";
        }
        else
        {
            TextMeshProUGUI newEntry = Instantiate(entryPrefab, transform);
            spawnedEntries.Add(entryName, newEntry);

            newEntry.name = $"Entry \"{entryName}\"";
            newEntry.text = $"{entryName} = \"{formattedVal}\"";
        }
    }

    private void OnRemoveEntry(string entryName)
    {
        if (entryName != null)
        {
            spawnedEntries.Remove(entryName);
        }
    }
}