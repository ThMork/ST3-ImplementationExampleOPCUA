using System;
using System.Collections.Generic;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace OPCUA_demo
{
    class Program
    {
        //Class-wide server object for easy accessibility
        OpcClient accessPoint; 
        //string serverURL = "opc.tcp://192.168.0.122:4840";  //Physical PLC
        string serverURL = "opc.tcp://127.0.0.1:4840";      //Simulated PLC

        //Main for testing
        static void Main(string[] args)
        {
            Program prg = new Program();
            //The using statement of .NET automatically cleans up and automatically disposes 
            //of objects that implements the IDisposable interface.
            using (prg.accessPoint = new OpcClient(prg.serverURL))
            {
                //Connect to server
                prg.ConnectToServer(); 
                //Keep true for testing purposes
                while (true)
                {
                    //prg.ReadFromServer();
                    //prg.ReadTreagerWay();
                    prg.WriteToServer();

                    System.Threading.Thread.Sleep(500); //Hang out for half a second (testing)
                }
            }
        }

        // MAPPING OF OPC UA NODES
        // Example: 'ns=6;s=::Program:Cube.Status.Parameter[3].Value'
        // The node destination address can be a little tricky, but can be easily found for each variable through UaExpert.
        // The 'ns=6' indicates that the variable is on NamespaceIndex 6, which all of the Beer Production variables will be. 
        // The 's=' following it, indicates that the value to be read is a string type ('i=' would indicate integer etc.),
        // after which the actual path of the variable is defined ('::Program:Cube.Status.Parameter[3]'). 
        // Note: some values will require specifically targeting the value ('...Status.Parameter[3].Value').
        // Read further: https://documentation.unified-automation.com/uasdkhp/1.0.0/html/_l2_ua_node_ids.html
        // REMEMBER TO READ THE MANUAL CAREFULLY!

        public void ReadFromServer()
        {
            //Read example node values
            var temperature = accessPoint.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[3].Value");
            var movement = accessPoint.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[4].Value");

            var ctrlcmd = accessPoint.ReadNode("ns=6;s=::Program:Cube.Command.CntrlCmd");
            var param2 = accessPoint.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[2].Value");

            var value = accessPoint.ReadNode("ns=6;s=::Program:Cube.Status.Parameter[0].Value");

            //Print values to console window
            Console.WriteLine("Temp: {0,-5} Movement: {1,-5} param2: {2,-5} CtrlCmd: {3, -5} Value: {4, -5}", temperature, movement.Value, param2.Value, ctrlcmd.Value, value.Value);
        }

        public void ReadValuesFromServer()
        {
            OpcReadNode[] commands = new OpcReadNode[] {
                new OpcReadNode("ns=6;s=::Program:Cube.Command.Parameter[0].Value"),
                new OpcReadNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value"),
                new OpcReadNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value")
            };

            IEnumerable<OpcValue> job = accessPoint.ReadNodes(commands);
            foreach(OpcValue o in job)
            {
                Console.WriteLine((float)o.Value);
            }
        }

        public void WriteToServer()
        {
            float f1 = 1, f2 = 3, f3 = 3;

            OpcWriteNode[] commands = new OpcWriteNode[] {  
                new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[0].Value", f1),
                new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value", f2),
                new OpcWriteNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value", f3)};
            OpcStatusCollection results = accessPoint.WriteNodes(commands);
        }

        public void ConnectToServer()
        {
            accessPoint.Connect();
        }
        public void DisconnectFromServer()
        {
            accessPoint.Disconnect();
            accessPoint.Dispose(); //Clean up in case it wasn't automatically handled
        }
    }
}
