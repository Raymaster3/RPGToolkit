using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueTreeView _graphView;
    private EditorWindow _window;
    private Texture2D _identationIcon;

    public void Init(EditorWindow window, DialogueTreeView graphView) 
    {
        _graphView = graphView;
        _window = window;

        // Identation hack for search window as a transparent icon
        _identationIcon = new Texture2D(1, 1);
        _identationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _identationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry> {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Dialoge Node", _identationIcon))
                {
                    userData = new DialogNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Property Node", _identationIcon))
                {
                    userData = new PropertyNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("End Node", _identationIcon))
                {
                    userData = new EndNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Next Node", _identationIcon))
                {
                    userData = new NextNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Give Quest Node", _identationIcon))
                {
                    userData = new GiveMissionNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Give Item Node", _identationIcon))
                {
                    userData = new GiveItemNodeView(), level = 2
                },
                new SearchTreeEntry(new GUIContent("Hello world")) 
        };
        return tree;
    }  

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousPos = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePos = _graphView.contentViewContainer.WorldToLocal(worldMousPos);

        switch(SearchTreeEntry.userData)
        {
            case DialogNodeView dialogueNode:
                //_graphView.CreateNode("Dialogue Node", localMousePos);
                _graphView.AddElement(dialogueNode.createNodeVisual(new DialogNode
                {
                    nodeGUID = Guid.NewGuid().ToString(),
                    Position = localMousePos,
                    DialogueText = "Dialogue Node"
                }));
                return true;
            case PropertyNodeView nodeView:
                ExposedProperty prop = new StringProperty { PropertyName = "String Property" };
                prop.setValue("Hola");
                _graphView.AddElement(nodeView.createNodeVisual(new PropertyNode { 
                    nodeGUID = Guid.NewGuid().ToString(), 
                    property = _graphView.exposedProperties.Count > 0 ? _graphView.exposedProperties[0] : prop,  
                    Position = localMousePos
                }));
                return true;
            case EndNodeView nodeView:
                _graphView.AddElement(nodeView.createNodeVisual(new EndNode
                {
                    nodeGUID = Guid.NewGuid().ToString(),
                    Position = localMousePos,
                }));
                return true;

            case GiveMissionNodeView nodeView:
                _graphView.AddElement(nodeView.createNodeVisual(new GiveMissionNode
                {
                    nodeGUID = Guid.NewGuid().ToString(),
                    Position = localMousePos,
                    quest = null
                }));
                return true;
            case NextNodeView nodeView:
                _graphView.AddElement(nodeView.createNodeVisual(new NextNode
                {
                    nodeGUID = Guid.NewGuid().ToString(),
                    Position = localMousePos,
                }));
                return true;
            case GiveItemNodeView nodeView:
                _graphView.AddElement(nodeView.createNodeVisual(new GiveItemNode
                {
                    nodeGUID = Guid.NewGuid().ToString(),
                    Position = localMousePos,
                    item = null
                }));
                return true;
            default:
                return false;

        }
    }
}
