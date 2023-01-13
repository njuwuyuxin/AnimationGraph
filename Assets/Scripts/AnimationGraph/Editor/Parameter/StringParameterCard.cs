using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StringParameterCard : ParameterCard
    {
        public StringParameterCard(ParameterBoard parameterBoard, string name) : base(parameterBoard, name)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "String";
        }
    }
}
