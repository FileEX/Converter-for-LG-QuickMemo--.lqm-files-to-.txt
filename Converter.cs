using System;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using System.IO.Compression;

namespace LQMConverterToTXT
{
    public partial class Converter : Form
    {
        private ZipArchive zip;
        public Converter()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog() { Filter="LQM Files (*.lqm)|*.lqm|All files (*.*)|*.*", RestoreDirectory=true};

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath.Text = openFileDialog1.FileName;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog() { ShowNewFolderButton = true, RootFolder = Environment.SpecialFolder.Desktop };

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                savePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePath.Text) && Directory.Exists(savePath.Text))
            {
                try
                {
                    string fileContent = "";
                    zip = ZipFile.Open(openFileDialog1.FileName, ZipArchiveMode.Read);
                    ZipArchiveEntry data = zip.GetEntry("memoinfo.jlqm");

                    if (data != null)
                    {
                        var entryStream = data.Open();

                        StreamReader reader = new StreamReader(entryStream);
                        fileContent = reader.ReadToEnd();
                    }

                    JsonElement jsonData = JsonSerializer.Deserialize<JsonElement>(fileContent);
                    JsonElement ObjectList = jsonData.GetProperty("MemoObjectList");
                    string rawText = ObjectList[0].GetProperty("DescRaw").ToString();
                    
                    FileStream newFile = File.Create(savePath.Text + "\\" + Path.GetFileNameWithoutExtension(openFileDialog1.SafeFileName) + ".txt");

                    byte[] utf8Text = new System.Text.UTF8Encoding(true).GetBytes(rawText);
                    newFile.Write(utf8Text, 0, utf8Text.Length);

                    newFile.Flush();
                    newFile.Dispose();

                    MessageBox.Show("File has been converted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    zip.Dispose();
                    openFileDialog1.Dispose();
                    folderBrowserDialog1.Dispose();
                }
            } else
            {
                MessageBox.Show("The specified file path or saving path is invalid", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
