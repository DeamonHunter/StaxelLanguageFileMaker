using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace LangFileMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Staxel.Launcher\\gamedata\\content\\mods";
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (Directory.Exists(textBox1.Text))
            {
                StreamWriter sw = CreateLangFile(textBox1.Text);
                progressBar1.Value = 0;

                string modDirectory = textBox1.Text;

                MessageLabelWithRefresh("Finding Files.");
                List<string> tileFiles = GetListOfFiles(modDirectory, "tile");
                List<string> itemFiles = GetListOfFiles(modDirectory, "item");
                List<string> accesoryFiles = GetListOfFiles(modDirectory, "accessory");
                List<string> emoteFiles = GetListOfFiles(modDirectory, "emote");
                progressBar1.Maximum = tileFiles.Count + itemFiles.Count + accesoryFiles.Count + emoteFiles.Count;

                try
                {
                    MessageLabelWithRefresh("Creating Tile localisations.");
                    if (tileFiles.Count > 0)
                    {
                        sw.WriteLine("//Tiles");
                        CreateTileLocalisation(sw, tileFiles);
                    }

                    MessageLabelWithRefresh("Creating Item localisations.");
                    if (itemFiles.Count > 0)
                    {
                        sw.WriteLine("\n\n//Items");
                        CreateTileLocalisation(sw, itemFiles);
                    }
                    MessageLabelWithRefresh("Creating accesory localisations.");
                    if (accesoryFiles.Count > 0)
                    {
                        sw.WriteLine("\n\n//Accessories");
                        CreateLocalisation(sw, accesoryFiles);
                    }

                    MessageLabelWithRefresh("Creating Emote localisations.");
                    if (emoteFiles.Count > 0) {
                        sw.WriteLine("\n\n//Emotes");
                        CreateTileLocalisation(sw, emoteFiles);
                    }
                }
                catch (ArgumentException except)
                {
                    MessageBox.Show(except.Message);
                }
                sw.Close();
                label2.Text = "Done.";
            }
            else
            {
                MessageBox.Show("Invalid folder was chose.");
            }
        }

        private void MessageLabelWithRefresh(string message)
        {
            label2.Text = message;
            label2.Refresh();
        }

        private List<string> GetListOfFiles(string modFolderPath, string fileType)
        {
            List<string> filePaths = new List<string>();
            filePaths.AddRange(Directory.GetFiles(modFolderPath, "*." + fileType));
            if (Directory.GetDirectories(modFolderPath) != null)
            {
                foreach (string subDirectory in Directory.GetDirectories(modFolderPath))
                {
                    filePaths.AddRange(GetListOfFiles(subDirectory, fileType));
                }
            }
            return filePaths;
        }

        private StreamWriter CreateLangFile(string modFolderPath)
        {
            StreamWriter sw;
            if (checkBox1.Checked)
            {
                sw = new StreamWriter(modFolderPath + "\\EN-GB.lang", false);
            }
            else
            {
                int i = 0;
                string filename = "\\EN-GB.lang";
                bool noDuplicateFile = true;
                do {
                    if(i == 0)
                    {
                        noDuplicateFile = File.Exists(modFolderPath + filename);
                    }
                    else
                    {
                        filename = "\\EN-GB (" + i + ").lang";
                        noDuplicateFile = File.Exists(modFolderPath + filename);
                    }
                    i++;
                } while (noDuplicateFile);
                sw = new StreamWriter(modFolderPath + filename, false);
            }

            sw.WriteLine("language.code=en-GB\nlanguage=English\n\n");
            return sw;
        }

        private void CreateTileLocalisation(StreamWriter sw, List<string> tileFiles)
        {
            foreach (string tileFile in tileFiles)
            {
                StreamReader sr = new StreamReader(tileFile);
                string jsontext = sr.ReadToEnd();
                sr.Close();

                dynamic itemArray = JsonConvert.DeserializeObject(jsontext);
                if (itemArray == null)
                {
                    throw new ArgumentException("Malform Json in the file:\n" + tileFile);
                }
                else if (itemArray.code == null)
                {
                    throw new ArgumentException("Json missing the property \"code\" in the file:\n" + tileFile);
                }
                sw.WriteLine(itemArray.code + ".name=");
                sw.WriteLine(itemArray.code + ".description=");
                progressBar1.PerformStep();
            }
        }

        private void CreateLocalisation(StreamWriter sw, List<string> tileFiles)
        {
            foreach (string tileFile in tileFiles)
            {
                StreamReader sr = new StreamReader(tileFile);
                string jsontext = sr.ReadToEnd();
                sr.Close();

                dynamic itemArray = JsonConvert.DeserializeObject(jsontext);
                if (itemArray == null)
                {
                    throw new ArgumentException("Malform Json in the file:\n" + tileFile);
                }
                else if (itemArray.code == "")
                {
                    throw new ArgumentException("Json missing the property \"code\" in the file:\n" + tileFile);
                }
                sw.WriteLine(itemArray.code + "=");
                progressBar1.PerformStep();
            }
        }
    }
}
