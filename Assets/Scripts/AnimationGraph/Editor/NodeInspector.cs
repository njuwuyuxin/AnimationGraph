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
        
        private NodeConfig m_NodeConfig;
        private NodeInspectorObject m_NodeInspectorObject;
        private SerializedObject m_SerializedObject;

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
            if (m_SerializedObject != null)
            {
                var nodeConfig = m_SerializedObject.FindProperty("m_NodeConfig");

                while (nodeConfig.NextVisible(true))
                {
                    EditorGUILayout.PropertyField(nodeConfig);
                }

                m_SerializedObject.ApplyModifiedProperties();
            }
        }

        public void SetNodeConfig(NodeConfig nodeConfig)
        {
            if (m_NodeInspectorObject != null)
            {
                DestroyImmediate(m_NodeInspectorObject);
            }
            m_NodeConfig = nodeConfig;
            m_NodeInspectorObject = CreateInstance<NodeInspectorObject>();
            m_NodeInspectorObject.m_NodeConfig = m_NodeConfig;
            m_SerializedObject = new SerializedObject(m_NodeInspectorObject);
        }
    }
}
