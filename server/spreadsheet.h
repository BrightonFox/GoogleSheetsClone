/* This spreadsheet class will hold the general information of
 * a "spreadsheet" as well as a collection of the "cells" that hold
 * any data.
 */
 #ifndef SPREADSHEET_H
  #define SPREADSHEET_H
#include "cell.h"
#include "dependencyGraph.h"

#include <map>
#include <iterator>
#include <iostream>
#include <fstream>
#include <utility>

namespace ttspreadsheet
{
    class spreadsheet
    {                         // holds the order in which cells have been edited

    public:
         /**
    * Name Constructor, intitializes backing data and sets the spreadsheet name to the passed string
    */
    spreadsheet(std::string name)
    {
		sheetName = name;
		cellMap = std::map<std::string, ttspreadsheet::cell>();
		edits = std::vector<std::pair<std::string, std::string>>();
    }

    spreadsheet(std::string filename, bool isFile)
    {
		sheetName = "spreadsheets/"+filename;
		cellMap = std::map<std::string, ttspreadsheet::cell>();
		edits = std::vector<std::pair<std::string, std::string>>();
	
		std::string temp = filename+".sp";
		std::string line;
		std::ifstream file(temp);
	
		if (file.is_open())
		{
			getline (file,line);
			int numberOfEdits = std::stoi(line);
			getline (file,line);
	
			for (int i = 0; i < numberOfEdits; i++)
			{
			    getline (file,line);
			    std::string cellName(line);
			    
			    getline (file,line);
			    std::string contents(line);
			    
			    if (this->cellMap[cellName].getPenultimateHistory() != contents)
			    	setCellContents(cellName, contents);
			    else
			    	revertCellContents(cellName, true);
			    	
			    getline (file,line);
			}
		    file.close();
		}
		else
			std::cout << "Unable to open " << temp;
    }

    /**
    * Copy constructor
    */
    spreadsheet(const spreadsheet &obj)
    {
	this->cellMap = obj.cellMap;
	this->cellDependencies = obj.cellDependencies;
    }

    /**
     * Destructor
     */
    ~spreadsheet()
    {
	this->cellMap.clear();
	//this->cellDependencies.clear();
	this->edits.clear();
    }

    /**
     * returns the name of the spreadsheet
     */
    std::string getName()
    {
		return sheetName;
    }

    void pushLastEditedCell(std::string cellName, std::string contents)
    {
		edits.push_back(std::pair<std::string, std::string>(cellName, contents));
    }

    std::string popLastEditedCell()
    {
    if (edits.size() == 0)
    	return "";
	std::pair<std::string, std::string> edit = edits.back();
	edits.pop_back();
	return edit.first;
    }

    /**
     * returns a string of the contents of the cell located at the passed name, returns an empty string
     * if the cellName does not have any contents
     */
    std::string getCellContents(std::string cellName)
    {
		std::map<std::string, ttspreadsheet::cell>::iterator itr = cellMap.find(cellName);
		
		if (itr == cellMap.end() || itr->second.contentsHistory.size() == 0)
		    return "";
	
		return itr->second.contentsHistory.back();
    }

    /**
     * returns a vector of strings of the content history of the cell located at the passed name, returns
     * an empty string if the cellName does not have any contents
     */
    std::vector<std::string> getCellContentsHistory(std::string cellName)
    {
		std::map<std::string, ttspreadsheet::cell>::iterator itr = cellMap.find(cellName);
	
		if (itr == cellMap.end())
		    return std::vector<std::string> ();
	
		return itr->second.contentsHistory;
    }

    /**
     * Returns a copy of a vector holding the order all edits made to this spreadsheet
     */
    std::vector<std::pair<std::string, std::string>> getEdits()
    {
		return edits;
    }

    /**
     * sets contents of the cell located at the passed name to the passed contents, this returns 0 if
     * the cell was changed, otherwise, a dependency error must've occured and 1 is returned
     */
    int setCellContents(std::string cellName, std::string contents)
    {
	if (cellDependencies.cellExistsinGraph(cellName))
	    cellDependencies.clearDependentsOf(cellName);

	if (contents[0] == '=')
	    cellDependencies.setupDependenciesOf(cellName, contents);

	if (cellDependencies.errorOccured())
	    return 1;

	cellMap[cellName].updateCell(contents);
	pushLastEditedCell(cellName, contents);
	return 0;
    }

    /**
     * reverts the contents of the cell located at the passed name, this returns 0 if
     * the cell was changed, otherwise, a dependency error must've occured and 1 is returned
     * if the cell has no history, a 2 is returned
     */
    int revertCellContents(std::string cellName, bool isEdit)
    {
		if (cellDependencies.cellExistsinGraph(cellName))
		    cellDependencies.clearDependentsOf(cellName);
		if (cellMap.find(cellName) == cellMap.end()
				|| (isEdit && cellMap[cellName].contentsHistory.size() == 0))
			return 2;
			
		std::string curContents;
		if(cellMap[cellName].contentsHistory.size() == 0)
			curContents = "";
		else
			curContents = cellMap[cellName].contentsHistory.back();
		
		std::string revertedContents = cellMap[cellName].revertCell();
		
		if (revertedContents[0] == '=')
		{
		    cellDependencies.setupDependenciesOf(cellName, revertedContents);
			if (cellDependencies.errorOccured())
			{
		    	cellMap[cellName].contentsHistory.push_back(curContents);
		    	return 1;
			}
		}
	
		if (isEdit)
			pushLastEditedCell(cellName, revertedContents);

		return 0;
    }
    
    std::string undo()
    {
    	std::string cellToUndo = popLastEditedCell();

    	if (cellToUndo == "")
    		return "";
    	
    	for (int i = edits.size() - 1; i >= 0; i--)
    	{
    		if (edits[i].first == cellToUndo)
    		{
    			if (cellMap[cellToUndo].getPenultimateHistory() == edits[i].second)
    				cellMap[cellToUndo].contentsHistory.pop_back();
    			else
    				cellMap[cellToUndo].contentsHistory.push_back(edits[i].second);
    			return cellToUndo;
    		}
    	}
    	cellMap[cellToUndo].contentsHistory.pop_back();
    	return cellToUndo;
    }

    /**
     * returns a vector containting the names of all cells that have any contents (in no specific order)
     */
    std::vector<std::string> getAllNonemptyCellNames()
    {
		std::vector<std::string> cellNames;
	
		for (auto const& cell : cellMap)
		    cellNames.push_back(cell.first);
	
		return cellNames;
    }

      private:
        std::string sheetName;
        std::map<std::string, ttspreadsheet::cell> cellMap;                       // holds a collection of "cells" linked to their name(locaiton)
        dependencyGraph cellDependencies;           // cellDependencyGraph used for error checking
        std::vector<std::pair<std::string, std::string>> edits;   
    };
}
#endif