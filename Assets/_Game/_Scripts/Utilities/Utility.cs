using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System;

namespace Centurion.CGSHC01.Game
{
    public enum CharacterState 
    {
        Idle=0,
        Running = 1,
        Dead = 2,
        Insensitive = 3
    };
    public enum GameState
    {
        Starting = 0,
        Menu = 1,
        Level = 2,
        Win = 3,
        Lose = 4,
    }
    public class Pack<TKey, TValue>
    {
        public int count { get; private set; }
        Dictionary<TKey, List<int>> keys;
        Dictionary<int, TValue> values;
        Dictionary<TValue,int> reversed_values;
        public Pack(){
            keys = new Dictionary<TKey, List<int>>();
            values = new Dictionary<int, TValue>();
            reversed_values = new Dictionary<TValue, int>();
            count = 0;
        }

        public void Add(TKey key, TValue value)
        {
            if (keys.ContainsKey(key))
            {
                keys[key].Add(count);
                values.Add(count, value);
                reversed_values.Add(value, count);
            }
            else
            {
                keys.Add(key, new List<int> {count});
                values.Add(count, value);
                reversed_values.Add(value, count);
            }
            count++;
        }
        public void Remove(TKey key, TValue value)
        {
            if (!reversed_values.TryGetValue(value, out int index)) return;
            keys[key].Remove(index);
            if (keys[key].Count == 0) keys.Remove(key);
            values.Remove(index);
            reversed_values.Remove(value);
        }
        public bool Contains(TValue value)
        {
            return values.Values.Contains(value);
        }
        public bool TryGetFirstValue(TKey key, out TValue value)
        {
            if (keys.ContainsKey(key))
            {
                List<int> indices = keys[key];
                value = values[indices[0]];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }
        public bool TryGetLastValue(TKey key, out TValue value)
        {
            if (keys.ContainsKey(key))
            {
                List<int> indices = keys[key];
                value = values[indices[indices.Count-1]];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }
        public bool TryGetAllValues(TKey key, out TValue[] values)
        {
            if (keys.ContainsKey(key))
            {
                List<int> indices = keys[key];
                TValue[] result = new TValue[indices.Count];
                for(int i = 0; i < indices.Count; i++)
                {
                    result[i] = this.values[indices[i]];
                }
                values = result;
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }
        public TValue[] AllValues()
        {
           return values.Values.ToArray();
        }
    }

    [Serializable]
    public class PawnGroup
    {
        public List<Pawn> pawns;
        public PawnGroup()
        {
            pawns = new List<Pawn>();
        }
        public void AddPawn(Pawn pawn)
        {
            pawns.Add(pawn);
        }

        public void RemovePawn(Pawn pawn)
        {
            pawns.Remove(pawn);
        }
        public void AddGroup(PawnGroup pawn_group)
        {
            for (int i=0;i<pawn_group.pawns.Count;i++)
            {
                AddPawn(pawn_group.pawns[i]);
                pawn_group.pawns[i].group = this;
            }

        }
    }
}
