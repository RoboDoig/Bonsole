using Bonsai.Editor;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BonsaiApi
{
    public static class WorkflowElementProvider
    {
        public static IEnumerable<WorkflowElementDescriptor> GetElements()
        {
            // Get assembly by reflection
            Assembly bonsaiAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Bonsai.Core");

            // Parse out workflow element descriptors
            var types = bonsaiAssembly.GetTypes();

            var elementDescriptors = new List<WorkflowElementDescriptor>();
            foreach (Type type in types)
            {
                bool isExpressionBuilder = type.IsSubclassOf(typeof(ExpressionBuilder));
                var visibleAttribute = type.GetCustomAttribute<DesignTimeVisibleAttribute>() ?? DesignTimeVisibleAttribute.Default;
                bool isVisibleElement = visibleAttribute.Visible;

                // Get appropriate operators
                if (type.IsPublic && !type.IsValueType && !type.ContainsGenericParameters &&
                    !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null && !type.IsDefined(typeof(ObsoleteAttribute)) &&
                    isExpressionBuilder && isVisibleElement)
                {
                    var descriptionAttribute = (DescriptionAttribute)TypeDescriptor.GetAttributes(type)[typeof(DescriptionAttribute)];
                    elementDescriptors.Add(
                        new WorkflowElementDescriptor
                        {
                            Name = ExpressionBuilder.GetElementDisplayName(type),
                            Namespace = type.Namespace,
                            FullyQualifiedName = type.AssemblyQualifiedName,
                            Description = descriptionAttribute.Description
                        }
                    );
                }
            }

            return elementDescriptors;
        }
    }
}
