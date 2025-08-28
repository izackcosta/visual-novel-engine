using UnityEngine;
using UnityEngine.UIElements;

public interface IClickable
{

    public void RegisterClickEvent(EventCallback<ClickEvent> clickEvent);

    public void UnregisterClickEvent(EventCallback<ClickEvent> clickEvent);


}
