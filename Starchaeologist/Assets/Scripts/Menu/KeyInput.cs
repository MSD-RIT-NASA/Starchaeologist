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

    [SerializeField]
    private string searchNameEdit;

    private bool editingName;

    private bool scoreCanvasActive;

    public string NameEdit
    {
        get { return nameEdit; }
    }
    public string DateEdit
    {
        get { return dateEdit; }
    }
    public string SearchNameEdit
    {
        get { return searchNameEdit; }
    }

    public bool EditingName
    {
        get { return editingName; }
        set { editingName = value; }
    }
    public bool ScoreCanvasActive
    {
        get { return scoreCanvasActive; }
        set { scoreCanvasActive = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        editingName = true;
        scoreCanvasActive = true;
    }

    public void AddKeyInput(TMP_Text label)
    {
        if (!scoreCanvasActive)
        {
            if (label.text == "<[x]" && searchNameEdit.Length > 0)
            {
                string newText = searchNameEdit.Substring(0, searchNameEdit.Length - 1);
                searchNameEdit = newText;
            }
            else if(label.text == "<[x]")
            {

            }
            else
            {
                searchNameEdit += label.text;
            }
        }
        else
        {
            switch (editingName)
            {
                case true:
                    if (label.text == "<[x]" && nameEdit.Length > 0)
                    {
                        string newText = nameEdit.Substring(0, nameEdit.Length - 1);
                        nameEdit = newText;
                    }
                    else if (label.text == "<[x]")
                    {

                    }
                    else
                    {
                        nameEdit += label.text;
                    }
                    break;
                case false:
                    if (label.text == "<[x]" && dateEdit.Length > 0)
                    {
                        string newText = dateEdit.Substring(0, dateEdit.Length - 1);
                        dateEdit = newText;
                    }
                    else if (label.text == "<[x]")
                    {

                    }
                    else
                    {
                        dateEdit += label.text;
                    }
                    break;
            }
        }
    }
}
