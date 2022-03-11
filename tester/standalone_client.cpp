/**
* Main test suite class for testing our server connection, handshake, and messages
* Credit to elhayra on Github, as all testing files besides spreadsheet_tester.sh and test_suite.cpp were written by them
* The rest of this code was written by Sam Peters for CS 3505
**/

#include <boost/test/unit_test.hpp>
#include <boost/json/src.hpp>
#include <string>
#include <iostream>
#include <signal.h>
#include <cstdlib>
#include <queue>
#include <thread>
#include <mutex>


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
  while(sizeof(currentMessage) > 2) {
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

    try {
      ourID = std::stoi(currentMessage);
    }
    catch(...) {}
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



void runStress(std::string ip, int port, int id, bool *result) {
  if (!setUpClient(ip, port)) {
    std::cout << "Failed to connect dummy" << std::endl;
    (*result) = false;
  }

  std::string currentMessage = "";

  //Sends our name to the server
  if(!send("Harriet\n")) (*result) = false;

  currentMessage = getAvailableSpreadsheets();

  //Send name of  spreadsheet to edit
  if(!send("testingtestingspreadsheet8\n")) (*result) = false;

  //Get our unique ID and/or the spreadsheet info for the current spreadsheet
  if(!getID(currentMessage)) (*result) = false;

  //Send request
  std::string msg = "{ \"requestType\": \"editCell\", \"cellName\": \"I"+std::to_string(id)+"\", \"contents\": \"23\" }\n";
  if(!send(msg)) (*result) = false;

  //Response to check for
  std::string response = "{\"messageType\":\"cellUpdated\",\"cellName\":\"I"std::to_string(id)+"\",\"contents\":\"23\"}";

  //Check for it
  if(checkForMessage(response)) (*result) = true;
  else (*result) = false;
}
