
//This class is a controller for a specific spreadsheet and the clients that wish to accsess it.


//POTENTIAL ERRORS
//*We need to make sure nothing is updated when sending all the updates to the spreadsheet.
 #ifndef SPREADSHEETSESSION_H
  #define SPREADSHEETSESSION_H
#define BOOST_BIND_NO_PLACEHOLDERS
#include "connection.h"
#include "spreadsheet.h"
#include <iostream>
#include <fstream>
#include <boost/json/src.hpp>
#include <mutex>

namespace ttspreadsheet
{
    class spreadsheetsession : public boost::enable_shared_from_this<spreadsheetsession>
    {
    public:
        //This method accepts a client, and completes the rest of the handshake.


        
    //Defualt constructor, this object does not need to take anything.
    spreadsheetsession(std::string sheetName)
    {
		theName = sheetName;
		//Set the current id to 0
		currentID = 0;
	//theName = sheetName+ "Session";

	queue = new tsqueue();
	clientList = std::vector<connection::pointer>();
	//Construct the spreadsheet
	theSpreadsheet = new spreadsheet(sheetName);
    }
    
    spreadsheetsession(std::string sheetName, bool isFile)
    {
		theName = sheetName;
		//Set the current id to 0
		currentID = 0;
	//theName = sheetName+ "Session";

	queue = new tsqueue();
	clientList = std::vector<connection::pointer>();
	//Construct the spreadsheet
	theSpreadsheet = new spreadsheet(sheetName, true);
    }

	//Returns the name of this session
	std::string getSessionName()
	{
		return theName;
	}



    //Callback we give to a connection class
    void onMessage(std::string nameOfRequestor)
    {

		muxQueue.lock();
	while(!queue->empty())
	{
	   std::string jsonRequest = queue->pop_front();
	   onReadNewCommand(jsonRequest, nameOfRequestor);
	}
		muxQueue.unlock();
    }

	
    //Callback we give to a connection class
    void onError(int connectionID)
    {
		int idOfDisconnectedClient;
		muxQueue.lock();
		for(int i = 0; i < clientList.size(); i++)
		{
			if(clientList.at(i)->getID() == connectionID)
			{
				//Removing the client from our list
				clientList.erase(clientList.begin()+i);

			}
		}
			//Tell all the other clients that this clients disconnected
			boost::json::object disconnectNotify;
			disconnectNotify["messageType"]="disconnected";
			disconnectNotify["user"]=connectionID;
			sendUpdateToClients(disconnectNotify);
		muxQueue.unlock();


    }

    //This method accepts a client, and completes the rest of the handshake.
    void acceptClient(connection::pointer client)
    {
    	try{
		//I lock here because I want the client to be connected before any request are sent back to the client.
		//#noclientleftbehind
		muxQueue.lock();
        // Add the client to our clientList
		clientList.push_back(client);
        //We give the connection our threadsafe q
        client-> giveTsQueue(queue);
		//Give the connection the ID
		client->setID(currentID);
	//Took me 1 hour to find this code, it just gives the connection class a callback here is the refrence
	// https://stackoverflow.com/questions/3381829/how-do-i-implement-a-callback-in-c
	std::function<void(std::string s)> callback;
	callback=boost::bind(&spreadsheetsession::onMessage,this,std::placeholders::_1);
	client->giveCallBack(callback);
	//Give an error call back to a connection class, this callback removes the connection from the session.
	std::function<void(int)> errorCallBack;
	errorCallBack=boost::bind(&spreadsheetsession::onError,this, std::placeholders::_1);
	client->giveDisconnectCallBack(errorCallBack);
		updateNewClient(client);
		std::string message =  std::to_string(currentID) + "\n";
		client->sendMessage(message);//Sending the client their ID with the new line char.
		currentID++;
		muxQueue.unlock();
		// Hand shake is over, we tell the client to start listning and reading and writing.
        client->start();
    	}
    	catch(std::exception& e)
    	{	
    		//This catch occurs if something goes wrong with the hand shake, like a socket closing
    	//	clientList.remove(clientList.size() -1);//Removes the client we just added to this list
    		muxQueue.unlock();
    	}
    }


    // Sends the update to all the clients.
    void sendUpdateToClients(boost::json::object message)
    {
	saveFile();
	for (int x = 0; x < clientList.size(); x++)
        {
        	try{
	    		clientList[x]->sendMessage(boost::json::serialize(message) + "\n");
        	}
        	catch(std::exception& e)
        	{
        		//Sometimes a client may disconnect and their disconnect call back has yet to be invoked.
        	}
        }
    }

    // Sends an error message to the client that requested a change that caused an error
    void sendErrorMessage(std::string cellName, std::string errorMessage)
    {
	boost::json::object requestError;
	requestError["messageType"]="requestError";
	requestError["cellName"]=cellName;
	requestError["message"]=errorMessage;

	requestingClient->sendMessage(boost::json::serialize(requestError) + "\n");
    }

    void updateNewClient(connection::pointer newClient)
    {
	boost::json::object cellUpdate;
	for (std::string cellName : theSpreadsheet->getAllNonemptyCellNames())
	{
	    cellUpdate["messageType"]="cellUpdated";
	    cellUpdate["cellName"]=cellName;
	    cellUpdate["contents"]=theSpreadsheet->getCellContents(cellName);

	    newClient->sendMessage(boost::json::serialize(cellUpdate) + "\n");
	}
    }

