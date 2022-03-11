#ifndef ASIOCLIENT_H
#define ASIOCLIENT_H

/**
* The asio client class uses boost::asio to connect to a server using an ip and port
* The class then has a method to send a message to the server, used for stress testing a connection
*
* Code written by Jens Phanich, Sam Peters, and the rest of Team Team for CS 3505
**/

#include <iostream>
#include <boost/asio.hpp>
#include <boost/bind.hpp>

using namespace boost::asio;
using ip::tcp;
using std::string;
using std::cout;
using std::endl;

//Client socket
tcp::socket *theSocket;

static boost::asio::io_context ic;

//Class to simply connect and be able to send to serve for stress test
class asioclient {

public:
  //Constructor iwht basic io_context, ip, and port connection
  asioclient(std::string ip, int port)
  {
    //socket creation
    theSocket = new tcp::socket(ic);
    //connection
    theSocket->connect( tcp::endpoint( boost::asio::ip::address::from_string(ip), port ));
  }

  //Empty send handler
  static void handler(const boost::system::error_code& error, std::size_t bytes_transferred) {}

  //Send method that sends parameter
  void send(std::string msg) {
    theSocket->write_some(boost::asio::buffer(msg.c_str(),msg.size()));
  }
};

#endif
