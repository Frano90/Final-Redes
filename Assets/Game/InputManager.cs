using UnityEngine;


public class InputManager : MonoBehaviour
{
    PlayerInput_Controller playerController = new PlayerInput_Controller();

    private void Start()
    {
        // playerController = new PlayerInput_Controller()
        //     .ConfigureKeys(KeyCode.Mouse0, KeyCode.Mouse1)
        //     .ConfigureCallbacks(              
        //         () => Main.instance.eventManager.TriggerEvent(GameEvent.Mouse0Down),
        //         () => Main.instance.eventManager.TriggerEvent(GameEvent.Mouse1Down),
        //         () => Main.instance.eventManager.TriggerEvent(GameEvent.Mouse0Up),
        //         () => Main.instance.eventManager.TriggerEvent(GameEvent.Mouse1Up));
    }


    private void Update() => playerController.OnUpdate();
}

