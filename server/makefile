server: spreadsheetsession.o tsqueue.o server.h server.cpp
	g++ server.cpp -o server -std=c++11 -L /usr/lib/ -lboost_system -lboost_thread -lpthread

spreadsheetsession.o: spreadsheet.o connection.o tsqueue.o spreadsheetsession.h spreadsheetsession.cpp
	g++ -c spreadsheetsession.cpp -std=c++11

tsqueue.o: tsqueue.h
	g++ -c tsqueue.h -std=c++11

connection.o: connection.h
	g++ -c connection.h -std=c++11

spreadsheet.o: dependencyGraph.o cell.o spreadsheet.h  spreadsheet.cpp
	g++ -c spreadsheet.cpp -std=c++11

dependencyGraph.o: cell.o dependencyGraph.h dependencyGraph.cpp
	g++ -c dependencyGraph.cpp -std=c++11

cell.o: cell.h cell.cpp
	g++ -c cell.cpp -std=c++11

clean:
	rm -f cell.o dependencyGraph.o spreadsheet.o connection.o spreadsheetsession.o tsqueue.o server
