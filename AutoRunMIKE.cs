using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.Objects;
using Microsoft.VisualBasic;
using System.Management;
using System.Runtime.InteropServices;
using CSSPAppModel;

namespace AutoRunMike
{
    public partial class AutoRunMikeMzLaunchFileName : Form
    {
        //Variables
        //private const uint GW_CHILD = 5;
        //private const uint GW_HWNDNEXT = 2;
        //private const uint WM_LBUTTONDOWN = 0x201;
        //private const uint WM_LBUTTONUP = 0x202;
        //private const uint WM_CHAR = 0x102;
        //private const uint WM_KEYDOWN = 0x100;
        //private const uint WM_KEYUP = 0x101;
        //private const uint WM_SYSKEYDOWN = 0x104;
        //private const uint WM_SYSKEYUP = 0x105;
        //private const uint VK_SHIFT = 0x10;
        //private const uint VK_CONTROL = 0x11;

        // Enumeration
        public enum PurposeType
        {
            Input = 0,
            InputPol = 1,
            MikeResult = 2,
            KMZResult = 3,
            Original = 4,
            Other = 5
        }
        public enum ScenarioStatusType
        {
            Created = 0,    // scenario has just been created
            ReadyToRun = 1, // scenario is ready to be run
            Running = 2,    // scenario being run
            Completed = 3,  // scenario ran without error
            Error = 4,      // scenario ran but has error
            Canceled = 5,     // scenario was cancelled
            Changed = 6       // scenario was saved
        }
        CSSPAppDBEntities vpse;

        // Properties
        public string CurrentFile { get; set; }
        public M21fm m21fm { get; set; }
        public MikeScenario CurrentMikeScenario { get; set; }
        public M21fmLog m21fmLog { get; set; }
        public bool PleasePause { get; set; }

        //List<WndHandleAndTitle> DesktopChildrenWindowsList;
        //APIFunc af = new APIFunc() as APIFunc;

        // Constructor
        public AutoRunMikeMzLaunchFileName()
        {
            InitializeComponent();
            timerMzLaunchStillRunning.Interval = 10000; // 10 seconds
            timerAutoRunMike.Interval = 10000; // 10 seconds
            CurrentFile = "";
            vpse = new CSSPAppDBEntities();
        }

