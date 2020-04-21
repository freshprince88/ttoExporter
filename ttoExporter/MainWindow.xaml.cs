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
using ttoExporter.Statistics;

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
        //List<Rally> rallies = new List<Rally>();
        public IEnumerable<Score> FinalRallyScores { get; private set; }


        public int ttoCounter
        {
            get; set;
        }

        public string disClass
        {
            get; set; }
        

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

                // Matches Sheet
                #region

                ExcelWorksheet MatchesSheet = ExcelPkg.Workbook.Worksheets.Add("Matches");
                ExcelWorksheet RalliesSheet = ExcelPkg.Workbook.Worksheets.Add("Rallies");

                MatchesSheet.TabColor = System.Drawing.Color.Blue;
                RalliesSheet.TabColor = System.Drawing.Color.Red;

                MatchesSheet.Row(1).Style.Font.Bold = true;
                RalliesSheet.Row(1).Style.Font.Bold = true;


                int rowIndexMatch = 2;
                int rowIndexRally = 2;
                int matchNumber = 1;
                int rallyNumber = 1;

                MatchesSheet.Column(2).Style.Numberformat.Format = "dd.mm.yyyy";

                #region Headlines

                MatchesSheet.Cells[1, 1].Value = "Match#";
                MatchesSheet.Cells[1, 2].Value = "ID";
                MatchesSheet.Cells[1, 3].Value = "Date";
                MatchesSheet.Cells[1, 4].Value = "Tournament";
                MatchesSheet.Cells[1, 5].Value = "Year";
                MatchesSheet.Cells[1, 6].Value = "Competition";
                MatchesSheet.Cells[1, 7].Value = "Category";
                MatchesSheet.Cells[1, 8].Value = "Round";
                MatchesSheet.Cells[1, 9].Value = "Sex";
                MatchesSheet.Cells[1, 10].Value = "Player A";
                MatchesSheet.Cells[1, 11].Value = "Country A";
                MatchesSheet.Cells[1, 12].Value = "Ranking A";
                MatchesSheet.Cells[1, 13].Value = "Player B";
                MatchesSheet.Cells[1, 14].Value = "Country B";
                MatchesSheet.Cells[1, 15].Value = "Ranking B";
                MatchesSheet.Cells[1, 16].Value = "Set 1 A";
                MatchesSheet.Cells[1, 17].Value = "Set 1 B";
                MatchesSheet.Cells[1, 18].Value = "Set 2 A";
                MatchesSheet.Cells[1, 19].Value = "Set 2 B";
                MatchesSheet.Cells[1, 20].Value = "Set 3 A";
                MatchesSheet.Cells[1, 21].Value = "Set 3 B";
                MatchesSheet.Cells[1, 22].Value = "Set 4 A";
                MatchesSheet.Cells[1, 23].Value = "Set 4 B";
                MatchesSheet.Cells[1, 24].Value = "Set 5 A";
                MatchesSheet.Cells[1, 25].Value = "Set 5 B";
                MatchesSheet.Cells[1, 26].Value = "Set 6 A";
                MatchesSheet.Cells[1, 27].Value = "Set 6 B";
                MatchesSheet.Cells[1, 28].Value = "Set 7 A";
                MatchesSheet.Cells[1, 29].Value = "Set 7 B";
                MatchesSheet.Cells[1, 30].Value = "Total Points A";
                MatchesSheet.Cells[1, 31].Value = "Total Points B";
                MatchesSheet.Cells[1, 32].Value = "CP A";
                MatchesSheet.Cells[1, 33].Value = "CP B";
                MatchesSheet.Cells[1, 34].Value = "#Serves A";
                MatchesSheet.Cells[1, 35].Value = "#Serves B";
                MatchesSheet.Cells[1, 36].Value = "WP Serves A";
                MatchesSheet.Cells[1, 37].Value = "WP Serves B";
                MatchesSheet.Cells[1, 38].Value = "WP Serves A (direct)";
                MatchesSheet.Cells[1, 39].Value = "WP Serves B (direct)";
                MatchesSheet.Cells[1, 40].Value = "WP Serves A (first attack)";
                MatchesSheet.Cells[1, 41].Value = "WP Serves B (first attack)";
                MatchesSheet.Cells[1, 42].Value = "LP Serves A";
                MatchesSheet.Cells[1, 43].Value = "LP Serves B";

                RalliesSheet.Cells[1, 1].Value = "TotalRally#";
                RalliesSheet.Cells[1, 2].Value = "Match ID";
                RalliesSheet.Cells[1, 3].Value = "Match Competition";
                RalliesSheet.Cells[1, 4].Value = "Match Category";
                RalliesSheet.Cells[1, 5].Value = "Sex";
                RalliesSheet.Cells[1, 6].Value = "Rally # in Match";
                RalliesSheet.Cells[1, 7].Value = "Rally ID";
                RalliesSheet.Cells[1, 8].Value = "Winner";
                RalliesSheet.Cells[1, 9].Value = "Server";
                RalliesSheet.Cells[1, 10].Value = "Length";
                RalliesSheet.Cells[1, 11].Value = "Comment";
                RalliesSheet.Cells[1, 12].Value = "Opening Shot";
                RalliesSheet.Cells[1, 13].Value = "OS # in Rally";
                RalliesSheet.Cells[1, 14].Value = "OS Player";
                RalliesSheet.Cells[1, 15].Value = "OS FH/BH";
                RalliesSheet.Cells[1, 16].Value = "OS Technique";
                RalliesSheet.Cells[1, 17].Value = "OS Technique Option";
                RalliesSheet.Cells[1, 18].Value = "Point of Ball Contact";


                #endregion



                #region Matches

                foreach (Match value in matches)
                {
                    var rallies = new List<Rally>(value.Rallies);
                    value.Year = value.DateTime.Year;
                    value.Sex = setSex(value);

                    MatchesSheet.Cells[rowIndexMatch, 1].Value = matchNumber;
                    MatchesSheet.Cells[rowIndexMatch, 2].Value = value.ID;
                    MatchesSheet.Cells[rowIndexMatch, 3].Value = value.DateTime.ToShortDateString(); ;
                    MatchesSheet.Cells[rowIndexMatch, 4].Value = value.Tournament;
                    MatchesSheet.Cells[rowIndexMatch, 5].Value = value.Year;
                    MatchesSheet.Cells[rowIndexMatch, 6].Value = value.Category;
                    MatchesSheet.Cells[rowIndexMatch, 7].Value = setMatchCategory(value.FirstPlayer.Rank.Position, value.SecondPlayer.Rank.Position);
                    MatchesSheet.Cells[rowIndexMatch, 8].Value = value.Round;
                    MatchesSheet.Cells[rowIndexMatch, 9].Value = value.Sex;
                    MatchesSheet.Cells[rowIndexMatch, 10].Value = value.FirstPlayer.Name;
                    MatchesSheet.Cells[rowIndexMatch, 11].Value = value.FirstPlayer.Nationality;
                    MatchesSheet.Cells[rowIndexMatch, 12].Value = value.FirstPlayer.Rank.Position;
                    MatchesSheet.Cells[rowIndexMatch, 13].Value = value.SecondPlayer.Name;
                    MatchesSheet.Cells[rowIndexMatch, 14].Value = value.SecondPlayer.Nationality;
                    MatchesSheet.Cells[rowIndexMatch, 15].Value = value.SecondPlayer.Rank.Position;

                    this.FinalRallyScores = value.FinishedRallies
                     .Where(r => r.IsEndOfSet)
                     .Select(r => r.FinalRallyScore)
                     .ToList();

                    var scores = this.FinalRallyScores
                     .Select((score, i) => Tuple.Create(i, score));
                    foreach (var pair in scores)
                    {
                        MatchesSheet.Cells[rowIndexMatch, 16 + (pair.Item1) * 2].Value = pair.Item2.First;
                        MatchesSheet.Cells[rowIndexMatch, 17 + (pair.Item1) * 2].Value = pair.Item2.Second;
                    }



                    MatchesSheet.Cells[rowIndexMatch, 30].Value = value.FinishedRallies
                    .Where(r => r.Winner == MatchPlayer.First)
                    .Count();
                    MatchesSheet.Cells[rowIndexMatch, 31].Value = value.FinishedRallies
                    .Where(r => r.Winner == MatchPlayer.Second)
                    .Count();

                    var sets = value.FinishedRallies
                   .Where(r => r.IsEndOfSet)
                   .ToArray();
                    var rallyPerformanceFirst = sets
                        .Sum(r => this.Performance(r, MatchPlayer.First));
                    var rallyPerformanceSecond = sets
                        .Sum(r => this.Performance(r, MatchPlayer.Second));


                    MatchesSheet.Cells[rowIndexMatch, 32].Value = 22 - Math.Sqrt(rallyPerformanceFirst / sets.Length);
                    MatchesSheet.Cells[rowIndexMatch, 33].Value = 22 - Math.Sqrt(rallyPerformanceSecond / sets.Length);

                    int ServiceFrequencyFirst = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.First)
                    .Count();

                    int ServiceFrequencySecond = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.Second)
                    .Count();

              
                    MatchesSheet.Cells[rowIndexMatch, 34].Value = ServiceFrequencyFirst;
                    MatchesSheet.Cells[rowIndexMatch, 35].Value = ServiceFrequencySecond;

                    double ProbabilityOfWinningAfterServiceFirst = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.First && r.Winner == MatchPlayer.First)
                    .Count() / (double) ServiceFrequencyFirst;

                    double ProbabilityOfWinningAfterServiceSecond = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.Second && r.Winner == MatchPlayer.Second)
                    .Count() / (double)ServiceFrequencySecond;

                    double ProbabilityOfWinningAfterServiceFirstDirectly = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.First && r.Winner == MatchPlayer.First && r.Length<3)
                    .Count() / (double)ServiceFrequencyFirst;

                    double ProbabilityOfWinningAfterServiceSecondDirectly = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.Second && r.Winner == MatchPlayer.Second && r.Length<3)
                    .Count() / (double)ServiceFrequencySecond;

                    double ProbabilityOfWinningAfterServiceFirstFirstAttack = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.First && r.Winner == MatchPlayer.First && r.Length < 5)
                    .Count() / (double)ServiceFrequencyFirst;

                    double ProbabilityOfWinningAfterServiceSecondFirstAttack = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.Second && r.Winner == MatchPlayer.Second && r.Length < 5)
                    .Count() / (double)ServiceFrequencySecond;

                    double ProbabilityOfLosingAfterServiceFirst = value.FinishedRallies
                   .Where(r => r.Server == MatchPlayer.First && r.Winner == MatchPlayer.Second)
                   .Count() / (double)ServiceFrequencyFirst;

                    double ProbabilityOfLosingAfterServiceSecond = value.FinishedRallies
                    .Where(r => r.Server == MatchPlayer.Second && r.Winner == MatchPlayer.First)
                    .Count() / (double)ServiceFrequencySecond;



                    MatchesSheet.Cells[rowIndexMatch, 36].Value = ProbabilityOfWinningAfterServiceFirst;
                    MatchesSheet.Cells[rowIndexMatch, 37].Value = ProbabilityOfWinningAfterServiceSecond;

                    MatchesSheet.Cells[rowIndexMatch, 38].Value = ProbabilityOfWinningAfterServiceFirstDirectly;
                    MatchesSheet.Cells[rowIndexMatch, 39].Value = ProbabilityOfWinningAfterServiceSecondDirectly;
                    MatchesSheet.Cells[rowIndexMatch, 40].Value = ProbabilityOfWinningAfterServiceFirstFirstAttack;
                    MatchesSheet.Cells[rowIndexMatch, 41].Value = ProbabilityOfWinningAfterServiceSecondFirstAttack;
                    MatchesSheet.Cells[rowIndexMatch, 42].Value = ProbabilityOfLosingAfterServiceFirst;
                    MatchesSheet.Cells[rowIndexMatch, 43].Value = ProbabilityOfLosingAfterServiceSecond;

                    #region Rallies of the match

                    foreach (Rally rallyValue in value.Rallies)
                    {
                        RalliesSheet.Cells[rowIndexRally, 1].Value = rallyNumber;
                        RalliesSheet.Cells[rowIndexRally, 2].Value = value.ID;
                        RalliesSheet.Cells[rowIndexRally, 3].Value = value.Category ;
                        RalliesSheet.Cells[rowIndexRally, 4].Value = setMatchCategory(value.FirstPlayer.Rank.Position, value.SecondPlayer.Rank.Position);
                        RalliesSheet.Cells[rowIndexRally, 5].Value = value.Sex;
                        RalliesSheet.Cells[rowIndexRally, 6].Value = rallyValue.Number;
                        RalliesSheet.Cells[rowIndexRally, 7].Value = rallyValue.ID;
                        RalliesSheet.Cells[rowIndexRally, 8].Value = rallyValue.Winner;
                        RalliesSheet.Cells[rowIndexRally, 9].Value = rallyValue.Server;
                        RalliesSheet.Cells[rowIndexRally, 10].Value = rallyValue.Length;
                        RalliesSheet.Cells[rowIndexRally, 11].Value = rallyValue.Comment;
            




                        foreach (Stroke strokeValue in rallyValue.Strokes)
                        {
                            if (strokeValue.OpeningShot == true)
                            {
                                RalliesSheet.Cells[rowIndexRally, 12].Value = strokeValue.OpeningShot;
                                RalliesSheet.Cells[rowIndexRally, 13].Value = strokeValue.Number;
                                RalliesSheet.Cells[rowIndexRally, 14].Value = strokeValue.Player;
                                RalliesSheet.Cells[rowIndexRally, 15].Value = strokeValue.Side;
                                RalliesSheet.Cells[rowIndexRally, 16].Value = strokeValue.Stroketechnique.Type;
                                RalliesSheet.Cells[rowIndexRally, 17].Value = strokeValue.Stroketechnique.Option;
                                RalliesSheet.Cells[rowIndexRally, 18].Value = strokeValue.PointOfContact;
                                break;
                            }
                            else
                            {

                            }
                        }

                        rowIndexRally++;
                        rallyNumber++;
                    }

                    #endregion

                    rowIndexMatch++;
                    matchNumber++;  
                }

                MatchesSheet.Protection.IsProtected = false;
                MatchesSheet.Protection.AllowSelectLockedCells = false;
                #endregion
                #endregion


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
            //string completeListOfFiles = string.Join("\n\n", fileNames.ToArray());
            //System.Windows.Forms.MessageBox.Show("Complete list of files:\n" + completeListOfFiles);

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
            AnyFilesToExport = matches.Count > 0;




            //Match Test = matches[0];
            //string FirstPlayer = Test.FirstPlayer.FullName.ToString();
            //System.Windows.Forms.MessageBox.Show("1.Spieler des 1. Matches:\n" + FirstPlayer);

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
        private string AdaptDisabilityClass (DisabilityClass c)
        {
            

            switch (c)
            {
                case DisabilityClass.C1:
                    disClass = "C01";
                    break;

                case DisabilityClass.C2:
                    disClass = "C02";
                    break;
                case DisabilityClass.C3:
                    disClass = "C03";
                    break;
                case DisabilityClass.C4:
                    disClass = "C04";
                    break;
                case DisabilityClass.C5:
                    disClass = "C05";
                    break;
                case DisabilityClass.C6:
                    disClass = "C06";
                    break;
                case DisabilityClass.C7:
                    disClass = "C07";
                    break;
                case DisabilityClass.C8:
                    disClass = "C08";
                    break;
                case DisabilityClass.C9:
                    disClass = "C09";
                    break;
                case DisabilityClass.C10:
                    disClass = "C10";
                    break;
                case DisabilityClass.C11:
                    disClass = "C11";
                    break;
                case DisabilityClass.C1_2:
                    disClass = "C02";
                    break;
                case DisabilityClass.C1_3:
                    disClass = "C01_03";
                    break;
                case DisabilityClass.C1_5:
                    disClass = "C01_05";
                    break;
                case DisabilityClass.C4_5:
                    disClass = "C04";
                    break;
                case DisabilityClass.C6_10:
                    disClass = "C06_10";
                    break;
                case DisabilityClass.C6_7:
                    disClass = "C07";
                    break;
                case DisabilityClass.C6_8:
                    disClass = "C08";
                    break;
                case DisabilityClass.C8_10:
                    disClass = "C08_10";
                    break;
                case DisabilityClass.C8_9:
                    disClass = "C08_09";
                    break;
                case DisabilityClass.C9_10:
                    disClass = "C09";
                    break;
            }
            return disClass;
        }
        private String setMatchCategory(int PositionA, int PositionB)
        {
            if  (PositionA<=50 && PositionB <= 50)
            {
                return "T50vsT50";
            }
            else if (PositionA <= 50 && PositionB > 50)
            {
                return "T50vsO50";
            }
            else if (PositionA > 50 && PositionB <= 50)
            {
                return "T50vsO50";
            }
            else if (PositionA > 50 && PositionB > 50)
            {
                return "O50vsO50";
            }
            else
            {
                return null;
            }
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

        private double Performance(Rally rally, MatchPlayer mp)
        {
            var score = rally.FinalRallyScore;
            var winningScore = score.Highest;

            double bias = rally.Winner == mp ? +1 : -1;
            var @base = winningScore == 11 ?
                score.Of(mp) - score.Of(mp.Other()) - 11 :
                -11 + (bias / (double)(winningScore - 11));

            return Math.Pow(@base, 2);
        }

        #endregion

    }
}

