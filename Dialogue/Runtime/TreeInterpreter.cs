using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInterpreter : MonoBehaviour
{
    public DialogueContainer _tree;

    [HideInInspector] public RuntimeDialogueContainer runtimeContainer;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRuntimeContainer(_tree);
        //Run();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Run()
    {
        //GenerateRuntimeContainer(_tree);
        runtimeContainer.DialogueNodes.Find(x => x.nodeGUID == runtimeContainer.NodeLinks[0].TargetNodeGUID).Run(runtimeContainer);
    }

    public void GenerateRuntimeContainer(DialogueContainer container)
    {
        _tree = container;
        // Aqui cambiaremos las exposedProperties del runtimeContainer
        runtimeContainer = new RuntimeDialogueContainer();

        _tree.DialogueNodes.ForEach(x => runtimeContainer.DialogueNodes.Add(x.createCopy()));
        _tree.ExposedProperties.ForEach(x => runtimeContainer.ExposedProperties.Add(x.createDataCopy()));

        //runtimeContainer.ExposedProperties.AddRange(_tree.ExposedProperties);
        runtimeContainer.NodeLinks.AddRange(_tree.NodeLinks);
    }
}
