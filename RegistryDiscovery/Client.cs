/*
 * Derived from https://www.codeproject.com/articles/23673/dns-net-resolver-c
*/

#region Using Namespaces

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

#endregion

namespace com.intechiq.dls
{
    enum DLSReturnCode
    {
        ServiceCheckOK = 0,
        ServiceCheckErr = 1,
        ServiceCheckInputErr = 601,
        DNS_InitErr = 602,
        DNS_ResponseErr = 603,
        DNS_ResponseNoAnswer = 604,
        DNS_ThrowErr = 605
    }

    public class Client
    {
        #region Public Methods

        public static (int returnCode, string returnString) DNSLookup(string strServer, string strQuery, string strRecType, int intTimeOut, int intRetries)
        {
            string returnString     = string.Empty;
            int returnCode          = (int)DLSReturnCode.DNS_InitErr;

            try
            {
                Resolver resolver       = new Resolver();
                IPAddress[] addresses   = Dns.GetHostAddresses(strServer);

                resolver.Recursion      = true;
                resolver.UseCache       = false;
                resolver.TimeOut        = intTimeOut;
                resolver.Retries        = intRetries;
                resolver.DnsServer      = addresses[0].ToString();
                resolver.TransportType  = TransportType.Tcp;
                resolver.OnVerbose      += new Resolver.VerboseEventHandler(resolver_OnVerbose);

                Thread.CurrentThread.CurrentCulture     = new CultureInfo("en-US", false);
                Thread.CurrentThread.CurrentUICulture   = new CultureInfo("en-US", false);

                returnString = $"; <<>> Dig.Net {resolver.Version} <<>> @{resolver.DnsServer} {(QType)Enum.Parse(typeof(QType), strRecType)} {strQuery}\n";
                returnString += ";; global options: printcmd\n";

                Stopwatch sw = new Stopwatch();
                sw.Start();
                Response response = resolver.Query(strQuery, (QType)Enum.Parse(typeof(QType), strRecType), QClass.IN);

                if (response.Error != "")
                {
                    returnCode      = (int)DLSReturnCode.DNS_ResponseErr;
                    returnString    += response.Error;
                    
                    return (returnCode, returnString);
                }

                returnString += ";; Got answer:\n";
                returnString += $";; ->>HEADER<<- opcode: {response.header.OPCODE}, status: {response.header.RCODE}, id: {response.header.ID}\n";
                returnString += $";; flags: {(response.header.QR ? " qr" : string.Empty)}{(response.header.AA ? " aa" : string.Empty)}{(response.header.RD ? " rd" : string.Empty)}{(response.header.RA ? " ra" : string.Empty)}; QUERY: {response.header.QDCOUNT}, ANSWER: {response.header.ANCOUNT}, AUTHORITY: {response.header.NSCOUNT}, ADDITIONAL: {response.header.ARCOUNT}\n\n";

                if (response.header.QDCOUNT > 0)
                {
                    returnString += string.Format(";; QUESTION SECTION:\n");
                    foreach (Question question in response.Questions)
                        returnString += $";{question}";
                    returnString += string.Format("\n\n");
                }

                if (response.header.ANCOUNT > 0)
                {
                    returnCode = (int)DLSReturnCode.ServiceCheckOK;
                    returnString = string.Empty;
                    foreach (AnswerRR answerRR in response.Answers)
                        returnString += answerRR.ToString();

                    return (returnCode, returnString);
                }

                if (response.header.NSCOUNT > 0)
                {
                    returnString += ";; AUTHORITY SECTION:\n";
                    foreach (AuthorityRR authorityRR in response.Authorities)
                        returnString += authorityRR.ToString();
                    returnString += string.Format("\n\n");
                }

                if (response.header.ARCOUNT > 0)
                {
                    returnString += string.Format(";; ADDITIONAL SECTION:\n");
                    foreach (AdditionalRR additionalRR in response.Additionals)
                        returnString = additionalRR.ToString();
                    returnString += string.Format("\n\n");
                }

                returnString += $";; Query time: {sw.ElapsedMilliseconds} msec\n";
                returnString += $";; SERVER: {response.Server.Address}#{response.Server.Port}({response.Server.Address})\n";
                returnString += $";; WHEN: {response.TimeStamp.ToString("ddd MMM dd HH:mm:ss yyyy\n", new CultureInfo("en-US"))}";
                returnString += $";; MSG SIZE rcvd: {response.MessageSize}";
                returnCode = (int)DLSReturnCode.DNS_ResponseNoAnswer;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                if (ex.InnerException != null)
                {
                    sb.AppendLine(ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                        sb.AppendLine(ex.InnerException.InnerException.Message);
                }

                returnCode = (int)DLSReturnCode.DNS_ThrowErr;
                returnString = $"Error:{sb}";
            }

            return (returnCode, returnString);
        }

        public static async Task<(int returnCode, string returnString)> DNSLookupAsync(string strServer, string strQuery, string strRecType, int intTimeOut, int intRetries)
        {
            string returnString     = string.Empty;
            int returnCode          = (int)DLSReturnCode.DNS_InitErr;

            try
            {
                Resolver resolver       = new Resolver();
                IPAddress[] addresses   = await Dns.GetHostAddressesAsync(strServer);

                resolver.Recursion      = true;
                resolver.UseCache       = false;
                resolver.TimeOut        = intTimeOut;
                resolver.Retries        = intRetries;
                resolver.DnsServer      = addresses[0].ToString();
                resolver.TransportType  = TransportType.Tcp;
                resolver.OnVerbose      += new Resolver.VerboseEventHandler(resolver_OnVerbose);

                Thread.CurrentThread.CurrentCulture     = new CultureInfo("en-US", false);
                Thread.CurrentThread.CurrentUICulture   = new CultureInfo("en-US", false);

                returnString = $"; <<>> Dig.Net {resolver.Version} <<>> @{resolver.DnsServer} {(QType)Enum.Parse(typeof(QType), strRecType)} {strQuery}\n";
                returnString += ";; global options: printcmd\n";

                Stopwatch sw = new Stopwatch();
                sw.Start();
                Response response = resolver.Query(strQuery, (QType)Enum.Parse(typeof(QType), strRecType), QClass.IN);

                if (response.Error != "")
                {
                    returnCode      = (int)DLSReturnCode.DNS_ResponseErr;
                    returnString    += response.Error;
                    
                    return (returnCode, returnString);
                }

                returnString += ";; Got answer:\n";
                returnString += $";; ->>HEADER<<- opcode: {response.header.OPCODE}, status: {response.header.RCODE}, id: {response.header.ID}\n";
                returnString += $";; flags: {(response.header.QR ? " qr" : string.Empty)}{(response.header.AA ? " aa" : string.Empty)}{(response.header.RD ? " rd" : string.Empty)}{(response.header.RA ? " ra" : string.Empty)}; QUERY: {response.header.QDCOUNT}, ANSWER: {response.header.ANCOUNT}, AUTHORITY: {response.header.NSCOUNT}, ADDITIONAL: {response.header.ARCOUNT}\n\n";

                if (response.header.QDCOUNT > 0)
                {
                    returnString += string.Format(";; QUESTION SECTION:\n");
                    foreach (Question question in response.Questions)
                        returnString += $";{question}";
                    returnString += string.Format("\n\n");
                }

                if (response.header.ANCOUNT > 0)
                {
                    returnCode = (int)DLSReturnCode.ServiceCheckOK;
                    returnString = string.Empty;
                    foreach (AnswerRR answerRR in response.Answers)
                        returnString += answerRR.ToString();
                    
                    return (returnCode, returnString);
                }

                if (response.header.NSCOUNT > 0)
                {
                    returnString += ";; AUTHORITY SECTION:\n";
                    foreach (AuthorityRR authorityRR in response.Authorities)
                        returnString += authorityRR.ToString();
                    returnString += string.Format("\n\n");
                }

                if (response.header.ARCOUNT > 0)
                {
                    returnString += string.Format(";; ADDITIONAL SECTION:\n");
                    foreach (AdditionalRR additionalRR in response.Additionals)
                        returnString = additionalRR.ToString();
                    returnString += string.Format("\n\n");
                }

                returnString += $";; Query time: {sw.ElapsedMilliseconds} msec\n";
                returnString += $";; SERVER: {response.Server.Address}#{response.Server.Port}({response.Server.Address})\n";
                returnString += $";; WHEN: {response.TimeStamp.ToString("ddd MMM dd HH:mm:ss yyyy\n", new CultureInfo("en-US"))}";
                returnString += $";; MSG SIZE rcvd: {response.MessageSize}";
                returnCode = (int)DLSReturnCode.DNS_ResponseNoAnswer;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                if (ex.InnerException != null)
                {
                    sb.AppendLine(ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                        sb.AppendLine(ex.InnerException.InnerException.Message);
                }

                returnCode = (int)DLSReturnCode.DNS_ThrowErr;
                returnString = $"Error:{sb}";
            }

            return (returnCode, returnString);
        }

        #endregion

        #region Internal Methods

        private static void resolver_OnVerbose(object sender, Resolver.VerboseEventArgs e)
        {
            //Console.WriteLine(e.Message);
        }

        #endregion
    }
}