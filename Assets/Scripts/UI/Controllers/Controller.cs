using UnityEngine;

public abstract class Controller<T> : MonoBehaviour where T : View
{

    protected T _view;

    protected virtual void Awake()
    {
        _view = GetComponent<T>();
    }

}
