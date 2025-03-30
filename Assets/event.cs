using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    public InputAction clickAction; // New Input System action

    void Start()
    {
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
        clickAction.Enable();
    }

    void Update()
    {
        if (clickAction.WasPressedThisFrame()) // Detects clicks in the new Input System
        {
            Debug.Log("Press");
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue()
                //position = Mouse.current.position.ReadValue() // Get pointer position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                Debug.Log("Hit: " + result.gameObject.name);
            }
        }
    }

    void OnDestroy()
    {
        clickAction.Disable(); // Properly clean up InputAction
    }
}
