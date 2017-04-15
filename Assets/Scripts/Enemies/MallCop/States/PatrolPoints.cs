using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour {

    [SerializeField] List<Transform> patrolPoints;
    [SerializeField] PatrolStyle patrolStyle;

    public Transform currentTarget {
        get { return patrolPoints[currentTargetIndex]; }
        
    }
    private int currentTargetIndex;
    private bool movingUp=true;

    public void UpdateTarget() {
        switch (patrolStyle) {
            case PatrolStyle.Loop:
                currentTargetIndex++;
                if (currentTargetIndex >= patrolPoints.Count) {
                    currentTargetIndex = 0;
                }
                break;
            case PatrolStyle.Bounce:
                int change = movingUp ? 1 : -1;                
                currentTargetIndex += change;
                if (currentTargetIndex >= patrolPoints.Count || currentTargetIndex < 0) {
                    currentTargetIndex = movingUp ? patrolPoints.Count - 1 : 0;
                    movingUp = !movingUp;
                }
                break;
            case PatrolStyle.Random:
                int lastIndex = currentTargetIndex;
                currentTargetIndex = Random.Range(0, patrolPoints.Count - 1);
                if (currentTargetIndex == lastIndex && patrolPoints.Count>1) {
                    UpdateTarget();
                }
                break;
        }
    }

}

public enum PatrolStyle {
    Loop=0,
    Bounce=1,
    Random=2
}