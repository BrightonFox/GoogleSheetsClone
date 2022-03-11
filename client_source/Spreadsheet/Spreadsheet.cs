// Spreadsheet class for storing cell contents by Nicholas Vaskelis for CS3500, October 2020.
//
// File was copied over from Nick's spreadsheet project as a base for CS3505

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// The backing class of the spreadsheet program.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Holds the contents and values of a spreadsheet shell. Contents can be of type String, Double, or Formula.
        /// Values are set and recalculated outsside of the class and are done via the set value function.
        /// </summary>
        private class Cell
        {
            private object cell;
            private object value;

            /// <summary>
            /// Creates a cell and stores a string.
            /// </summary>
            /// <param name="s"></param>
            public Cell(string s)
            {
                cell = s;
            }

            /// <summary>
            /// Creates a cell and stores a formula.
            /// </summary>
            /// <param name="f"></param>
            public Cell(Formula f)
            {
                cell = f;         
            }

            /// <summary>
            /// Creates a cell and stores a double.
            /// </summary>
            /// <param name="d"></param>
            public Cell(double d)
            {
                cell = d;
            }

            /// <summary>
            /// Returns the content of the cell.
            /// </summary>
            /// <returns></returns>
            public object getCell()
            {
                return cell;
            }

            /// <summary>
            /// Sets the content of the cell.
            /// </summary>
            /// <returns></returns>
            public void setCell(object c)
            {
                cell = c;
            }

            /// <summary>
            /// Returns the value of the cell.
            /// </summary>
            /// <returns></returns>
            public object getValue()
            {
                return value;
            }

            /// <summary>
            /// Sets the value of the cell.
            /// </summary>
            /// <returns></returns>
            public void setValue(object o)
            {
                value = o;
            }
        }

        //Maps a valid cell name string to a Cell object.
        private Dictionary<string, Cell> Contents;
        private DependencyGraph DGraph;
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Finds the value of variable. Throws if variable value is not a double.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private double Lookup(string s)
        {
            object temp = GetCellValue(s);
            if (temp is double)
            {
                return (double) temp;
            }

            else
            {
                throw new ArgumentException("");
            }
        }

        /// <summary>
        /// Creates an empty Spreadsheet.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Contents = new Dictionary<string, Cell>();
            DGraph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Creates an empty Spreadsheet with provided validity and normalization delegates along with a version property.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Contents = new Dictionary<string, Cell>();
            DGraph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Creates a Spreadsheet from an XML file with provided validity and normalization delegates along with a version property.
        /// </summary>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Contents = new Dictionary<string, Cell>();
            DGraph = new DependencyGraph();

            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case ("spreadsheet"):
                                    if (!reader.GetAttribute("version").Equals(version))
                                    {
                                        throw new SpreadsheetReadWriteException("Version mismatch.");
                                    }
                                    break;
                                case ("cell"):
                                    using (XmlReader reader2 = reader.ReadSubtree())
                                    {
                                        reader2.Read();
                                        string name = "";
                                        string content = "";
                                        while (reader2.Read())
                                        {
                                            while (reader2.IsStartElement())
                                            {
                                                if (reader2.Name.Equals("name"))
                                                {
                                                    name = reader2.ReadElementContentAsString();
                                                }

                                                else if (reader2.Name.Equals("contents"))
                                                {
                                                    content = reader2.ReadElementContentAsString();
                                                }

                                                else
                                                {
                                                    throw new SpreadsheetReadWriteException("Unknown element in file. Check file format.");
                                                }
                                            }
                                        }

                                        SetContentsOfCell(name, content);
                                    }
                                    break;
                                default:
                                    throw new SpreadsheetReadWriteException("Unknown element in file. Check file format.");
                            }
                        }
                    }


                }
            }
            catch(SpreadsheetReadWriteException e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
            catch
            {
                throw new SpreadsheetReadWriteException("An error occured while loading the spreadsheet");
            }

            Changed = false;
        }

        /// <summary>
        /// Returns whether or not the given string is a variable of proper format.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsVar(string s)
        {
            String varPattern = @"^[a-zA-Z]*(?:\d)*$";
            return Regex.IsMatch(s, varPattern);
        }

        /// <summary>
        /// Updates the values of the cells in list. Returns list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<string> UpdateCells(IList<string> list)
        {
            foreach(string s in list)
            {
                object o = GetCellContents(s);
                if(o is Formula)
                {
                    Formula temp = (Formula) o;
                    Contents[s].setValue(temp.Evaluate(Lookup));
                }
                else
                {
                    if(Contents.ContainsKey(s))
                        Contents[s].setValue(o);
                }
            }

            Changed = true;
            return list;
        }

        public override object GetCellContents(string name)
        {
            name = Normalize(name);

            if (name is null || !IsVar(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            if(!Contents.ContainsKey(name))
            {
                return "";
            }

            return Contents[name].getCell();

        }

        public override object GetCellValue(string name)
        {
            name = Normalize(name);

            if (name is null || !IsVar(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            if (!Contents.ContainsKey(name))
            {
                return "";
            }

            return Contents[name].getValue();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return Contents.Keys;
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = Normalize(name);

            if (name is null || !IsVar(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            else if (content is null)
            {
                throw new ArgumentNullException();
            }

            else if(Double.TryParse(content, out double temp))
            {
                return UpdateCells(SetCellContents(name, temp));
            }

            else if(!content.Equals("") && content[0].Equals('='))
            {
                return UpdateCells(SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid)));
            }

            else
            {
                return UpdateCells(SetCellContents(name, content));
            }
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            if (Contents.ContainsKey(name))
            {
                //if double replaces a formula, remove its dependecies and update.
                Contents[name] = new Cell(number);
                foreach (string s in DGraph.GetDependees(name).ToList())
                {
                    DGraph.RemoveDependency(s, name);
                }
                return GetCellsToRecalculate(name).ToList();
            }

            else
            {
                Contents.Add(name, new Cell(number));
                return GetCellsToRecalculate(name).ToList();
            }
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            if(Contents.ContainsKey(name))
            {
                //If something is set to nothing, remove it from the spreadsheet and dependency graph.
                if (text.Equals(""))
                {
                    Contents.Remove(name);
                    foreach (string s in DGraph.GetDependees(name).ToList())
                    {
                        DGraph.RemoveDependency(s, name);
                    }
                    return GetCellsToRecalculate(name).ToList();

                }

                else
                {
                    //if string replaces a formula, remove its dependecies and update.
                    Contents[name].setCell(text);
                    foreach (string s in DGraph.GetDependees(name).ToList())
                    {
                        DGraph.RemoveDependency(s, name);
                    }
                    return GetCellsToRecalculate(name).ToList();
                }
                
            }

            else
            {
                //Setting nothing to nothing creates no change. Nothing needs to be updated.
                if (text.Equals(""))
                {
                    return new List<string>();
                }

                else
                {
                    Contents.Add(name, new Cell(text));
                    return GetCellsToRecalculate(name).ToList();
                }
                    
            }
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (Contents.ContainsKey(name))
            {
                object temp = Contents[name].getCell();
                HashSet<string> temp2 = new HashSet<string>(DGraph.GetDependees(name));
                try
                {
                    Contents[name].setCell(formula);
                    DGraph.ReplaceDependees(name, formula.GetVariables());
                    return GetCellsToRecalculate(name).ToList();
                }

                //if a circular dependency is detected, reset to previous state and throw.
                catch
                {
                    Contents[name].setCell(temp);
                    DGraph.ReplaceDependees(name, temp2);
                    throw new CircularException();
                }
            }

            else
            {
                try
                {
                    Contents.Add(name, new Cell(formula));
                    foreach (string s in formula.GetVariables().ToList())
                    {
                        DGraph.AddDependency(s, name);
                    }
                    return GetCellsToRecalculate(name).ToList();
                }

                //if a circular dependency is detected, reset to previous state and throw.
                catch
                {
                    Contents.Remove(name);
                    foreach (string s in formula.GetVariables().ToList())
                    {
                        DGraph.RemoveDependency(s, name);
                    }
                    throw new CircularException();
                }
            }
            
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return DGraph.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement("spreadsheet"))
                        {
                            if(!(reader.GetAttribute("version") is null))
                                return reader.GetAttribute("version");
                            else
                                throw new SpreadsheetReadWriteException("Version not found.");
                        }
                    }
                    throw new SpreadsheetReadWriteException("Invalid File.");
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("An error occured while loading the spreadsheet");
            }
        }

        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    foreach (string s in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", s);
                        if (Contents[s].getCell() is Formula)
                        {
                            writer.WriteElementString("contents", "=" + Contents[s].getCell().ToString());
                        }
                        else
                        {
                            writer.WriteElementString("contents", Contents[s].getCell().ToString());
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("An error occured while loading the spreadsheet");
            }
        }
        
    }
}
