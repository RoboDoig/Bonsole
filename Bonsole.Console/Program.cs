using BonsaiApi;

// See https://aka.ms/new-console-template for more information
var workflow = new WorkflowApi();
var elements = WorkflowElementProvider.GetElements();
var element = elements.Where(e => e.Name.Equals("Unit")).FirstOrDefault();

workflow.InsertNode(element.FullyQualifiedName, Bonsai.ElementCategory.Source);

Console.WriteLine(element.Name);
