﻿using Bonsai.Expressions;
using Bonsai;
using System.ComponentModel;

namespace BonsaiApi
{
    class GraphNode
    {
        public GraphNode(ExpressionBuilder value, int layer, IEnumerable<GraphEdge> successors)
        {
            Index = -1;
            Value = value;
            Layer = layer;
            Successors = successors;

            if (value != null)
            {
                var expressionBuilder = ExpressionBuilder.Unwrap(value);
                var elementAttributes = TypeDescriptor.GetAttributes(expressionBuilder);
                var elementCategoryAttribute = (WorkflowElementCategoryAttribute)elementAttributes[typeof(WorkflowElementCategoryAttribute)];
                var obsolete = (ObsoleteAttribute)elementAttributes[typeof(ObsoleteAttribute)] != null;
                if (expressionBuilder is DisableBuilder) Flags |= NodeFlags.Disabled;
                if (expressionBuilder is AnnotationBuilder) Flags |= NodeFlags.Annotation;

                var workflowElement = ExpressionBuilder.GetWorkflowElement(expressionBuilder);
                if (workflowElement != expressionBuilder)
                {
                    var builderCategoryAttribute = elementCategoryAttribute;
                    elementAttributes = TypeDescriptor.GetAttributes(workflowElement);
                    elementCategoryAttribute = (WorkflowElementCategoryAttribute)elementAttributes[typeof(WorkflowElementCategoryAttribute)];
                    obsolete |= (ObsoleteAttribute)elementAttributes[typeof(ObsoleteAttribute)] != null;
                    if (elementCategoryAttribute == WorkflowElementCategoryAttribute.Default)
                    {
                        elementCategoryAttribute = builderCategoryAttribute;
                    }
                }

                if (obsolete) Flags |= NodeFlags.Obsolete;
                if (expressionBuilder.IsBuildDependency()) Flags |= NodeFlags.BuildDependency;
                Category = elementCategoryAttribute.Category;
                if (workflowElement is IWorkflowExpressionBuilder)
                {
                    if (Category == ElementCategory.Workflow)
                    {
                        Category = ElementCategory.Combinator;
                        Flags |= NodeFlags.NestedGroup;
                    }
                    else Flags |= NodeFlags.NestedScope;
                }
            }

            InitializeDummySuccessors();
        }

        void InitializeDummySuccessors()
        {
            foreach (var successor in Successors)
            {
                if (successor.Node.Value == null)
                {
                    if (IsBuildDependency) successor.Node.Flags |= NodeFlags.BuildDependency;
                    successor.Node.InitializeDummySuccessors();
                }
            }
        }

        private NodeFlags Flags { get; set; }

        public int Index { get; internal set; }

        public int Layer { get; internal set; }

        public int LayerIndex { get; internal set; }

        public int ArgumentCount { get; internal set; }

        public Range<int> ArgumentRange
        {
            get { return (Flags & NodeFlags.Disabled) != 0 || Value == null ? EmptyRange : Value.ArgumentRange; }
        }

        public ExpressionBuilder Value { get; private set; }

        public IEnumerable<GraphEdge> Successors { get; private set; }

        public object Tag { get; set; }

        public ElementCategory? NestedCategory
        {
            get
            {
                if ((Flags & NodeFlags.NestedScope) != 0) return ElementCategory.Nested;
                else if ((Flags & NodeFlags.NestedGroup) != 0) return ElementCategory.Workflow;
                else return null;
            }
        }

        public ElementCategory Category { get; private set; }

        public bool IsDisabled => (Flags & NodeFlags.Disabled) != 0;

        public bool IsBuildDependency => (Flags & NodeFlags.BuildDependency) != 0;

        public bool IsAnnotation => (Flags & NodeFlags.Annotation) != 0;

        public string Text
        {
            get { return Value != null ? ExpressionBuilder.GetElementDisplayName(Value) : string.Empty; }
        }

        public bool Highlight
        {
            get { return (Flags & NodeFlags.Highlight) != 0; }
            set
            {
                if (value) Flags |= NodeFlags.Highlight;
                else Flags &= ~NodeFlags.Highlight;
            }
        }


        /// <summary>
        /// Returns a string that represents the value of this <see cref="GraphNode"/> instance.
        /// </summary>
        /// <returns>
        /// The string representation of this <see cref="GraphNode"/> object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{{{0}}}", Text);
        }

        [Flags]
        enum NodeFlags
        {
            None = 0x0,
            Highlight = 0x1,
            Obsolete = 0x2,
            Disabled = 0x4,
            BuildDependency = 0x8,
            NestedScope = 0x10,
            NestedGroup = 0x20,
            Annotation = 0x40
        }

        //static class CategoryColors
        //{
        //    public static readonly Color Source = Color.FromArgb(91, 178, 126);
        //    public static readonly Color Transform = Color.FromArgb(68, 154, 223);
        //    public static readonly Color Sink = Color.FromArgb(155, 91, 179);
        //    public static readonly Color Combinator = Color.FromArgb(238, 192, 75);
        //    public static readonly Color Property = Color.Gray;
        //}
    }
}