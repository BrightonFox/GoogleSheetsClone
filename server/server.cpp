/* This server class is used to create the networking required
 * to connect to multiple clietns and allow them to access and
 * edit the spreadsheet objects stored within.
 * This class and any other networking class is based on the
 * example classes given by boost libraries
 * https://www.boost.org/doc/libs/1_54_0/doc/html/boost_asio/tutorial/tutdaytime3/src.html
 */
#define BOOST_BIND_NO_PLACEHOLDERS
 #ifndef SERVER_CPP
  #define SERVER_CPP
#include <ctime>
#include <iostream>
#include <string>
#include <boost/bind.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <boost/asio.hpp>
#include "tsqueue.h"
#include "spreadsheetsession.h"
#include <boost/thread.hpp>
#include <boost/chrono.hpp>
#include <vector>
#include <fstream>

namespace ttspreadsheet
{
 // this is class is the server class
 // it gives clients to spreadsheet sessions
class server 
{
  //This struct is custom exception we throw to shut down the server.
  struct serverShutDown : std::exception {
  const char* what() const noexcept {return "Server intentionally shutdown";}
      };
public:
  server(boost::asio::io_context& io_context,std::string theKillWord)
    : io_context_(io_context),
      acceptor_(io_context,  boost::asio::ip::tcp::endpoint( boost::asio::ip::tcp::v4(), 1100))
  {
    this->killWord = theKillWord;

      //Load file 
    
   sessions = new std::vector<spreadsheetsession *>;
   this->loadFiles();
    //Tell the server to start listing
    start_accept();
  }
  //Sends messages to all clients
  void sendMessageToClients(std::string message)
  {

  }
    //This method shutsdown the server
  void shutDownServer()
  {
        std::cout << "Shutting down the server " << killWord << std::endl;
    for(int i = 0; i < sessions->size(); i++)
    {
      sessions->at(i)->sendServerShutDownError("Server shutting down");
    }
  std::cout << "Throwing server shut down " << killWord << std::endl;
    throw serverShutDown();
  }

private:
//Accepts clients
  void start_accept()
  {
    connection::pointer new_connection =
      connection::create(io_context_);

    
    //Start on new thread
    acceptor_.async_accept(new_connection->socket(),
        boost::bind(&server::handle_accept, this, new_connection,
          boost::asio::placeholders::error));
             //Start on new thread

  }
int saveHeader()
{
  std::ofstream file("spreadsheets/names.txt", std::ios::out | std::ios::trunc);
  if (file.is_open())
  {
    //Write the name to the file
for(int i = 0; i < sessions->size(); i++)
  {
    file << sessions->at(i)->getSessionName();
    file << "\n";
  }

  file.close();
  }
  return 0;
}

int loadFiles()
{
  std::string line;
  std::ifstream file("spreadsheets/names.txt");
  if (file.is_open())
  {
    while ( getline (file,line) )
    {
      spreadsheetsession* temp = new spreadsheetsession(line, true);
      sessions->push_back(temp);
    }

  file.close();
  }
  return 0;
}
  //First callback that is invoked in the handshake
  void onReadName( boost::asio::streambuf * response_ , connection::pointer new_connection )
  {
    std::string userName(boost::asio::buffer_cast<const char*>(response_->data()), response_->size());
   //This gets rid of the new line charaecter.
    userName.erase(std::remove(userName.begin(),userName.end(),'\n'), userName.end());

    response_->consume(response_->size());

      //Give the connection the clients username
      new_connection->setName(userName);
      std::stringstream spreadsheetList;
      //Here we iterate through all the current spreadsheet sessions and get their names
      for(int i = 0; i < sessions->size(); i++)
      {
          //We then send the client the name of the spreadsheet
          //This one way to do it but I dont think it correct.
          // new_connection->sendMessage(sessions->at(i)->getSessionName() + "\n");
          spreadsheetList << sessions->at(i)->getSessionName();
          spreadsheetList << "\n";
      }
      spreadsheetList << "\n";
      std::string spreadsheetListMessage = spreadsheetList.str();//Convert our string stream into one big string that has all the spreadsheet data.
        //Send the data to the client.
          new_connection->sendMessage(spreadsheetListMessage);

        //Here we read until we get the second message from the client , the desired spreadsheet name and handle it 
        //in on readselection
          boost::asio::async_read_until(new_connection->socket(), *response_, '\n',
          boost::bind(&server::onReadSelection,this,response_,new_connection));
  }

