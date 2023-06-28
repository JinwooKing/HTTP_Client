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
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,          // JSON 문자열 인코딩에 사용할 인코더를 설정합니다.
            WriteIndented = true,                                           // JSON을 들여쓰기하여 가독성을 높입니다.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,   // null 값이 있는 속성을 직렬화할 때 무시하는 기본 동작을 설정합니다.
            MaxDepth = 8,                                                   // 직렬화할 수 있는 개체의 최대 깊이를 설정합니다. 너무 깊은 개체는 예외를 발생시킬 수 있습니다.
            ReadCommentHandling = JsonCommentHandling.Skip,                 // JSON 데이터에서 주석을 처리하는 방법을 설정합니다. 여기서는 주석을 건너뜁니다.
            ReferenceHandler = ReferenceHandler.Preserve                    // 개체 참조를 유지하기 위해 참조 처리기를 설정합니다. 이렇게 하면 참조 루프가 발생할 수 있습니다.
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
