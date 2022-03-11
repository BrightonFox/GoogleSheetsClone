/**
* Main test suite class for testing our server connection, handshake, and messages
* Credit to elhayra on Github(https://github.com/elhayra/tcp_server_client), as all testing files besides test_suite.cpp, asioclient.h, makefile, and Dockerfile were written by them
* The rest of this code was written by Sam Peters and the rest of Team Team for CS 3505
**/

#include <boost/json/src.hpp>
#include <string>
#include <iostream>
#include <signal.h>
#include <cstdlib>
#include <queue>
#include <thread>
#include <mutex>
#include "include/tcp_client.h"
#include "asioclient.h"


TcpClient client;

std::vector<std::string> spreadsheets;
std::queue<std::string> receivedMessages;
int ourID = -1;

// on sig_exit, close client
void sig_exit(int s)
{
  pipe_ret_t finishRet = client.finish();
  if (!finishRet.success) {
    //Idc
  }
  exit(0);
}

// observer callback. will be called for every new message received by the server
void onIncomingMsg(const char * msg, size_t size) {
  std::string segment;
  std::stringstream unsplitMessages;
  unsplitMessages.str(msg);

  while(std::getline(unsplitMessages, segment, '\n'))
  {
    receivedMessages.push(segment);
    //std::cout << segment <<std::endl;
  }
}

// observer callback. will be called when server disconnects
void onDisconnection(const pipe_ret_t & ret) {
  std::cout << "Server disconnected. " << ret.msg << std::endl;
  pipe_ret_t finishRet = client.finish();
  if (!finishRet.success) {
    //Doesn't matter for now
  }
}

//Helper method to initialize and set up client
bool setUpClient(std::string ip_address, int port){
  // configure and register observer
  client_observer_t observer;
  observer.wantedIp = ip_address;
  observer.incoming_packet_func = onIncomingMsg;
  observer.disconnected_func = onDisconnection;
  client.subscribe(observer);

  // connect client to an open server
  pipe_ret_t connectRet = client.connectTo(ip_address, port);
  if (!connectRet.success) {
    std::cout << "failed to connect to server" <<std::endl;
    return false;
  }
  return true;
}

//Helper method to parse spreadsheets sent to us
std::string getAvailableSpreadsheets() {
  std::stringstream unsplitMessages;
  std::string segment;
  std::string previousSegment = "";
  std::string currentMessage = "";

  if(receivedMessages.size() > 0) {
    currentMessage = receivedMessages.front();
    receivedMessages.pop();
  }

  //Goes through names of spreadsheets server sent to us, and adds them into a vector of strings
  while(currentMessage.size() > 1) {
    unsplitMessages.str(currentMessage);

    while(std::getline(unsplitMessages, segment, '\n'))
    {
      if(previousSegment == segment) break;
      spreadsheets.push_back(segment);
      previousSegment = segment;
    }

    sleep(1);

    if(receivedMessages.size() > 0) {
      currentMessage = receivedMessages.front();
      receivedMessages.pop();
    }
    else break;
  }

  return currentMessage;
}

//Helper method to send a message
bool send(std::string ourMsg) {
  pipe_ret_t sendRet = client.sendMsg(ourMsg.c_str(), ourMsg.size());
  if(!sendRet.success) {
    sleep(1);
    return false;
  }
  sleep(1);
  return true;
}

//Helper method to parse int ID from messages
bool getID(std::string currentMessage) {
  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  while (ourID == -1) {
    try {
      ourID = std::stoi(currentMessage);
    }
    catch(...) {
      sleep(1);
      if(receivedMessages.size() > 0){
        currentMessage = receivedMessages.front();
        receivedMessages.pop();
      }
    }
  }
  sleep(1);
  return true;
}

//Helper method to see if correct message was sent
bool checkForMessage(std::string response) {
  std::string currentMessage;
  while(receivedMessages.size() > 0) {
    currentMessage = receivedMessages.front();
    receivedMessages.pop();
    currentMessage.erase(std::remove(currentMessage.begin(),currentMessage.end(),' '),currentMessage.end());
    if (currentMessage == response) return true;
  }
  return false;
}


//Our first test case to simply test the connection and initial handshake with the server
bool connectionDisconnectionTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "10" << std::endl;
  std::cout << "Connection/Disconnection Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if (!send("Anthony\n")) return false;

  //Get spreadsheets
  currentMessage = getAvailableSpreadsheets();

  //Send name of our spreadsheet
  if (!send("testingtestingspreadsheet1\n")) return false;

  //Get ID
  if (!getID(currentMessage)) return false;

  //Output passed if no problems arose throughout the test
  return true;
}

