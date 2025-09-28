using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderOnClick : MonoBehaviour

{

    public void OnClick(InputAction.CallbackContext context)
    {

        Destroy(gameObject);
    }
    
}
