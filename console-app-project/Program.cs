using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace UIAutomationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string SvnRootPath ="";
            string TestProjectPath ="";
            string SourceBranch ="";
            string Sources = "";

            try
            {
                if (args.Length > 0)
                {
                    SvnRootPath = args[0];
                    TestProjectPath = args[1];
                    SourceBranch = args[2];
                    Sources = args[3];

                    Logger.LogWriter(SvnRootPath, 2);
                    Logger.LogWriter(TestProjectPath, 2);
                    Logger.LogWriter(SourceBranch, 2);
                    Logger.LogWriter(Sources, 2);

                    Process process = new Process();
                    Console.InputEncoding = System.Text.Encoding.Default;
                    Console.WriteLine("-----Changing VM Scaling");

                    Process newProcess = new Process();
                    newProcess.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
                    newProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    newProcess.StartInfo.UseShellExecute = false;
                    newProcess.StartInfo.RedirectStandardInput = true;
                    process = newProcess;
                    process.Start();
                    using (StreamWriter sw = process.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            sw.WriteLine("change-scaling.exe 100");
                        }
                    }

                    Console.WriteLine("Scaling Changed------");

                    string folderPath = @"C:\Workspace";
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    
                    if (!string.IsNullOrEmpty(Sources))
                    {
                        string[] sourceLocationDetails = Sources.Split(';');
                        foreach (string src in sourceLocationDetails)
                        {
                            string[] srcDetails = src.Split(',');
                            string repository = srcDetails[0];
                            string branch = srcDetails[1] == "null" ? SourceBranch : srcDetails[1];
                            string subfolderPath = @"C:\Workspace\" + repository;
                            if (!Directory.Exists(subfolderPath))
                            {
                                Directory.CreateDirectory(subfolderPath);
                            }
                            else
                            {
                                continue;
                            }

                            Console.WriteLine(repository + "------Cloning has be started");
                            string command = "git clone -b" + " " + branch + " " + "https://" + "SubashiniMahendran" + ":" + "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" + "@github.com/" + "essential-studio/" + repository + ".git";
                            System.Diagnostics.ProcessStartInfo sourceCloneProcStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                            sourceCloneProcStartInfo.RedirectStandardOutput = true;
                            sourceCloneProcStartInfo.UseShellExecute = false;
                            sourceCloneProcStartInfo.WorkingDirectory = "C:\\Workspace";

                            Process sourceCloneProcess = new Process();
                            sourceCloneProcess.StartInfo = sourceCloneProcStartInfo;
                            sourceCloneProcess.Start();
                            string sourceCloneResult = sourceCloneProcess.StandardOutput.ReadToEnd();
                            Console.WriteLine(repository + "------cloned");

                            string[] filePaths = Directory.GetFiles("C:\\Workspace\\" + repository, "*.csproj", SearchOption.AllDirectories) as string[];
                            string assemblyfolderPath = @"C:\Workspace\Assemblies";
                            if (!Directory.Exists(assemblyfolderPath))
                            {
                                Directory.CreateDirectory(assemblyfolderPath);
                            }

                            string targetFrameworkVersion = "";
                            string BuildVersion = null;
                            string Buildframeworkversion = null;
                            foreach (string sourceName in filePaths)
                            {
                                var lines = File.ReadAllLines(sourceName.ToString());
                                var linetext = File.ReadAllText(sourceName);
                                MatchCollection Frameworkversion = new Regex("<TargetFrameworkVersion>(.*?)</TargetFrameworkVersion>").Matches(linetext);
                                if (sourceName.Contains("_NET"))
                                {
                                    continue;
                                }

                                foreach (var version in Frameworkversion)
                                {
                                    targetFrameworkVersion = version.ToString();
                                }

                                if (targetFrameworkVersion == "<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>")
                                {
                                    BuildVersion = "3.5";
                                    Buildframeworkversion = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5";
                                }
                                else if (targetFrameworkVersion == "<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>")
                                {
                                    BuildVersion = "4.0";
                                    Buildframeworkversion = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";
                                }
                                else if (targetFrameworkVersion == "<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>")
                                {
                                    BuildVersion = "4.5";
                                    Buildframeworkversion = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";
                                }
                                else if (targetFrameworkVersion == "<TargetFrameworkVersion>v4.6</TargetFrameworkVersion>")
                                {
                                    BuildVersion = "4.6";
                                    Buildframeworkversion = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";
                                }

                                if (!string.IsNullOrEmpty(BuildVersion))
                                {
                                    string assemblyfolderPathnew = @"C:\Workspace\Assemblies\" + BuildVersion;
                                    if (!Directory.Exists(assemblyfolderPathnew))
                                    {
                                        Directory.CreateDirectory(assemblyfolderPathnew);
                                    }

                                    UpdateReferencePath(sourceName, BuildVersion);
                                    string arguments = null;
                                    if (BuildVersion == "3.5")
                                    {
                                        arguments = string.Format("/c MSBuild {0} /p:Configuration=Release-xml  /t:rebuild /p:OutDir={1}", sourceName, assemblyfolderPathnew + "\\");
                                    }
                                    else
                                    {
                                        arguments = string.Format("/c MSBuild {0} /p:Configuration=Release-xml  /t:rebuild /p:OutDir={1}", sourceName, assemblyfolderPathnew);
                                    }

                                    ProcessStartInfo info = new ProcessStartInfo("cmd.exe", arguments);
                                    info.WorkingDirectory = Buildframeworkversion;
                                    info.UseShellExecute = false;
                                    info.RedirectStandardOutput = true;
                                    Process process1 = Process.Start(info);
                                    process1.BeginOutputReadLine();
                                    process1.WaitForExit();
                                }
                            }
                        }
                    }

                    string svnSourcePath = @"D:\SVN";
                    if (!Directory.Exists(svnSourcePath))
                    {
                        Directory.CreateDirectory(svnSourcePath);
                    }

                    string testpath = TestProjectPath;
                    testpath = testpath.Replace("/", "\\");
                    string subfolderPathNew = @"D:\SVN\" + testpath;
                    if (!Directory.Exists(subfolderPathNew))
                    {
                        Directory.CreateDirectory(subfolderPathNew);
                        Console.WriteLine("-----TestProject checkout");
                        string command = string.Format("svn checkout" + " {0} {1} --username " + "subashini.mahendran" + " --password " + "subashini@123", SvnRootPath + "/" + TestProjectPath, subfolderPathNew);
                    System.Diagnostics.ProcessStartInfo sourceCloneProcStartInfo1 = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                        sourceCloneProcStartInfo1.RedirectStandardOutput = true;
                        sourceCloneProcStartInfo1.UseShellExecute = false;
                        sourceCloneProcStartInfo1.WorkingDirectory = "D:\\SVN";

                        Process sourceCloneProcess = new Process();
                        sourceCloneProcess.StartInfo = sourceCloneProcStartInfo1;
                        sourceCloneProcess.Start();
                        string sourceCloneResult = sourceCloneProcess.StandardOutput.ReadToEnd();
                        Console.WriteLine("------TestProject source cloned");
                    }

                    if (Directory.Exists(subfolderPathNew))
                    {
                        DirectoryInfo info = new DirectoryInfo(subfolderPathNew);
                        FileInfo[] files = info.GetFiles("*.sts");
                        DirectoryInfo directoryInfo = files.Count() > 0 ? new DirectoryInfo(Environment.CurrentDirectory + "\\TestStudio-OneStep") :
                                                                          new DirectoryInfo(Environment.CurrentDirectory + "\\TestComplete-OneStep");
                        if (files.Count() > 0)
                        {
                            CopyTestStudioFilesToVM(new DirectoryInfo(Environment.CurrentDirectory + "\\TeststudioApp"));
                        }

                        CopyFolder(directoryInfo, subfolderPathNew + "\\Tools");
                        Process process3 = new Process();
                        process3.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
                        process3.StartInfo.WorkingDirectory = subfolderPathNew + "\\Tools";
                        process3.StartInfo.UseShellExecute = false;
                        process3.StartInfo.RedirectStandardInput = true;
                        process = process3;
                        process.Start();
                        using (StreamWriter sw = process.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                sw.WriteLine("OneStep.exe");
                            }
                        }

                        process.WaitForExit();
                        process.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The catched exception is " + e.ToString());
            }

        }

        public static void CopyFolder(DirectoryInfo source, string targetpath)
        {
            DirectoryInfo targetPathInfo = new DirectoryInfo(targetpath);
            foreach (FileInfo file in source.GetFiles())
            {
                if (File.Exists(targetpath + "\\" + file.Name))
                {
                    File.Delete(targetpath + "\\" + file.Name);
                }

                file.CopyTo(Path.Combine(targetpath, file.Name));
            }
        }

        public static void CopyTestStudioFilesToVM(DirectoryInfo source)
        {
            string zipFolderPath = @"C:\TestStudioApp";
            if (!Directory.Exists(zipFolderPath))
            {
                Directory.CreateDirectory(zipFolderPath);
                foreach (FileInfo file in source.GetFiles())
                {
                    file.CopyTo(Path.Combine(zipFolderPath, file.Name));
                }
            }
        }

        private static void UpdateReferencePath(string csproject, string buildVersion)
        {
            XmlDocument document = new XmlDocument();
            document.Load(csproject);
            XmlNodeList projectnodes = document.GetElementsByTagName("HintPath");
            if (projectnodes != null && projectnodes.Count > 0)
            {
                for (int k = projectnodes.Count - 1; k >= 0; k--)
                {
                    if (projectnodes[k].ParentNode != null)
                        projectnodes[k].ParentNode.RemoveChild(projectnodes[k]);
                }
            }

            var childGroups = document.GetElementsByTagName("Project");
            var childGroup = childGroups.Item(childGroups.Count - 1);
            string referencePathLocation = "C:/Workspace" + "/Assemblies/" + buildVersion;
            foreach (var childNode in childGroup.ChildNodes)
            {
                if (childNode is XmlNode)
                {
                    var xmlChild = childNode as XmlNode;
                    var clone = xmlChild.Clone();
                    var xmlElement = xmlChild as XmlElement;
                    var itemGroup = document.GetElementsByTagName("Project").Item(0);
                    XmlNode importNode = itemGroup.OwnerDocument.ImportNode(clone, true);
                    if (xmlChild.Name == "ItemGroup")
                    {
                        foreach (var subchildNode in xmlElement.ChildNodes)
                        {
                            if (subchildNode is XmlNode)
                            {
                                var subxmlChild = subchildNode as XmlNode;
                                var subclone = subxmlChild.Clone();
                                var subxmlElement = subxmlChild as XmlElement;
                                if (subxmlElement != null && subxmlElement.Name == "Reference" && subxmlElement.HasAttribute("Include"))
                                {
                                    string referencename = subxmlElement.GetAttribute("Include");
                                    if (!string.IsNullOrEmpty(referencename) && (referencename.StartsWith("Syncfusion")))
                                    {
                                        XmlNodeList elemList = subxmlElement.OwnerDocument.GetElementsByTagName("HintPath");
                                        bool isContains = false;
                                        foreach (XmlElement node in elemList)
                                        {
                                            if (node.InnerXml.ToString().Contains(referencename + ".dll"))
                                            {
                                                isContains = true;
                                                break;
                                            }
                                        }
                                        if (!isContains)
                                        {
                                            var ns = "http://schemas.microsoft.com/developer/msbuild/2003";
                                            XmlElement hintElement = subxmlElement.OwnerDocument.CreateElement("HintPath", ns);
                                            hintElement.InnerText = ($"{referencePathLocation}/" + referencename + ".dll");
                                            subxmlElement.AppendChild(hintElement);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (xmlChild.Name == "PropertyGroup")
                    {
                        foreach (var subchildNode in xmlElement.ChildNodes)
                        {
                            if (subchildNode is XmlNode)
                            {
                                var subxmlChild = subchildNode as XmlNode;
                                var subxmlElement = subxmlChild as XmlElement;
                                if (subxmlElement != null && subxmlElement.Name == "PreBuildEvent")
                                {
                                    childGroup.RemoveChild(xmlChild);
                                }
                            }
                        }
                    }
                }
            }
            document.Save(csproject);
        }
    }

    public class Logger
    {
        private static StreamWriter stwriter;

        public static void FileDelete()
        {
            try
            {
                File.Delete(Environment.CurrentDirectory + "/BuildSuceess.txt");
                File.Delete(Environment.CurrentDirectory + "/BuildFailed.txt");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        public static string LogMessageFormat(string message, string stackTrace)
        {
            return ("Message : " + message + Environment.NewLine + "StackTrace : " + stackTrace + Environment.NewLine);
        }

        public static void LogWriter(string message, int buildStatus)
        {
            try
            {
                string str = string.Empty;
                if (buildStatus == 0)
                {
                    str = "/BuildSuceess.txt";
                }
                else if (buildStatus == 1)
                {
                    str = "/BuildFailed.txt";
                }
                else
                {
                    str = "/ApplicationLaunchErrorOneStep.txt";
                }

                if (File.Exists(Environment.CurrentDirectory + str))
                {
                    stwriter = File.AppendText(Environment.CurrentDirectory + str);
                    stwriter.WriteLine(message);
                    stwriter.Close();
                }
                else
                {
                    stwriter = File.CreateText(Environment.CurrentDirectory + str);
                    stwriter.WriteLine(message);
                    stwriter.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
