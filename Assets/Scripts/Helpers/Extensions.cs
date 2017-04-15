using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

namespace ModMan {

    public static class ColorExtensions
    {
        public static bool IsAboutEqual(this Color color, Color color2, float tolerance = 0.02f)
        {
            float rDif = Mathf.Abs(color.r - color2.r);
            float gDif = Mathf.Abs(color.g - color2.g);
            float bDif = Mathf.Abs(color.b - color2.b);
            float aDif = Mathf.Abs(color.a - color2.a);

            return (rDif + gDif + bDif + aDif) < tolerance;
        }
        public static string ToHex(this Color color)
        {
            int r = (int)(color.r * 256);
            int g = (int)(color.g * 256);
            int b = (int)(color.b * 256);

            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
    public static class ListExtensions {


        public static T PopFront<T>(this List<T> list){
            T result = list[0];
            list.RemoveAt(0);
            return result;
        }

        public static T GetRandom<T>(this List<T> thisList) {
            int rand = Random.Range(0, thisList.Count);
            return thisList[rand];
        }

        public static T GetRandom<T>(this List<T> thisList, System.Predicate<T> match)
        {
            List<T> sublist = thisList.FindAll(match);
            T item = default(T);
            if(sublist.Count == 0)
            {
                return item;
            }
            int rand = Random.Range(0, sublist.Count);
            item = sublist[rand];
            return item;
        }    

    }

    public static class VectorExtensions {
        //Clamps at 0, 360
        public static Vector3 ToVector3(this float inputAngle)
        {
            return new Vector3(Mathf.Cos(Mathf.Deg2Rad * inputAngle), 0f, Mathf.Sin(Mathf.Deg2Rad * inputAngle));
        }
        public static Vector3 NormalizedNoY(this Vector3 vector){ 
            vector.y=0;
            return vector.normalized;
        }
        /// <summary>
        /// Returns the greatest of this vector's components.
        /// </summary>
	    public static float Max (this Vector3 v) {
            return Mathf.Max (v.x, v.y, v.z);
        }
    }

    public static class GameObjectExtensions {

        public static void DeActivate(this GameObject obj, float timeToDeactivate) {
            Timing.RunCoroutine(IEDeActivate(obj, timeToDeactivate), Segment.FixedUpdate);
        }
        static IEnumerator<float> IEDeActivate(GameObject obj, float timeToDeactivate) {
            yield return Timing.WaitForSeconds(timeToDeactivate);
            obj.SetActive(false);
        }
        
        /// <summary>
        /// Finds a GameObject with a certain name in the children of the given
        /// GameObject.
        /// </summary>
	    public static GameObject FindInChildren (this GameObject obj, string name) {
            var childTransforms = obj.GetComponentsInChildren<Transform>();
            foreach (var childTr in childTransforms) {
                var childObj = childTr.gameObject;
                if (childObj.name == name) return childObj;
            }

            return null;
        }

        /// <summary>
        /// Finds the given component type in ancestors (parents, grandparents, etc.)
        /// </summary>
        public static T GetComponentInAncestors<T> (this GameObject obj) {
            var parent = obj.transform.parent;
            while (parent != null) {
                var component = parent.GetComponent<T>();
                if (component != null) return component;
                parent = parent.transform.parent;
            }

            return default(T);
        }

        /// <summary>
        /// Copies the settings of a component from one GameObject to another.
        /// </summary>
        public static bool CopyComponentFromGameObject<T> (this GameObject obj, GameObject other) where T : Component {
            T otherComponent = other.GetComponent<T>();
            if (otherComponent == null) return false;

            T thisComponent = obj.GetComponent<T>();
            if (thisComponent == null) thisComponent = obj.AddComponent<T>();

            System.Reflection.FieldInfo[] fields = otherComponent.GetType().GetFields();
            foreach (var field in fields) {
                field.SetValue (thisComponent, field.GetValue (otherComponent));
            }

            return true;
        }

        /// <summary>
        /// Copies collider information from one object to another.
        /// </summary>
        public static bool CopyColliderFromGameObject (this GameObject obj, GameObject other) {
            if (CopyComponentFromGameObject<BoxCollider>(obj, other)) return true;
            if (CopyComponentFromGameObject<SphereCollider>(obj, other)) return true;
            if (CopyComponentFromGameObject<CapsuleCollider>(obj, other)) return true;
            if (CopyComponentFromGameObject<MeshCollider>(obj, other)) return true;
            return false;
        }        
    }

    public static class TransformExtensions {
        public static void Reset (this Transform tr) {
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
        }
        public static void ResetFull (this Transform tr) {
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
            tr.localScale = Vector3.one;
        }

        public static void Reset(this Transform tr, TransformMemento tMemento){
            tr.localPosition = tMemento.startPosition;
            tr.localScale = tMemento.startScale;
            tr.localRotation = tMemento.startRotation;
        }

        public static IEnumerator<float> ResetRotation(this Transform transformToRestore, float timeToDelay) {
            Quaternion startRotation = transformToRestore.rotation;
            yield return Timing.WaitForSeconds(timeToDelay);
            transformToRestore.rotation = startRotation;
        }
    
    }
}
