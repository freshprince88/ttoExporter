using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using OfficeOpenXml;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ttoExporter.Results;
using ttoExporter.Util;
using Caliburn.Micro;
using ttoExporter.Serialization;

namespace ttoExporter

{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged

    {
        ObservableCollection<string> folderNames = new ObservableCollection<string>();
        //List<string> folderNames = new List<string>();
        List<string> selectedFoldersInListView = new List<string>();
        List<Match> matches = new List<Match>();
        
        public int ttoCounter
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        private bool _anyFilesToExport;

        public bool AnyFilesToExport
        {
            get
            {
                return _anyFilesToExport;
            }
            set
            {
                _anyFilesToExport = value;
                OnPropertyChanged("AnyFilesToExport");

            }
        }

        private bool _anyFolderLoaded;

        public bool AnyFolderLoaded
        {
            get
            {
                return _anyFolderLoaded;
            }
            set
            {
                _anyFolderLoaded = value;
                OnPropertyChanged("AnyFolderLoaded");

            }
        }



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateSelectedFolders(selectedFoldersInListView);
            AnyFolderLoaded = false;
            AnyFilesToExport = false;
            folderNames.CollectionChanged += FolderNames_CollectionChanged;
        }

        private void FolderNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            OnPropertyChanged("AnyFolderLoaded");
        }

        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFolder = new CommonOpenFileDialog();
            openFolder.Multiselect = true;
            openFolder.IsFolderPicker = true;
            openFolder.Title = "Select folders with matches";
            DialogResult result = (System.Windows.Forms.DialogResult)openFolder.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Save selected folders in variable folderNames

                var obs = new ObservableCollection<string>(openFolder.FileNames);
                folderNames = new ObservableCollection<string>(folderNames.Concat(obs));
                UpdateSelectedFolders(folderNames);

                UpdateSelectedMatches(folderNames);
                             
            }
        }

        private void ExportMatches_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Export Excel File To";
            DialogResult result = (System.Windows.Forms.DialogResult)saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //System.Windows.Forms.MessageBox.Show("The Save button was clicked or the Enter key was pressed" +
                //                "\nThe file would have been saved as " +
                //                saveFileDialog.FileName);
                ExcelPackage ExcelPkg = new ExcelPackage();
                ExcelWorksheet Singles = ExcelPkg.Workbook.Worksheets.Add("Singles");
                Singles.TabColor = System.Drawing.Color.Blue;
                Singles.Row(1).Style.Font.Bold = true;
                Singles.Column(2).Style.Numberformat.Format = "dd.mm.yyyy";

                Singles.Cells[1, 1].Value = "ID";
                Singles.Cells[1, 2].Value = ".tto";
                Singles.Cells[1, 3].Value = "Date";
                Singles.Cells[1, 4].Value = "Tournament";
                Singles.Cells[1, 5].Value = "Year";
                Singles.Cells[1, 6].Value = "Category";
                Singles.Cells[1, 7].Value = "Class";
                Singles.Cells[1, 8].Value = "Round";
                Singles.Cells[1, 9].Value = "Sex";
                Singles.Cells[1, 10].Value = "Playtime (gross)";
                Singles.Cells[1, 11].Value = "Surname A";
                Singles.Cells[1, 12].Value = "First Name A";
                Singles.Cells[1, 13].Value = "Country A";
                Singles.Cells[1, 14].Value = "Class A";
                Singles.Cells[1, 15].Value = "Ranking A";
                Singles.Cells[1, 16].Value = "Surname B";
                Singles.Cells[1, 17].Value = "First Name B";
                Singles.Cells[1, 18].Value = "Country B";
                Singles.Cells[1, 19].Value = "Class B";
                Singles.Cells[1, 20].Value = "Ranking B";

                int rowIndex = 2;
                foreach (Match value in matches)
                {
                    ////Singles.Cells[rowIndex, 1].Value = "";
                   
                    rowIndex++;
                }

                Singles.Protection.IsProtected = false;
                Singles.Protection.AllowSelectLockedCells = false;
                ExcelPkg.SaveAs(new FileInfo(saveFileDialog.FileName));


            }
            
        }


        private void ClearAllFolders_Click(object sender, RoutedEventArgs e)
        {
            folderNames.Clear();
            matches.Clear();
            ttoCounter = 0;
          
            UpdateSelectedFolders(folderNames);

            UpdateSelectedMatches(folderNames);
            UpdateCounter(ttoCounter);

        }



        private void ClearSelectedFolders_Click(object sender, RoutedEventArgs e)
        {

            int countSelected = DisplaySelectedFolders.SelectedItems.Count;

            for (int i = 0; i < countSelected; i++)
            {
                string firstSelectedItem = DisplaySelectedFolders.SelectedItem.ToString();
                folderNames.Remove(firstSelectedItem);
            }
            UpdateSelectedFolders(folderNames);
            UpdateSelectedMatches(folderNames);
        }


        #region Update Methods

        


      

        public void UpdateSelectedMatches(IEnumerable<string> sf)
        {
            //Make a list of all files within the selected folders

            IEnumerable<string> fileNamesFullPath = new List<string>();
            IEnumerable<string> fileNames = new List<string>();

            matches.Clear();
            foreach (string value in folderNames)
            {
                fileNamesFullPath = fileNamesFullPath.Concat(Directory.GetFiles(value, "*", SearchOption.AllDirectories).Select(f => System.IO.Path.GetFullPath(f)).Where(s => s.EndsWith(".tto",StringComparison.OrdinalIgnoreCase)));
                fileNames = fileNames.Concat(Directory.GetFiles(value, "*", SearchOption.AllDirectories).Select(f => System.IO.Path.GetFileName(f)).Where(s => s.EndsWith(".tto", StringComparison.OrdinalIgnoreCase)));

            }
            string completeListOfFiles = string.Join("\n\n", fileNames.ToArray());
            System.Windows.Forms.MessageBox.Show("Complete list of files:\n" + completeListOfFiles);

            ttoCounter = 0;



            foreach (string value in fileNamesFullPath)
            {
                XmlMatchSerializer deseri = new XmlMatchSerializer();
                var source = File.OpenRead(value);

                Match tempMatch = deseri.Deserialize(source);
                matches.Add(tempMatch);
                

                ttoCounter++;
                
               

            }
            UpdateCounter(ttoCounter);
            Match Test = matches[0];
            string FirstPlayer = Test.FirstPlayer.FullName.ToString();
            System.Windows.Forms.MessageBox.Show("1.Spieler des 1. Matches:\n" + FirstPlayer);

        }

        private void UpdateSelectedFolders(IEnumerable<string> sf)
        {
            DisplaySelectedFolders.ItemsSource = sf.ToList();
            AnyFolderLoaded = folderNames.Count > 0;
        }

        private void UpdateCounter(int ttoC)
        {
            ttoCounterLabel.Content = ttoC;
        }

        #endregion

        #region Helper Methods

        public IEnumerable<IResult> DeserializeMatch (string path)
        {
            var deserialization = new DeserializeMatchResult(path, Format.XML.Serializer);
            yield return deserialization
                .Rescue()
                .WithMessage("Error loading the match", string.Format("Could not load a match from {0}.", path))
                .Propagate(); // Reraise the error to abort the coroutine            

            var tempMatch = deserialization.Result;

        }

        private string getNextWord(string currentFileName)
        {
            int index = currentFileName.IndexOf("_");
            string nextWord = "";
            if (index > 0)
            {
                nextWord = currentFileName.Substring(0, index);
                return nextWord;
            }
            else
                return currentFileName; ;
        }
        private string cutNextWord(string currentFileName)
        {
            int index = currentFileName.IndexOf("_");
            string remainingFileName = "";
            if (index > 0)
            {
                remainingFileName = currentFileName.Substring(index + 1, (currentFileName.Length - (index + 1)));
            }
            return remainingFileName;
        }

        private string getLastWord(string currentFileName)
        {
            int index = currentFileName.LastIndexOf("_");
            string nextWord = "";
            if (index > 0)
            {
                nextWord = currentFileName.Substring(index + 1, (currentFileName.Length - (index + 1)));
                return nextWord;
            }
            else
                return currentFileName; ;
        }
        public string cutLastWord(string currentFileName)
        {
            int index = currentFileName.LastIndexOf("_");
            string remainingFileName = "";
            if (index > 0)
            {
                remainingFileName = currentFileName.Substring(0, index);
            }
            return remainingFileName;
        }

        private bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }

        private MatchSex setSex(Match m)
        {
            switch (m.Category)
            {
                case MatchCategory.MS:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.MT:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.MD:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.JBS:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.JBT:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.JBD:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.CBS:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.CBT:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.CBD:
                    m.Sex = MatchSex.M;
                    break;
                case MatchCategory.U21BS:
                    m.Sex = MatchSex.M;
                    break;

                case MatchCategory.WS:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.WT:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.WD:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.JGS:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.JGT:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.JGD:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.CGS:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.CGT:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.CGD:
                    m.Sex = MatchSex.F;
                    break;
                case MatchCategory.U21GS:
                    m.Sex = MatchSex.F;
                    break;
                default:
                    m.Sex = MatchSex.Sex;
                    break;
            }
            return m.Sex;
        }

        #endregion

    }
}

