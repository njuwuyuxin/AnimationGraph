using System;
using System.Collections.Generic;
using UnityEditor;
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
        
        private List<string> m_Selections = new List<string>();

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
            m_NodeConfig = new StringSelectorPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Condition", NodePort.EPortType.ValuePort, 0);
        }

        public override void OnNodeInspectorGUI()
        {
            m_Selections = (m_NodeConfig as StringSelectorPoseNodeConfig).selections;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                m_Selections.Add("Default String");
                
                var config = m_NodeConfig as StringSelectorPoseNodeConfig;
                config.selections = m_Selections;
            }
            
            if (GUILayout.Button("-"))
            {
                if (m_Selections.Count > 0)
                {
                    m_Selections.RemoveAt(m_Selections.Count - 1);
                }
                
                var config = m_NodeConfig as StringSelectorPoseNodeConfig;
                config.selections = m_Selections;
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < m_Selections.Count; i++)
            {
                m_Selections[i] = GUILayout.TextField(m_Selections[i]);
            }
        }
    }
}
