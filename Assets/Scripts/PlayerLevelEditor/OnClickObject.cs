using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerLevelEditor
{
    public class OnClickObject : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void Click(GameObject UI)
        {
            Dropdown dp = UI.GetComponent<Dropdown>();

            switch (dp.value)
            {
                case 0:
                    gameObject.GetComponent<Renderer>().material.color = Color.green;
                    return;
                case 1:
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                    return;
                case 2:
                    gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    return;
            }
        }


    }

}

