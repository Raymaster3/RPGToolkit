using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using System.Linq;

public class DialogueTreeEditor : EditorWindow
{
    public DialogueTreeView _graphView;
    private string fileName = "New narrative";

    [MenuItem("RPGToolkit/Dialogue Graph")]
    public static void OpenDialogueWindow()
    {
        var window = GetWindow<DialogueTreeEditor>();
        window.titleContent = new GUIContent("Dialogue Tree");
    }

    [OnOpenAsset(1)]
    public static bool OpenDialogueEditor(int instanceID, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceID).GetType() != typeof(DialogueContainer)) return false;
        bool windowIsOpen = EditorWindow.HasOpenInstances<DialogueTreeEditor>();

        DialogueTreeEditor editorWindow = null;

        if (!windowIsOpen)
        {
            editorWindow = EditorWindow.CreateWindow<DialogueTreeEditor>();
        }
        else
        {
            EditorWindow.FocusWindowIfItsOpen<DialogueTreeEditor>();
            editorWindow = EditorWindow.GetWindow<DialogueTreeEditor>();
        }
        editorWindow.UpdateFileNameText(EditorUtility.InstanceIDToObject(instanceID).name);
        GraphSaveUtility.GetInstance(editorWindow._graphView).LoadGraph(EditorUtility.InstanceIDToObject(instanceID).name);

        // Window should now be open, proceed to next step to open file
        return false;
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolBar();
        GenerateMiniMap();
        GenerateBlackBoard();
    }

    private void GenerateBlackBoard()
    {
        var blackboard = new Blackboard(_graphView);
        BlackboardSection section = new BlackboardSection { title = "Exposed Properties" };
        section.canAcceptDrop = _selection =>
        {
            return true;
        };
        blackboard.Add(section);
        
        // Callback for when we press the add button
        blackboard.addItemRequested = _blackboard =>
        {
            var gm = new GenericMenu();
            gm.AddItem(new GUIContent("String"), false, () => _graphView.AddPropertyToBlackBoard(new StringProperty()));
            gm.AddItem(new GUIContent("Item"), false, () => _graphView.AddPropertyToBlackBoard(new ItemProperty()));
            gm.AddItem(new GUIContent("Quest"), false, () => _graphView.AddPropertyToBlackBoard(new QuestProperty()));
            gm.AddItem(new GUIContent("Int"), false, () => _graphView.AddPropertyToBlackBoard(new IntProperty()));
            gm.ShowAsContext();
        };
        // Callback to rename properties
        blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.exposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property exists already", "OK");
                return;
            }

            var propertyIndex = _graphView.exposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            _graphView.exposedProperties[propertyIndex].PropertyName = newValue;
            _graphView.nodes.ToList().FindAll(x =>
            {
                PropertyNodeView nodeView = x as PropertyNodeView;
                if (nodeView == null) return false;
                return nodeView.title == oldPropertyName;
            }).ForEach(x => {
                (x as PropertyNodeView).getProperty().PropertyName = newValue;
                x.title = newValue;
            });
            ((BlackboardField)element).text = newValue;
            
        };

        /*blackboard.moveItemRequested += (_blackboard, index, element) =>
        {
            int curIndex = 0;
            for (int i = 0; i < _blackboard[0].childCount; i++)
                if (_blackboard[0][i] == element) curIndex = i;

            // Updating properties position
            ExposedProperty tmp = _graphView.exposedProperties[curIndex];
            _graphView.exposedProperties[curIndex] = _graphView.exposedProperties[index];
            _graphView.exposedProperties[index] = tmp;
        };*/

        blackboard.SetPosition(new Rect(10, 30, 400, 300));

        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap {
            anchored = true
        };
        var coords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
    
    private void ConstructGraph()
    {
        _graphView = new DialogueTreeView(this)
        {
            name = "Dialogue Tree"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }
    public void UpdateFileNameText(string newValue)
    {
        fileName = newValue;
        var fileNameTextField = rootVisualElement.Q<Toolbar>().Q<TextField>();
        fileNameTextField.SetValueWithoutNotify(newValue);
    }
    private void GenerateToolBar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => { RequestDataOperation(true); }) { text = "Save Data" });
        toolbar.Add(new Button(() => { RequestDataOperation(false); }) { text = "Load Data" });

        /*var nodeCreationButton = new Button(() => {
            _graphView.CreateNode("Dialogue Node");
        });
        nodeCreationButton.text = "Create Node";
        toolbar.Add(nodeCreationButton);*/

        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "ok");
            return;
        }
        if (save) GraphSaveUtility.GetInstance(_graphView).SaveGraph(fileName);
        else GraphSaveUtility.GetInstance(_graphView).LoadGraph(fileName);
    }
}

public class Utils
{
    public static List<KeyValuePair<Type, Color>> typeColors = new List<KeyValuePair<Type, Color>>()
    {
        new KeyValuePair<Type, Color>(typeof(QuestData), Color.magenta),
        new KeyValuePair<Type, Color>(typeof(Item), Color.green)
    };    
}
