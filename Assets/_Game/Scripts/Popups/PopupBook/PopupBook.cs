using UnityEngine;
using UnityEngine.UI;

public class PopupBook : Popup
{
    enum State
    {
        Category,
        Element,
    }

    [SerializeField] CategoryTable categoryTable;
    [SerializeField] ElementTable elementTable;
    [SerializeField] Button backButton;

    State currentState;

    private void Awake()
    {
        backButton.onClick.RemoveListener(OnClickBack);
        backButton.onClick.AddListener(OnClickBack);
    }

    public override void Open()
    {
        base.Open();

        categoryTable.Init();
        categoryTable.Show();
        elementTable.Hide();
    }

    public void SelectCategory(CategoryDefinition category)
    {
        currentState = State.Element;
        categoryTable.Disappear();
        elementTable.Init(category);
        elementTable.Appear();
    }

    void OnClickBack()
    {
        if (currentState == State.Category)
        {
            Close();
        }
        else if (currentState == State.Element)
        {
            currentState = State.Category;
            categoryTable.Appear();
            elementTable.Disappear();
        }
    }
}
