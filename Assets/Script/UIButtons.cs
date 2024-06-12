using System.Collections.Generic;
using System.Linq;
using Script;
using Unity.Assertions;
using UnityEngine;
using UnityEngine.UIElements;

public class UIButtons : MonoBehaviour
{
    //public VisualTreeAsset uxmlVisualTree;
    public UIDocument uiDocument;
    private List<UnitActionsEnum> _currentActions = new();
    
    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(uiDocument);
        
        //uiDocument.rootVisualElement
        //uiDocument.rootVisualElement.Clear();
        //uiDocument.rootVisualElement.Add(container);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private Button AddButton(UnitActionsEnum action)
    {
        var button = new Button() { text = GetText.Get(action) };
        button.AddToClassList("swarmies-button");
        button.clickable.clicked += () => ButtonOnclicked(action);

        return button;
    }

    private void ButtonOnclicked(UnitActionsEnum action)
    {
        throw new System.NotImplementedException();
    }

    public void SetButtons(List<UnitActionsEnum> actions)
    {
        if (_currentActions.SequenceEqual(actions))
            return;
        
        var root = uiDocument.rootVisualElement;
        var container = root.Q<VisualElement>("SwarmiesActions");
        
        Assert.IsNotNull(container);
        
        container.Clear();
        foreach (var action in actions)
        {
            container.Add(AddButton(action));
        }

        _currentActions = actions;
    }
}
