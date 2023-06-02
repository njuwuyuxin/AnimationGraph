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

        private TableListView m_TableList;
        
        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            Label conditionLabel = new Label("Conditions");
            conditionLabel.style.fontSize = 12;
            conditionLabel.style.paddingTop = conditionLabel.style.paddingBottom = 2;
            conditionLabel.style.borderBottomWidth = 1f;
            conditionLabel.style.borderBottomColor = Color.gray;
            root.Add(conditionLabel);

            m_TableList = new TableListView();
            m_TableList.AddColumn(CreateParameterColumn());
            m_TableList.AddColumn(CreateOperationColumn());
            m_TableList.AddColumn(CreateValueColumn());
            m_TableList.onAddRow = (i) => OnAddCondition(i);
            m_TableList.onDeleteRow = (element, i) => OnRemoveCondition(i); 
            LoadConditionsFromConfig();
            
            root.Add(m_TableList);
            
            return root;
        }

        private void LoadConditionsFromConfig()
        {
            var transitionConfig = edgeConfig as TransitionConfig;
            foreach (var condition in transitionConfig.conditions)
            {
                m_TableList.AddRow();
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
            var parameterCard = m_StateMachineGraphView.parameterBoard.parameterCards.First();
            condition.parameterId = parameterCard.id;
            condition.conditionType = EConditionType.NotEqual;
            condition.value = CreateValueByParameterCard(parameterCard);
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
                popup.value = m_StateMachineGraphView.parameterBoard.TryGetParameterById(transitionConfig.conditions[row].parameterId);

            };
            
            return column;
        }

        private void OnParameterChange(ChangeEvent<ParameterCard> evt)
        {
            var popup = evt.target as PopupField<ParameterCard>;
            var transitionConfig = edgeConfig as TransitionConfig;
            int row = (int)(popup.userData);
            var parameterCard = evt.newValue;
            
            var condition = transitionConfig.conditions[row];
            condition.parameterId = parameterCard.id;
            condition.conditionType = EConditionType.NotEqual;
            condition.value = CreateValueByParameterCard(parameterCard);

            m_TableList.RefreshRow(row);
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
                var transitionConfig = edgeConfig as TransitionConfig;
                var popup = element as PopupField<EConditionType>;
                popup.userData = row;

                bool isNumberParameter = false;
                switch (transitionConfig.conditions[row].value)
                {
                    case FloatParameter.FloatValue:
                    case IntParameter.IntValue:
                        isNumberParameter = true;
                        break;
                    case BoolParameter.BoolValue:
                    case StringParameter.StringValue:
                        isNumberParameter = false;
                        break;
                }

                popup.choices = isNumberParameter ? s_NumberConditions : s_ValueConditions;
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
                var value = new VisualElement()
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
                var transitionConfig = edgeConfig as TransitionConfig;
                DrawValueField(element, transitionConfig.conditions[row].value);
            };
            
            return column;
        }

        private void DrawValueField(VisualElement element, GraphParameter.Value value)
        {
            switch (value)
            {
                case BoolParameter.BoolValue boolValue:
                    DrawBoolValue(element, boolValue);
                    break;
                case FloatParameter.FloatValue floatValue:
                    DrawFloatValue(element, floatValue);
                    break;
                case IntParameter.IntValue intValue:
                    DrawIntValue(element, intValue);
                    break;
                case StringParameter.StringValue stringValue:
                    DrawStringValue(element, stringValue);
                    break;
            }
        }

        private void DrawBoolValue(VisualElement element, BoolParameter.BoolValue boolValue)
        {
            element.Clear();
            var boolField = new Toggle();
            boolField.value = boolValue.boolValue;
            boolField.RegisterValueChangedCallback(evt =>
            {
                boolValue.boolValue = evt.newValue;
            });
            element.Add(boolField);
        }
        
        private void DrawFloatValue(VisualElement element, FloatParameter.FloatValue floatValue)
        {
            element.Clear();
            var floatField = new FloatField();
            floatField.value = floatValue.floatValue;
            floatField.RegisterValueChangedCallback(evt =>
            {
                floatValue.floatValue = evt.newValue;
            });
            element.Add(floatField);
        }

        private void DrawIntValue(VisualElement element, IntParameter.IntValue intValue)
        {
            element.Clear();
            var intField = new IntegerField();
            intField.value = intValue.intValue;
            intField.RegisterValueChangedCallback(evt =>
            {
                intValue.intValue = evt.newValue;
            });
            element.Add(intField);
        }
        
        private void DrawStringValue(VisualElement element, StringParameter.StringValue stringValue)
        {
            element.Clear();
            var textField = new TextField();
            textField.value = stringValue.stringValue;
            textField.RegisterValueChangedCallback(evt =>
            {
                stringValue.stringValue = evt.newValue;
            });
            element.Add(textField);
        }

        private GraphParameter.Value CreateValueByParameterCard(ParameterCard parameterCard)
        {
            switch (parameterCard)
            {
                case BoolParameterCard: return new BoolParameter.BoolValue();
                case FloatParameterCard: return new FloatParameter.FloatValue();
                case IntParameterCard: return new IntParameter.IntValue();
                case StringParameterCard: return new StringParameter.StringValue();
            }

            return null;
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