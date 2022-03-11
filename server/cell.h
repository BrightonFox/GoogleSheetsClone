/* This class will be used to hold the contents of a "cell"
 * in a "spreadsheet" object.
 */

#include <string>
#include <vector>

namespace ttspreadsheet
{
    class cell
    {
        //friend class ttspreadsheet::spreadsheet;

    public:
        std::vector<std::string> contentsHistory; // contents and history of contents of the cell
       
    /** Default cosntructor, sets cells contents to an empty string
    */
    cell()
    {
        cell("");
    }

    /**
     * Cell constructor that sets the contents to the passed string
     */
    cell(std::string contents)
    {
        this->contentsHistory.push_back(contents);
    }

    /**
     * Copy constructor
     */
    cell(const cell &obj)
    {
        this->contentsHistory = obj.contentsHistory;
    }

    /**
     * Destructor
     */
    ~cell()
    {
        while(!contentsHistory.empty())
            contentsHistory.pop_back();
    }

    /**
     * pushes the passed string to the cell's contentsHistory stack
     */
    void updateCell(std::string contents)
    {
        this->contentsHistory.push_back(contents);
    }

    /**
     * pops the top element of the cell's contentHistory and returns the new top
     */
    std::string revertCell()
    {
        if (contentsHistory.size() > 1)
        {
            this->contentsHistory.pop_back();
            return this->contentsHistory.back();
        }
        else if (contentsHistory.size() == 1)
        {
            this->contentsHistory.pop_back();
            return "";
        }
        else
        {
            return "";
        }
    }
    
    std::string getPenultimateHistory()
    {
        if (this->contentsHistory.size() < 2)
            return "";
        return this->contentsHistory.end()[-2];
    }
    };
}
