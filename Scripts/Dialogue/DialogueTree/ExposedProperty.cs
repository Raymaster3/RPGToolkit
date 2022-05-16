using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ExposedProperty
{
    public string PropertyName = "New Property";
    public bool exposed = false;

    public Action<object> valueChangeCallback = null;
    public virtual object getValue() {
        return "New String";
    }
    public virtual void setValue(object param)
    {

    }
    public virtual Type getType()
    {
        return typeof(string);
    }
    public virtual ExposedProperty createCopy()
    {
        return new ExposedProperty();
    }
    public virtual ExposedProperty createDataCopy()
    {
        ExposedProperty prop = createCopy();
        prop.PropertyName = PropertyName;
        prop.setValue(getValue());
        prop.exposed = exposed;
        return prop;
    }
    public virtual BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        return null;
    }
    public virtual VisualElement getRows()
    {
        VisualElement container = new VisualElement();

        var toggle = new Toggle("Exposed") { value = exposed };
        toggle.RegisterValueChangedCallback((evt) => { exposed = evt.newValue; });

        container.Add(toggle);
        //container.Add(getBlackBoardRow(field));

        return container;
    }
    public virtual object createInputMethod(object defaultValue)
    {
        return null;
    }
}

[System.Serializable]
public class StringProperty : ExposedProperty
{
    [SerializeField] private string PropertyValue;

    public override object getValue()
    {
        return PropertyValue;
    }
    public override ExposedProperty createCopy()
    {
        return new StringProperty();
    }
    public override void setValue(object param)
    {
        // Callback to update all nodes using this property
        PropertyValue = param as string;
        valueChangeCallback?.Invoke(PropertyValue);
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        var propertyValueTextField = new TextField("Value:")
        {
            value = PropertyValue 
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            setValue(evt.newValue);
        });
        var container = new VisualElement();

        container.Add(getRows());
        container.Add(propertyValueTextField);

        BlackboardRow row = new BlackboardRow(field, container);
        return row;
    }
    public override object createInputMethod(object defaultValue)
    {
        return GUILayout.TextArea(defaultValue as string);
    }

}
[System.Serializable]
public class ItemProperty : ExposedProperty
{
    [SerializeReference] private Item PropertyValue;

    public override object getValue()
    {
        return PropertyValue;
    }
    public override Type getType()
    {
        return typeof(Item);
    }
    public override ExposedProperty createCopy()
    {
        return new ItemProperty();
    }
    public override void setValue(object param)
    {
        PropertyValue = (Item)param;
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        object temp1 = PropertyValue;
        var objectField = new ObjectField("Value:")
        {
            objectType = typeof(Item),
            value = (UnityEngine.Object)temp1
        };
        objectField.RegisterValueChangedCallback(evt => {
            object temp = evt.newValue;
            PropertyValue = (Item)temp;
        });

        var container = new VisualElement();

        container.Add(getRows());
        container.Add(objectField);

        return new BlackboardRow(field, container);
    }
    public override object createInputMethod(object defaultValue)
    {
        return EditorGUILayout.ObjectField("", PropertyValue, typeof(Item), false);
    }
}
[System.Serializable]
public class QuestProperty : ExposedProperty
{
    [SerializeField] private QuestData PropertyValue;
    public override object getValue()
    {
        return PropertyValue;
    }
    public override Type getType()
    {
        return typeof(QuestData);
    }
    public override ExposedProperty createCopy()
    {
        return new QuestProperty();
    }
    public override void setValue(object param)
    {
        PropertyValue = (QuestData) param;
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        object temp1 = PropertyValue;
        var objectField = new ObjectField("Value:")
        {
            objectType = typeof(QuestData),
            value = (UnityEngine.Object) temp1
        };
        objectField.RegisterValueChangedCallback(evt => {
            object temp = evt.newValue;
            PropertyValue = (QuestData) temp;
        });

        var container = new VisualElement();

        container.Add(getRows());
        container.Add(objectField);

        return new BlackboardRow(field, container);
    }
    public override object createInputMethod(object defaultValue)
    {
        return EditorGUILayout.ObjectField("", PropertyValue, typeof(QuestData), false);
    }
}

[System.Serializable]
public class IntProperty : ExposedProperty
{
    [SerializeField] private int PropertyValue;
    public override object getValue()
    {
        return PropertyValue;
    }
    public override Type getType()
    {
        return typeof(int);
    }
    public override ExposedProperty createCopy()
    {
        return new IntProperty();
    }
    public override void setValue(object param)
    {
        PropertyValue = (int) param;
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        object temp1 = PropertyValue;
        var intField = new IntegerField("Value:");
        intField.value = PropertyValue;
        intField.RegisterValueChangedCallback(evt => {
            PropertyValue = evt.newValue;
        });
        var container = new VisualElement();

        container.Add(getRows());
        container.Add(intField);

        return new BlackboardRow(field, container);
    }
    public override object createInputMethod(object defaultValue)
    {
        return EditorGUILayout.IntField("", PropertyValue);
    }
}

[System.Serializable]
public class ObjectProperty<T> : ExposedProperty
{
    [SerializeReference] private T PropertyValue;

    public override object getValue()
    {
        return PropertyValue;
    }
    public override Type getType()
    {
        return typeof(T);
    }
    public override ExposedProperty createCopy()
    {
        return new ObjectProperty<T>();
    }
    public override void setValue(object param)
    {
        PropertyValue = (T) param;
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        object temp1 = PropertyValue;
        var objectField = new ObjectField("Value:")
        {
            objectType = typeof(T),
            value = (UnityEngine.Object) temp1
        };
        objectField.RegisterValueChangedCallback(evt => {
            object temp   = evt.newValue;
            PropertyValue = (T) temp;   
        });
        objectField.StretchToParentWidth();
        return new BlackboardRow(field, objectField);
    }
}

[System.Serializable]
public class ExposedProperty<T> : ExposedProperty
{
    public T PropertyValue;
    public override object getValue()
    {
        return PropertyValue;
    }
    public override Type getType()
    {
        return typeof(T);
    }
    public override void setValue(object param)
    {
        T value = (T) param;
        PropertyValue = value;
    }
    public override ExposedProperty createCopy()
    {
        return new ExposedProperty<T>();
    }
    public override BlackboardRow getBlackBoardRow(BlackboardField field)
    {
        VisualElement finalElement = new VisualElement();
        if (PropertyValue.GetType() == typeof(string) || PropertyValue.GetType() == typeof(int) || PropertyValue.GetType() == typeof(double) || PropertyValue.GetType() == typeof(float))
        {
            // Is number or string, so we will use a textfield for the input
            var propertyValueTextField = new TextField("Value:")
            {
                value = PropertyValue as string // Hay que cambiarlo
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var val = evt.newValue;
                if (PropertyValue.GetType() == typeof(string))
                {
                    setValue(val);
                }else
                {
                    int tmp;
                    float tmp2;
                    if (int.TryParse(val, out tmp)) setValue(tmp);
                    else if (float.TryParse(val, out tmp2)) setValue(tmp2);
                }
            });
            finalElement = propertyValueTextField;
        }
        else
        {
            var objectField = new ObjectField("Value:")
            {
                objectType = typeof(T)
            };

            finalElement = objectField;
        }
        
        return new BlackboardRow(field, finalElement);
    }
}
