using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonsaiApi
{
    class GraphEdge
    {
        public GraphEdge(ExpressionBuilderArgument label, GraphNode successor)
        {
            Label = label;
            Node = successor;

            Text = string.Empty;
            if (label != null)
            {
                Text = label.Name;
            }
        }

        internal GraphEdge(GraphEdge edge, GraphNode successor)
        {
            Label = edge.Label;
            Node = successor;
            Text = edge.Text;
        }

        public ExpressionBuilderArgument Label { get; private set; }

        public GraphNode Node { get; set; }

        public object? Tag { get; set; } = null;

        public string Text { get; private set; }
    }
}
