using System;
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
        private VisualElement m_InspectorArea;

        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";

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
            
            m_GraphArea = templateContainer.Q("GraphArea");

            InitAnimGraphView();
        }

        private void InitAnimGraphView()
        {
            m_AnimationGraphView = new AnimationGraphView();
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
        }
    }
}