// File base was copied from Nick's CS3500 spreadsheet
// Modified for CS3505 by Clay Ankeny & Glorien Roque

using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{
    public partial class Window : Form
    {
        private ClientNetworking.ClientNetworking Network;
        private Spreadsheet Sheet;
        private const string VERSION = "ps6";
        private int ID;
        private SocketState theServer;

        public Window()
        {
            InitializeComponent();
            Sheet = new Spreadsheet(IsVar, ToUpper, VERSION);
            Network = new ClientNetworking.ClientNetworking();
            AcceptButton = SetButton;
            SPPanel.SelectionChanged += OnSelectionChanged;
            SPPanel.PreviewKeyDown += Shift;
            NameBox.Text = "A1";

            SPPanel.MouseClicked += HandleMouseClick;
            Network.OnGetSheets += ReceiveSheets;
            Network.OnGetID += ReceiveID;
            Network.OnSelection += ReceiveSelection;
            Network.OnUpdate += ReceiveUpdate;
            Network.OnTest += Test;
            Network.OnServerShutdown += ServerShutdown;
            Network.OnClientDisconnect += ClientDisconnect;

            ID = -1234;
        }       

        private void Test(string output)
        {
            MessageBox.Show(output, "Test Output");
        }

        private void HandleMouseClick(int col, int row)
        {
            if (ID != -1234)
            {
                Network.SendSelectionRequest(ConvertCoord(col, row));
            }
        }

        /// <summary>
        /// Receive Spreadsheet selections
        /// </summary>
        /// <param name="selection"></param>
        private void ReceiveSelection(ClientNetworking.CellSelection selection)
        {
            if (selection.selector == ID)
            {
                int col = ConvertCell(selection.cellName).Item1;
                int row = ConvertCell(selection.cellName).Item2;
                SPPanel.SetSelection(col, row);

                OnSelectionChanged(SPPanel);
            }
        }

        /// <summary>
        /// Receive Spreadsheet updates
        /// </summary>
        /// <param name="update"></param>
        private void ReceiveUpdate(ClientNetworking.CellUpdate update)
        {
            string con = update.contents;
            int col = ConvertCell(update.cellName).Item1;
            int row = ConvertCell(update.cellName).Item2;

            try
            {
                string cell = ConvertCoord(col, row);
                List<string> temp = Sheet.SetContentsOfCell(cell, con).ToList<string>();
                foreach (string s in temp)
                {
                    if (Sheet.GetCellValue(s) is FormulaError)
                    {
                        FormulaError fe = (FormulaError)Sheet.GetCellValue(s);
                        SPPanel.SetValue(col, row, fe.Reason);
                    }
                    else
                    {
                        SPPanel.SetValue(col, row, Sheet.GetCellValue(s).ToString());
                    }
                }
            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("Invalid Formula.", "Error");
            }
            catch (CircularException)
            {
                MessageBox.Show("Circular dependency detected. Changes have been reverted.", "Error");
            }
            catch
            {
                MessageBox.Show("An error has occured, please try again.", "Error");
            }

        }

        private void ClientDisconnect(ClientNetworking.ClientDisconnect disconnect)
        {
            MessageBox.Show("User " + disconnect.user + " has disconnected");
        }

        private void ServerShutdown(ClientNetworking.ServerShutdown shutdown)
        {
            MessageBox.Show(shutdown.message);
        }

        /// <summary>
        /// Gets ID
        /// </summary>
        /// <param name="id"></param>
        private void ReceiveID(int id)
        {
            ID = id;
        }

        /// <summary>
        /// Receive the sheets
        /// </summary>
        /// <param name="sheetNames"></param>
        private void ReceiveSheets(List<string> sheetNames)
        {
            string sheetNamesList = "";
            foreach (string s in sheetNames)
            {
                sheetNamesList += s + "\n";
            }
            MessageBox.Show(sheetNamesList, "Sheet Names");
            sheetNameBox.Enabled = true;
        }

        /// <summary>
        /// Returns whether or not the given string is a variable of proper format.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsVar(string s)
        {
            String varPattern = @"^[A-Z][1-9]?[0-9]$";
            return Regex.IsMatch(s, varPattern);
        }

        /// <summary>
        /// Converts the string to upper case.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string ToUpper(string s)
        {
            return s.ToUpper();
        }

        /// <summary>
        /// Converts coordinates to a useable cell name.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static string ConvertCoord(int c, int r)
        {
            return "" + (char)(c + 65) + (r + 1);
        }

        /// <summary>
        /// Converts a cell name into useable coordinates.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static Tuple<int, int> ConvertCell(string s)
        {
            int a = (int)(s[0]) - 65;
            int b = int.Parse(s.Substring(1)) - 1;
            return new Tuple<int, int>(a, b);
        }

        /// <summary>
        /// Moves the selected cell based on arrow key inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shift(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
            SpreadsheetPanel ssp = (SpreadsheetPanel)sender;
            ssp.GetSelection(out int col, out int row);
            switch (e.KeyData)
            {
                case Keys.Up:
                    e.IsInputKey = true;
                    if (ID != -1234)
                        Network.SendSelectionRequest(ConvertCoord(col, row - 1));
                    break;

                case Keys.Down:
                    e.IsInputKey = true;
                    if (ID != -1234)
                        Network.SendSelectionRequest(ConvertCoord(col, row + 1));
                    break;

                case Keys.Left:
                    e.IsInputKey = true;
                    if (ID != -1234)
                        Network.SendSelectionRequest(ConvertCoord(col - 1, row));
                    break;

                case Keys.Right:
                    e.IsInputKey = true;
                    if (ID != -1234)
                        Network.SendSelectionRequest(ConvertCoord(col + 1, row));
                    break;

                default:
                    break;

            }
        }

        /// <summary>
        /// Updates ui when the selected cell changes.
        /// </summary>
        /// <param name="ssp"></param>
        private void OnSelectionChanged(SpreadsheetPanel ssp)
        {
            ContentsBox.Clear();
            ssp.Focus();
            ssp.GetSelection(out int col, out int row);
            ssp.GetValue(col, row, out string v);
            string cell = ConvertCoord(col, row);
            NameBox.Text = cell;
            ValueBox.Text = v;
            object o = Sheet.GetCellContents(cell);
            if (o is Formula)
                ContentsBox.Text = "=" + Sheet.GetCellContents(cell).ToString();
            else
                ContentsBox.Text = Sheet.GetCellContents(cell).ToString();
        }

        /// <summary>
        /// Updates the cell, value, and contents boxes on the top of the program.
        /// </summary>
        private void UpdateBoxes()
        {
            SPPanel.GetSelection(out int col, out int row);
            string cell = ConvertCoord(col, row);
            NameBox.Text = cell;
            ValueBox.Text = Sheet.GetCellValue(cell).ToString();
            object o = Sheet.GetCellContents(cell);
            if (o is Formula)
                ContentsBox.Text = "=" + Sheet.GetCellContents(cell).ToString();
            else
                ContentsBox.Text = Sheet.GetCellContents(cell).ToString();
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            SPPanel.Focus();
            string con = ContentsBox.Text;
            SPPanel.GetSelection(out int col, out int row);

            Network.SendEditRequest(ConvertCoord(col, row), con);
        }

        
        //Background process for setting a cell.
        private void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BGWorker.ReportProgress(0);
                Tuple<string, int, int> args = (Tuple<string, int, int>)e.Argument;
                string cell = ConvertCoord(args.Item2, args.Item3);
                BGWorker.ReportProgress(25);
                List<string> temp = Sheet.SetContentsOfCell(cell, args.Item1).ToList<string>();
                BGWorker.ReportProgress(50);
                int prog = 0;
                foreach (string s in temp)
                {
                    BGWorker.ReportProgress(50 + ((prog * 50) / temp.Count));
                    Tuple<int, int> tp = ConvertCell(s);
                    int c = tp.Item1;
                    int r = tp.Item2;
                    if (Sheet.GetCellValue(s) is FormulaError)
                    {
                        FormulaError fe = (FormulaError)Sheet.GetCellValue(s);
                        SPPanel.SetValue(c, r, fe.Reason);
                    }
                    else
                    {
                        SPPanel.SetValue(c, r, Sheet.GetCellValue(s).ToString());
                    }
                    prog++;
                }
                BGWorker.ReportProgress(100);
            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("Invalid Formula.", "Error");
                BGWorker.ReportProgress(0);
            }
            catch (CircularException)
            {
                MessageBox.Show("Circular dependency detected. Changes have been reverted.", "Error");
                BGWorker.ReportProgress(0);
            }
            catch
            {
                MessageBox.Show("An error has occured, please try again.", "Error");
                BGWorker.ReportProgress(0);
            }
        }

        private void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateBoxes();
            SetButton.Enabled = true;
        }

        private void BGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgress.Value = e.ProgressPercentage;
        } 

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HelpMenuButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(System.IO.File.ReadAllText("..\\..\\..\\HelpMenu.txt"), "Help Menu");
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            Network.ConnectToServer(addressBox.Text, usernameBox.Text);
        }

        private void retreiveButton_Click(object sender, EventArgs e)
        {
            Network.RequestSpreadsheet(sheetNameBox.Text, theServer);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            Network.SendUndoRequest();
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            SPPanel.GetSelection(out int col, out int row);
            Network.SendRevertRequest(ConvertCoord(col, row));
        }
        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Network.disconnectClient();
        }
    }
}