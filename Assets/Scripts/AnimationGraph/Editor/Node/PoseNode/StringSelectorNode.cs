using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StringSelectorNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StringSelectorNode;

        [Serializable]
        public class StringSelectorData : CustomSerializableData
        {
            public string testString;
        }

        private StringSelectorData m_StringSelectorData => customData as StringSelectorData;

        public StringSelectorNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "StringSelector";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
            //创建Node时创建Data，后续Load NodeData时存在gc，后续优化
            m_CustomData = new StringSelectorData();
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new BoolSelectorPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Condition", NodePort.EPortType.ValuePort, 0);
        }

        public override void OnNodeInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                
            }

            if (GUILayout.Button("-"))
            {
                
            }
            GUILayout.EndHorizontal();
            m_StringSelectorData.testString = GUILayout.TextField(m_StringSelectorData.testString);

        }
    }
}
