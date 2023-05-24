using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace HTTP_Client
{
    public partial class Form1 : Form
    {
        static RichTextBox richTextBox;
        static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public Form1()
        {
            InitializeComponent();
            richTextBox = richTextBox2;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //1. HTTP POST로 JSON  데이터 전송
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(textBox1.Text);
                httpWebRequest.ContentType = "application/json";
                string Method = string.Empty;
                this.Invoke(new Action(delegate ()
                {
                    Method = comboBox1.Text;
                }));
                httpWebRequest.Method = Method;

                if ("POST".Equals(Method))
                {
                    var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                    streamWriter.Write(richTextBox1.Text);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                //2. Response 확인
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var tagInfos = JsonSerializer.Deserialize<List<TagInfo>>(streamReader.ReadToEnd());
                //var JsonString = JsonSerializer.Serialize(tagInfos, options);
                var JsonString = JsonSerializer.Serialize(tagInfos);

                WriteRichTextBoxText($"{JsonString}");
            }
            catch (Exception ex)
            {
                WriteRichTextBoxText($"{ex.ToString()}");
            }
        }

        public void WriteRichTextBoxText(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    richTextBox.Text = $"{text}\r\n";
                }));
            }
            else
            {
                richTextBox.Text = $"{text}\r\n";
            }
        }

        public class TagInfo
        {
            public string tagName { get; set; }
            public string timestamp { get; set; }
            public double? value { get; set; }
            public int lValue { get; set; }
            public string sValue { get; set; }
            public int quality { get; set; }
            public string sQuality { get; set; }
            public int historianQuality { get; set; }
            public string sHistorianQuality { get; set; }
        }
    }
}