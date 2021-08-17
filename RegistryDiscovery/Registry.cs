#region Using Namespaces

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

#endregion

namespace com.intechiq.dls
{
    public class Registry
    {
        #region Internal Members

        private string _dnsServer;
        private string _registryType;
        private string _metaSchemeType;
        private string _registryNameSpace;

        private enum HashType : ushort
        {
            SHA256 = 0,
            MD5 = 1
        };

        private static readonly string SCHEME_TYPE_1 = "urn:oasis:names:tc:ebcore:partyid-type:iso6523";
        private static readonly string SCHEME_TYPE_2 = "iso6523-actorid-upis";

        // Example 1.1: urn:oasis:names:tc:ebcore:partyid-type:iso6523:0037:123456789
        private const string PATTERN_1_1 = @"\A(urn:oasis:names:tc:ebcore:partyid-type:iso6523):(\d{4}):(\d{8,13})\Z";

        // Example 1.2: urn:oasis:names:tc:ebcore:partyid-type:iso6523:0037::123456789
        private const string PATTERN_1_2 = @"\A(urn:oasis:names:tc:ebcore:partyid-type:iso6523):(\d{4})::(\d{8,13})\Z";

        // Example 2.1: iso6523-actorid-upis::0037:22501236
        private const string PATTERN_2_1 = @"\A(iso6523-actorid-upis):(\d{4}):(\d{8,13})\Z";

        // Example 2.2: iso6523-actorid-upis::0037:22501236
        private const string PATTERN_2_2 = @"\A(iso6523-actorid-upis)::(\d{4}):(\d{8,13})\Z";

        private const string REGISTRY_TYPE = "bdxl1Root";

        #endregion

        #region Constructors

        public Registry(string registryNameSpace)
        {
            _registryNameSpace = registryNameSpace;
            _dnsServer = getDefaultPrimaryDns();
            _registryType = REGISTRY_TYPE;
            _metaSchemeType = "";
        }

        public Registry(string registryNameSpace, string dnsServer)
        {
            _registryNameSpace = registryNameSpace;
            _dnsServer = string.IsNullOrWhiteSpace(dnsServer) ? getDefaultPrimaryDns() : dnsServer;
            _registryType = REGISTRY_TYPE;
            _metaSchemeType = "";
        }

        public Registry(string registryNameSpace, string dnsServer, string registryType, string metaSchemeType)
        {
            _registryNameSpace = registryNameSpace;
            _dnsServer = string.IsNullOrWhiteSpace(dnsServer) ? getDefaultPrimaryDns() : dnsServer;
            _registryType = string.IsNullOrWhiteSpace(registryType) ? REGISTRY_TYPE : registryType;
            _metaSchemeType = string.IsNullOrWhiteSpace(metaSchemeType) ? "" : metaSchemeType;
        }

        #endregion

        #region Properties

        public string DnsServer
        {
            get { return _dnsServer; }
        }

        public string RegistryType
        {
            get { return _registryType; }
        }

        public string MetaSchemeType
        {
            get { return _metaSchemeType; }
        }

        public string RegistryNameSpace
        {
            get { return _registryNameSpace; }
        }

        #endregion

        #region Structures

        public struct Participant
        {
            #region Properties

            public string identifierIdValue { get; set; }

            public string identifierMetaScheme { get; set; }

            public string identifierSchemeValue { get; set; }

            public string fullIdentifier { get; set; }

            #endregion
        }

        public struct ParticipantRecord
        {
            #region Properties

            public string returnCode { get; set; }

            public string returnString { get; set; }

            public string providerServiceName { get; set; }

            public string providerServiceAddress { get; set; }

            #endregion
        }

        #endregion

        #region Public Methods

        public string computeHash(string participantIdentifier)
        {
            return computeHash(participantIdentifier, "", "");
        }

        public string computeHash(string participantIdentifier, string registryType)
        {
            return computeHash(participantIdentifier, registryType, "");
        }

        public string computeHash(string participantIdentifier, string registryType, string metaSchemeType)
        {
            string strDataByte;

            // If the registryType value is not passed, then set the value from the defaults.
            registryType = string.IsNullOrWhiteSpace(registryType) ? _registryType : registryType;

            Participant participant = parseParticipantIdentifier(participantIdentifier, metaSchemeType);

            // Check if participant is valid or not. 
            if (isParticipantValid(participant))
            {
                switch (registryType.ToUpperInvariant())
                {
                    case "BDXL1ROOT":
                        strDataByte = getHashValue(participant.fullIdentifier);
                        break;
                    case "BDXL1ROOTSUBDIRSCHEME":
                        strDataByte = $"{getHashValue(participant.fullIdentifier)}.{getHashValue($"{participant.identifierMetaScheme}:{participant.identifierSchemeValue}")}";
                        break;
                    case "BDXL1SUBDIRSCHEME":
                        strDataByte = $"{getHashValue(participant.identifierIdValue)}.{getHashValue($"{participant.identifierMetaScheme}:{participant.identifierSchemeValue}")}";
                        break;
                    case "PEPPOL1CNAME":
                        strDataByte = $"b-{getHashValue($"{participant.identifierMetaScheme}:{participant.identifierIdValue}", HashType.MD5)}";
                        break;
                    case "PEPPOL1NAPTR":
                        strDataByte = $"{getHashValue($"{participant.identifierMetaScheme}:{participant.identifierIdValue}")}";
                        break;
                    default:
                        strDataByte = string.Empty;
                        break;
                }
            }
            else
            {
                strDataByte = string.Empty;
            }

            return strDataByte;
        }

