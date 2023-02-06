using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        private void RegisterCallbacks()
        {
            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);
        }

        void OnDragEnter(DragEnterEvent evt)
        {
        }
        
        void OnDragLeave(DragLeaveEvent evt)
        {
            
        }
        
        void OnDragUpdate(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        
        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            
            var parameterCard = DragAndDrop.GetGenericData("parameterCard") as ParameterCard;
            if (parameterCard != null)
            {
                GraphNode node = CreateParameterNode(parameterCard, MouseToViewPosition(evt.mousePosition));
                parameterCard.associatedNodes.Add(node.id);
            }
            
            Debug.Log(parameterCard.parameterName);
        }
        
        void OnDragExit(DragExitedEvent evt)
        { 
            
        }
    }
}
