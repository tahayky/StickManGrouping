using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
namespace Centurion.WallMaker
{
    public enum DrawingState { Empty = 0,Drawing =1,Remove=2 };
    public enum Mode { EditMode = 0, ObjectMode = 1 };

    public enum Axis { X = 0, Y = 1 };
    [System.Serializable]
    public struct LineDictionary
    {
        public int key;
        public Line value;

        public LineDictionary(int key, Line value) : this()
        {
            this.value = value;
            this.key = key;
        }

    }
    [System.Serializable]
    public struct Vertices
    {
        public Vector3 v1;
        public Vector3 v2;

    }
}
