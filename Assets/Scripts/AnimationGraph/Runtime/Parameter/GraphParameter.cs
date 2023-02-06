using System;
using System.Collections.Generic;

namespace AnimationGraph
{
    [Serializable]
    public class GraphParameter
    {
        public int id;
        public string name;
        public List<int> associatedNodes;
    }
}
