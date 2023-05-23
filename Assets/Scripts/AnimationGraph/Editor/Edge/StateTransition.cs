using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateTransition : GraphElement
    {
        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        
        private static CustomStyleProperty<int> s_TranstionWidthProperty = new CustomStyleProperty<int>("--edge-width");
        private static CustomStyleProperty<Color> s_TransitionEdgeColorProperty = new CustomStyleProperty<Color>("--selected-edge-color");
        private static CustomStyleProperty<Color> s_TransitionColorProperty = new CustomStyleProperty<Color>("--edge-color");
        
        public int id { get; set; }

        private StateNode m_SourceState;
        private StateNode m_TargetState;
        private StateMachineGraphView m_StateMachineGraphView;
        
        private static readonly float s_ArrowWidth = 20;
        private const float k_MaxInterval = 10;
        private const float k_InterceptWidth = 6.0f;

        public int transitionWidth { get; set; } = s_DefaultTransitionWidth;
        public Color selectedColor { get; private set; } = s_DefaultSelectedColor;
        public Color defaultColor { get; private set; } = s_DefaultColor;
        private static readonly int s_DefaultTransitionWidth = 2;
        private static readonly Color s_DefaultSelectedColor = new Color(240 / 255f, 240 / 255f, 240 / 255f);
        private static readonly Color s_DefaultColor = new Color(146 / 255f, 146 / 255f, 146 / 255f);
        
        private TransitionControl m_TransitionControl;

        private TransitionConfig m_EdgeConfig;
        public TransitionConfig edgeConfig => m_EdgeConfig;

        public TransitionControl transitionControl
        {
            get
            {
                if (m_TransitionControl == null)
                {
                    m_TransitionControl = CreateTransitionControl();
                }

                return m_TransitionControl;
            }
        }
        
        protected virtual TransitionControl CreateTransitionControl()
        {
            return new TransitionControl(m_StateMachineGraphView)
            {
                interceptWidth = k_InterceptWidth
            };
        }

        private Vector2 m_From;

        public Vector2 from
        {
            get => m_From;
            set
            {
                m_From = value;
                transitionControl.from = m_From;
            }
        }

        private Vector2 m_To;
        public Vector2 to
        {
            get => m_To;
            set
            {
                m_To = value;
                transitionControl.to = m_To;
            }
        }
        
        private Vector2 m_CandidatePosition;
        private Vector2 m_GlobalCandidatePosition;

        public Vector2 candidatePosition
        {
            get => m_CandidatePosition;
            set
            {
                if (!Approximately(m_CandidatePosition, value))
                {
                    m_CandidatePosition = value;

                    m_GlobalCandidatePosition = this.WorldToLocal(m_CandidatePosition);

                    UpdateTransitionControl();
                }
            }
        }
        
        public StateTransition(StateMachineGraphView stateMachineGraphView, StateNode source, StateNode target)
        {
            m_StateMachineGraphView = stateMachineGraphView;
            m_SourceState = source;
            m_TargetState = target;
            if (source != null)
            {
                source.AddOutputTransition(this);
                m_From = m_SourceState.GetPosition().center;
            }

            if (target != null)
            {
                target.AddInputTransition(this);
                m_To = m_TargetState.GetPosition().center;
            }

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StyleSheetPrefix + "StateTransition.uss"));
            AddToClassList("state-transition");

            style.position = Position.Absolute;
            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Copiable;

            transitionControl.from = m_From;
            transitionControl.to = m_To;
            transitionControl.color = Color.gray;
            Add(transitionControl);
            UpdateTransitionControl();
            
            RegisterCallback<AttachToPanelEvent>(OnTransitionAttach);
        }

        public void InitializeDefault()
        {
            id = Animator.StringToHash(Guid.NewGuid().ToString());
            m_EdgeConfig = new TransitionConfig();
            m_EdgeConfig.SetId(id);
        }

        public void LoadConfig(TransitionConfig transitionConfig)
        {
            m_EdgeConfig = transitionConfig;
            id = transitionConfig.id;
        }
        
        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);

            if (styles.TryGetValue(s_TranstionWidthProperty, out var edgeWidthValue))
            {
                transitionWidth = edgeWidthValue;
            }

            if (styles.TryGetValue(s_TransitionEdgeColorProperty, out var selectColorValue))
            {
                selectedColor = selectColorValue;
            }

            if (styles.TryGetValue(s_TransitionColorProperty, out var edgeColorValue))
            {
                defaultColor = edgeColorValue;
            }

            UpdateTransitionControlColorsAndWidth();
        }
        
        private void OnTransitionAttach(AttachToPanelEvent evt)
        {
            UpdateTransitionControl();
        }

        #region UNITY_CALLS
        public override bool ContainsPoint(Vector2 localPoint)
        {
            var result = UpdateTransitionControl() &&
                         transitionControl.ContainsPoint(this.ChangeCoordinatesTo(transitionControl, localPoint));
            return result;
        }
        
        public override bool Overlaps(Rect rectangle)
        {
            if (!UpdateTransitionControl())
            {
                return false;
            }

            rectangle.height = 5f;
            rectangle.width = 5f;
            return transitionControl.Overlaps(this.ChangeCoordinatesTo(transitionControl, rectangle));
        }
        
        public override void OnSelected()
        {
            UpdateTransitionControlColorsAndWidth();
            m_StateMachineGraphView.inspector.SetEdge(this, false);
        }

        public override void OnUnselected()
        {
            UpdateTransitionControlColorsAndWidth();
            m_StateMachineGraphView.inspector.ClearInspector();
        }
        

        #endregion

        //Update All TransitionControl Properties 
        public virtual bool UpdateTransitionControl()
        {
            if (m_SourceState == null && m_SourceState == null)
            {
                return false;
            }

            if (m_StateMachineGraphView == null)
            {
                return false;
            }

            UpdateTransitionPoints();
            transitionControl.UpdateLayout();
            UpdateTransitionControlColorsAndWidth();

            return true;
        }
        
        private void UpdateTransitionPoints()
        {
            if (m_TargetState == null && m_SourceState == null)
            {
                return;
            }
            
            if (m_TargetState == null)
            {
                transitionControl.from = m_SourceState.GetPosition().center;
                transitionControl.to = m_GlobalCandidatePosition;
            }
            else if (m_SourceState == null)
            {
                transitionControl.from = m_GlobalCandidatePosition;
                transitionControl.to = m_TargetState.GetPosition().center;
            }
            else
            {
                ComputeControlPoints();
            }
        }
        
        private void UpdateTransitionControlColorsAndWidth()
        {
            if (selected)
            {
                transitionControl.color = selectedColor;
                transitionControl.transitionWidth = transitionWidth;
            }
            else
            {
                transitionControl.color = defaultColor;
                transitionControl.transitionWidth = transitionWidth;
            }
        }
        
        private void ComputeControlPoints()
        {
            var inputTransitionCount = 0;
            var outputTransitionCount = 0;
            var index = -1;

            foreach (var transition in m_SourceState.outputTransitions)
            {
                if (transition.m_TargetState == m_TargetState)
                {
                    if (ReferenceEquals(this, transition))
                    {
                        index = outputTransitionCount;
                    }

                    outputTransitionCount++;
                }
            }
            
            foreach (var transition in m_TargetState.outputTransitions)
            {
                if (transition.m_TargetState == m_SourceState)
                {
                    if (ReferenceEquals(this, transition))
                    {
                        index = outputTransitionCount;
                    }

                    outputTransitionCount++;
                }
            }

            if (index == -1)
            {
                return;
            }

            var sourceCenter = m_SourceState.GetPosition().center;
            var destinationCenter = m_TargetState.GetPosition().center;

            var sourceToDestination = destinationCenter - sourceCenter;
            var normal = new Vector2(-sourceToDestination.y, sourceToDestination.x).normalized;

            var totalCount = inputTransitionCount + outputTransitionCount;

            var radius = m_SourceState.GetPosition().width * 0.5f;
            var length = k_MaxInterval * (totalCount - 1) * 0.5f;
            if (length > radius - 2)
            {
                length = radius - 2;
            }

            var fraction = totalCount == 1 ? 0 : (float)index / (totalCount - 1) * 2f - 1f; // [0,1] -> [-1, 1]
            var p1 = sourceCenter + normal * length * fraction;
            var p2 = destinationCenter + normal * length * fraction;

            transitionControl.from = ClosestIntersection(sourceCenter, radius, p1, p2);
            transitionControl.to = ClosestIntersection(destinationCenter, radius, p1, p2);
        }

        public void OnEdgeInspectorGUI()
        {
            
        }

        public void OnEdgeConfigUpdate()
        {
            
        }
        
        private static Vector2 ClosestIntersection(Vector2 center, float radius, Vector2 lineStart, Vector2 lineEnd)
        {
            var intersections = FindLineCircleIntersections(center.x, center.y, radius, lineStart, lineEnd,
                out var intersection1, out var intersection2);

            if (intersections == 1)
            {
                return intersection1; // one intersection
            }

            if (intersections == 2)
            {
                if (IsPointOnLineSegmentViaCrossProduct(lineStart, lineEnd, intersection1))
                {
                    return intersection1;
                }

                return intersection2;
            }

            return Vector2.zero;
        }
        
        // Find the points of intersection.
        private static int FindLineCircleIntersections(float cx, float cy, float radius,
            Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.x - point1.x;
            dy = point2.y - point1.y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
            C = (point1.x - cx) * (point1.x - cx) + (point1.y - cy) * (point1.y - cy) - radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Mathf.Sqrt(det)) / (2 * A));
                intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
                t = (float)((-B - Mathf.Sqrt(det)) / (2 * A));
                intersection2 = new Vector2(point1.x + t * dx, point1.y + t * dy);
                return 2;
            }
        }

        private static bool IsPointOnLineSegmentViaCrossProduct(Vector2 v1, Vector2 v2, Vector2 p)
        {
            if (!((v1.x <= p.x && p.x <= v2.x) || (v2.x <= p.x && p.x <= v1.x)))
            {
                // test point not in x-range
                return false;
            }

            if (!((v1.y <= p.y && p.y <= v2.y) || (v2.y <= p.y && p.y <= v1.y)))
            {
                // test point not in y-range
                return false;
            }

            return IsPointOnLineviaPDP(v1, v2, p);
        }
        
        private static bool IsPointOnLineviaPDP(Vector2 v1, Vector2 v2, Vector2 p)
        {
            var a = PerpDotProduct(v1, v2, p);
            return Mathf.Abs(PerpDotProduct(v1, v2, p)) < GetEpsilon(v1, v2);
        }
        
        private static float PerpDotProduct(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
        }
        
        private static double GetEpsilon(Vector2 v1, Vector2 v2)
        {
            var dx1 = (int)(v2.x - v1.x);
            var dy1 = (int)(v2.y - v1.y);
            var epsilon = 0.003 * (dx1 * dx1 + dy1 * dy1);
            return epsilon;
        }
        
        private static bool Approximately(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
        }
    }
}