//This is a queue, whose operations are locked to prevent thread errors
//It is based on the following example below.
// https://developer.gnome.org/glib/2.36/glib-Asynchronous-Queues.html#g-async-queue-new
//https://www.justsoftwaresolutions.co.uk/threading/implementing-a-thread-safe-queue-using-condition-variables.html
//https://github.com/OneLoneCoder/olcPixelGameEngine/blob/master/Videos/Networking/Parts1%262/net_tsqueue.h
#ifndef TSQUEUE_H
#define TSQUEUE_H
#include <queue>
#include <string>
#include <mutex>
#include <iostream>
#include <condition_variable>
  class tsqueue
  {

  public:
        tsqueue()
        {
          // q = new queue<std::string>();
        }
        virtual ~tsqueue() { clear(); }
  public:
    //Clear the queue
    void clear()
    {
      std::queue<std::string> empty;
      std::swap( q, empty );
    }

    //Pop and returns the front item in the Queue
    std::string pop_front()
			{
				//std::lock_guard lock(muxQueue);
        muxQueue.lock();
				auto t = std::move(q.front());
				q.pop();
        muxQueue.unlock();
				return t;
			}

      //Gets the front of the queue
      std::string front()
        {
         // std::scoped_lock lock(muxQueue);
          muxQueue.lock();
          return q.front();
          muxQueue.unlock();
        }

      // Adds a string to the back of the queue
			void push_back(const std::string& clientRequest)
			{
        muxQueue.lock();
				//std::scoped_lock lock(muxQueue);
				q.push(std::move(clientRequest));

				// std::unique_lock<std::mutex> ul(muxBlocking);
				// cvBlocking.notify_one();
         muxQueue.unlock();
			}

    // Returns true if Queue has no items
    bool empty()
    {
      bool b;
     muxQueue.lock();
     //Be very carefull here.
      b = q.empty();
     muxQueue.unlock();
     return b;
    }




  protected:
  			std::mutex muxQueue;
  			std::queue<std::string> q;
  			std::condition_variable cvBlocking;
  			std::mutex muxBlocking;

  };
#endif
