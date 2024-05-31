using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bonsai;
using Bonsai.Dag;
using Bonsai.Expressions;

namespace BonsaiApi
{
    public class WorkflowApi
    {
        ExpressionBuilderGraph Workflow = new ExpressionBuilderGraph();

        public WorkflowApi() { 
            
        }

        public void CreateGraphNode(string typeName, ElementCategory elementCategory, CreateGraphNodeType nodeType, bool branch, bool group, string arguments)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            ExpressionBuilder builder;
            //var selectedNodes = graphView.SelectedNodes.ToArray();
            //var selectedNode = selectedNodes.Length > 0 ? selectedNodes[0] : null;
            //if (group && selectedNode != null)
            //{
            //    if (branch)
            //    {
            //        if (selectedNodes.Length > 1)
            //        {
            //            GroupGraphNodes(
            //                selectedNodes,
            //                graph => new GroupWorkflowBuilder(graph),
            //                node =>
            //                {
            //                    var groupBuilder = node.Value;
            //                    var builder = CreateReplacementBuilder(
            //                        groupBuilder,
            //                        typeName,
            //                        elementCategory,
            //                        arguments,
            //                        allowGroupReplacement: false);
            //                    ReplaceNode(node, builder);
            //                    return CreateUpdateSelectionDelegate(builder);
            //                });
            //        }
            //        else ReplaceGraphNode(selectedNode, typeName, elementCategory, arguments);
            //        return;
            //    }
            //    else if (elementCategory == ~ElementCategory.Combinator)
            //    {
            //        typeName = MakeGenericType(typeName, selectedNode.Value, out elementCategory);
            //    }
            //    else if (elementCategory > ~ElementCategory.Source)
            //    {
            //        GroupGraphNodes(selectedNodes, typeName);
            //        selectedNode = graphView.SelectedNodes.First();
            //        ConfigureBuilder(selectedNode.Value, selectedNode, arguments);
            //        return;
            //    }
            //}

            builder = CreateBuilder(typeName, elementCategory, group);
            //ConfigureBuilder(builder, selectedNode, arguments);

            if (builder is WorkflowInputBuilder workflowInput)
            {
                workflowInput.Index = Workflow.Count(node => ExpressionBuilder.Unwrap(node.Value) is WorkflowInputBuilder);
            }

            var inspectBuilder = builder.AsInspectBuilder();
            var inspectNode = new Node<ExpressionBuilder, ExpressionBuilderArgument>(inspectBuilder);
            var inspectParameter = new ExpressionBuilderArgument();
        }

        public enum CreateGraphNodeType
        {
            Successor,
            Predecessor
        }

        ExpressionBuilder CreateBuilder(string typeName, ElementCategory elementCategory, bool group)
        {
            if (elementCategory == ~ElementCategory.Workflow)
            {
                return new IncludeWorkflowBuilder { Path = typeName };
            }
            else if (elementCategory == ~ElementCategory.Source)
            {
                if (group) return new MulticastSubject { Name = typeName };
                else return new SubscribeSubject { Name = typeName };
            }

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new ArgumentException("Type not found.", nameof(typeName));
            }

            ExpressionBuilder builder;
            if (!type.IsSubclassOf(typeof(ExpressionBuilder)))
            {
                var element = Activator.CreateInstance(type);
                builder = ExpressionBuilder.FromWorkflowElement(element, elementCategory);
            }
            else builder = (ExpressionBuilder)Activator.CreateInstance(type);
            return builder;
        }

        //private void ConfigureBuilder(ExpressionBuilder builder, GraphNode selectedNode, string arguments)
        //{
        //    if (string.IsNullOrEmpty(arguments)) return;
        //    // TODO: This special case for binary operator operands should be avoided in the future
        //    if (builder is BinaryOperatorBuilder binaryOperator && selectedNode != null)
        //    {
        //        if (GetGraphNodeTag(selectedNode).Value is InspectBuilder inputBuilder &&
        //            inputBuilder.ObservableType != null)
        //        {
        //            binaryOperator.Build(Expression.Parameter(typeof(IObservable<>).MakeGenericType(inputBuilder.ObservableType)));
        //        }
        //    }

        //    var workflowElement = ExpressionBuilder.GetWorkflowElement(builder);
        //    var defaultProperty = TypeDescriptor.GetDefaultProperty(workflowElement);
        //    if (defaultProperty != null &&
        //        !defaultProperty.IsReadOnly &&
        //        defaultProperty.Converter != null &&
        //        defaultProperty.Converter.CanConvertFrom(typeof(string)))
        //    {
        //        try
        //        {
        //            var context = new TypeDescriptorContext(workflowElement, defaultProperty, serviceProvider);
        //            var propertyValue = defaultProperty.Converter.ConvertFromString(context, arguments);
        //            defaultProperty.SetValue(workflowElement, propertyValue);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new SystemException(ex.Message, ex);
        //        }
        //    }
        //}
    }
}
