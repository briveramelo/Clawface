using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Reflection;
using System;
using UnityEngine.UI;
using System.Linq;
namespace ModMan {

    public static class IntExtensions {
        public static string ToCommaSeparated(this int myValue) {
            return string.Format("{0:n0}",myValue);
        }

    }
    public static class BoolExtensions {
        public static int ToInt(this bool myBool) {
            return myBool ? 1 : -1;
        }
    }
    public static class StringExtension
    {
        public static string DisplayName(this SpawnType spawn) {
            switch (spawn) {
                case SpawnType.Blaster: return "Blaster";
                case SpawnType.BlasterShotgun: return "Shotgun Blaster";
                case SpawnType.Bouncer: return "Bouncer";
                case SpawnType.GreenBouncer: return "Green Bouncer";
                case SpawnType.Kamikaze: return "Kamikaze";
                case SpawnType.KamikazeMommy: return "Kamikaze Mommy";
                case SpawnType.KamikazePulser: return "Kamikaze Pulser";
                case SpawnType.Keira: return "Keira";
                case SpawnType.RedBouncer: return "Red Bouncer";
                case SpawnType.Zombie: return "Zombie";
                case SpawnType.ZombieAcider: return "Acid Zombie";
                case SpawnType.ZombieBeserker: return "Berserker Zombie";
            }
            return "unnamed";
        }
        public static int MaxPerWave(this SpawnType spawn) {
            switch (spawn) {
                case SpawnType.Keira: return 1;
                case SpawnType.Blaster: return 15;
                case SpawnType.BlasterShotgun: return 15;
                case SpawnType.Zombie: return 15;
                case SpawnType.ZombieAcider: return 15;
                case SpawnType.ZombieBeserker: return 15;
                case SpawnType.Bouncer: return 5;
                case SpawnType.GreenBouncer: return 5;
                case SpawnType.RedBouncer: return 3;
                case SpawnType.Kamikaze: return 25;
                case SpawnType.KamikazeMommy: return 2;
                case SpawnType.KamikazePulser: return 25;
            }
            return 0;
        }

        public static string TryCleanName(this string myStringName, string suffixToRemove) {
            if (!string.IsNullOrEmpty(myStringName) && !string.IsNullOrEmpty(suffixToRemove) && myStringName.Contains(suffixToRemove)) {
                myStringName = myStringName.Substring(0, myStringName.Length - suffixToRemove.Length);
            }
            return myStringName;
        }

        public static string AddSpacesBetweenUpperCase (string str)
        {
            string result = str;
            for (int i = 1; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsUpper (c))
                {
                    result = result.Insert(i, " ");
                    i++;
                }
            }

