//////-------------------------------------------------------------------------------------------------------
////// Copyright © WTS Paradigm 2013-2014, 2016
//////
////// Name:            ServiceCreator
////// Created:         *bjk 12/26/13 WPL029750 move to better shared location
////// Purpose:         Shared service creator logic
////// Modifications:   
//////                  *JEB 03/07/14 WPL030401 Add new stress test service
//////                  *JEB 07/25/14 WPL031850 Add new Web Centerpoint API service
//////                  *BCL 12/10/14 WPL032979 Allow anonymous access to WCF reporting service
//////                  *nrv 08/12/16 WEB-169   Copied here from TQ2
//////-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace ASI.WCF
{
    /// <summary>
    /// Static class to create objects for the WCF services
    /// </summary>
    internal static class ServiceCreator
    {
        //*BCL+noTCPSecurity 12/10/14 WPL032979 Allow anonymous access to WCF reporting service
        //*bjk++ 12/18/13 WPL029750 moved from service static to here to share logic
        //*BCL 12/20/12 WPL025158 Move touchquote reports to server processing
        /// <summary>
        /// Creates a NetTCP binding for communications with a WCF service
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="b">The b.</param>
        /// <param name="UpdateServer">The update server.</param>
        /// <param name="noTCPSecurity">if set to <c>true</c> [the TCP security bindings will be set to none].</param>
        public static void GetServiceObjectsTCP(out System.ServiceModel.EndpointAddress e, out System.ServiceModel.NetTcpBinding b, string UpdateServer, bool noTCPSecurity)
        {
            b = new System.ServiceModel.NetTcpBinding();
            b.MaxConnections = 256;
            b.MaxReceivedMessageSize = 671088640;
            b.ReaderQuotas.MaxStringContentLength = 671088640;
            b.ReaderQuotas.MaxArrayLength = 671088640;
            b.ReceiveTimeout = TimeSpan.MaxValue;
            b.SendTimeout = TimeSpan.MaxValue;
            b.Namespace = "http://wtsparadigm.com/binding";

            //*BCL+7 12/10/14 WPL032979 Allow anonymous access to WCF reporting service
            //Set the security and transport modes to 'none'
            if (noTCPSecurity)
            {
                b.Security.Mode = SecurityMode.None;
                b.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                b.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
            }

            e = new System.ServiceModel.EndpointAddress(UpdateServer);
        }
    }

    /// <summary>
    /// WCF Helper class that exposes ClientBase object easier so the ability to
    /// manipulate Behaviors is more exposed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class WcfClient<T> : System.ServiceModel.ClientBase<T> where T : class
    {
        /// <summary>
        /// The Binding and Endpoint Address are required for constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="endpointAddress"></param>
        public WcfClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpointAddress) :
            base(binding, endpointAddress) { }

        /// <summary>
        /// Returns a new client channel in a factory-style pattern
        /// </summary>
        public new T Channel { get { return base.Channel; } }
    }

}
