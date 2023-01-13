using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ParameterBoard : VisualElement
    {
        private Button m_AddParameterButton;
        private AnimationGraphAsset m_AnimationGraphAsset;
        private ToolbarMenu m_AddParameterToolBar;
        private VisualElement m_ToolBarArea;
        private VisualElement m_ParameterArea;
        private List<ParameterCard> m_ParameterCards;
        public ParameterBoard()
        {
            var parameterBoardVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/AnimationGraph/Editor/UIDocuments/ParameterBoard.uxml");
            VisualElement parameterBoardElement = parameterBoardVisualTree.CloneTree();
            Add(parameterBoardElement);
            style.flexGrow = 1;

            m_ToolBarArea = parameterBoardElement.Q("ToolBarArea");
            m_AddParameterToolBar = new ToolbarMenu();
            m_AddParameterToolBar.text = "Add Parameter";
            m_AddParameterToolBar.menu.AppendAction(
                "Add Bool Parameter",
                actionEvent => CreateDefaultBoolParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add Int Parameter",
                actionEvent => CreateDefaultIntParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add Float Parameter",
                actionEvent => CreateDefaultFloatParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add String Parameter",
                actionEvent => CreateDefaultStringParameter()
            );
            
            m_ToolBarArea.Add(m_AddParameterToolBar);

            m_ParameterArea = parameterBoardElement.Q("ParameterArea");
            m_ParameterCards = new List<ParameterCard>();
        }

        public void ClearParameterBoard()
        {
            foreach (var parameterCard in m_ParameterCards)
            {
                m_ParameterArea.Remove(parameterCard);
            }
            m_ParameterCards.Clear();
        }
        
        public void LoadAnimGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            foreach (var parameterData in m_AnimationGraphAsset.parameters)
            {
                CreateParameterCard(parameterData);
            }
        }

        public void Save()
        {
            m_AnimationGraphAsset.parameters.Clear();
            foreach (var parameterCard in m_ParameterCards)
            {
                ParameterData parameterData = null;
                if (parameterCard is BoolParameterCard)
                {
                    parameterData = new BoolParameterData();
                    parameterData.name = parameterCard.parameterName;
                }
                else if (parameterCard is IntParameterCard)
                {
                    parameterData = new IntParameterData();
                    parameterData.name = parameterCard.parameterName;
                }
                else if (parameterCard is FloatParameterCard)
                {
                    parameterData = new FloatParameterData();
                    parameterData.name = parameterCard.parameterName;
                }
                else if (parameterCard is StringParameterCard)
                {
                    parameterData = new StringParameterData();
                    parameterData.name = parameterCard.parameterName;
                }
                else
                {
                    parameterData = new ParameterData();
                    parameterData.name = parameterCard.parameterName;
                }
                m_AnimationGraphAsset.parameters.Add(parameterData);
            }
        }

        private void CreateParameterCard(ParameterData parameterData)
        {
            ParameterCard parameterCard = null;
            if (parameterData is BoolParameterData)
            {
                parameterCard = new BoolParameterCard(this, parameterData.name);
            }
            else if (parameterData is IntParameterData)
            {
                parameterCard = new IntParameterCard(this, parameterData.name);
            }
            else if (parameterData is FloatParameterData)
            {
                parameterCard = new FloatParameterCard(this, parameterData.name);
            }
            else if (parameterData is StringParameterData)
            {
                parameterCard = new StringParameterCard(this, parameterData.name);
            }
            else
            {
                parameterCard = new ParameterCard(this, parameterData.name);
            }
            
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }

        public void DeleteParameterCard(ParameterCard parameterCard)
        {
            m_ParameterCards.Remove(parameterCard);
            m_ParameterArea.Remove(parameterCard);
        }

        private void CreateDefaultBoolParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("boolParameter");
            BoolParameterCard parameterCard = new BoolParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultIntParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("intParameter");
            IntParameterCard parameterCard = new IntParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultFloatParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("floatParameter");
            FloatParameterCard parameterCard = new FloatParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultStringParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("stringParameter");
            StringParameterCard parameterCard = new StringParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }

        private string GenerateDefaultParameterName(string defaultName)
        {
            foreach (var parameterCard in m_ParameterCards)
            {
                if (parameterCard.parameterName.Equals(defaultName))
                {
                    return GenerateDefaultParameterName(defaultName + "(copy)");
                }
            }

            return defaultName;
        }
    }
}