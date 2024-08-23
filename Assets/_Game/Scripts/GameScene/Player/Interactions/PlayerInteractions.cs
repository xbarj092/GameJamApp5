using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private KeyboardInputHandler _keyboardInputHandler = new();

    private void Update()
    {
        _keyboardInputHandler.HandleInteraction();
    }
}
