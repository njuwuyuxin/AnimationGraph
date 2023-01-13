using UnityEditor;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ParameterCard : VisualElement
    {
        private Label m_NameLabel;

        public string parameterName
        {
            get => m_Name;
            set => m_Name = value;
        }
        protected string m_Name;
        protected ParameterBoard m_ParameterBoard;
        protected TemplateContainer m_ParameterCardTemplateContainer;

        public ParameterCard(ParameterBoard parameterBoard, string name)
        {
            m_ParameterBoard = parameterBoard;
            m_Name = name;
            
            var parameterCardVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/AnimationGraph/Editor/UIDocuments/ParameterCard.uxml");
            m_ParameterCardTemplateContainer = parameterCardVisualTree.CloneTree();
            m_ParameterCardTemplateContainer.style.flexGrow = 1;
            Add(m_ParameterCardTemplateContainer);
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/AnimationGraph/Editor/StyleSheet/ParameterCardStyleSheet.uss");
            styleSheets.Add(styleSheet);
            
            m_NameLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterName");
            m_NameLabel.text = m_Name;
            
            this.AddManipulator(CreateContextualMenu());
        }
        
        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    menuEvent.menu.AppendAction(
                        "Delete",
                        actionEvent => Delete()
                    );
                });
            return contextualMenuManipulator;
        }

        public void Delete()
        {
            m_ParameterBoard.DeleteParameterCard(this);
        } 
    }
}
