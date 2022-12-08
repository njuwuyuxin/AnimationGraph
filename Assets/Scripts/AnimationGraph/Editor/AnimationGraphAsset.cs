using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimationGraph.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationGraphAsset", menuName = "ScriptableObjects/AnimationGraphAsset")]
    public class AnimationGraphAsset : ScriptableObject
    {
        public FinalPosePoseNodeConfig finalPosePoseNode;

        public List<NodeData> nodes;
        public List<PortData> ports;
        public List<EdgeData> edges;
        
        
#if UNITY_EDITOR
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            var animationGraphAsset = EditorUtility.InstanceIDToObject(instanceID) as AnimationGraphAsset;
            if (animationGraphAsset == null)
            {
                return false;
            }

            AnimationGraphEditorWindow.ShowWindow(animationGraphAsset);
            return true;
        }
#endif
    }

    public enum ENodeType
    {
        BaseNode = 0,
        FinalPoseNode = 1,
        AnimationClipNode = 2,
        BoolSelectorNode = 3,
    }

    public enum EPortDirection
    {
        Input = 0,
        Output = 1,
    }

    public enum EPortCapacity
    {
        Single = 0,
        Multi =1,
    }
    
    [Serializable]
    public class NodeData
    {
        public int id;
        public ENodeType nodeType;
        public float positionX;
        public float positionY;
    }

    [Serializable]
    public class PortData
    {
        public string portName;
        public int portId;
        public int nodeId;
        public EPortDirection direction;
        public EPortCapacity capacity;
    }

    [Serializable]
    public class EdgeData
    {
        public int inputPort;
        public int outputPort;
    }
}