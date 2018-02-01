using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static int wave = 0;

    public void DoTheThing()
    {
        Debug.Log(wave);
        wave = wave < 2 ? ++wave : 0;
    }
}