        // Functions
        private void CreateNewM21FMWithEvents()
        {
            if (m21fm != null)
            {
                m21fm = null;
            }
            m21fm = new M21fm();

            m21fm.M21fmMessageEvent += new M21fm.M21fmMessageEventHandler(m21fm_MessageEvent);

            return;
        }
        private void m21fm_MessageEvent(object sender, M21fm.M21fmMessageEventArgs e)
        {
            richTextBoxStatus.AppendText(e.Message);
            Application.DoEvents();
        }
        private MemoryStream fileToMemoryStream(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            MemoryStream myMemoryStream = new MemoryStream();
            if (!fi.Exists)
            {
                richTextBoxStatus.AppendText("File: \r\n\r\n[" + fileName + "] \r\n\r\ndoes not exist ... \r\n");
                return null;
            }
            richTextBoxStatus.AppendText("Reading file: \r\n\r\n [" + fileName + "] ... \r\n\r\n");
            FileStream myFileStream = fi.OpenRead();
            myFileStream.CopyTo(myMemoryStream);
            myFileStream.Flush();
            myMemoryStream.Position = 0;
            return myMemoryStream;
        }
        private void CheckForMoreReadyToRun()
        {
            richTextBoxStatus.Clear();
            if (!DongleIsPlugged())
            {
                lblStatusWorking.Text = "Dongle ... ";
                lblStatusUpdate.Text = "Not found ... ";
                richTextBoxStatus.AppendText("MIKE 21 dongle not found ...\r\n");
                timerAutoRunMike.Enabled = true;
                return;
            }
            timerAutoRunMike.Enabled = false;

            if (PleasePause)
            {
                butStart.Enabled = true;
                butPause.Enabled = false;
                richTextBoxStatus.AppendText("AutoRunMike was paused ...\r\n");
                lblStatusWorking.Text = "Paused ... ";
                lblStatusUpdate.Text = "Press start ... ";
                return;
            }

            lblStatusWorking.Text = "Working ... ";
            lblStatusUpdate.Text = "Reading ... ";

            string CurrentStatus = ScenarioStatusType.ReadyToRun.ToString();

            List<MikeScenario> mikeScenarioList = (from ms in vpse.MikeScenarios
                                                   where ms.ScenarioStatus == CurrentStatus
                                                   select ms).ToList<MikeScenario>();

            dataGridViewScenarios.DataSource = mikeScenarioList;

            if (mikeScenarioList.Count > 0)
            {

                mikeScenarioList[0].ScenarioStatus = ScenarioStatusType.Running.ToString();
                try
                {
                    richTextBoxStatus.AppendText("Updating MikeScenario ScenarioStatus to Running ...\r\n");
                    vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    richTextBoxStatus.AppendText("Updated MikeScenario ScenarioStatus to Running ...\r\n");
                }
                catch (Exception)
                {
                    richTextBoxStatus.AppendText("Error while updating MikeScenario ScenarioStatus to Running ...\r\n");
                    lblStatusWorking.Text = "Error ... ";
                    lblStatusUpdate.Text = "See details below ... ";
                    return;
                }
                timerMzLaunchStillRunning.Enabled = true;
                CurrentMikeScenario = mikeScenarioList[0];
                richTextBoxStatus.AppendText("Trying to run MikeScenarioID = [" + mikeScenarioList[0].MikeScenarioID + "] ...\r\n");
                lblStatusUpdate.Text = "Loading ... " + mikeScenarioList[0].ScenarioName;
                if (RunScenario(CurrentMikeScenario))
                {
                    richTextBoxStatus.AppendText("Running MikeScenarioID = [" + mikeScenarioList[0].MikeScenarioID + "] ...\r\n");
                }
                else
                {
                    richTextBoxStatus.AppendText("Error while trying to run MikeScenarioID = [" + mikeScenarioList[0].MikeScenarioID + "] ...\r\n");
                    Pause();
                    timerAutoRunMike.Enabled = false;

                    mikeScenarioList[0].ScenarioStatus = ScenarioStatusType.Running.ToString();
                    try
                    {
                        richTextBoxStatus.AppendText("Updating MikeScenario ScenarioStatus to Error ...\r\n");
                        vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                        richTextBoxStatus.AppendText("Updated MikeScenario ScenarioStatus to Error ...\r\n");
                    }
                    catch (Exception ex)
                    {
                        richTextBoxStatus.AppendText("Error while updating MikeScenario ScenarioStatus to Running ...\r\n");
                        richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                        lblStatusWorking.Text = "Error ... ";
                        lblStatusUpdate.Text = "See details below ... ";
                        return;
                    }
                }
            }
            else
            {
                timerAutoRunMike.Enabled = true;
                lblStatusWorking.Text = "Waiting ... ";
                lblStatusUpdate.Text = "No scenario to run at this time ... ";
            }
        }
        private bool RunScenario(MikeScenario mikeScenario)
        {
            lblStatusUpdate.Text = "Downloading ... " + mikeScenario.ScenarioName;

            if (!DownloadInputFiles(mikeScenario))
            {
                return false;
            }

            richTextBoxStatus.AppendText("Preparing processMIKE to be run ...\r\n");

            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.Arguments = " \"" + CurrentFile + "\" " + "-x";
            if (radioButtonRunMzLaunchHidden.Checked)
            {
                pInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            else if (radioButtonRunMzLaunchNormal.Checked)
            {
                pInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
            else // default to Minimized
            {
                pInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            pInfo.UseShellExecute = true;
            processMike.StartInfo = pInfo;
            bool ProcessWorking = false;
            richTextBoxStatus.AppendText("processMIKE will run with [" + CurrentFile + "] ...\r\n");
            try
            {
                pInfo.FileName = @"C:\Program Files (x86)\DHI\2011\bin\MzLaunch.exe";
                richTextBoxStatus.AppendText("Trying to run file [" + pInfo.FileName + "] ...\r\n");
                processMike.Start();
                ProcessWorking = true;
                richTextBoxStatus.AppendText("Process running ...\r\n");
            }
            catch (Exception ex)
            {
                richTextBoxStatus.AppendText("Process could not run. File not found ...\r\n");
                richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                // don't return here we need to try another file name
            }
            if (!ProcessWorking)
            {
                try
                {
                    pInfo.FileName = @"C:\Program Files\DHI\2011\bin\MzLaunch.exe";
                    richTextBoxStatus.AppendText("Trying to run file [" + pInfo.FileName + "] ...\r\n");
                    processMike.Start();
                    ProcessWorking = true;
                    richTextBoxStatus.AppendText("Process running ...\r\n");
                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Process could not run. File not found ...\r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                    lblStatusWorking.Text = "Error ... ";
                    lblStatusUpdate.Text = "See details below ... ";
                    return false;
                }
            }

            processMike.WaitForInputIdle(2000);

            lblStatusWorking.Text = "Working ... ";
            lblStatusUpdate.Text = "Doing ... " + CurrentFile;

            richTextBoxStatus.AppendText(string.Format("processMIKE started at [{0:yyyy-MM-dd HH:mm:ss tt}] ...\r\n", DateTime.Now));

            //        MegaDoEvents();
            //        MegaDoEvents();
            //        MegaDoEvents();

            //        IntPtr hWndDesktop = APIFunc.GetDesktopWindow();
            //        DesktopChildrenWindowsList.Clear();
            //        FillDesktopWindowsChildrenList(true);

            //        string ShortFileName = CurrentFile.Substring(CurrentFile.LastIndexOf("\\") + 1);
            //        WndHandleAndTitle wht = DesktopChildrenWindowsList.Where(u => u.Title == "MzLaunch - [" + ShortFileName + "]").FirstOrDefault();
            //        while (wht != null)
            //        {
            //            IntPtr hWndParentOfTimeRemaining = af.APIGetWindow(
            //af.APIGetWindow(
            //af.APIGetWindow(wht.Handle, GW_CHILD),
            //GW_CHILD),
            //GW_CHILD);
            //            DesktopChildrenWindowsList.Clear();
            //            List<WndHandleAndTitle> whtList = af.GetChildrenWindowsHandleAndTitle(hWndParentOfTimeRemaining);
            //}

            return true;
        }
        private bool DongleIsPlugged()
        {
            bool Mike21DonglePlugged = false;

            richTextBoxStatus.AppendText(@"Checking if MIKE 21 Dongle is plugged Dependent: ""VID_04B9&PID_0300""" + "\r\n");
            lblStatusUpdate.Text = "Looking for Dongle ...";

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_USBControllerDevice");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["Dependent"].ToString().Contains("VID_04B9&PID_0300"))
                    {
                        richTextBoxStatus.AppendText(string.Format("MIKE21 Dongle was found\r\nDependent: {0}\r\n", queryObj["Dependent"]));
                        Mike21DonglePlugged = true;
                        lblStatusUpdate.Text = "Dongle found ...";
                        break;
                    }
                }

                if (!Mike21DonglePlugged)
                {
                    richTextBoxStatus.AppendText("Could not find MIKE21 Dongle\r\n");
                    lblStatusUpdate.Text = "Dongle not found ...";
                    return false;
                }
            }
            catch (ManagementException ex)
            {
                richTextBoxStatus.AppendText("Error: while searching for MIKE21 Dongle.\r\n");
                richTextBoxStatus.AppendText("Error: Message = [" + ex.Message + "].\r\n");
                return false;
            }

            return true;
        }
        //private void MegaDoEvents()
        //{
        //    for (int i = 0; i < 20000; i++)
        //    {
        //        Application.DoEvents();
        //    }
        //}
        //private void FillDesktopWindowsChildrenList(bool ShowResults)
        //{
        //    IntPtr hWndDesktop = af.APIGetDesktopWindow();
        //    DesktopChildrenWindowsList.Clear();
        //    DesktopChildrenWindowsList = af.GetChildrenWindowsHandleAndTitle(hWndDesktop);

        //    if (ShowResults)
        //    {
        //        richTextBoxStatus.Text = "";
        //        richTextBoxStatus.Text = "DesktopWindow = [" + hWndDesktop + "]\r\n";
        //        richTextBoxStatus.AppendText("Handle count = [" + DesktopChildrenWindowsList.Count() + "\r\n");

        //        foreach (WndHandleAndTitle t in DesktopChildrenWindowsList)
        //        {
        //            richTextBoxStatus.AppendText("Handle = [" + t.Handle.ToString("X") + "] Window Title = [" + t.Title + "]\r\n");
        //        }
        //    }
        //}
        private void CheckIfProcessMikeStillRunning()
        {
            if (!processMike.HasExited)
            {
                richTextBoxStatus.AppendText(string.Format("processMIKE still running at [{0:yyyy-MM-dd hh:mm:ss tt}] ...\r\n", DateTime.Now));
            }
        }
        private void processMikeExited()
        {
            richTextBoxStatus.AppendText(string.Format("processMIKE ended at [{0:yyyy-MM-dd hh:mm:ss tt}] ...\r\n", DateTime.Now));
            timerMzLaunchStillRunning.Enabled = false;

            lblStatusWorking.Text = "Process ended ...";
            lblStatusUpdate.Text = string.Format("at [{0:yyyy-MM-dd hh:mm:ss tt}]", DateTime.Now);

            if (!CopyDfsuAndLogToDB())
            {
                lblStatusWorking.Text = "Error ...";
                lblStatusUpdate.Text = "";
                return;
            }


            // should be changing MikeScenario status
            MikeScenario mikeScenarioToChange = (from ms in vpse.MikeScenarios
                                                 where ms.MikeScenarioID == CurrentMikeScenario.MikeScenarioID
                                                 select ms).FirstOrDefault<MikeScenario>();

            if (mikeScenarioToChange == null)
            {
                richTextBoxStatus.AppendText("Could not find MikeScenario to update. Looking for MikeScenarioID = [" + CurrentMikeScenario.MikeScenarioID + "] ... \r\n");
                lblStatusWorking.Text = "Error ...";
                lblStatusUpdate.Text = "";
                return;
            }

            // should Check if the log file shows the normal completion line as the last line
            string LogFileName = CurrentFile.Substring(0, CurrentFile.LastIndexOf(".")) + ".log";
            MemoryStream mems = new MemoryStream();
            mems = fileToMemoryStream(LogFileName);

            richTextBoxStatus.AppendText("Parsing the log file [" + LogFileName + "]\r\n");
            if (!m21fmLog.StreamToM21fmLog(mems))
            {
                richTextBoxStatus.AppendText("Error while parsing the log file\r\n");
                mikeScenarioToChange.ScenarioStatus = ScenarioStatusType.Error.ToString();
            }

            if (m21fmLog.CompletionTxt == "Run cancelled by user")
            {
                richTextBoxStatus.AppendText("Run cancelled by user\r\n");
                mikeScenarioToChange.ScenarioStatus = ScenarioStatusType.Canceled.ToString();
            }

            if (m21fmLog.CompletionTxt == "Abnormal run completion")
            {
                richTextBoxStatus.AppendText("Abnormal run completion\r\n");
                mikeScenarioToChange.ScenarioStatus = ScenarioStatusType.Error.ToString();
            }

            if (m21fmLog.CompletionTxt == "Normal run completion")
            {
                richTextBoxStatus.AppendText("Normal run completion\r\n");
                mikeScenarioToChange.ScenarioStatus = ScenarioStatusType.Completed.ToString();
            }

            mikeScenarioToChange.ScenarioStartExecutionDateAndTime = m21fmLog.StartExecutionDate;
            mikeScenarioToChange.ScenarioExecutionTimeInMinutes = m21fmLog.TotalElapseTimeInSeconds / 60;

            try
            {
                richTextBoxStatus.AppendText("Updating mikeScenario ScenarioStatus to [" + mikeScenarioToChange.ScenarioStatus + "] ... \r\n");
                vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                richTextBoxStatus.AppendText("Updated mikeScenario ScenarioStatus to [" + mikeScenarioToChange.ScenarioStatus + "] ... \r\n");
            }
            catch (Exception ex)
            {
                richTextBoxStatus.AppendText("Error while updating mikeScenario ScenarioStatus to [" + mikeScenarioToChange.ScenarioStatus + "] ... \r\n");
                richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ... \r\n");
                lblStatusWorking.Text = "Error ...";
                lblStatusUpdate.Text = ex.Message;
                return;
            }

            lblStatusWorking.Text = "Done ... ";
            lblStatusUpdate.Text = " ";

            FileInfo fi = new FileInfo(CurrentFile);

            string AutoRunMikeLogFileName = string.Format("[{0}] AutoRunMike.log", mikeScenarioToChange.MikeScenarioID);
            string FilePath = fi.DirectoryName + "\\";

            CSSPFile csspFileExist = (from cf in vpse.CSSPFiles
                                      from msf in vpse.MikeScenarioFiles
                                      where cf.CSSPFileID == msf.CSSPFile.CSSPFileID
                                      && cf.FileType == ".log"
                                      && cf.FileOriginalPath == fi.DirectoryName + "\\"
                                      && cf.FileName == AutoRunMikeLogFileName
                                      && cf.Purpose == "Other"
                                      select cf).FirstOrDefault<CSSPFile>();

            if (csspFileExist == null)
            {
                CSSPFile csspFile = new CSSPFile();
                csspFile.CSSPGuid = Guid.NewGuid();
                csspFile.FileName = AutoRunMikeLogFileName;
                csspFile.FileOriginalPath = FilePath;
                csspFile.FileSize = richTextBoxStatus.Text.Length;
                csspFile.FileDescription = "";
                csspFile.FileCreatedDate = DateTime.Now;
                csspFile.FileType = ".log";
                csspFile.FileContent = Encoding.ASCII.GetBytes(richTextBoxStatus.Text);
                csspFile.Purpose = PurposeType.Other.ToString();

                try
                {
                    richTextBoxStatus.AppendText("Adding log file [" + AutoRunMikeLogFileName + "] of AutoRunMike to [" + mikeScenarioToChange.MikeScenarioID + "] ... \r\n");
                    vpse.AddToCSSPFiles(csspFile);
                    vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    richTextBoxStatus.AppendText("Added log file ... \r\n");
                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Error while adding log file ... \r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ... \r\n");
                    lblStatusWorking.Text = "Error ...";
                    lblStatusUpdate.Text = ex.Message;
                    return;
                }

                // linking the file to the scenario
                MikeScenarioFile mikeScenarioFile = new MikeScenarioFile();
                mikeScenarioFile.MikeScenario = mikeScenarioToChange;
                mikeScenarioFile.CSSPFile = csspFile;
                mikeScenarioFile.CSSPParentFile = csspFile;

                try
                {
                    richTextBoxStatus.AppendText("Linking log file [" + AutoRunMikeLogFileName + "] of AutoRunMike to [" + mikeScenarioToChange.MikeScenarioID + "] ... \r\n");
                    vpse.AddToMikeScenarioFiles(mikeScenarioFile);
                    vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    richTextBoxStatus.AppendText("Linked log file ... \r\n");
                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Error while linking file to scenario ... \r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ... \r\n");
                    lblStatusWorking.Text = "Error ...";
                    lblStatusUpdate.Text = ex.Message;
                    return;
                }
            }
            else
            {
                richTextBoxStatus.AppendText("Adding log file [" + AutoRunMikeLogFileName + "] of AutoRunMike to [" + mikeScenarioToChange.MikeScenarioID + "] ... \r\n");

                csspFileExist.FileSize = richTextBoxStatus.Text.Length;
                csspFileExist.FileCreatedDate = DateTime.Now;
                csspFileExist.FileContent = Encoding.ASCII.GetBytes(richTextBoxStatus.Text);

                try
                {
                    vpse.SaveChanges();
                    richTextBoxStatus.AppendText("Added log file ... \r\n"); // this will not be shown in the log file if successful
                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Error while adding log file ... \r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ... \r\n");
                    lblStatusWorking.Text = "Error ...";
                    lblStatusUpdate.Text = ex.Message;
                    return;
                }
            }
              
            richTextBoxStatus.Clear();

            timerAutoRunMike.Enabled = true;
            CheckForMoreReadyToRun();
        }
        private bool CopyDfsuAndLogToDB()
        {
            richTextBoxStatus.AppendText("For file [" + CurrentFile + "] ...\r\n");
            MemoryStream ms = new MemoryStream();
            ms = fileToMemoryStream(CurrentFile);
            if (!m21fm.StreamToM21fm(ms))
            {
                richTextBoxStatus.AppendText("Could not read file ...\r\n");
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            List<string> FileNameList = new List<string>();

            //__________________________________________________
            // uploading Result Files
            //__________________________________________________
            FileNameList.Clear();
            if (!GetAllResultFilesToUpload(CurrentFile, FileNameList, CurrentMikeScenario))
            {
                richTextBoxStatus.AppendText("Error while collecting all Result Files ...\r\n");
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            // will try to load the scenario associated log file
            FileNameList.Add(CurrentFile.Substring(0, CurrentFile.LastIndexOf(".")) + ".log");

            richTextBoxStatus.AppendText("Trying to save Result File(s) to DB: \r\n\r\n");
            Application.DoEvents();

            foreach (string f in FileNameList)
            {
                if (!AddNewScenarioResultAndLogFiles(f, CurrentMikeScenario))
                {
                    lblStatusWorking.Text = "Error ... ";
                    lblStatusUpdate.Text = "See details below ... ";
                    return false;
                }
            }
            Application.DoEvents();

            return true;
        }
        private bool AddNewScenarioResultAndLogFiles(string FileName, MikeScenario CurrentMikeScenario)
        {
            richTextBoxStatus.AppendText("\r\n\r\nFile [" + FileName + "]\r\n");

            FileInfo fi = new FileInfo(FileName);
            FileStream fs = fi.OpenRead();

            string FilePath = FileName.Substring(0, FileName.LastIndexOf("\\") + 1);
            string ShortFileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);

            Application.DoEvents();

            //Read all file bytes into an array from the specified file.
            int nBytes = (int)fi.Length;
            Byte[] ByteArray = new byte[nBytes];
            int nBytesRead = fs.Read(ByteArray, 0, nBytes);

            fs.Close();

            richTextBoxStatus.AppendText("Checking if file already in DB ...\r\n");
            Application.DoEvents();

            //string TheFileName = fi.FullName.Substring(fi.FullName.LastIndexOf("\\") + 1);
            CSSPFile csspFileExist = (from f in vpse.CSSPFiles
                                      where f.FileName == ShortFileName
                                      && f.FileOriginalPath == FilePath
                                      && f.FileType == fi.Extension
                                      select f).FirstOrDefault<CSSPFile>();

            if (csspFileExist != null)
            {
                // just replace the content of the file
                richTextBoxStatus.AppendText("File already exist in DB ...\r\n");
                richTextBoxStatus.AppendText("Updating the fileContent in DB ...\r\n");
                csspFileExist.FileContent = ByteArray;

                try
                {
                    vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    richTextBoxStatus.AppendText("fileContent updated ...\r\n");
                    return true;
                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Error while updating fileContent ...\r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                    lblStatusWorking.Text = "Error ... ";
                    lblStatusUpdate.Text = "See details below ... ";
                    return false;
                }
            }

            richTextBoxStatus.AppendText("File does not exist in DB ...\r\n");
            richTextBoxStatus.AppendText("Saving file in DB ...\r\n");
            Application.DoEvents();

            CSSPFile csspFile = new CSSPFile();
            csspFile.CSSPGuid = Guid.NewGuid();
            csspFile.FileName = ShortFileName;
            csspFile.FileOriginalPath = FilePath;
            csspFile.FileSize = fi.Length;
            csspFile.FileDescription = "";
            csspFile.FileCreatedDate = fi.CreationTime;
            csspFile.FileType = fi.Extension;
            csspFile.FileContent = ByteArray;
            if (fi.Extension.ToLower() == ".dfsu")
            {
                csspFile.Purpose = PurposeType.MikeResult.ToString();

                MemoryStream ms = new MemoryStream(ByteArray);

                Dfs dfs = new Dfs(Dfs.DFSType.DFSU, true);

                dfs.StreamToDfs(ms);

                csspFile.DataStartDate = dfs.DataStartDate;
                csspFile.DataEndDate = dfs.DataStartDate.AddSeconds(dfs.TimeSteps * dfs.XValueList.Count);
                csspFile.TimeStepsInSecond = dfs.TimeSteps;
                string ParamNameTxt = "";
                string ParamUnitTxt = "";
                foreach (Dfs.Parameter p in dfs.ParameterList)
                {
                    ParamUnitTxt += string.Format("[{0}]-", p.UnitCode.ToString());
                    ParamNameTxt += string.Format("[{0}]-", p.Description);
                }
                csspFile.ParameterNames = ParamNameTxt.Substring(0, ParamNameTxt.Length - 1);
                csspFile.ParameterUnits = ParamUnitTxt.Substring(0, ParamUnitTxt.Length - 1);
            }
            else if (fi.Extension.ToLower() == ".log")
            {
                csspFile.Purpose = PurposeType.MikeResult.ToString();
            }
            else
            {
                richTextBoxStatus.AppendText("File extension should only be .dfsu and .log. It is [" + fi.Extension + "] ...\r\n");
                return false;
            }

            try
            {
                vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                richTextBoxStatus.AppendText("CSSPFile saved in DB ...\r\n");
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not store file [" + FileName + "] in DB\r\n" + ex.Message + "\r\n");
                richTextBoxStatus.AppendText("Could not store file [" + FileName + "] in DB\r\n" + ex.Message + "\r\n");
                Application.DoEvents();
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            int msID = CurrentMikeScenario.MikeScenarioID;
            CurrentMikeScenario = (from ms in vpse.MikeScenarios
                                   where ms.MikeScenarioID == msID
                                   select ms).FirstOrDefault<MikeScenario>();

            if (CurrentMikeScenario == null)
            {
                richTextBoxStatus.AppendText("Could not find MikeScenario with MikeScenarioID = [" + msID + "] ...\r\n");
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            MikeScenarioFile NewMikeScenarioFile = new MikeScenarioFile();
            NewMikeScenarioFile.MikeScenario = CurrentMikeScenario;
            NewMikeScenarioFile.CSSPFile = csspFile;
            NewMikeScenarioFile.CSSPParentFile = csspFile;

            try
            {
                richTextBoxStatus.AppendText("Linking MikeScenario to CSSPFile using MikeScenarioFile ...\r\n");
                Application.DoEvents();
                vpse.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                richTextBoxStatus.AppendText("Linked MikeScenario to CSSPFile using MikeScenarioFile ...\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not link MikeScenario to CSSPFile\r\n" + ex.Message + "\r\n");
                richTextBoxStatus.AppendText("Could not link MikeScenario to CSSPFile\r\n" + ex.Message + "\r\n");
                Application.DoEvents();
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            return true;
        }
        private bool GetAllResultFilesToUpload(string m21fmFileName, List<string> FileNameList, MikeScenario NewMikeScenario)
        {
            try
            {
                AddFileToFileNameList(m21fmFileName, m21fm.femEngineHD.hydrodynamic_module.structures.turbines.output_file_name, FileNameList);
                if (m21fm.femEngineHD.hydrodynamic_module.outputs != null && m21fm.femEngineHD.hydrodynamic_module.outputs.output != null)
                {
                    foreach (KeyValuePair<string, M21fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT> kvp in m21fm.femEngineHD.hydrodynamic_module.outputs.output)
                    {
                        FileInfo fi = new FileInfo(m21fmFileName.Substring(0, m21fmFileName.LastIndexOf("\\") + 1)
                            + m21fm.system.ResultRootFolder.Substring(1, m21fm.system.ResultRootFolder.Length - 2)
                            + m21fmFileName.Substring(m21fmFileName.LastIndexOf("\\"))
                            + " - Result Files\\");
                        AddFileToFileNameList(fi.FullName, kvp.Value.file_name, FileNameList);
                        AddFileToFileNameList(fi.FullName, kvp.Value.input_file_name, FileNameList);
                    }
                }
                if (m21fm.femEngineHD.transport_module.outputs != null && m21fm.femEngineHD.transport_module.outputs.output != null)
                {
                    foreach (KeyValuePair<string, M21fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT> kvp in m21fm.femEngineHD.transport_module.outputs.output)
                    {
                        FileInfo fi = new FileInfo(m21fmFileName.Substring(0, m21fmFileName.LastIndexOf("\\") + 1)
                            + m21fm.system.ResultRootFolder.Substring(1, m21fm.system.ResultRootFolder.Length - 2)
                            + m21fmFileName.Substring(m21fmFileName.LastIndexOf("\\"))
                            + " - Result Files\\");
                        AddFileToFileNameList(fi.FullName, kvp.Value.file_name, FileNameList);
                        AddFileToFileNameList(fi.FullName, kvp.Value.input_file_name, FileNameList);
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBoxStatus.AppendText("Error while trying to get results file names from [" + m21fmFileName + "] ...\r\n");
                richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                lblStatusWorking.Text = "Error ... ";
                lblStatusUpdate.Text = "See details below ... ";
                return false;
            }

            return true;

        }
        private FileInfo AddFileToFileNameList(string m21fmFileName, string file_name, List<string> FileNameList)
        {
            string TheFile = "";
            if (file_name == null)
            {
                return null;
            }
            FileInfo fi = new FileInfo(m21fmFileName.Substring(0, m21fmFileName.LastIndexOf("\\") + 1));
            if (file_name.Length > 2)
            {
                TheFile = fi.FullName + file_name.Substring(1, file_name.Length - 2);
                fi = new FileInfo(TheFile);
                if (fi.Exists)
                {
                    if (!FileNameList.Contains(fi.FullName))
                    {
                        richTextBoxStatus.AppendText(string.Format("Collecting file [" + fi.FullName + "]\r\n"));
                        FileNameList.Add(fi.FullName);
                    }
                    return fi;
                }
                else
                {
                    richTextBoxStatus.AppendText(string.Format("------------- Could not find file [" + fi.FullName + "]. This can happen at times. It's not a major error.\r\n"));
                    return null;
                }
            }
            return null;
        }
        private bool DownloadInputFiles(MikeScenario mikeScenario)
        {
            richTextBoxStatus.AppendText("Trying to download input files from DB for Scenario [" + mikeScenario.MikeScenarioID + "] ...\r\n");
            Application.DoEvents();

            List<CSSPFile> csspFileList = (from cf in vpse.CSSPFiles
                                           from msf in vpse.MikeScenarioFiles
                                           where cf.CSSPFileID == msf.CSSPFile.CSSPFileID
                                           && msf.MikeScenarioID == mikeScenario.MikeScenarioID
                                           && (cf.Purpose == "Input" || cf.Purpose == "InputPol")
                                           select cf).ToList<CSSPFile>();

            foreach (CSSPFile cf in csspFileList)
            {
                FileInfo fi = new FileInfo(cf.FileOriginalPath + cf.FileName);

                if (fi.Extension == ".m21fm")
                {
                    CurrentFile = cf.FileOriginalPath + cf.FileName;
                    lblCurrentFile.Text = CurrentFile;
                }

                DirectoryInfo di = new DirectoryInfo(cf.FileOriginalPath);
                if (!di.Exists)
                {
                    richTextBoxStatus.AppendText("Required path does not exist. Creating path [" + cf.FileOriginalPath + "] ...\r\n");

                    di.Create();
                }

                richTextBoxStatus.AppendText("Doing file [" + cf.FileOriginalPath + cf.FileName + "] ...\r\n");

                if (fi.Exists)
                {
                    richTextBoxStatus.AppendText("File already exist ...\r\n");
                    // remove existing file before downloading new file from DB
                    try
                    {
                        richTextBoxStatus.AppendText("Trying to remove file ...\r\n");
                        fi.Delete();
                        richTextBoxStatus.AppendText("File removed ...\r\n");
                    }
                    catch (Exception ex)
                    {
                        richTextBoxStatus.AppendText("Error while trying to remove file ...\r\n");
                        richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                        lblStatusWorking.Text = "Error ... ";
                        lblStatusUpdate.Text = "See details below ... ";
                        return false;
                    }
                }

                try
                {
                    richTextBoxStatus.AppendText("Downloading file from DB ...\r\n");

                    FileStream fs = fi.Create();
                    BinaryWriter writer = new BinaryWriter(fs);
                    writer.Write(cf.FileContent);
                    writer.Close();
                    fs.Close();

                    richTextBoxStatus.AppendText("File downloaded ...\r\n\r\n");

                }
                catch (Exception ex)
                {
                    richTextBoxStatus.AppendText("Error while trying to download file from DB ...\r\n");
                    richTextBoxStatus.AppendText("Error message [" + ex.Message + "] ...\r\n");
                    lblStatusWorking.Text = "Error ... ";
                    lblStatusUpdate.Text = "See details below ... ";
                    return false;
                }
            }
            return true;
        }
        private void Start()
        {
            richTextBoxStatus.AppendText("AutoRunMike started ...\r\n");
            lblStatusWorking.Text = "Started ... ";
            lblStatusUpdate.Text = "No scenario to run at this time ... ";
            butStart.Enabled = false;
            butPause.Enabled = true;
            PleasePause = false;
            CheckForMoreReadyToRun();
        }
        private void Pause()
        {
            butStart.Enabled = true;
            butPause.Enabled = false;
            lblStatusWorking.Text = "Paused ... ";
            lblStatusUpdate.Text = "Press start ... ";
            richTextBoxStatus.AppendText("AutoRunMike Paused ...\r\n");
            PleasePause = true;
        }
        // Events
        private void AutoRunMike_Load(object sender, EventArgs e)
        {
            richTextBoxStatus.Clear();
            butStart.Enabled = true;
            butPause.Enabled = false;
            CreateNewM21FMWithEvents();
            CurrentMikeScenario = new MikeScenario();
            m21fmLog = new M21fmLog();
            lblStatusWorking.Text = "Paused ... ";
            lblStatusUpdate.Text = "Press start ... ";
            Start();
            CheckForMoreReadyToRun();
            //DesktopChildrenWindowsList = new List<WndHandleAndTitle>();
        }
        private void butStart_Click(object sender, EventArgs e)
        {
            Start();
        }
        private void butPause_Click(object sender, EventArgs e)
        {
            Pause();
        }
        private void dataGridViewScenarios_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            switch (e.Column.DataPropertyName)
            {
                case "MikeScenarioID":
                    e.Column.HeaderText = "ID";
                    break;
                case "ScenarioName":
                    e.Column.HeaderText = "Name";
                    e.Column.Width = 300;
                    break;
                case "ScenarioStartDateAndTime":
                    e.Column.HeaderText = "Start Time";
                    break;
                case "ScenarioEndDateAndTime":
                    e.Column.HeaderText = "End Time";
                    break;
                case "ScenarioStartExecutionDateAndTime":
                    e.Column.HeaderText = "Start execution at";
                    break;
                case "ScenarioExecutionTimeInMinutes":
                    e.Column.HeaderText = "Execution time (min)";
                    break;
                case "CSSPItemID":
                case "ScenarioStatus":
                case "LastModifiedDate":
                case "ModifiedByID":
                case "IsActive":
                case "CSSPItem":
                case "MikeParameters":
                case "MikeScenarioFiles":
                case "MikeSources":
                    e.Column.Visible = false;
                    break;
                default:
                    break;
            }

        }
        private void dataGridViewScenarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewScenarios.SelectedRows.Count > 0)
            {
                MikeScenario mikeScenario = (MikeScenario)dataGridViewScenarios.SelectedRows[0].DataBoundItem;
                if (dataGridViewScenarios.SelectedRows.Count == 1)
                {
                    // nothing yet
                }
                else
                {
                    // nothing yet
                }
            }
            else
            {
                // nothing yet
            }
        }
        private void timerMzLaunchStillRunning_Tick(object sender, EventArgs e)
        {
            CheckIfProcessMikeStillRunning();
        }
        private void processMike_Exited(object sender, EventArgs e)
        {
            processMikeExited();
        }
        private void timerAutoRunMike_Tick(object sender, EventArgs e)
        {
            CheckForMoreReadyToRun();
        }
        
    }
}
