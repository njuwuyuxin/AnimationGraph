using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateTransition : GraphElement
    {
        public int id { get; set; }
        private readonly List<Vector2> m_RenderPoints = new List<Vector2>();
        private StateNode m_SourceState;
        private StateNode m_TargetState;
        
        private static readonly float s_ArrowCosAngle = Mathf.Cos(60);
        private static readonly float s_ArrowSinAngle = Mathf.Sin(60);
        private static readonly float s_ArrowWidth = 20;
        private const float k_MinTransitionWidth = 1.75f;
        private const float k_ArrowLength = 4f;

        private Vector2 m_From;

        public Vector2 from
        {
            get => m_From;
            set
            {
                m_From = value;
                OnTransitionPointChange();
            }
        }

        private Vector2 m_To;
        public Vector2 to
        {
            get => m_To;
            set
            {
                m_To = value;
                OnTransitionPointChange();
            }
        }
        
        public StateTransition(StateNode source, StateNode target)
        {
            m_SourceState = source;
            m_TargetState = target;
            source.AddOutputTransition(this);
            target.AddInputTransition(this);
            
            m_From = m_SourceState.GetPosition().center;
            m_To = m_TargetState.GetPosition().center;
            generateVisualContent += OnGenerateVisualContent;
        }

        public void OnTransitionPointChange()
        {
            MarkDirtyRepaint();
        }
        
        protected virtual void UpdateRenderPoints()
        {
            var p1 = parent.ChangeCoordinatesTo(this, m_From);
            var p2 = parent.ChangeCoordinatesTo(this, m_To);
            
            m_RenderPoints.Clear();
            RenderArrow(p1, p2);
        }
        
        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            UpdateRenderPoints();
            
            if (m_RenderPoints.Count == 0)
            {
                return;
            }

            var md = context.Allocate(8, 12, null);
            if (md.vertexCount == 0)
            {
                return;
            }

            // setup line
            var halfWidth = 1f * 0.5f;
            
            var p0 = m_RenderPoints[0];
            var p1 = m_RenderPoints[1];
            
            var v = p1 - p0;
            v *= halfWidth / v.magnitude;
            v = new Vector2(-v.y, v.x);

            md.SetNextVertex(new Vertex { position = p0 + v, tint = Color.gray });
            md.SetNextVertex(new Vertex { position = p0 - v, tint = Color.gray });
            md.SetNextVertex(new Vertex { position = p1 + v, tint = Color.gray });
            md.SetNextVertex(new Vertex { position = p1 - v, tint = Color.gray });
            
            md.SetNextIndex(0);
            md.SetNextIndex(1);
            md.SetNextIndex(2);
            md.SetNextIndex(1);
            md.SetNextIndex(3);
            md.SetNextIndex(2);

            // Setup arrow
            md.SetNextVertex(new Vertex { position = m_RenderPoints[2], tint = Color.gray });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[3], tint = Color.gray });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[4], tint = Color.gray });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[5], tint = Color.gray });
            
            md.SetNextIndex(4);
            md.SetNextIndex(5);
            md.SetNextIndex(6);
            md.SetNextIndex(4);
            md.SetNextIndex(7);
            md.SetNextIndex(5);
        }

        private static bool Approximately(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
        }
        
        private void RenderArrow(Vector2 p1, Vector2 p2)
        {
            if (Approximately(p1, p2))
            {
                return;
            }
            
            // line
            m_RenderPoints.Add(p1);
            m_RenderPoints.Add(p2);
            
            // arrow
            var v = p2 - p1;
            v *= s_ArrowWidth / v.magnitude;
            var v1 = new Vector2(v.x * s_ArrowCosAngle - v.y * s_ArrowSinAngle, v.x * s_ArrowSinAngle + v.y * s_ArrowCosAngle);
            var v2 = new Vector2(v.x * s_ArrowCosAngle + v.y * s_ArrowSinAngle, v.x * -s_ArrowSinAngle + v.y * s_ArrowCosAngle);
            
            m_RenderPoints.Add(p2);
            m_RenderPoints.Add(p2 + (p1 - p2).normalized * s_ArrowWidth * 0.5f);
            m_RenderPoints.Add(p2 + v1);
            m_RenderPoints.Add(p2 + v2);
        }
    }
}