    //Second callback that is invoked in the handshake
   void onReadSelection( boost::asio::streambuf * response_ , connection::pointer new_connection)
  {
    std::string spreadSheetName(boost::asio::buffer_cast<const char*>(response_->data()), response_->size());
    //This gets rid of the new line charaecter.
    spreadSheetName.erase(std::remove(spreadSheetName.begin(),spreadSheetName.end(),'\n'), spreadSheetName.end());
    response_->consume(response_->size());

          //Check to see if it is the kill word for gracefully shutting down the server
      if(spreadSheetName.compare(killWord) == 0)
      {
          shutDownServer();
          return;
      }
  //Here we iterate through all the current spreadsheet sessions and see if the users requested spreadsheet match any of the ones that we have.
      for(int i = 0; i < sessions->size(); i++)
      {
        if(sessions->at(i)-> getSessionName().compare(spreadSheetName) == 0)
        {
          //We have found a match, so we give this session the connection
          sessions->at(i) -> acceptClient(new_connection);
            return;
        }

      }

      // No spreadsheetsessions were found that had the desired name
      //Make a new spreadshhetsession.
      spreadsheetsession* session = new spreadsheetsession (spreadSheetName);
      //give this session the connection
      session->acceptClient(new_connection);
      //Save the header or spreadsheetsession name
      sessions->push_back(session);
      this->saveHeader();
  }

  //Handels accepting client, takes in the connection between server and client.
  void handle_accept(connection::pointer new_connection,
      const boost::system::error_code& error)
  {
    if (!error)
    {
            //This buffer is to be used throught setting up the initial connection with the clien
           boost::asio::streambuf * r = new boost::asio::streambuf();
           //We want to do a async read until we find the terminal charecter
           boost::asio::async_read_until(new_connection->socket(), *r, '\n',
           boost::bind(&server::onReadName,this,r,new_connection));
           //Here we are telling the server to keep on accepting clients.
          start_accept();
    }

  }
  //Io context for asyn threads and sockets
  boost::asio::io_context& io_context_;
  //Objects that accepts TCP connection
   boost::asio::ip::tcp::acceptor acceptor_;
   //List of pointer of spreadsheetsession used for "gracefully " shutting down the server
    std::vector<spreadsheetsession *>* sessions;
    //The killWord the shutsdown the server
    std::string killWord;

};
#endif
}

  static void getCommandLineInput(ttspreadsheet::server * theServer)
  {
    std::string input;
    std::cin >> input;
    while(true)
    {
      //Looking for the kill command
      if(input.compare("kill") == 0)
      {
         theServer->shutDownServer();
         return;
      }
      input.clear();
       std::cin >> input;
    }
          theServer->shutDownServer();
          return;
    

  }
//This main method fires off the server.
  int main()
  {
     try
     {
        ttspreadsheet::server * theServer2;
    //   //The role of the io_service is to provide sockets and run asynchronous methods
      boost::asio::io_service io_service;
    //   //Here we create our spreadsheet server
     ttspreadsheet::server * theServer = new  ttspreadsheet::server(io_service,"DaBaby");
    //   //Run the server
    std::thread t1(getCommandLineInput,theServer);
    //std::async(std::launch::async,getCommandLineInput,theServer);
       io_service.run();
     }
     catch (std::exception& e)
     {
       std::string s = e.what();
       if(s.compare("Server intentionally shutdown") == 0)
       {
       std::cout << e.what() << std::endl;
       return 0;
       }
       std::cerr << e.what() << std::endl;

     }
    return -1;
  // }
  }

