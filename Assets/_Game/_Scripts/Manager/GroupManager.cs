using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MEC;
using System.Diagnostics;
using System;
namespace Centurion.CGSHC01.Game
{
    public class GroupManager : Singleton<GroupManager>
    {
        public Action on_groups_updated;

        public List<PawnGroup> pawn_groups = new List<PawnGroup>();

        public Transform cam_transform;
        List<Pawn> all_pawns = new List<Pawn>();
        public bool cluster = false;
        public PawnGroup largest_group
        {
            get
            {
                if (pawn_groups != null && pawn_groups.Count != 0)
                {
                    return pawn_groups[pawn_groups.Count - 1];
                }
                else return null;
            }
        }
        
        public Vector3 largest_group_center { get; private set; }
        private new void Awake()
        {
            on_groups_updated += RemoveEmpties;
            on_groups_updated += SortGroups;
            base.Awake();
        }

        private void Start()
        {
            GameObject[] pawns = GameObject.FindGameObjectsWithTag("Pawn");
            for(int i = 0; i < pawns.Length; i++)
            {
                Pawn pawn = pawns[i].GetComponent<Pawn>();
                CreateGroup(pawn);
                all_pawns.Add(pawn);
            }
        }
        private void Update()
        {
            if (cluster)
            {
                Clustering();
                cluster = false;
            }
        }
        public void Clustering()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<Pawn> _all_pawns = new List<Pawn>(all_pawns);
            pawn_groups = new List<PawnGroup>();
            Pawn _pawn = _all_pawns[0];
            PawnGroup _new_pawn_group = new PawnGroup();
            _new_pawn_group.AddPawn(_pawn);
            _pawn.group = _new_pawn_group;
            pawn_groups.Add(_new_pawn_group);
            _all_pawns.Remove(_pawn);
            while (_all_pawns.Count >0)
            {
                if (_pawn == null)
                {
                    _pawn = _all_pawns[0];
                    PawnGroup new_pawn_group = new PawnGroup();
                    new_pawn_group.AddPawn(_pawn);
                    _pawn.group = new_pawn_group;
                    pawn_groups.Add(new_pawn_group);
                    _all_pawns.Remove(_pawn);
                }
                

                Pawn[] detected_pawns = _pawn.detected_pawns;
                PawnGroup pawn_group = _pawn.group;
                _pawn = null;
                for (int i = 0; i < detected_pawns.Length; i++)
                {
                    Pawn me = detected_pawns[i];
                    if (_all_pawns.Contains(me))
                    {
                        _all_pawns.Remove(me);
                        pawn_group.AddPawn(me);
                        me.group = pawn_group;
                        _pawn = me;
                        continue;
                    }
                    else
                    {
                        if (me.group == pawn_group) continue;

                        if(me.group.pawns.Count < pawn_group.pawns.Count)
                        {
                            pawn_group.AddGroup(me.group);
                            pawn_groups.Remove(me.group);
                        }
                        else
                        {
                            me.group.AddGroup(pawn_group);
                            pawn_groups.Remove(pawn_group);
                            pawn_group = me.group;
                        }
                    }
                }
            }


            OnGroupsUpdate();


            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            //UnityEngine.Debug.Log(ts);
        }
        public void OnGroupsUpdate()
        {
                on_groups_updated?.Invoke();
        }
        public void CreateGroup(Pawn first_member)
        {
            PawnGroup new_pawn_group = new PawnGroup();
            pawn_groups.Add(new_pawn_group);
            first_member.group.RemovePawn(first_member);
            JoinGroup(first_member, new_pawn_group);
            first_member.group = new_pawn_group;
        }
        public void JoinGroup(Pawn member,PawnGroup pawn_group)
        {
            if (member.group == pawn_group) return;
            member.group.RemovePawn(member);
            pawn_group.AddPawn(member);
            member.group = pawn_group;
        }
        public void SortGroups()
        {
            pawn_groups = pawn_groups.OrderBy(item => item.pawns.Count).ToList();
        }

        public void RemoveEmpties()
        {
            var empty_groups = pawn_groups.Where(item => item.pawns.Count == 0).ToArray();
            for (int i = 0; i < empty_groups.Length; i++)
            {
                pawn_groups.Remove(empty_groups[i]);
            }
        }
        private void LateUpdate()
        {
            largest_group_center = GetBiggestGroupCenter();
        }
        public Vector3 GetBiggestGroupCenter()
        {
            Vector3[] center_points = largest_group.pawns.Select(item => item.transform.position).ToArray();

            return Helpers.GetCentroid(center_points);
        }
    }
}
