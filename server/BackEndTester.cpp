
#include "spreadsheetsession.h"
#include <boost/json/src.hpp>
class BackEndTester
{
    public:
void testJSON(const::std::string g, ttspreadsheet::spreadsheetsession* s)
{
    s->onReadNewCommand(g,"name");
}
};
int main()
{
    ttspreadsheet::spreadsheetsession * session= new  ttspreadsheet::spreadsheetsession("");
    BackEndTester * tester= new BackEndTester();
    
    // boost::json::value select = {
    // { "requestType", "selectCell" },
    // { "cellName", "a1" },
    // };
    
    boost::json::value edit = {
    { "requestType", "editCell" },
    { "cellName", "a1" },
    { "contents", "test :D" },
    };
    
    boost::json::value revert = {
    { "requestType", "revertCell" },
    { "cellName", "a1" },
    };
    
    boost::json::value undo = {
    { "requestType", "undo" },
    };
    
    std::cout << "testing bad JSON\n";
    tester->testJSON("thisIsBad",session);
    
    // std::cout << "testing select JSON\n";
    // tester->testJSON(serialize(select),session);
    
    std::cout << "testing edit JSON\n";
    tester->testJSON(serialize(edit),session);
    
    std::cout << "testing revert JSON\n";
    tester->testJSON(serialize(revert),session);
    
    std::cout << "testing undo JSON\n";
    tester->testJSON(serialize(undo),session);
    
    return 0;
}
