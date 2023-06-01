using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class StateTransition
    {
        private static List<EConditionType> s_ValueConditions = new List<EConditionType>()
        {
            EConditionType.NotEqual,
            EConditionType.Equal
        };
        
        private static List<EConditionType> s_NumberConditions = new List<EConditionType>()
        {
            EConditionType.NotEqual,
            EConditionType.Equal,
            EConditionType.Less,
            EConditionType.LessEqual,
            EConditionType.Greater,
            EConditionType.GreaterEqual
        };

        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            Label conditionLabel = new Label("Conditions");
            conditionLabel.style.fontSize = 12;
            conditionLabel.style.paddingTop = conditionLabel.style.paddingBottom = 2;
            conditionLabel.style.borderBottomWidth = 1f;
            conditionLabel.style.borderBottomColor = Color.gray;
            root.Add(conditionLabel);

            var tableList = new TableListView();
            tableList.AddColumn(CreateParameterColumn());
            tableList.AddColumn(CreateOperationColumn());
            tableList.AddColumn(CreateValueColumn());
            tableList.onAddRow = (i) => OnAddCondition(i);
            tableList.onDeleteRow = (element, i) => OnRemoveCondition(i); 
            LoadConditionsFromConfig(tableList);
            
            root.Add(tableList);
            
            return root;
        }

        private void LoadConditionsFromConfig(TableListView tableListView)
        {
            var transitionConfig = edgeConfig as TransitionConfig;
            foreach (var condition in transitionConfig.conditions)
            {
                tableListView.AddRow();
            }
        }
        
        private void OnAddCondition(int row)
        {
            var transitionConfig = edgeConfig as TransitionConfig;
            
            //Load condition from config
            if (row < transitionConfig.conditions.Count)
            {
                return;
            }

            //Add new condition
            TransitionCondition condition = new TransitionCondition();
            condition.parameterId = m_StateMachineGraphView.parameterBoard.parameterCards.First().id;
            condition.conditionType = EConditionType.NotEqual;
            transitionConfig.conditions.Add(condition);
        }

        private void OnRemoveCondition(int row)
        {
            var transitionConfig = edgeConfig as TransitionConfig;
            transitionConfig.conditions.RemoveAt(row);
        }
        
        private TableListView.Column CreateParameterColumn()
        {
            var column = new TableListView.Column();
            column.name = "Parameter";
            column.title = () =>
            {
                var parameterLabel = new Label("Parameter")
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };
                return parameterLabel;
            };

            column.cellTemplate = () =>
            {
                var parameterList = m_StateMachineGraphView.parameterBoard.parameterCards;
                var parameter = new PopupField<ParameterCard>(parameterList, parameterList[0], ConvertParameterToString,
                    ConvertParameterToString)
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };

                parameter.RegisterValueChangedCallback(OnParameterChange);

                return parameter;
            };

            column.refreshCell = (element, row) =>
            {
                var popup = element as PopupField<ParameterCard>;
                popup.userData = row;

                var transitionConfig = edgeConfig as TransitionConfig;
                popup.value = m_StateMachineGraphView.parameterBoard.parameterCards.Find(p =>
                        p.id == transitionConfig.conditions[row].parameterId);
            };
            
            return column;
        }

        private void OnParameterChange(ChangeEvent<ParameterCard> evt)
        {
            var popup = evt.target as PopupField<ParameterCard>;
            var transitionConfig = edgeConfig as TransitionConfig;
            int row = (int)(popup.userData);
            transitionConfig.conditions[row].parameterId = evt.newValue.id;
            
            //TODO: operation type change to default
        }
        
        private TableListView.Column CreateOperationColumn()
        {
            var column = new TableListView.Column();
            column.name = "Operation";
            column.title = () =>
            {
                var operationLabel = new Label("Operation")
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };
                return operationLabel;
            };

            column.cellTemplate = () =>
            {
                var operation = new PopupField<EConditionType>(s_NumberConditions, EConditionType.Equal,
                    ConvertConditionToString, ConvertConditionToString)
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };
                operation.RegisterValueChangedCallback(OnOperationTypeChange);

                return operation;
            };
            
            column.refreshCell = (element, row) =>
            {
                var popup = element as PopupField<EConditionType>;
                popup.userData = row;
                var transitionConfig = edgeConfig as TransitionConfig;
                popup.value = transitionConfig.conditions[row].conditionType;
            };
            
            return column;
        }

        private void OnOperationTypeChange(ChangeEvent<EConditionType> evt)
        {
            var popup = evt.target as PopupField<EConditionType>;
            var transitionConfig = edgeConfig as TransitionConfig;
            int row = (int)(popup.userData);
            transitionConfig.conditions[row].conditionType = evt.newValue;
        }
        
        private TableListView.Column CreateValueColumn()
        {
            var column = new TableListView.Column();
            column.name = "Value";
            column.title = () =>
            {
                var valueLabel = new Label("Value")
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };
                
                return valueLabel;
            };

            column.cellTemplate = () =>
            {
                var value = new TextField()
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.gray
                    }
                };

                return value;
            };
            
            column.refreshCell = (element, row) =>
            {

            };
            
            return column;
        }
        
        private TableListView.Column CreateDeleteColumn()
        {
            var column = new TableListView.Column();
            column.name = " ";
            column.title = () =>
            {
                var placeHolderLabel = new Label(" ")
                {
                    style =
                    {
                        width = 20,
                    }
                };
                return placeHolderLabel;
            };

            column.cellTemplate = () =>
            {
                var deleteButton = new Button(() => { });
                deleteButton.Add(new Label("×"));
                deleteButton.style.width = 20;
                return deleteButton;
            };
            
            return column;
        }

        private string ConvertParameterToString(ParameterCard parameterCard)
        {
            return parameterCard != null ? parameterCard.parameterName : " ";
        }
        
        private string ConvertConditionToString(EConditionType conditionType)
        {
            switch (conditionType)
            {
                case EConditionType.NotEqual: return "!=";
                case EConditionType.Equal: return "==";
                case EConditionType.Greater: return ">";
                case EConditionType.GreaterEqual: return ">=";
                case EConditionType.Less: return "<";
                case EConditionType.LessEqual: return "<=";
            }

            return string.Empty;
        }
    }
}