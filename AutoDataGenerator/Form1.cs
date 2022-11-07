using GenerateRandomSensorData;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoDataGenerator
{
    public partial class Form1 : Form
    {

        // In the class

        private bool currentlyGenerating = false;
        static HttpClient httpClient = new HttpClient();
        Random rnd = new Random();

        public TagDataForGenerating[] TagDataForGen { get; set; }

        public string ComboBox1 { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            // Put the following code where you want to initialize the class
            // It can be the static constructor or a one-time initializer
            httpClient.BaseAddress = new Uri("http://localhost:8085/tunglabb/");

            this.TagDataForGen = await this.GetTags();
            this.checkedListBox1.Items.Clear();
            var tagNames = this.TagDataForGen.Select(x => x.Tag).ToArray();
            this.checkedListBox1.Items.AddRange(tagNames);

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            currentlyGenerating = !currentlyGenerating;
            if (currentlyGenerating)
            {
                buttonGenerate.Text = "Stop Generating";
            }
            else
            {
                buttonGenerate.Text = "Generate!";
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (currentlyGenerating && checkedListBox1.CheckedItems.Count > 0)
            {
                List<measurementRequest> request = new List<measurementRequest>();
                foreach (var selectedItem in checkedListBox1.CheckedItems)
                {
                    var selectedTag = this.TagDataForGen.FirstOrDefault(x => x.Tag == selectedItem.ToString());
                    if (selectedItem is null)
                    {
                        continue;
                    }
                    
                    request.Add(new measurementRequest
                    {
                        Tag = selectedItem.ToString(),
                        Measurement = rnd.Next((int)selectedTag.LowerRange, (int)selectedTag.UpperRange+1),
                    });
                }

                await httpClient.PostAsJsonAsync("AddMeasurements", request);
            }
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.timer1.Interval = Convert.ToInt32(numericUpDown1.Value * 1000);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async Task<TagDataForGenerating[]> GetTags()
        {
            var response = await httpClient.GetAsync("GetTagnames");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var tagnames = JsonSerializer.Deserialize<TagDataResponse[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = false});

            var result = tagnames.Select(x => new TagDataForGenerating(x)).ToArray();

            return result;
            //this.comboBox1.Items.AddRange(tagnames);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
            }
        }
    }
}
