//This class repersent a connection between the client and the server.
//This means this connection class has the functionality to read and write from
//the connection buffer.
//This code is based on the boost example code found here https://www.boost.org/doc/libs/1_75_0/doc/html/boost_asio/tutorial.html
//Auth Jens Phanich

  #ifndef CONNECTION_H
  #define CONNECTION_H
  #include "tsqueue.h"
  #include <ctime>
  #include <iostream>
  #include <string>
  #include <boost/bind.hpp>
  #include <boost/shared_ptr.hpp>
  #include <boost/enable_shared_from_this.hpp>
  #include <boost/asio.hpp>
  #include "tsqueue.h"

class connection : public boost::enable_shared_from_this<connection>
{
  public:
  typedef boost::shared_ptr<connection> pointer; //This is the pointer that the server or who ever makes this connection needs to keep track of
  

  //Here we invoke the constructor to the object and return its pointer.
  static pointer create(boost::asio::io_context& io_context)
  {
    return pointer(new connection(io_context));
  }
  //This method returns the socket of this connection.
  boost::asio::ip::tcp::socket& socket()
  {
    return socket_;
  }
  //This sends a message to the client
  //By writting in the socket
  void sendMessage(const std::string & message)
  {
   boost::asio::write( socket_, boost::asio::buffer(message));
  }
  
  void sendMessage2(const std::string & message)
  {
    boost::asio::write( socket_, boost::asio::buffer(message));
  }

  //Sets the name that this connection wants to go by.
  void setName(const std::string & newName)
  {
      name = newName;
  }
  //Returns the ID of this connection
  int getID()
  {
    return id;
  }
  //Sets the ID of this connection
  void setID(const int idValue)
  {
    id = idValue;
  }

  //Returns the name of this connection
  std::string getName()
  {
    return name;
  }
  //Starts reading and writting.
  void start()
  {
      	 //   if(socket_.is_open())
      	 //   {
     boost::asio::async_read_until(socket_, response_, '\n',
          boost::bind(&connection::readData, shared_from_this(), boost::asio::placeholders::error ));
      	 //   }
      	 //std::cout << "Socket is closed dropping connection for " << name << std::endl;
        // //Invoke call back here
        // dropConnection(id);
        // std::cout << "Socket is closed " << name << std::endl;
  }

  //Gives a Thread safe queue to this objet that it can read and write to.
  void giveTsQueue(tsqueue * q)
  {
    commandLog = q;
  }
  //Gives the connection a call back to invoke when new data is read
  void giveCallBack(std::function<void(std::string s)> func)
  {
     callBack = func;
  }

   //Gives the connection a call back to invoke when an error occurs in the connection
  void giveDisconnectCallBack(std::function<void(int i)> func)
  {
     dropConnection = func;
  }
  //This is a special read method for debugging purposes only, it does not fire of an event loop.
  std::string handShakeRead()
  {
      //We are going to read until we find the terminal charecter
     boost::asio::streambuf buf;

    boost::asio::read_until(socket_, buf, "\n");
    std::string data(boost::asio::buffer_cast<const char*>(buf.data()), buf.size());
   // std::string data = boost::asio::buffer_cast(buf.data());
    return data;
  }


private:
  connection(boost::asio::io_context& io_context)
    : socket_(io_context)
  {
    //Here in the constructor we intialize our socket by giving it an io_context.
  }
  //readData here
  void readData( const boost::system::error_code& error)
  {
      if(!error)
      {
    std::string data(boost::asio::buffer_cast<const char*>(response_.data()), response_.size());
    //This gets rid of the new line charecter.
    data.erase(std::remove(data.begin(),data.end(),'\n'), data.end());
    //Put the data into the q.
    commandLog->push_back(data);
    //Clear the buffer.
    response_.consume(response_.size());

      //Invoke call back here
      callBack(this->name);
      //Continue listing
      start();
      return;
      }
      // //Remove this connection
      std::cout << "an error has occured " << "the ID is  " << id << std::endl;
       std::cout << "The error is " << error<< std::endl;
      dropConnection(id);

  }
  void handle_write(const boost::system::error_code& /*error*/,
      size_t /*bytes_transferred*/)
  {
  }
  //No destutor neaded since nothing is being alllocated on heap with new keyword
   boost::asio::ip::tcp::socket socket_;//The connection socket
   tsqueue * commandLog;//The log of json commands, this is shared amongst all other connections 
   //in the spreadsheet session as well as the spreadsheet session
  boost::asio::streambuf response_;//A buffer used to record data from the socket
  //The userName of the connection.
  std::string name;
  //The main delgate/callBack function used to tell the spreadsheetsession of new data.
  std::function<void(std::string s)> callBack;
  //The ID of this connection
  int id;
  //The main delgate/callBack function used to tell the spreadsheetsession to disconnect.
  std::function<void(int)> dropConnection;

};
#endif
