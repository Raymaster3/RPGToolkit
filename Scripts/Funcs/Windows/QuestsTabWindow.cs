using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsTabWindow : Window
{
    public GameObject QuestsTab;

    private GameObject prefab;
    private void Start()
    {
        prefab = Instantiate(QuestsTab.transform.GetChild(0).gameObject);    // Cache the reference to the first element in the list
        Populate();
    }
    public override void Open()
    {
    }
    public override void Close()
    {
    }
    public override void Populate()
    {
        // We clear the list before refilling it with the new info
        foreach (Transform child in QuestsTab.transform)
        {
            Destroy(child.gameObject);
        }
        Quest[] quests = Player.instance.getActiveQuests();
        foreach (Quest q in quests)
        {
            GameObject go;
            go = Instantiate(prefab, QuestsTab.transform);
            Transform questInfoHolder = go.transform.GetChild(1);

            Text questTitle = go.transform.GetChild(0).GetComponent<Text>();
            Text questDescription = questInfoHolder.GetChild(0).GetComponent<Text>();
            Text questObjectiveCounter = questInfoHolder.GetChild(1).GetComponent<Text>();

            questTitle.text = q.name;
            questDescription.text = q.description;
            questObjectiveCounter.text = q.getObjectiveCounter()[0] + "/" + q.getObjectiveCounter()[1];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(QuestsTab.GetComponent<RectTransform>());
    }
    public override bool isOpened()
    {
        return true;
    }
}
