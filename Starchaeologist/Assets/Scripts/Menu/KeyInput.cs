using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyInput : MonoBehaviour
{
    [SerializeField]
    private List<Button> keys;

    [SerializeField]
    private Button keyA;

    [SerializeField]
    private string nameEdit;

    [SerializeField]
    private string dateEdit;

    private bool editingName;

    public string NameEdit
    {
        get { return nameEdit; }
    }

    public string DateEdit
    {
        get { return dateEdit; }
    }

    public bool EditingName
    {
        get { return editingName; }
        set { editingName = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        editingName = true;
    }

    public void AddKeyInput(TMP_Text label)
    {
        switch (editingName) {
            case true:
                if (label.text == "<[x]" && nameEdit.Length > 0)
                {
                    string newText = nameEdit.Substring(0, nameEdit.Length - 1);
                    nameEdit = newText;
                }
                else {
                    nameEdit += label.text;
                }
                break;
            case false:
                if (label.text == "<[x]" && dateEdit.Length > 0)
                {
                    string newText = dateEdit.Substring(0, dateEdit.Length - 1);
                    dateEdit = newText;
                }
                else
                {
                    dateEdit += label.text;
                }
                break;
        }
    }
}
