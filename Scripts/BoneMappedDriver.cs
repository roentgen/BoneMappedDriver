﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class BoneMappedDriver : MonoBehaviour
{
        public HumanBodyBones DeformingBone;
        public GameObject Reference;
        public bool Absolute; 
        public bool Clamp; // whether a dp's clamped to positive [0.f - 1.f]
        public bool Linear; // use an angle (uniformed to PI/2) linear correction  instead of dot
        public bool Additive; // Additive or Selective for list: All of unmasked ranges effect to final mesh. If not, last unmasked range in list only effects.
        private Animator Anim;
        private SkinnedMeshRenderer Mesh; 
        private int[] BlendShapeIndices;
        public bool DebugDraw;
        [System.Serializable]
        public struct BlendShapeRange
        {
                public string Name;
                public AnimationCurve Func; // function curve; x is a dot that be clamped if you need. y shall not be clamped.
                public Vector2 Mask; // indicates active range [X - Y] for a dot; inclusive 
        }

        [HideInInspector] public BlendShapeRange[] BlendShapes;

        public BoneMappedDriver ()
        {
                Additive = true;
                Clamp = true;
                Linear = false;
                DebugDraw = false;
        } 

        // Start is called before the first frame update
        void Start()
        {
                Mesh = GetComponent<SkinnedMeshRenderer>();
                var owner = transform.root.gameObject;
                Anim = owner.GetComponent<Animator>();
                if (Mesh)
                        BlendShapeIndices = (from r in BlendShapes select Mesh.sharedMesh.GetBlendShapeIndex(r.Name)).ToArray();
                if (Mesh == null) 
                        Debug.LogError("Error: BoneMappedDriver needs a sibling, SkinnedMeshRenderer component " );
                else if (BlendShapeIndices.Length == 0) 
                        Debug.LogWarning("Warning:  there are no BlendShapes in SkinnedMeshComponent" );
                if (Anim == null) 
                        Debug.LogError("Error: Component owner does not have Animator component." );
                if (Reference == null /* || DeformingBone == None*/) 
                        Debug.LogError("Error: Reference and DeformingBone must be set." );
        }

        // Update is called once per frame
        void Update()
        {
                if (Anim == null || Reference == null || /* DeformingBone == None || */ Mesh == null || BlendShapeIndices.Length == 0)
                        return;
                var b = Anim.GetBoneTransform(DeformingBone);
                var v = new Vector3(0.0f, 0.1f, 0.0f); // a mecanim bone shall be Y-axis bone
                Func< Transform, Vector3 > head = (tr) => tr.rotation * v;

                Func< Vector3, string > to_str = (vec) =>  "(" + vec.x + "," + vec.y + "," + vec.z + ")";
                Func< Transform, string > dump = (t) => {
                        return "parent: position: " + to_str(t.parent.position) +
                                "\n             localPosition: "  + to_str(t.parent.localPosition) +
                                "\n             rotation: " + to_str(t.parent.rotation.eulerAngles) +
                                "\n             localRotaion: " + to_str(t.parent.localRotation.eulerAngles) +
                                "\ncurrent: position: " + to_str(t.position) +
                                "\n              localPosition: " + to_str(t.localPosition) +
                                "\n              rotation: " + to_str(t.rotation.eulerAngles) +
                                "\n              localRotation: " + to_str(t.localRotation.eulerAngles);
                };
        
                var boundref = head(Reference.transform);
                var boundbone = head(b);
                float dot = Vector3.Dot(boundref.normalized, boundbone.normalized);
                if (DebugDraw) {
                        //Debug.Log("dot: " + dot + " (" + (float)((Math.PI / 2.0 - Math.Acos(dot)) / (Math.PI / 2.0)) + " rad) ref:"  + to_str(boundref) + " bone:" + to_str(boundbone));
                        // Debug.Log("Ref:");
                        // Debug.Log(dump(Reference.transform));
                        // Debug.Log("Bone:");
                        // Debug.Log(dump(b));
                        Debug.DrawLine(b.position, b.position + head(b), Color.yellow);
                        Debug.DrawLine(Reference.transform.position, Reference.transform.position + head(Reference.transform), Color.magenta);
                }
                dot = Absolute ? Math.Abs(dot) : dot;
                dot = Clamp ? Math.Max(Math.Min(dot, 1.0f), 0.0f) : dot;
                dot = Linear ? (float)((Math.PI / 2.0 - Math.Acos(dot)) / (Math.PI / 2.0)) : dot;
                float e = 0.0f;
                int last = -1;
                for (var i = 0; i < BlendShapes.Length; i ++)
                {
                        if (BlendShapeIndices[i] == -1)
                                continue;
                        var r = BlendShapes[i];
                        if (r.Mask.x != r.Mask.y && (dot + float.Epsilon) > r.Mask.x)
                        {
                                var remapped_dot = (dot - r.Mask.x) / (r.Mask.y - r.Mask.x);
                                /* if Additive, hold an input high when it's over active range */
                                remapped_dot = (Additive && (dot - float.Epsilon) > r.Mask.y) ? 1.0f : remapped_dot;
                                e = (r.Func != null) ? r.Func.Evaluate(remapped_dot) : remapped_dot;
                                if (Additive)
                                        Mesh.SetBlendShapeWeight(BlendShapeIndices[i], e * 100.0f);
                                else if ((dot - float.Epsilon) < r.Mask.y)
                                        last = i;
                        }
                }
                if (last != -1)
                        Mesh.SetBlendShapeWeight(last, e * 100.0f);
        }
}
