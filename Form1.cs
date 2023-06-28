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
        static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,          // JSON ���ڿ� ���ڵ��� ����� ���ڴ��� �����մϴ�.
            WriteIndented = true,                                           // JSON�� �鿩�����Ͽ� �������� ���Դϴ�.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,   // null ���� �ִ� �Ӽ��� ����ȭ�� �� �����ϴ� �⺻ ������ �����մϴ�.
            MaxDepth = 8,                                                   // ����ȭ�� �� �ִ� ��ü�� �ִ� ���̸� �����մϴ�. �ʹ� ���� ��ü�� ���ܸ� �߻���ų �� �ֽ��ϴ�.
            ReadCommentHandling = JsonCommentHandling.Skip,                 // JSON �����Ϳ��� �ּ��� ó���ϴ� ����� �����մϴ�. ���⼭�� �ּ��� �ǳʶݴϴ�.
            ReferenceHandler = ReferenceHandler.Preserve                    // ��ü ������ �����ϱ� ���� ���� ó���⸦ �����մϴ�. �̷��� �ϸ� ���� ������ �߻��� �� �ֽ��ϴ�.
        };

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            await HttpRequestAsync();
            button1.Enabled = true;
        }

        private async Task HttpRequestAsync()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                string method = comboBox1.Text;
                string url = textBox1.Text;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = method;

                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        await streamWriter.WriteAsync(richTextBox1.Text);
                    }
                }

                var response = (HttpWebResponse)await request.GetResponseAsync();
                var streamReader = new StreamReader(response.GetResponseStream());
                var tagInfos = JsonSerializer.Deserialize<List<TagInfo>>(await streamReader.ReadToEndAsync());
                var jsonString = JsonSerializer.Serialize(tagInfos, options);

                WriteRichTextBoxText($"{jsonString}");
            }
            catch (Exception ex)
            {
                WriteRichTextBoxText($"{ex.ToString()}");
            }
        }

        public void WriteRichTextBoxText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => richTextBox2.Text = $"{text}\r\n"));
            }
            else
            {
                richTextBox2.Text = $"{text}\r\n";
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
