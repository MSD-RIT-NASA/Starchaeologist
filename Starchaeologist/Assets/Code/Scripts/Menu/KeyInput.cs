using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class KeyInput : MonoBehaviour
{
    /// <summary>
    /// Input field that the keyboard is currently typing in
    /// </summary>
    public TMP_InputField EditingField { get; private set; }

    /// <summary>
    /// Indicates how recently the user interacted with the keyboard. If the timer is 0 when the input field is deselected,
    /// the user did not recently interact with the keyboard so we can disable it.
    /// Based on how Unity events work, it must be set to 2 when the user interacts with the keyboard in order for it to stay on screen.
    /// (This timer basically creates its own "selection system" to keep the keyboard on screen when pushing keys.)
    /// </summary>
    private int refocusTimer = 0;

    /// <summary>
    /// Activates the keyboard
    /// </summary>
    /// <param name="editingField">Input field to type in</param>
    public void BeginEditing(TMP_InputField editingField)
    {
        EditingField = editingField;
        gameObject.SetActive(true);
        refocusTimer = 2;
    }

    /// <summary>
    /// Manually maintains focus on the editing field and ensures that the keyboard stays on screen
    /// </summary>
    public void ContinueEditing()
    {
        refocusTimer = 2;
        if (EditingField != null)
            EventSystem.current.SetSelectedGameObject(EditingField.gameObject);
    }

    /// <summary>
    /// Attempts to stop editing the input field. If the user has not just clicked a key on the keyboard,
    /// then the keyboard will disappear and the input field will fully be deselected.
    /// </summary>
    public void EndEditing()
    {
        StartCoroutine(AttemptDeactivate());
    }

    /// <summary>
    /// Every frame, decrement the refocus timer
    /// </summary>
    private void Update()
    {
        if (refocusTimer > 0)
            refocusTimer--;
    }

    /// <summary>
    /// Reselects the input field and adds a character to it
    /// </summary>
    /// <param name="label">Text component containing the character to add</param>
    public void AddKeyInput(TMP_Text label)
    {
        if (EditingField != null)
        {
            // Maintain focus on the editing field
            ContinueEditing();

            // Simulate a capital letter key press
            Event e = Event.KeyboardEvent(label.text);
            e.character = char.ToUpper(e.character);
            EditingField.ProcessEvent(e);
            EditingField.ForceLabelUpdate();
        }
    }

    /// <summary>
    /// Reselects the input field and removes a character from it (emulating the backspace key)
    /// </summary>
    public void RemoveKeyInput()
    {
        if (EditingField != null)
        {
            // Maintain focus on the editing field
            ContinueEditing();

            // Simulate a backscape key press
            Event e = Event.KeyboardEvent("backspace");
            EditingField.ProcessEvent(e);
            EditingField.ForceLabelUpdate();
        }
    }

    /// <summary>
    /// Whenever the input field is deselected, wait one frame for events to propagate and then
    /// deactivate the keyboard if the input field has truly been deselected (i.e. if the user clicked out of the keyboard)
    /// </summary>
    private IEnumerator AttemptDeactivate()
    {
        yield return new WaitForEndOfFrame();
        if (EditingField == null || refocusTimer == 0)
        {
            EditingField = null;
            gameObject.SetActive(false);
        }
    }
}
