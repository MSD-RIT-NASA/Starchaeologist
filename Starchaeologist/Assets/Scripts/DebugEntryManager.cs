using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugEntryManager : MonoBehaviour
{
    /// <summary>
    /// Updates an entry with a given name in the debug canvas, or adds one if no entry with the given name exists.<br/><br/>
    /// <b>Arguments:</b><br/>
    /// - <see cref="string"/>: name/key of the entry<br/>
    /// - <see cref="string"/>: formatted value to display on the canvas (will be enclosed in quotation marks)<br/>
    /// - <see cref="float"/>: How long this entry will stay on the canvas. Timer resets on every update. If &lt;= 0, it will stay indefinitely.
    /// </summary>
    public static Action<string, string, float> updateEntry;
    public static Action<string> removeEntry;

    // ! Add an `|| true` to this preprocessor directive to make the DebugCanvas work in builds ! //
#if UNITY_EDITOR || true
    [SerializeField] private TextMeshProUGUI entryPrefab = null;

    /// <summary>
    /// A dictionary of spawned entries for the debug canvas to show, and their given names.
    /// </summary>
    private Dictionary<string, TextMeshProUGUI> spawnedEntries = new Dictionary<string, TextMeshProUGUI>(StringComparer.Ordinal);
    /// <summary>
    /// A dictionary of keys that correspond to <see cref="spawnedEntries"/>, where the values are the coroutines that will remove them
    /// </summary>
    private Dictionary<string, Coroutine> expirationTimers = new Dictionary<string, Coroutine>(StringComparer.Ordinal);

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

    private void OnUpdateEntry(string entryName, string formattedVal, float duration = -1)
    {
        if (spawnedEntries.ContainsKey(entryName))
        {
            spawnedEntries[entryName].text = $"{entryName} = \"{formattedVal}\"";

            if (expirationTimers.TryGetValue(entryName, out Coroutine timer))
                Coroutilities.TryStopCoroutine(this, timer);
            expirationTimers.Remove(entryName);
        }
        else
        {
            TextMeshProUGUI newEntry = Instantiate(entryPrefab, transform);
            spawnedEntries.Add(entryName, newEntry);

            newEntry.name = $"Entry \"{entryName}\"";
            newEntry.text = $"{entryName} = \"{formattedVal}\"";
        }

        if (duration >= 0)
        {
            Coroutilities.DoAfterDelay(this, () => removeEntry?.Invoke(entryName), duration);
        }
    }

    private void OnRemoveEntry(string entryName)
    {
        if (entryName != null)
        {
            if (spawnedEntries.TryGetValue(entryName, out TextMeshProUGUI entry))
                Destroy(entry);

            spawnedEntries.Remove(entryName);
        }
    }
#endif
}