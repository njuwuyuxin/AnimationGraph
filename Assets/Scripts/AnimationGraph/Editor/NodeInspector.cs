using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class NodeInspector : UnityEditor.Editor
    {
        class NodeInspectorObject : ScriptableObject
        {
            [SerializeReference]
            public NodeConfig m_NodeConfig;
        }

        private GraphNode m_GraphNode;
        private NodeConfig m_NodeConfig;
        private NodeInspectorObject m_NodeInspectorObject;
        private SerializedObject m_SerializedObject;
        private bool m_DrawInspectorCustomize;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Inspector", new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            });
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (!m_DrawInspectorCustomize)
            {
                if (m_SerializedObject != null)
                {
                    var nodeConfig = m_SerializedObject.FindProperty("m_NodeConfig");
                    if (nodeConfig != null)
                    {
                        while (nodeConfig.NextVisible(true))
                        {
                            EditorGUILayout.PropertyField(nodeConfig);
                        }

                        if (m_SerializedObject.hasModifiedProperties)
                        {
                            m_SerializedObject.ApplyModifiedProperties();
                            m_GraphNode.OnNodeConfigUpdate();
                        }
                    }
                }
            }

            if (m_GraphNode != null)
            {
                m_GraphNode.OnNodeInspectorGUI();
            }
        }

        public void SetGraphNode(GraphNode graphNode, bool drawInspectorCustomize)
        {
            if (m_NodeInspectorObject != null)
            {
                DestroyImmediate(m_NodeInspectorObject);
            }

            m_DrawInspectorCustomize = drawInspectorCustomize;
            m_GraphNode = graphNode;
            m_NodeConfig = graphNode.nodeConfig;
            m_NodeInspectorObject = ScriptableObject.CreateInstance<NodeInspectorObject>();
            m_NodeInspectorObject.m_NodeConfig = m_NodeConfig;
            m_SerializedObject = new SerializedObject(m_NodeInspectorObject);
        }
    }
}
