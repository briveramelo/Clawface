using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerLevelEditor
{
    public abstract class IMapObject
    {
        protected GameObject gameObject;


        public abstract void Update();

        public abstract void OnClick();

    }
}