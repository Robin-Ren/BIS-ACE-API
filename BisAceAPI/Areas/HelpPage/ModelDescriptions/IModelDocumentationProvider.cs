using System;
using System.Reflection;
#pragma warning disable CS1591

namespace BisAceAPI.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}