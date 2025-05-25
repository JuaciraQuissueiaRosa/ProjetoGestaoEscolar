using System.IO;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;

namespace APISchool.Models
{
    public class RemoteXmlDocumentationProvider : IDocumentationProvider
    {
        private readonly XPathNavigator _documentNavigator;

        public RemoteXmlDocumentationProvider(string xmlUrl)
        {
            using (var client = new WebClient())
            {
                string xmlContent = client.DownloadString(xmlUrl);
                var xmlDoc = new XPathDocument(new StringReader(xmlContent));
                _documentNavigator = xmlDoc.CreateNavigator();
            }
        }

        public string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            return null; // você pode implementar isso se quiser
        }

        public string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            string controllerName = actionDescriptor.ControllerDescriptor.ControllerType.FullName;
            string actionName = actionDescriptor.ActionName;

            string expression = $"//member[starts-with(@name, 'M:{controllerName}.{actionName}')]";
            XPathNodeIterator iterator = _documentNavigator.Select(expression);

            if (iterator.MoveNext())
            {
                XPathNavigator nav = iterator.Current;
                return nav.SelectSingleNode("summary")?.Value.Trim();
            }

            return null;
        }

        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            string controllerName = controllerDescriptor.ControllerType.FullName;
            string expression = $"//member[starts-with(@name, 'T:{controllerName}')]";
            XPathNodeIterator iterator = _documentNavigator.Select(expression);

            if (iterator.MoveNext())
            {
                XPathNavigator nav = iterator.Current;
                return nav.SelectSingleNode("summary")?.Value.Trim();
            }

            return null;
        }

        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            string controllerName = actionDescriptor.ControllerDescriptor.ControllerType.FullName;
            string actionName = actionDescriptor.ActionName;

           
            string expression = $"//member[starts-with(@name, 'M:{controllerName}.{actionName}')]";
            XPathNodeIterator iterator = _documentNavigator.Select(expression);

            if (iterator.MoveNext())
            {
                XPathNavigator nav = iterator.Current;
                return nav.SelectSingleNode("returns")?.Value.Trim();
            }

            return null;
        }
    }

}