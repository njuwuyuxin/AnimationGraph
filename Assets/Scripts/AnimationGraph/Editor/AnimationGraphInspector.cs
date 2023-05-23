using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class AnimationGraphInspector : UnityEditor.Editor
    {
        class NodeInspectorObject : ScriptableObject
        {
            [SerializeReference]
            public NodeConfig m_NodeConfig;
        }

        class EdgeInspectorObject : ScriptableObject
        {
            [SerializeReference]
            public EdgeConfig m_EdgeConfig;
        }

        public enum EInspectorType
        {
            Null,
            Node,
            Edge,
        }

        private EInspectorType m_InspectorType = EInspectorType.Null;

        private GraphNode m_GraphNode;
        private NodeConfig m_NodeConfig => m_GraphNode.nodeConfig;
        private StateTransition m_StateTransition;

        private EdgeConfig m_EdgeConfig => m_StateTransition.edgeConfig;
        
        private NodeInspectorObject m_NodeInspectorObject;
        private EdgeInspectorObject m_EdgeInspectorObject;
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

            if (m_InspectorType == EInspectorType.Null)
            {
                return;
            }
            else if (m_InspectorType == EInspectorType.Node)
            {
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
            else if (m_InspectorType == EInspectorType.Edge)
            {
                if (!m_DrawInspectorCustomize)
                {
                    if (m_SerializedObject != null)
                    {
                        var edgeConfig = m_SerializedObject.FindProperty("m_EdgeConfig");
                        if (edgeConfig != null)
                        {
                            while (edgeConfig.NextVisible(true))
                            {
                                EditorGUILayout.PropertyField(edgeConfig);
                            }

                            if (m_SerializedObject.hasModifiedProperties)
                            {
                                m_SerializedObject.ApplyModifiedProperties();
                                m_StateTransition.OnEdgeConfigUpdate();
                            }
                        }
                    }
                }

                if (m_StateTransition != null)
                {
                    m_StateTransition.OnEdgeInspectorGUI();
                }
            }
        }

        public void SetGraphNode(GraphNode graphNode, bool drawInspectorCustomize)
        {
            ClearInspector();
            m_InspectorType = EInspectorType.Node;
            m_DrawInspectorCustomize = drawInspectorCustomize;
            m_GraphNode = graphNode;
            m_NodeInspectorObject = ScriptableObject.CreateInstance<NodeInspectorObject>();
            m_NodeInspectorObject.m_NodeConfig = m_NodeConfig;
            m_SerializedObject = new SerializedObject(m_NodeInspectorObject);
        }

        public void SetEdge(StateTransition edge, bool drawInspectorCustomize)
        {
            ClearInspector();
            m_InspectorType = EInspectorType.Edge;
            m_DrawInspectorCustomize = drawInspectorCustomize;
            m_StateTransition = edge;
            m_EdgeInspectorObject = ScriptableObject.CreateInstance<EdgeInspectorObject>();
            m_EdgeInspectorObject.m_EdgeConfig = m_EdgeConfig;
            m_SerializedObject = new SerializedObject(m_EdgeInspectorObject);
        }

        public void ClearInspector()
        {
            if (m_NodeInspectorObject != null)
            {
                DestroyImmediate(m_NodeInspectorObject);
            }

            if (m_EdgeInspectorObject != null)
            {
                DestroyImmediate(m_EdgeInspectorObject);
            }

            m_DrawInspectorCustomize = false;
            m_GraphNode = null;

            m_NodeInspectorObject = null;
            m_SerializedObject = null;
            m_InspectorType = EInspectorType.Null;
        }
    }
}
