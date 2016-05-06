using UnityEngine;
using System.Collections;
using Mystery.Graphing;
using System.Collections.Generic;

namespace Mystery.Graphing
{
    public class DebugGraphRenderer : IGraphConsoleRenderer
    {
        public string GraphName = typeof(double).Name;

        protected override IGraphConsole LoadGraph()
        {
            IEnumerator<IGraphConsole> graphs = DebugGraph.GetGraphEnumerator();
            while (graphs.MoveNext())
            {
                if (graphs.Current.Name == GraphName)
                    return graphs.Current;
            }
            return null;
        }
    }
}