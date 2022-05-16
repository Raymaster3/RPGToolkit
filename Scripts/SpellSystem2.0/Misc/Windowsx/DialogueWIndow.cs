using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWIndow : Window
{
    [HideInInspector] public DialogNode currentNode;
    [HideInInspector] public RuntimeDialogueContainer context;


    public Text dialogueTextHolder;
    public GameObject choicesHolder;

    public override void Open()
    {
        gameObject.SetActive(true);
        Clear();
        Populate();
    }
    public override void Populate()
    {
        Clear();
        List<NodeLink> connections = context.NodeLinks.FindAll(x => x.BaseNodeGUID == currentNode.nodeGUID);

        dialogueTextHolder.text = currentNode.DialogueText;

        //int i = 0;
        for (int i = 0; i < connections.Count; i++)
        {
            NodeLink connection = connections.Find(x => x.originPort == i);
            if (i == 0)
            {
                // Just set the first element
                Button button = choicesHolder.transform.GetChild(0).GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.GetComponentInChildren<Text>().text = connection.PortName;
                button.onClick.AddListener(() => { context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID).Run(context); });
            }
            else
            {
                GameObject choice = Instantiate(choicesHolder.transform.GetChild(0).gameObject, choicesHolder.transform); // Create a button for the choice
                Button but = choice.GetComponent<Button>();
                but.GetComponentInChildren<Text>().text = connection.PortName;
                but.onClick.AddListener(() => { context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID).Run(context); });
            }
        }


        /*foreach (NodeLink connection in connections)
        {
            if (i == 0)
            {
                // Just set the first element
                Button button = choicesHolder.transform.GetChild(0).GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.GetComponentInChildren<Text>().text = connection.PortName;
                button.onClick.AddListener(() => { context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID).Run(context); });
            }
            else
            {
                GameObject choice = Instantiate(choicesHolder.transform.GetChild(0).gameObject, choicesHolder.transform); // Create a button for the choice
                Button but = choice.GetComponent<Button>();
                but.GetComponentInChildren<Text>().text = connection.PortName;
                but.onClick.AddListener(() => { context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID).Run(context); });
            }
            i++;
        }*/
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
    public override bool isOpened()
    {
        return gameObject.activeSelf;
    }
    private void Clear()
    {
        int i = 0;
        // Clear all buttons but the first one, it'll act as a prefab
        foreach (Transform child in choicesHolder.transform)
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            Destroy(child.gameObject);
            i++;
        }
    }
}
