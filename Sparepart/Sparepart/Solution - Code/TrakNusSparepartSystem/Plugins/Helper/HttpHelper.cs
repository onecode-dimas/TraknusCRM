using System;
using System.Text;
using System.Web;

namespace TrakNusSparepartSystem.Plugins.Helper
{
    public class HttpHelper
    {
        /// <summary>
        /// Gets the server information.
        /// </summary>
        /// <returns>Server info object.</returns>
        public static ServerInfo GetServerInfo()
        {
            var webContext = HttpContext.Current;

            if (webContext != null)
            {
                var request = webContext.Request;
                var absolutePath = request.Url.AbsolutePath.Split('/');

                return new ServerInfo
                {
                    MachineName = webContext.Server.MachineName,
                    IsSSL = request.IsSecureConnection,
                    HostName = request.Url.Host,
                    Port = request.Url.Port,
                    HostAddress = request.UserHostAddress,
                    UserAgent = request.UserAgent,
                    OrganizationName = absolutePath.Length > 1 ? absolutePath[1] : "Unknown",
                    FormAddress = request.Url.AbsoluteUri
                };
            }

            return new ServerInfo();
        }
    }

    public class ServerInfo
    {
        public String MachineName { get; set; }
        public String HostName { get; set; }
        public int Port { get; set; }
        public bool IsSSL { get; set; }
        public String HostAddress { get; set; }
        public String UserAgent { get; set; }
        public String OrganizationName { get; set; }
        public String FormAddress { get; set; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine(String.Format("{0} : {1}", "Machine Name : ", MachineName));
            str.AppendLine(String.Format("{0} : {1}", "Host Name : ", HostName));
            str.AppendLine(String.Format("{0} : {1}", "Host Address : ", HostAddress));
            str.AppendLine(String.Format("{0} : {1}", "Port : ", Port));
            str.AppendLine(String.Format("{0} : {1}", "IsSSL : ", IsSSL));
            str.AppendLine(String.Format("{0} : {1}", "User Agent : ", UserAgent));
            str.AppendLine(String.Format("{0} : {1}", "Organization Name : ", OrganizationName));
            str.AppendLine(String.Format("{0} : {1}", "Form Address : ", FormAddress));
            return str.ToString();
        }
    }

}
