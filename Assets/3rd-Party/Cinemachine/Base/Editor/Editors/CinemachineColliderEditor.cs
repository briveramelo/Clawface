using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineCollider))]
    public sealed class CinemachineColliderEditor : UnityEditor.Editor
    {
        private CinemachineCollider Target { get { return target as CinemachineCollider; } }
        private static readonly string[] m_excludeFields = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string[] excluded = m_excludeFields;
            if (!Target.m_PreserveLineOfSight)
            {
                excluded = new string[] 
                {
                    "m_Script",
                    SerializedPropertyHelper.PropertyName(() => Target.m_DistanceLimit),
                    SerializedPropertyHelper.PropertyName(() => Target.m_CameraRadius),
                    SerializedPropertyHelper.PropertyName(() => Target.m_Strategy),
                    SerializedPropertyHelper.PropertyName(() => Target.m_MaximumEffort),
                    SerializedPropertyHelper.PropertyName(() => Target.m_Damping)
                };
            }
            else if (Target.m_Strategy == CinemachineCollider.ResolutionStrategy.PullCameraForward)
            {
                excluded = new string[] 
                {
                    "m_Script",
                    SerializedPropertyHelper.PropertyName(() => Target.m_MaximumEffort),
                };
            }
            EditorGUI.BeginChangeCheck();

            if (Target.m_PreserveLineOfSight && !Target.VirtualCamera.State.HasLookAt)
                EditorGUILayout.HelpBox(
                    "Preserve Line Of Sight requires a LookAt target.", 
                    MessageType.Error);

            DrawPropertiesExcluding(serializedObject, excluded);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineCollider))]
        private static void DrawColliderGizmos(CinemachineCollider collider, GizmoType type)
        {
            CinemachineVirtualCameraBase vcam = (collider != null) ? collider.VirtualCamera : null;
            if (vcam != null && collider.enabled)
            {
                Color oldColor = Gizmos.color;
                Vector3 pos = vcam.State.FinalPosition;
                if (collider.m_PreserveLineOfSight && vcam.State.HasLookAt)
                {
                    Gizmos.color = CinemachineColliderPrefs.FeelerColor;
                    if (collider.m_CameraRadius > 0)
                        Gizmos.DrawWireSphere(pos, collider.m_CameraRadius);

                    Vector3 forwardFeelerVector = (vcam.State.ReferenceLookAt - pos).normalized;
                    float distance = collider.m_DistanceLimit;
                    Gizmos.DrawLine(pos, pos + forwardFeelerVector * distance);

                    // Show the avoidance path, for debugging
                    List<List<Vector3>> debugPaths = collider.DebugPaths;
                    foreach (var path in debugPaths)
                    {
                        Gizmos.color = CinemachineColliderPrefs.FeelerHitColor;
                        Vector3 p0 = vcam.State.ReferenceLookAt;
                        foreach (var p in path)
                        {
                            Gizmos.DrawLine(p0, p);
                            p0 = p;
                        }
                        Gizmos.DrawLine(p0, pos);
                    }
                }
                Gizmos.color = oldColor;
            }
        }
    }
}
