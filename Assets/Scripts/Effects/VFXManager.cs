using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager> {

    protected VFXManager() { }

    #region Public Methods
    public void CreateEffect(ref VFXRequest vfxRequest) {
        switch (vfxRequest.weaponType) {
            case WeaponType.Blaster:
                HandleBlasterEffects(ref vfxRequest);
                break;
            case WeaponType.Baton:
                break;
        }
    }
    #endregion

    #region Private Methods
    private void HandleBlasterEffects(ref VFXRequest vfxRequest) {
        switch (vfxRequest.victimType) {
            case VictimType.MallCop:
                GameObject bloodSprayEffect = ObjectPool.Instance.GetObject(PoolObjectType.BloodEmitter);
                if (bloodSprayEffect) {
                    bloodSprayEffect.transform.SetParent(vfxRequest.targetParent);
                    bloodSprayEffect.transform.localPosition = Vector3.zero;
                    bloodSprayEffect.transform.localRotation = Quaternion.identity;
                }

                break;
            case VictimType.WallOrGround:
                break;
        }
    }
    #endregion

    #region Public Structures
    public struct VFXRequest {
        public WeaponType weaponType;
        public VictimType victimType;
        public Transform targetParent;

        public VFXRequest(WeaponType weaponType, VictimType victimType, Transform targetParent) {
            this.weaponType = weaponType;
            this.victimType = victimType;
            this.targetParent = targetParent;
        }
    }
    #endregion
}
