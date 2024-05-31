using BonsaiApi;

var workflow = new WorkflowApi();
var elements = WorkflowElementProvider.GetElements();
var element = elements.Where(e => e.Name.Equals("Unit")).FirstOrDefault();

workflow.CreateGraphNode(element.FullyQualifiedName, Bonsai.ElementCategory.Source, WorkflowApi.CreateGraphNodeType.Successor, false, false, "");
