using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AnimationGraph.Editor
{
    public class AnimationGraphEditorWindow : EditorWindow
    {
        private VisualElement m_ParameterArea;
        private VisualElement m_GraphArea;
        private IMGUIContainer m_InspectorArea;
        private NodeInspector m_NodeInspector;

        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        private const string k_CompiledGraphSavePath = "Assets/Data/AnimationGraph/";

        private AnimationGraphAsset m_AnimationGraphAsset;
        private AnimationGraphView m_AnimationGraphView;
        
        
        [MenuItem("Window/UI Toolkit/AnimationGraphEditorWindow")]
        public static void ShowWindow()
        {
            ShowWindow(null);
        }
        
        public static void ShowWindow(AnimationGraphAsset animationGraphAsset)
        {
            AnimationGraphEditorWindow wnd = GetWindow<AnimationGraphEditorWindow>();
            wnd.titleContent = new GUIContent("AnimationGraphEditorWindow");

            if (animationGraphAsset != null)
            {
                wnd.LoadAnimationGraphAsset(animationGraphAsset);
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Scripts/AnimationGraph/Editor/UIDocuments/AnimationGraphEditorWindow.uxml");
            VisualElement templateContainer = visualTree.CloneTree();
            templateContainer.style.flexGrow = 1;
            
            var editorWindowStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StyleSheetPrefix + "AnimationGraphEditorWindow.uss");
            if (editorWindowStyleSheet != null)
            {
                templateContainer.styleSheets.Add(editorWindowStyleSheet);
            }

            root.Add(templateContainer);
            
            Button saveButton = new Button(Save);
            saveButton.Add(new Label("Save"));
            templateContainer.Add(saveButton);
            
            Button compileButton = new Button(CompileGraph);
            compileButton.Add(new Label("Compile"));
            templateContainer.Add(compileButton);
            
            m_GraphArea = templateContainer.Q("GraphArea");
            m_ParameterArea = templateContainer.Q("ParameterArea");
            m_InspectorArea = templateContainer.Q("InspectorArea") as IMGUIContainer;
            m_NodeInspector = CreateInstance<NodeInspector>();
            m_InspectorArea.onGUIHandler = m_NodeInspector.OnInspectorGUI;

            InitAnimGraphView();
        }

        private void InitAnimGraphView()
        {
            m_AnimationGraphView = new AnimationGraphView(m_NodeInspector);
            m_GraphArea.Add(m_AnimationGraphView);
            m_AnimationGraphView.StretchToParentSize();
        }

        private void LoadAnimationGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            m_AnimationGraphView.LoadAnimGraphAsset(m_AnimationGraphAsset);
        }

        private void Save()
        {
            m_AnimationGraphView.Save();
            EditorUtility.DisplayDialog("Success", "Animation Graph Save Successfully!", "OK");
        }

        private void CompileGraph()
        {
            AnimationGraph compiledGraph = null;
            string savePath = k_CompiledGraphSavePath + m_AnimationGraphAsset.name + "_Compiled.asset";
            if (!System.IO.Directory.Exists(k_CompiledGraphSavePath))
            {
                System.IO.Directory.CreateDirectory(k_CompiledGraphSavePath);
            }
            
            if (System.IO.File.Exists(savePath))
            {
                compiledGraph = AssetDatabase.LoadAssetAtPath<AnimationGraph>(savePath);
                m_AnimationGraphView.Compile(compiledGraph);
                compiledGraph.parameters = new List<GraphParameter>();
                EditorUtility.SetDirty(compiledGraph);
                AssetDatabase.SaveAssets();
            }
            else
            {
                compiledGraph = CreateInstance<AnimationGraph>();
                m_AnimationGraphView.Compile(compiledGraph);
                compiledGraph.parameters = new List<GraphParameter>();
                AssetDatabase.CreateAsset(compiledGraph, savePath);
            }
            
            EditorUtility.DisplayDialog("Success", "Animation Graph Compile Successfully!", "OK");

        }
    }
}