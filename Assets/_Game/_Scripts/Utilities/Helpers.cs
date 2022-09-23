using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Helpers 
{
    public static Vector3 GetCentroid(Vector3[] points)
    {
        float centroidX = 0f;
        float centroidY = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            centroidX += points[i].x;
            centroidY += points[i].z;
        }
        centroidX /= points.Length;
        centroidY /= points.Length;

        return (new Vector3(centroidX,0, centroidY));
    }

    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]++;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }
        foreach (T s in list2)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]--;
            }
            else
            {
                return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }
}