    // This method is "given" to the connection class to invoke on a read
    void onReadNewCommand(std::string jsonRequest, std::string requestingClientName)
    {
    std::string requestType;
    std::string cellName;
    std::string contents;
    
	for(int i = 0; i < clientList.size(); i++)
	{
	    requestingClient = clientList[i];
	    if (requestingClient->getName().compare(requestingClientName) == 0)
		{
		i = clientList.size();//Break out of the loop
		}
	}

	try
	{
		boost::json::value parsedRequest = boost::json::parse(jsonRequest);
		boost::json::string request = parsedRequest.get_object().at("requestType").get_string();
		requestType = std::string(request.begin(), request.end());
		if (requestType != "undo")
		{
			boost::json::value parsedName = boost::json::parse(jsonRequest);
			boost::json::string name = parsedName.get_object().at("cellName").get_string();
			cellName = std::string(name.begin(), name.end());
		}
		
		if (requestType == "editCell")
		{
			boost::json::value parsedContents = boost::json::parse(jsonRequest);
			boost::json::string cont = parsedContents.get_object().at("contents").get_string();
			contents = std::string(cont.begin(), cont.end());
		}
	}
	catch (...)
	{
		return;
	}
	
	
	if (requestType == "selectCell")
	{
	    cellSelect(cellName);
	}
	else if (requestType == "editCell")
	{
		cellEdit(cellName, contents);
	}
	else if (requestType == "revertCell")
	{
	    cellRevert(cellName);
	}
	else if (requestType == "undo")
	{
	    undo();
	}
    }
    
    void cellSelect(std::string cellName)
    {
	boost::json::object cellSelect;
	
	cellSelect["messageType"]="cellSelected";
	cellSelect["cellName"]=cellName;
	//cellSelect["selector"]=find(clientList.begin(), clientList.end(), requestingClient) - clientList.begin();
	cellSelect["selector"]=std::to_string(requestingClient->getID());
	cellSelect["selectorName"]=requestingClient->getName();

	sendUpdateToClients(cellSelect);
    }

    void cellEdit(std::string cellName, std::string contents)
    {
	// attempt to set the cell, if it fails, create and send an error message
	if (theSpreadsheet->setCellContents(cellName, contents) != 0)
	{
	    sendErrorMessage(cellName, "Edit would cause a circular dependency");
	    return;
	}

	boost::json::object cellUpdate;
	cellUpdate["messageType"]="cellUpdated";
	cellUpdate["cellName"]=cellName;
	cellUpdate["contents"]=contents;

	sendUpdateToClients(cellUpdate);
    }

	void sendServerShutDownError(std::string errorMessage)
	{
	boost::json::object serverShutDownUpdate;
	serverShutDownUpdate["messageType"]="serverError";
	serverShutDownUpdate["message"]= errorMessage;
	
	sendUpdateToClients(serverShutDownUpdate);
	}

    void cellRevert(std::string cellName)
    {
		// attempt to revert the cell, if it fails, create and send an error message
		int revert = theSpreadsheet->revertCellContents(cellName, true);
		if (revert == 1)
		{
		    sendErrorMessage(cellName, "Revert would cause a circular dependency");
		    return;
		}
		else if (revert == 2)
		    return;
		
		boost::json::object cellUpdate;
		cellUpdate["messageType"]="cellUpdated";
		cellUpdate["cellName"]=cellName;
		cellUpdate["contents"]=theSpreadsheet->getCellContents(cellName);
	
		sendUpdateToClients(cellUpdate);
    }
    
    void undo()
    {
		std::string undoneCell = theSpreadsheet->undo();
		
		boost::json::object cellUpdate;
		cellUpdate["messageType"]="cellUpdated";
		cellUpdate["cellName"]=undoneCell;
		cellUpdate["contents"]=theSpreadsheet->getCellContents(undoneCell);
	
		sendUpdateToClients(cellUpdate);
    }

  int saveFile()
  {
    std::string temp = "spreadsheets/"+theName+".sp";
    std::ofstream file;
	file.open(temp,std::ios::out | std::ios::trunc);

    if (file.is_open())
    {
    	std::vector<std::pair<std::string, std::string>> edits = theSpreadsheet->getEdits();
    	file << edits.size();
		file << "\n";
    	
    	for (int i = 0; i < edits.size(); i++)
    	{
    		file << "\n";
			file << edits[i].first;
			file << "\n";
			file << edits[i].second;
			file << "\n";
    	}
    }
    else std::cout << "Unable to save " << temp;

  return 0;
  }


    private:

      //This is a list of our clients, usefull for sends
		std::vector<connection::pointer> clientList;
        //This is the vector that contains all the commands.
        std::vector<std::string> commandStack;
        //This is our thredsafe queue.
        tsqueue * queue;
        //This is the spreadsheet that this spreadsheet session contains.
		spreadsheet * theSpreadsheet;
		connection::pointer requestingClient;

		int currentID;
        //This the Name of the Spreadsheet.
        std::string theName;
        //Sends the update to all the clients.

          protected:
          //This is to handle potential multi thread errors
  			std::mutex muxQueue;
    };
}
#endif
