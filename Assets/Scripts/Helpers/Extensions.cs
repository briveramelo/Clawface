using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModMan {
    public static class Extensions
    {
        public static bool IsAboutEqual(this Color color, Color color2, float tolerance = 0.02f)
        {
            float rDif = Mathf.Abs(color.r - color2.r);
            float gDif = Mathf.Abs(color.g - color2.g);
            float bDif = Mathf.Abs(color.b - color2.b);
            float aDif = Mathf.Abs(color.a - color2.a);

            return (rDif + gDif + bDif + aDif) < tolerance;
        }


        public static T GetRandom<T>(this List<T> thisList) {
            int rand = Random.Range(0, thisList.Count);
            return thisList[rand];
        }

        public static T GetRandom<T>(this List<T> thisList, System.Predicate<T> match)
        {
            List<T> sublist = thisList.FindAll(match);            
            int rand = Random.Range(0, sublist.Count);            
            return sublist[rand];
        }

        public static string ToHex(this Color color)
        {
            int r = (int)(color.r * 256);
            int g = (int)(color.g * 256);
            int b = (int)(color.b * 256);

            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        //Clamps at 0, 360
        public static Vector3 ToVector3(this float inputAngle)
        {
            return new Vector3(Mathf.Cos(Mathf.Deg2Rad * inputAngle), 0f, Mathf.Sin(Mathf.Deg2Rad * inputAngle));
        }

    }

    

}
