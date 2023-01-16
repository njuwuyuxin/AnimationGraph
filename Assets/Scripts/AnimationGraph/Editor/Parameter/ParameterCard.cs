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
            m_NameLabel.RegisterCallback<ClickEvent>(OnNameLabelClicked);
        }

        private void OnNameLabelClicked(ClickEvent evt)
        {
            if (evt.clickCount == 2)
            {
                m_NameLabel.visible = false;
                TextField textField = new TextField();
                textField.value = m_NameLabel.text;
                textField.style.position = new StyleEnum<Position>(Position.Absolute);
                textField.style.height = new StyleLength(new Length(26));
                textField.style.marginTop = new StyleLength(new Length(8));
                textField.style.marginLeft = new StyleLength(new Length(10));
                textField.RegisterCallback<FocusOutEvent>(focusEvt =>
                {
                    m_NameLabel.text = parameterName = textField.text;
                    Remove(textField);
                    m_NameLabel.visible = true;
                });
                Add(textField);
                schedule.Execute(() =>
                {
                    textField.Focus();
                    textField.SelectAll();
                });
            }
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
