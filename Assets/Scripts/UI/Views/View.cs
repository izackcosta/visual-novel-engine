using UnityEngine;
using UnityEngine.UIElements;

public abstract class View : MonoBehaviour
{

    protected UIDocument _document;

    protected VisualElement _root;

    protected virtual void Awake()
    {
        if(_document == null)
            _document = GetComponent<UIDocument>();
        if(_root == null)
            _root = _document.rootVisualElement;
    }

}
