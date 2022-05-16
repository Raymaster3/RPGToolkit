using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueTreeView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public static List<int> selectedIndexes = new List<int>();

    private EditorWindow editorWindow;

    public Blackboard Blackboard;
    public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindowProvider;

    private static DialogueTreeView _instance;

    public static DialogueTreeView GetInstance()
    {
        return _instance;
    }

    public DialogueTreeView (EditorWindow window)
    {
        _instance = this;
        styleSheets.Add(Resources.Load<StyleSheet>("DialogTreeEditor"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);


        graphViewChanged = _changes => {
            OnGraphviewChanged(_changes);
            return _changes;
        };
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        RegisterCallback<DragUpdatedEvent>(evt => { OnDragUpdatedEvent(evt); });
        RegisterCallback<DragPerformEvent>(evt => { OnDragPerformedEvent(evt); });

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(window);
        editorWindow = window;
    }


    private void OnDragUpdatedEvent(DragUpdatedEvent evt)
    {

        if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> selection && (selection.OfType<VisualElement>().Count() >= 0))
        {
            DragAndDrop.visualMode = evt.actionKey ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Move;
        }
    }
    private void OnDragPerformedEvent(DragPerformEvent evt)
    {
        foreach (int index in selectedIndexes)
        {
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(evt.localMousePosition);
            PropertyNodeView propertyNodeView = new PropertyNodeView();
            propertyNodeView.createNodeVisual(new PropertyNode
            {
                nodeGUID = Guid.NewGuid().ToString(),
                Position = localMousePosition,
                property = exposedProperties[index].createDataCopy()
            });
            AddElement(propertyNodeView);
        }
    }
    private void AddSearchWindow(EditorWindow window)
    {
        _searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindowProvider.Init(window, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindowProvider);
    }
    public void OpenSearchWindow()
    {
        _searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindow>();
    }

    private void OnGraphviewChanged(GraphViewChange changes)
    {
        if (changes.elementsToRemove != null)
        {
            foreach (GraphElement element in changes.elementsToRemove)
            {
                MyBlackBoardField field = element as MyBlackBoardField;
                if (field != null)
                {
                    exposedProperties.RemoveAt(field.index);
                    nodes.ToList().FindAll(x =>
                    {
                        PropertyNodeView view = x as PropertyNodeView;
                        if (view == null) return false;
                        return view.getProperty().PropertyName == field.text;
                    }).ForEach(x => RemoveElement(x));
                }
                Edge edge = element as Edge;
                if (edge == null) continue;
                PropertyNodeView outputNode = edge.output.node as PropertyNodeView;
                if (outputNode == null) continue;
                (edge.input.node as NodeView).UnSetInput(edge.input);
            }
        }

        if (changes.edgesToCreate == null) return;
        foreach (Edge edge in changes.edgesToCreate)
        {
            PropertyNodeView outputNode = edge.output.node as PropertyNodeView;
            if (outputNode != null)
            {
                for (int i = 0; i < edge.input.node.inputContainer.childCount; i++)
                {
                    Port port = edge.input.node.inputContainer[i].Q<Port>();
                    if (port == edge.input)
                    {
                        exposedProperties.Find(x => x.PropertyName == outputNode.getProperty().PropertyName)
                            .valueChangeCallback += (newValue) => {
                            (edge.input.node as NodeView).SetInputValue(i, newValue);
                        };
                        (edge.input.node as NodeView).SetInputValue(i, outputNode.getProperty().getValue());
                    }
                }
            }
        }
    }

    private Port GeneratePort(NodeView node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private NodeView GenerateEntryPointNode()
    {
        var node = new NodeView {
            title = "ROOT",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true
        };
        Port port = GeneratePort(node, Direction.Output);
        port.portName = "Next";
        node.outputContainer.Add(port);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void ClearBlackBoard()
    {
        exposedProperties.Clear();
        Blackboard.Clear();
        Blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
    }
   
    internal void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;

        while (exposedProperties.Any(x => x.PropertyName == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
         
        var property = exposedProperty.createDataCopy();
        property.PropertyName = localPropertyName;

        property.valueChangeCallback += (newValue) => {
            nodes.ToList().FindAll(x => {
                PropertyNodeView nodeView = x as PropertyNodeView;
                if (nodeView == null) return false;
                return nodeView.getProperty().PropertyName == property.PropertyName;
            }).ForEach(x => {
                PropertyNodeView node = x as PropertyNodeView;
                node.getProperty().setValue(newValue);
                foreach (Edge edge in node.outputContainer[0].Q<Port>().connections)
                {
                    (edge.input.node as NodeView).SetInputValue(edge.output, newValue);
                }
            });
        };
        exposedProperties.Add(property);

        var container = new VisualElement();
        var blackboardField = new MyBlackBoardField { text = property.PropertyName, typeText = property.getType().ToString(), index = Blackboard[0].childCount};

        container.Add(blackboardField);
        
        container.Add(property.getBlackBoardRow(blackboardField));

        Blackboard[0].Add(container);
        //Blackboard.Add(container);
        Blackboard.UpdatePresenterPosition();
    }

    public void CreateNode(string nodeName, Vector2 position)
    {
        AddElement(CreateDialogNode(nodeName, position));
    }
    public void CreateNode(string nodeName, Vector2 position, string type)
    {
        switch (type)
        {
            case "Dialogue Node":
                AddElement(CreateDialogNode(nodeName, position));
                break;
            case "Property Node":

                break;
            case "End Node":
                break;
            default:
                break;
        }
    }
    public void AddNode(NodeView node)
    {
        AddElement(node);
    }

    public NodeView CreateNewNode(NodeData data)
    {
        switch (data.getType())
        {
            case "Dialogue Node":
                DialogNodeView nodeView = new DialogNodeView();
                return nodeView.createNodeVisual(data);
            case "Property Node":
                PropertyNodeView propertyNode = new PropertyNodeView();
                return propertyNode.createNodeVisual(data);
            case "End Node":
                EndNodeView endNode = new EndNodeView();
                return endNode.createNodeVisual(data);
            case "Give Mission Node":
                GiveMissionNodeView giveMissionNode = new GiveMissionNodeView();
                return giveMissionNode.createNodeVisual(data);
            case "Next Node":
                NextNodeView nextNode = new NextNodeView();
                return nextNode.createNodeVisual(data);
            case "Give Item Node":
                GiveItemNodeView giveItemNode = new GiveItemNodeView();
                return giveItemNode.createNodeVisual(data);
            default: 
                return new NodeView();
        }
    }


    public DialogNodeView CreateDialogNode(string nodeName, Vector2 position)
    {
        var dialogueNode = new DialogNodeView
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };
        var inputPort = dialogueNode.GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => {AddChoicePort(dialogueNode);});
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(DialogNodeView dialogueNode, string overritenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        /*oldLabel.RegisterValueChangedCallback(evt => {
            oldLabel.text = "  ";
        });*/

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputPortName = $"Choice {outputPortCount}";

        var portName = string.IsNullOrEmpty(overritenPortName) ? $"Choice{outputPortCount + 1}" : overritenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = portName
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort)) { text = "X"};
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = portName;
        //oldLabel.text = "  ";
        generatedPort.AddManipulator(new EdgeConnector<Edge>(new MyIEdgeConnector(this)));
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    private void RemovePort(NodeView dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (!targetEdge.Any()) return;
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) => {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction && startPort.portType == port.portType)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }
    
    
}