        public ParticipantRecord lookupParticipantRecord(string participantIdentifier, string dnsServer = null, string registryType = null, string metaSchemeType = null)
        {
            ParticipantRecord result = new ParticipantRecord();
            result.returnCode = "0";
            
            // Call the computeHash method to convert the Participant Identifier into its Hash equivalent that is stored in the Registry.
            string record = $"{computeHash(participantIdentifier, registryType, metaSchemeType)}";

            // Check if record is valid
            if (string.IsNullOrWhiteSpace(record))
            {
                // Return an error message
                result.returnCode = "1";
                result.returnString = $"Participant Identifier: {participantIdentifier} is invalid.";
            }
            else
            {
                // Add Registry Namespace (Record Domain)
                record = $"{record}{(RegistryNameSpace.StartsWith(".") ? RegistryNameSpace : $".{RegistryNameSpace}")}";

                // Perform a DNS Lookup to retrieve the DNS records for this Business Identifier.
                if (string.IsNullOrWhiteSpace(dnsServer))
                    dnsServer = DnsServer;

                (int returnCode, string returnString) = Client.DNSLookup(dnsServer, record, "NAPTR", 10, 1);

                if (returnCode == 0)
                {
                    result.returnCode = "" + returnCode;
                    result.returnString = returnString;

                    //populate other fields from returnString
                    var collection = Regex.Matches(returnString, "\\\"(.*?)\\\"");
                    if (collection.Count >= 2)
                    {
                        result.providerServiceName = collection[collection.Count - 2].ToString().Trim('"');
                        result.providerServiceAddress = collection[collection.Count - 1].ToString();
                        string[] addressCollection = result.providerServiceAddress.Split('!');
                        result.providerServiceAddress = addressCollection.Length > 2 ? addressCollection[addressCollection.Length - 2] : "";
                    }

                }
                else 
                {
                    result.returnCode = "" + returnCode;
                    result.returnString = returnCode == 0 ? returnString : $"{record} Not Found";
                }
            }

            return result;
        }

