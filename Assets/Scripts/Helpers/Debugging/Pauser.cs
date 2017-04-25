using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pauser : MonoBehaviour
{

    public static Pauser Instance;

    private bool isPaused;

    private void Awake() {
        if (Instance!=null) {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.PAUSE, ButtonMode.DOWN))
        {        
            Pause();
        }        
    }

    void Pause()
    {
        //SFXManager.Instance.Play(SFXType.Pause);
        //TODO display pause menu
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
    
}
