// Credits: Past me (Clay Ankeny), from 3500. -- source of basis for networking code
//          Professor Kopta, basis for my modified network code 
//          Modified by Clay Ankeny & Glorien Roque


using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace ClientNetworking
{

    public class ClientNetworking
    {

        // For sending updates to the client
        public delegate void UpdateHandler(CellUpdate update);
        public event UpdateHandler OnUpdate;

        // For sending selection updates to the client
        public delegate void SelectionHandler(CellSelection selection);
        public event SelectionHandler OnSelection;

        // For sending client disconnect messages to other clients
        public delegate void ClientDisconnectHandler(ClientDisconnect disconnect);
        public event ClientDisconnectHandler OnClientDisconnect;

        // For sending clients a message that server will be shutdown
        public delegate void ServerShutdownHandler(ServerShutdown shutdown);
        public event ServerShutdownHandler OnServerShutdown;

        // For sending clients a message that a request error has been made
        public delegate void RequestErrorHandler(RequestError requestError);
        public event RequestErrorHandler OnRequestError;

        // For sending the list of sheets to the client
        public delegate void SheetHandler(List<string> sheetNames);
        public event SheetHandler OnGetSheets;

        // For sending the client's ID to the client
        public delegate void IDHandler(int ID);
        public event IDHandler OnGetID;

        public delegate void TestHandler(string testStuff);
        public event TestHandler OnTest;

        private SocketState theServer;
        // The username to send the server
        private string userName;
        private List<string> sheetNames;
        public bool IsConnected { get => theServer.TheSocket.Connected; private set => IsConnected = value; }


        public void ConnectToServer(string serverAddress, string name)
        {
            sheetNames = new List<string>();
            userName = name;
            Networking.ConnectToServer(FirstContact, serverAddress, 1100);
        }

        /// <summary>
        /// First callback for handshake with server
        /// </summary>
        /// <param name="state"></param>
        private void FirstContact(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            theServer = state;
            state.OnNetworkAction = ReceiveSpreadsheets;
            Networking.Send(theServer.TheSocket, userName + "\n");
            Networking.GetData(state);
        }

        /// <summary>
        /// The part that receives sheets
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveSpreadsheets(SocketState state)
        {

            if (state.ErrorOccured)
            {
                return;
            }

            string spreadSheetNames = state.GetData();
            string[] rawData = Regex.Split(spreadSheetNames, Environment.NewLine);

            foreach (string p in rawData)
            {
                if (p.Length == 0)
                    continue;
                if (p[p.Length - 1] != '\n')
                    break;

                sheetNames.Add(p);
                state.RemoveData(0, p.Length);

                if (p.StartsWith("\n"))
                    OnGetSheets(sheetNames);
            }

            OnGetSheets(sheetNames);
        }

        // Sends the spreadsheet request
        public void RequestSpreadsheet(string sheetName, SocketState state)
        {
            if (theServer != null && theServer.TheSocket != null && theServer.TheSocket.Connected)
            {
                Networking.Send(theServer.TheSocket, sheetName + "\n");
                theServer.OnNetworkAction = getSpreadsheetAndID;
                Networking.GetData(theServer);
            }
        }

        public void TestCallback(SocketState state)
        {
            OnGetID(-999);
            Networking.GetData(state);
        }

        public void disconnectClient()
        {
            if (theServer != null && theServer.TheSocket != null && theServer.TheSocket.Connected)
                theServer.TheSocket.Close();
        }

        /// <summary>
        /// Sends an undo request to the server
        /// </summary>
        public void SendUndoRequest()
        {
            // Send an undo request manually - simple enough that no object is needed
            if (theServer != null && theServer.TheSocket != null && theServer.TheSocket.Connected)
                Networking.Send(theServer.TheSocket, "{\"requestType\": \"undo\"}\n");
        }

        /// <summary>
        /// Sends a revert request to the server
        /// </summary>
        public void SendRevertRequest(string cellName)
        {
            // Send a revert request manually - simple enough that no object is needed
            if (theServer != null && theServer.TheSocket != null && theServer.TheSocket.Connected)
                Networking.Send(theServer.TheSocket, "{\"requestType\": \"revertCell\", \"cellName\": \"" + cellName + "\"}\n");
        }

        /// <summary>
        /// Sends a selection request to the server
        /// </summary>
        /// <param name="cell"></param>
        public void SendSelectionRequest(string cellName)
        {
            // Send a selection request manually - simple enough that no object is needed
            if (theServer != null && theServer.TheSocket.Connected)
                Networking.Send(theServer.TheSocket, "{ \"requestType\": \"selectCell\", \"cellName\": \"" + cellName + "\"}\n");
        }

        /// <summary>
        /// Sends an edit request to the server
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="theEdit"></param>
        public void SendEditRequest(string cellName, string theEdit)
        {
            // Send a edit request manually - to keep in line with other function, no object is used
            if (theServer != null && theServer.TheSocket != null && theServer.TheSocket.Connected)
                Networking.Send(theServer.TheSocket, "{ \"requestType\": \"editCell\", \"cellName\": \"" + cellName + "\", \"contents\": \"" + theEdit + "\" }\n");
                                               //"{ \"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"5\" }\n";
        }

        /// <summary>
        /// Splits a string into parts
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string[] SplitString(string data, bool keepNewlines)
        {
            string totalData = data;
            string[] parts;

            if (keepNewlines)
                parts = Regex.Split(totalData, @"\n*(?={)");
            else
                parts = Regex.Split(totalData, @"(?<=[\n])");

            /*if (keepNewlines)
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] += '\n';
                }*/

            return parts;
        }


        /// <summary>
        /// Get the json messages from the socket state buffer and returns them as a list of json messages
        /// </summary>
        /// <param name="state">The user's socket state</param>
        /// <returns></returns>
        private List<string> GetJsonMessages(SocketState state)
        {
            string s = state.GetData();
            string[] jsons = Regex.Split(s, @"(?<=[\n])");
            List<string> jsonMessages = new List<string>();

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string jsonString in jsons)
            {
                // Ignore empty strings added by the regex splitter
                if (jsonString.Length == 0)
                    continue;
                //Check to ee if the last character in the json string is a newline if it is it is a complete json 
                // message and thus we can end.
                if (jsonString[jsonString.Length - 1] != '\n')
                    break;
                // Remove this jsonString from the SocketState's growable buffer
                state.RemoveData(0, jsonString.Length);
                // Add this string to the array
                jsonMessages.Add(jsonString.Substring(0, jsonString.Length - 1));
            }
            return jsonMessages;
        }

        /// <summary>
        /// Parses spreadsheet json information and gets ID
        /// </summary>
        /// <param name="state"></param>
        private void getSpreadsheetAndID(SocketState state)
        {

            string[] parts = SplitString(state.GetData(), false);

            List<string> newMessages = new List<string>();

            // parse the messages into tokens
            foreach (string p in parts)
            {
                if (p.Length == 0)
                    continue;
                if (p[p.Length - 1] != '\n')
                    break;
                string g = p.Replace("\n", "").Replace("\r", "");
                Console.WriteLine("We are getting " + g + " from the server");
                newMessages.Add(g);
                state.RemoveData(0, g.Length);
            }

            foreach (string s in newMessages)
            {
                if (s.Contains("cellSelected"))
                {
                    CellSelection newSelection = JsonConvert.DeserializeObject<CellSelection>(s);
                    OnSelection(newSelection);
                }
                else if (s.Contains("cellUpdated"))
                {
                    CellUpdate update = JsonConvert.DeserializeObject<CellUpdate>(s);
                    OnUpdate(update);
                }
                else if (s.Contains("requestError"))
                {
                    RequestError requestError = JsonConvert.DeserializeObject<RequestError>(s);
                    OnRequestError(requestError);
                }
                else if (s.Contains("serverError"))
                {
                    ServerShutdown serverShutdown = JsonConvert.DeserializeObject<ServerShutdown>(s);
                    OnServerShutdown(serverShutdown);
                }
                else if (s.Contains("disconnected"))
                {
                    ClientDisconnect clientDisconnect = JsonConvert.DeserializeObject<ClientDisconnect>(s);
                    OnClientDisconnect(clientDisconnect);
                }
                else // Assume message is initial ID
                {
                    if (int.TryParse(s, out int newID))
                    {
                        OnGetID(newID);

                        Networking.GetData(state);
                    }
                }
            }
            Networking.GetData(state);
        }
    }
}