        public async Task<ParticipantRecord> lookupParticipantRecordAsync(string participantIdentifier, string dnsServer = null, string registryType = null, string metaSchemeType = null)
        {
            ParticipantRecord result = new ParticipantRecord();
            result.returnCode = "0";

            // Call the computeHash method to convert the Participant Identifier into its Hash equivalent that is stored in the Registry.
            string record = $"{computeHash(participantIdentifier, registryType, metaSchemeType)}";

            // Check if record is valid
            if (string.IsNullOrWhiteSpace(record))
            {
                // Return an error message
                result.returnCode = "1";
                result.returnString = $"Participant Identifier: {participantIdentifier} is invalid.";
            }
            else
            {
                // Add Registry Namespace (Record Domain)
                record = $"{record}{(RegistryNameSpace.StartsWith(".") ? RegistryNameSpace : $".{RegistryNameSpace}")}";

                // Perform a DNS Lookup to retrieve the DNS records for this Business Identifier.
                if (string.IsNullOrWhiteSpace(dnsServer))
                    dnsServer = DnsServer;

                (int returnCode, string returnString) = await Client.DNSLookupAsync(dnsServer, record, "NAPTR", 10, 1);

                if (returnCode == 0)
                {
                    result.returnCode = "" + returnCode;
                    result.returnString = returnString;

                    //populate other fields from returnString
                    var collection = Regex.Matches(returnString, "\\\"(.*?)\\\"");
                    if (collection.Count >= 2)
                    {
                        result.providerServiceName = collection[collection.Count - 2].ToString().Trim('"');
                        result.providerServiceAddress = collection[collection.Count - 1].ToString();
                        string[] addressCollection = result.providerServiceAddress.Split('!');
                        result.providerServiceAddress = addressCollection.Length > 2 ? addressCollection[addressCollection.Length - 2] : "";
                    }

                }
                else
                {
                    result.returnCode = "" + returnCode;
                    result.returnString = returnCode == 0 ? returnString : $"{record} Not Found";
                }
            }

            return result;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Compute MD5 or SHA256 hash to be consistent across DLS & SML interface
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private string getHashValue(string rawData, HashType hashType = HashType.SHA256)
        {
            string strDataByte = string.Empty;

            if (hashType == HashType.MD5)
            {
                //strDataByte = computeMD5Hash(rawData.ToLower());
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData.ToLower()));
                    strDataByte = Base32.Encode(hashBytes);
                }
            }
            else
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData.ToLower()));
                    strDataByte = Base32.Encode(hashBytes);
                }
            }

            return strDataByte;
        }

        /// <summary>
        /// Convert the raw data into MD5 hash
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private string computeMD5Hash(string rawData)
        {
            // Create a SHA256   
            using (MD5 md5Hash = MD5.Create())
            {
                // ComputeHash - returns byte array  
                byte[] hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Convert the raw data into SHA-256 hash
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private string computeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string getDefaultPrimaryDns()
        {
            string primaryDns           = string.Empty;
            NetworkInterface adapter    = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.NetworkInterfaceType.Equals(NetworkInterfaceType.Ethernet)).FirstOrDefault();
            
            if (adapter != null)
            {
                IPAddress primaryDnsIP = adapter.GetIPProperties().DnsAddresses.FirstOrDefault();

                if (primaryDnsIP != null)
                {
                    try
                    {
                        IPHostEntry entry = Dns.GetHostEntry(primaryDnsIP.ToString());
                        primaryDns = entry.HostName;
                    }
                    catch (SocketException)
                    {
                        //unknown host or
                        //not every IP has a name
                        //log exception (manage it)
                        primaryDns = string.Empty;
                    }
                }
            }

            return primaryDns;
        }

        /// <summary>
        /// If any of the Participant object's properties is null/empty then the participant is invalid.
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        private bool isParticipantValid(Participant participant)
        {
            return !(string.IsNullOrWhiteSpace(participant.fullIdentifier) || string.IsNullOrWhiteSpace(participant.identifierIdValue)
                || string.IsNullOrWhiteSpace(participant.identifierMetaScheme) || string.IsNullOrWhiteSpace(participant.identifierSchemeValue));
        }

        private Participant parseParticipantIdentifier(string participantIdentifier, string metaSchemeType = null)
        {
            MatchCollection matches                     = null;
            bool isValid                                = false;
            string participantIdentifierMetaSchemeType  = string.Empty;
            Participant participant                     = new Participant();
            string participantIdentifierInput           = participantIdentifier.Trim().ToLower();

            RegexOptions opts = RegexOptions.Singleline; // | RegexOptions.ExplicitCapture;

            // Group 0 is the entire matched pattern,
            // Group 1 is the scheme type,
            // Group 2 is the scheme id,
            // Group 3 is the participant id

            for (int idx = 0; idx < 4; idx++)
            {
                switch (idx)
                {
                    case 0:
                        // Check Pattern 1.1
                        matches = Regex.Matches(participantIdentifierInput, PATTERN_1_1, opts);
                        if (matches.Count > 0 && matches[0].Groups.Count == 4)
                        {
                            isValid = true;
                            participantIdentifierMetaSchemeType = SCHEME_TYPE_1;
                        }
                        break;
                    case 1:
                        // Check Pattern 1.2
                        matches = Regex.Matches(participantIdentifierInput, PATTERN_1_2, opts);
                        if (matches.Count > 0 && matches[0].Groups.Count == 4)
                        {
                            isValid = true;
                            participantIdentifierMetaSchemeType = SCHEME_TYPE_1;
                        }
                        break;
                    case 2:
                        // Check Pattern 2.1
                        matches = Regex.Matches(participantIdentifierInput, PATTERN_2_1, opts);
                        if (matches.Count > 0 && matches[0].Groups.Count == 4)
                        {
                            isValid = true;
                            participantIdentifierMetaSchemeType = SCHEME_TYPE_2;
                        }
                        break;
                    case 3:
                        // Check Pattern 2.2
                        matches = Regex.Matches(participantIdentifierInput, PATTERN_2_2, opts);
                        if (matches.Count > 0 && matches[0].Groups.Count == 4)
                        {
                            isValid = true;
                            participantIdentifierMetaSchemeType = SCHEME_TYPE_2;
                        }
                        break;
                    default:
                        break;
                }

                if (isValid)
                    break;
            }

            if (isValid)
            {
                participant.identifierIdValue        = matches[0].Groups[3].Value;
                participant.identifierSchemeValue    = matches[0].Groups[2].Value;

                if (string.IsNullOrWhiteSpace(metaSchemeType))
                {
                    participant.fullIdentifier          = participantIdentifier;
                    participant.identifierMetaScheme    = participantIdentifierMetaSchemeType;
                }
                else
                {
                    participant.identifierMetaScheme = metaSchemeType;

                    // If metaSchemeType like ‘urn*’ 
                    if (metaSchemeType.StartsWith("urn", StringComparison.InvariantCultureIgnoreCase))
                        participant.fullIdentifier = $"{participant.identifierMetaScheme}:{participant.identifierSchemeValue}::{participant.identifierIdValue}";
                    // If metaSchemeType like ‘*upis’
                    else if (metaSchemeType.EndsWith("upis", StringComparison.InvariantCultureIgnoreCase))
                        participant.fullIdentifier = $"{participant.identifierMetaScheme}::{participant.identifierSchemeValue}:{participant.identifierIdValue}";
                    else
                        participant.fullIdentifier = participantIdentifier;
                }

            }

            return participant;
        }

        #endregion
    }
}