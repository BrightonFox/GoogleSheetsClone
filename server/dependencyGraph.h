/* This class will represent the connection of different cells
 * to one another to aid in error handling
 */

#include <string>
#include <map>
#include <vector>
#include <regex>

namespace ttspreadsheet
{
    class dependencyGraph
    {
    private:
        bool errorHasOccured;
        std::map<std::string, std::vector<std::string>> dependentsMap;
        //std::map<std::string, std::vector<std::string>> dependeesMap;
        std::regex varPattern;

    public:
          dependencyGraph()
    {
        errorHasOccured = false;
        varPattern = std::regex("[a-zA-Z_](?: [a-zA-Z_]|\\d)*");
    }

    /**
     * copy constructor
     */
    dependencyGraph(const dependencyGraph &obj)
    {
        this->errorHasOccured = obj.errorHasOccured;
        this->dependentsMap = obj.dependentsMap;
        //this->dependeesMap = obj.dependeesMap;
    }

    /**
     * destructor
     */
    ~dependencyGraph()
    {
        //this->errorHasOccured.clear();
        this->dependentsMap.clear();
        //this->dependeesMap.clear();
    }

    /**
     * returns true if a circular dependency would have been created
     */
    bool errorOccured()
    {
        bool errorCheck = errorHasOccured;
        errorHasOccured = false;
        return errorCheck;
    }

    /**
     * returns true if the passed cell already exists in the dependencyGraph
     */
    bool cellExistsinGraph(std::string cellName)
    {
        if (dependentsMap.count(cellName) == 1)
            return true;
        return false;
    }

    /**
     * returns true if the passed cell has any dependents
     */
    bool hasDependents(std::string cellName)
    {
        if (cellExistsinGraph(cellName))
            return dependentsMap[cellName].size() > 0;
        return false;
    }

    /**
     * returns a vector containing the names of all cells that depend on the passed cell
     */
    std::vector<std::string> getDependents(std::string cellName)
    {
        if (cellExistsinGraph(cellName))
            return std::vector<std::string>(dependentsMap[cellName]);
        return std::vector<std::string>();
    }

    /**
     * removes the dependents of the passed cell from the graph
     */
    void clearDependentsOf(std::string cellName)
    {
        dependentsMap[cellName].clear();
    }

    /**
     * assigns the dependents of the passed cell by extracting them from the passed
     * content; unless changing these would result in a circular dependency, in which case
     * the original depdendents will remain
     */
    void setupDependenciesOf(std::string cellName, std::string contents)
    {
        std::vector<std::string> oldDependents = dependentsMap[cellName];

        dependentsMap[cellName] = getVariables(contents);

        dependencyCheck(cellName);

        if (errorHasOccured)
            dependentsMap[cellName] = oldDependents;
    }

    /**
     * returns a list of cell names (variables) that are contained in the passed contents
     */
    std::vector<std::string> getVariables(std::string contents)
    {
        std::sregex_iterator iter(contents.begin(), contents.end(), varPattern);
        std::sregex_iterator end;
        std::vector<std::string> vars;
        std::smatch var;

        while (iter != end)
        {
            var = *iter;
            vars.push_back(var.str());
            ++iter;
        }

        return vars;
    }

    /**
     * checks the dependents of the passed cell to verify that no circular dependencies exist;
     * if one is found, the method returns and the graph will be marked as having an error
     */
    void dependencyCheck(std::string cellName)
    {
        recursiveDependencyCheck(cellName, cellName, std::vector<std::string>());
    }

    /**
     * recursive helper method to check all dependents of the originalCellName cell
     */
    void recursiveDependencyCheck(std::string originalCellName, std::string currentCellName, std::vector<std::string> visited)
    {
        // only proceed if an erro has not already occured
        if (!errorHasOccured)
        {
            // add the current cell to the list of visited cells
            visited.push_back(currentCellName);
            for (std::string cur : dependentsMap[currentCellName])
            {
                // if, while traversing thorugh the dependents, the parent cell is again found, a circular dependency has occured
                if (cur == originalCellName)
                {
                    errorHasOccured = true;
                    return;
                }
                else if (std::find(visited.begin(), visited.end(), cur) == visited.end())
                    recursiveDependencyCheck(originalCellName, cur, visited);
            }
        }
    }
    };
}