//Test cell selecion and server response
bool selectCellTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "11" << std::endl;
  std::cout << "Cell Selection Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Bill\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send our spreadsheet name
  if(!send("testingtestingspreadsheet2\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send it
  std::string msg = "{ \"requestType\": \"selectCell\", \"cellName\": \"M14\" }\n";
  if(!send(msg)) return false;

  //Response to check for
  std::string response = "{\"messageType\":\"cellSelected\",\"cellName\":\"M14\",\"selector\":\""+std::to_string(ourID)+"\",\"selectorName\":\"Bill\"}";

  //Return true or false depending on if message came through
  return checkForMessage(response);
}

//Test cell edit and server response
bool editCellTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "11" << std::endl;
  std::cout << "Cell Edit Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Charlotte\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of  spreadsheet to edit
  if(!send("testingtestingspreadsheet3\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send request
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"I7\", \"contents\": \"23\" }\n";
  if(!send(msg)) return false;

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"I7\",\"contents\":\"23\"}";

  //Check for it
  return checkForMessage(response);
}

//Test cell revert and server response
bool revertCellTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "18" << std::endl;
  std::cout << "Cell Revert Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Dani\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of first spreadsheet to edit
  if(!send("testingtestingspreadsheet4\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send it
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"52\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"no\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"A3\" }\n";
  if(!send(msg)) return false;

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"A3\",\"contents\":\"52\"}";
  //Return if response came in or not
  return checkForMessage(response);
}



//Test undo and server response
bool undoTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "23" << std::endl;
  std::cout << "Undo Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Eileen\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of first spreadsheet to edit
  if(!send("testingtestingspreadsheet5\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send it
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"52\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"no\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"C10\", \"contents\": \"1010101\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"undo\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"undo\" }\n";
  if(!send(msg)) return false;

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"A3\",\"contents\":\"52\"}";

  return checkForMessage(response);
}


//Test circular dependency and server's response
bool circularTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "23" << std::endl;
  std::cout << "Circular Dependency Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Fiona\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of first spreadsheet to edit
  if(!send("testingtestingspreadsheet6\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send it
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"52\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A4\", \"contents\": \"=A3\" }\n";
  if(!send(msg)) return false;

  //Send it
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"=A4\" }\n";
  if(!send(msg)) return false;

  //Response to check for
  std::string response = "{\"messageType\":\"requestError\",\"cellName\":\"A3\",";

  //Check for it
  while(receivedMessages.size() > 0) {
    currentMessage = receivedMessages.front();
    receivedMessages.pop();
    currentMessage.erase(std::remove(currentMessage.begin(),currentMessage.end(),' '),currentMessage.end());
    if (currentMessage.substr(0,response.length()) == response) return true;
  }
  return false;
}

//Test a whole bunch of edits
bool soManyEditsTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "120" << std::endl;
  std::cout << "Lot \'o Edits Test" << std::endl;

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("George\n")) return false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of first spreadsheet to edit
  if(!send("testingtestingspreadsheet7\n")) return false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) return false;

  //Send first cell change
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"5\" }\n";
  if(!send(msg)) return false;

  //Send cell changes for A2-A50, and check responses from server
  msg = "";
  for(int i = 2; i < 51; i++) {
    msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A"+std::to_string(i)+"\", \"contents\": \"=A"+std::to_string(i-1)+"\" }\n";
    if(!send(msg)) return false;
  }

  for(int i = 2; i < 51; i++) {
    std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"A"+std::to_string(i)+"\",\"contents\":\"=A"+std::to_string(i-1)+"\"}";
    if(!checkForMessage(response)) return false;
  }

  return true;
}

//Test with lots of clients - NOT WORKING
bool stressTest(std::string ip, int port) {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "120" << std::endl;
  std::cout << "Stress Test" << std::endl;

  std::string currentMessage = "";

  //asioclient::asioclient theClient;

  std::vector<asioclient*> clientlist;


  for(int i = 0; i < 5; i++) {
    clientlist.push_back(new asioclient(ip, port));
  }

  for(int i = 0; i < 5; i++) {
    clientlist.at(i)->send(std::to_string(i) + "\n");
    sleep(3);
    clientlist.at(i)->send("testingtestingspreadsheet9\n");
    sleep(3);
  }

  return true;
}


//Test a whole bunch of edits
bool breakServerTest() {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "120" << std::endl;
  std::cout << "Breaking the Server Test" << std::endl;

  std::string currentMessage = "";
  std::string msg = "";

  //Sends our name to the server
  if(!send("Henry\n")) return false;

  //Send name of first spreadsheet to edit
  if(!send("50\n")) return false;

  sleep(1);

  //Undo nothing
  msg = "{ \"requestType\": \"undo\" }\n";
  if(!send(msg)) return false;

  //Revert nothing
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"A3\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"G85\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"H1\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"P9\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"revertCell\", \"cellName\": \"V14\" }\n";
  if(!send(msg)) return false;

  //Bunch of selects
  msg = "{ \"requestType\": \"selectCell\", \"cellName\": \"M14\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"selectCell\", \"cellName\": \"U71\" }\n";
  if(!send(msg)) return false;
  msg = "{ \"requestType\": \"selectCell\", \"cellName\": \"I55\" }\n";
  if(!send(msg)) return false;

  //Send cell changes for A1-20
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"5\" }\n";
  if(!send(msg)) return false;
  for(int i = 2; i < 21; i++) {
    msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A"+std::to_string(i)+"\", \"contents\": \"=A"+std::to_string(i-1)+"\" }\n";
    if(!send(msg)) return false;
  }

  for(int i = 1; i < 21; i++) {
    msg = "{ \"requestType\": \"undo\" }\n";
    if(!send(msg)) return false;
  }

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"A1\",\"contents\":\"\"}";
  return checkForMessage(response);
}



//Connect, edit, reconnect, check for edit
bool saveLoadTest(std::string ip, int port) {
  //Outputs seconds test approximately takes as well as the name of the test
  std::cout << "30" << std::endl;
  std::cout << "Save/Load Test" << std::endl;

  std::string currentMessage = "";
  std::string msg = "";

  asioclient *ac = new asioclient(ip, port);

  ac->send("Ivy\n");
  sleep(1);
  ac->send("testingtestingspreadsheet8\n");
  sleep(1);
  msg = "{ \"requestType\": \"editCell\", \"cellName\": \"A3\", \"contents\": \"52\" }\n";
  ac->send(msg);

  sleep(1);

  //Sends our name to the server
  if(!send("James\n")) return false;

  getAvailableSpreadsheets();

  sleep(1);

  //Send name of first spreadsheet to edit
  send("testingtestingspreadsheet8\n");

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"A3\",\"contents\":\"52\"}";
  //Return if response came in or not
  return checkForMessage(response);
}


//Main method that parses ip and port from args, and then calls the correct test
int main (int argc, char *argv[]) {

  //If less than 2 args, output number of tests
  if(argc < 2) {
    std::cout << "9" << std::endl;
    return 0;
  }

  //Get ip and port from (ip):(port)
  std::stringstream ipAndPort(argv[2]);
  std::string part;
  std::vector<std::string> parts;
  while(std::getline(ipAndPort, part, ':'))
  {
    parts.push_back(part);
  }

  //Set up client if not the stress test
  if(std::stoi(argv[1]) != 10) {
    if (!setUpClient(parts[0], std::stoi(parts[1]))) {
      std::cout << "Fail" << std::endl;
      return 0;
    }
  }

  //Calls test corresponding to first argument, and then sets success to return value
  bool success;
  switch(std::stoi(argv[1])) {
    case 1:
    success = connectionDisconnectionTest();
    break;
    case 2:
    success = selectCellTest();
    break;
    case 3:
    success = editCellTest();
    break;
    case 4:
    success = revertCellTest();
    break;
    case 5:
    success = undoTest();
    break;
    case 6:
    success = circularTest();
    break;
    case 7:
    success = soManyEditsTest();
    break;
    case 8:
    success = breakServerTest();
    break;
    case 9:
    success = saveLoadTest(parts[0], std::stoi(parts[1]));
    break;
    //case 10:
    //success = stressTest(parts[0], std::stoi(parts[1]));
    break;
    default:
    std::cout << "No matching test" << std::endl;
    success = true;
    break;
  }

  //If not stress test, close client
  if(std::stoi(argv[1]) != 10) {pipe_ret_t finishRet = client.finish();}

  //Output pass or faul accordingly
  if(success) std::cout << "Pass" << std::endl;
  else std::cout << "Fail" << std::endl;
  return 0;
}
