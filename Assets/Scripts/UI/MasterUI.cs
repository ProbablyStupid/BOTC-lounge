using UnityEngine;

public class MasterUI : MonoBehaviour
{
    
    
    public void hideCursor()
    {
        Cursor.visible = false;
    }

    public void showCursor()
    {
        Cursor.visible = true;
    }

    // TODO: think about transitioning to a little UI mini-framework...
    public void switchToGameState()
    {
        hideCursor();
     
        // hide menu ui

        // show game ui
    }

    public void switchToUIState()
    {
        // hide game ui

        showCursor();

        // show menu ui

    }
}