            return result;
        }
    }

    public static class Rectstensions {
        public static Rect AddPosition(this Rect rect, Vector2 pos) {
            rect.x += pos.x;
            rect.y += pos.y;
            return rect;
        }
    }

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
        public static Color ChangeAlpha(this Color color, float newAlpha) {
            color.a = newAlpha;           
            return color;
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

        public static bool IsValidIndex<T>(this List<T> list, int index) {
            return list!=null && index >= 0 && index < list.Count;
        }

        public static T FindInDirection<T>(this List<T> list, Predicate<T> condition, int anchorIndex, bool findAhead, bool getLinearFirst=true) {
            Predicate<int> isInAppropriateDirection = (loopIndex)=> (findAhead ? loopIndex > anchorIndex : loopIndex < anchorIndex);
            if (getLinearFirst) {
                for (int i = 0; i < list.Count; i++) {
                    T item = list[i];
                    if (isInAppropriateDirection(i) && condition(item)) {
                        return item;
                    }
                }
            }
            else {
                for (int i = list.Count-1; i >= 0; i--) {
                    T item = list[i];
                    if (isInAppropriateDirection(i) && condition(item)) {
                        return item;
                    }
                }
            }
            return default(T);
        }
        public static void MoveToBack<T>(this List<T> list, T item) {
            list.Remove(item);
            list.Add(item);
        }
        public static void AddUntil<T>(this List<T> list, int index) {
            while (list.Count <= index) {
                list.Add(default(T));
            }
        }
        public static void AddNewUntil<T> (this List<T> list, int index) where T : new () {
            while (list.Count <= index) {
                list.Add(new T());
            }
        }

        public static T PopFront<T>(this List<T> list){
            T result = list[0];
            list.RemoveAt(0);
            return result;
        }

        public static T GetRandom<T>(this List<T> thisList) {
            int rand = UnityEngine.Random.Range(0, thisList.Count);
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
            int rand = UnityEngine.Random.Range(0, sublist.Count);
            item = sublist[rand];
            return item;
        }    

        public static T Tail<T> (this List<T> list)
        {
            if (list.Count == 0) return default(T);

            return list[list.Count-1];
        }
    }

    public static class FloatExtensions
    {
        public const float minDB = -80f;
        public static float ToDecibel(this float linear) {
            float dB;
            if (linear != 0) {
                dB = 40F * Mathf.Log10(linear);
            }
            else {
                dB = minDB;
            }
            return dB;
        }


        public static bool AboutEqual(this float flo, float other, float tolerance = 0.1f)
        {
            return Mathf.Abs(flo-other)<tolerance;
        }
    }

    public static class ArrayExtensions
    {
        public static int IndexOf<T> (this T[] array, T obj)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals (obj)) return i;
            }

            return -1;
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
        public static Vector3 NoY(this Vector3 vec) {
            vec.y = 0;
            return vec;
        }
        public static Vector3 NoZ(this Vector3 vec) {
            vec.z = 0;
            return vec;
        }
        public static Vector3 Abs(this Vector3 vec) {
            vec.x = Mathf.Abs(vec.x);
            vec.y = Mathf.Abs(vec.y);
            vec.z = Mathf.Abs(vec.z);
            return vec;
        }
        /// <summary>
        /// Returns the greatest of this vector's components.
        /// </summary>
	    public static float Max (this Vector3 v) {
            return Mathf.Max (v.x, v.y, v.z);
        }

        public static float As360Angle(this Vector3 inputVector) {
            float start = Mathf.Atan2(inputVector.z, inputVector.x);
            return (start > 0 ? start : (2 * Mathf.PI + start)) * 360 / (2 * Mathf.PI);
        }

        public static Float3 ToFloat3 (this Vector3 v)
        {
            return new Float3 (v.x, v.y, v.z);
        }
    }

    public static class UIExtensions {
        public static void SwitchListenerState(this UnityEngine.Events.UnityEventBase eventBase, UnityEngine.Events.UnityEventCallState newState) {
            for (int i = 0; i < eventBase.GetPersistentEventCount(); i++) {
                eventBase.SetPersistentListenerState(i, newState);
            }
        }
        public static bool Interactable(this Selectable selectable) {
            return selectable.interactable && selectable.IsActive();
        }
    }
    public static class ClassExtensions {
        public static bool IsNull<T>(this T thisInterface) where T : class {
            return thisInterface==null || thisInterface.Equals(null);
        }
    }

    public static class GameObjectExtensions {

        public static void DeActivate(this GameObject obj, float timeToDeactivate, string coroutineName) {
            Timing.RunCoroutine(IEDeActivate(obj, timeToDeactivate), coroutineName);
        }
        public static void FollowAndDeActivate(this GameObject obj, float timeToDeactivate, Transform objToFollow, Vector3 offset, string coroutineName) {
            Timing.RunCoroutine(IEDeActivate(obj, timeToDeactivate, objToFollow, offset), coroutineName);
        }
        static IEnumerator<float> IEDeActivate(GameObject obj, float timeToDeactivate) {
            yield return Timing.WaitForSeconds(timeToDeactivate);
            if (obj!=null) {
                obj.SetActive(false);
            }
        }
        static IEnumerator<float> IEDeActivate(GameObject obj, float timeToDeactivate, Transform objToFollow, Vector3 offset) {
            float timeRemaining=timeToDeactivate;
            while(timeRemaining>0f && obj!=null && objToFollow!=null) {
                timeRemaining-=Time.fixedDeltaTime;
                obj.transform.position = objToFollow.position + offset;
                yield return 0f;
            }            
            if (obj!=null) {
                obj.SetActive(false);
            }
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

        public static GameObject FindWithSubstring (string substring)
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                GameObject obj = allObjects[i];
                if (obj.name.Contains (substring))
                    return obj;
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

        /// <summary>
        /// Checks if a GameObject has been destroyed.
        /// </summary>
        /// <param name="gameObject">GameObject reference to check for destructedness</param>
        /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
        /// See: http://answers.unity3d.com/answers/1001265/view.html
        public static bool IsDestroyed(this GameObject gameObject)
        {
            // UnityEngine overloads the == opeator for the GameObject type
            // and returns null when the object has been destroyed, but 
            // actually the object is still there but has not been cleaned up yet
            // if we test both we can determine if the object has been destroyed.
            return gameObject == null && !ReferenceEquals(gameObject, null);
        }
    }

    public static class TransformExtensions {
        public static void SortChildrenByName(this Transform parent) {
            List<Transform> children = new List<Transform>();
            for (int i = parent.childCount - 1; i >= 0; i--) {
                Transform child = parent.GetChild(i);
                children.Add(child);
                child.parent = null;
            }
            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            children.ForEach(child => child.parent = parent);
        }

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

        public static void DestroyAllChildren(this Transform transform) {
            int count = transform.childCount;
            for (int i = count - 1; i >=0; i--) {
                Helpers.DestroyProper(transform.GetChild(i).gameObject);
            }
        }
        public static void ToggleAllChildren(this Transform transform, bool enabled) {
            int count = transform.childCount;
            for (int i = count - 1; i >= 0; i--) {
                transform.GetChild(i).gameObject.SetActive(enabled);
            }
        }
    }

    public static class Vector2Extensions {
        public static float As360Angle(this Vector2 inputVector){
            float start = Mathf.Atan2 (inputVector.y, inputVector.x);
            return (start > 0 ? start : (2*Mathf.PI + start)) * 360 / (2*Mathf.PI);
		}
        public static Vector2 AsVector2(this float inputAngle){
			return new Vector2(Mathf.Cos (Mathf.Deg2Rad * inputAngle),Mathf.Sin (Mathf.Deg2Rad * inputAngle));
		}
        public static bool IsAboutEqual(this Vector3 thisVector, Vector3 otherVector, float tolerance=0.02f) {
            float xDif = Mathf.Abs(thisVector.x - otherVector.x);
            float yDif = Mathf.Abs(thisVector.y - otherVector.y);
            float zDif = Mathf.Abs(thisVector.z - otherVector.z);
            float total = xDif + yDif + zDif;
            return total<tolerance;
        }
    }

    public static class DictionaryExtensions {
        public static void ForEach<T, U>(this Dictionary<T,U> thisDictionary, System.Action<T,U> action) {
            foreach(KeyValuePair<T, U> kvpElement in thisDictionary) {
                action(kvpElement.Key, kvpElement.Value);
            }
        }
    }

    public static class AnimationCurveExtensions
    {
        public static float MaxValue (this AnimationCurve curve)
        {
            float max = float.NegativeInfinity;
            for (int i = 0; i < curve.length; i++)
            {
                Keyframe key = curve.keys[i];
                if (key.value > max) max = key.value;
            }

            return max;
        }
    }

    public static class EnumExtensions
    {

        public static PoolObjectType ToPoolObject(this SpawnType spawnType)
        {
            switch (spawnType)
            {
                case SpawnType.Blaster:
                    return PoolObjectType.MallCopBlaster;
                case SpawnType.Zombie:
                    return PoolObjectType.Zombie;
                case SpawnType.Bouncer:
                    return PoolObjectType.Bouncer;
                case SpawnType.RedBouncer:
                    return PoolObjectType.RedBouncer;
                case SpawnType.GreenBouncer:
                    return PoolObjectType.GreenBouncer;
                case SpawnType.Kamikaze:
                    return PoolObjectType.Kamikaze;
                case SpawnType.KamikazePulser:
                    return PoolObjectType.KamikazePulser;
                case SpawnType.KamikazeMommy:
                    return PoolObjectType.KamikazeMommy;
                case SpawnType.ZombieBeserker:
                    return PoolObjectType.ZombieBeserker;
                case SpawnType.ZombieAcider:
                    return PoolObjectType.ZombieAcider;
                case SpawnType.BlasterShotgun:
                    return PoolObjectType.BlasterShotgun;
                case SpawnType.BlasterReanimator:
                    return PoolObjectType.BlasterReanimator;

            }
            return PoolObjectType.MallCopBlaster;
        }
    }

    public static class MinMaxCurveExtensions
    {
        public static float Max (this ParticleSystem.MinMaxCurve curve)
        {
            switch (curve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return curve.constant;
                case ParticleSystemCurveMode.Curve:
                    return curve.curve.MaxValue();
                case ParticleSystemCurveMode.TwoConstants:
                    return Mathf.Max (curve.constantMin, curve.constantMax);
                case ParticleSystemCurveMode.TwoCurves:
                    return Mathf.Max (curve.curveMin.MaxValue(), curve.curveMax.MaxValue());
                default:
                    return float.NaN;
            }
        }
    }

    public static class SpriteRendererExtensions
    {
        public static void SetAlpha (this SpriteRenderer spriteRenderer, float alpha)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    public static class Helpers
    {
        public static void DestroyProper (GameObject gameObject)
        {
            #if UNITY_EDITOR
            GameObject.DestroyImmediate (gameObject);
            return;
            #endif

            if (Application.isPlaying) GameObject.Destroy (gameObject);
            else GameObject.DestroyImmediate (gameObject);
        }

        public static List<FieldInfo> GetConstants(Type type) {
            List<FieldInfo> constants = new List<FieldInfo>();

            FieldInfo[] fieldInfos = type.GetFields(
                // Gets all public and static fields

                BindingFlags.Public | BindingFlags.Static |
                // This tells it to get the fields from all base types as well

                BindingFlags.FlattenHierarchy);

            // Go through the list and only pick out the constants
            foreach (FieldInfo fi in fieldInfos)
                // IsLiteral determines if its value is written at 
                //   compile time and not changeable
                // IsInitOnly determine if the field can be set 
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true 
                //   but a const field would have only IsLiteral equal to true
                if (fi.IsLiteral && !fi.IsInitOnly)
                    constants.Add(fi);

            // Return an array of FieldInfos
            return constants;//.ToArray(typeof(FieldInfo));
        }

        public static List<U> GetConstantsOfType<T, U>() {
            List<FieldInfo> fis = GetConstants(typeof(T));
            List<U> items = new List<U>();
            fis.ForEach(fi => {
                if (fi.IsLiteral && !fi.IsInitOnly) {
                    U item = (U)fi.GetRawConstantValue();
                    items.Add(item);
                }
            });
            return items;
        }
    }
}
