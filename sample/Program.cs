using com.intechiq.dls;
using System;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Declare variables
            string registryNameSpace;
            string participantId;

            // Display title
            Console.WriteLine("Sample code for DLS lookup\r");
            Console.WriteLine("------------------------\n");

            // Get dns url
            Console.Write("Enter the Registry Name Space : ");
            registryNameSpace = Console.ReadLine();

            // Set default value on enter
            if (registryNameSpace.Length == 0)
            {
                registryNameSpace = "dev.bpc.bdxl.services";// appSettings.DnsServer;
                Console.WriteLine($"Default: {registryNameSpace}\n");
            }

            // Get participant identifier
            Console.Write("Enter the participant identifier : ");
            participantId = Console.ReadLine();

            // Set default value on enter
            if (participantId.Length == 0)
            {
                participantId = "urn:oasis:names:tc:ebcore:partyid-type:iso6523:0151::112233445566";//appSettings.DLS_ParticipantID;
                Console.WriteLine($"Default: {participantId}\n");
            }

            // Lookup
            Registry reg = new Registry(registryNameSpace);
            Registry.ParticipantRecord result = reg.lookupParticipantRecord(participantId, "8.8.8.8");
            Console.WriteLine("Your result:");
            Console.WriteLine(result.returnCode=="0"?"Success":"Error");
            Console.WriteLine(result.returnString);
            Console.WriteLine(result.providerServiceName);
            Console.WriteLine(result.providerServiceAddress);

            // Wait for the user to respond before closing.
            Console.Write("Press any key to close the app...");
            Console.ReadKey();
        }
    }
}
