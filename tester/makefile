a.out: test_suite.o tcp_client.o client.o
	g++ -g -o spreadsheet_tester client.o tcp_client.o test_suite.o -pthread -lboost_unit_test_framework

test_suite.o: include/tcp_client.h test_suite.cpp
	g++ -g -c test_suite.cpp

tcp_client.o: include/tcp_client.h tcp_client.cpp
	g++ -g -c tcp_client.cpp

client.o: include/client.h client.cpp
	g++ -g -c client.cpp

clean:
	rm -f client.o tcp_client.o test_suite.o
