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

        bool reverseIsHigh = false;

        CustomKeyValuePair[] onlyWantedTags = new CustomKeyValuePair[] 
        { 
            new CustomKeyValuePair
            {
                Tag = "DG2-VELOC",
                NormalValue = 1500,
                AbnormalValue = 1500
            },
            new CustomKeyValuePair
            {
                Tag = "G2-LOAD-KVAR",
                NormalValue = 86,
                AbnormalValue = 86
            },
            new CustomKeyValuePair
            {
                Tag = "G2-LOAD-KVA",
                NormalValue = 144,
                AbnormalValue = 144
            },
            new CustomKeyValuePair
            {
                Tag = "G2-LOAD",
                NormalValue = 115,
                AbnormalValue = 115
            },
            new CustomKeyValuePair
            {
                Tag = "G2-I-L1",
                NormalValue = 69,
                AbnormalValue = 69
            },
            new CustomKeyValuePair
            {
                Tag = "G2-I-L2",
                NormalValue = 69,
                AbnormalValue = 69
            },
            new CustomKeyValuePair
            {
                Tag = "G2-I-L3",
                NormalValue = 69,
                AbnormalValue = 69
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-U-L1",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-U-L2",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-U-L3",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-I-L1",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-I-L2",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-FREQ-I-L3",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "G2-COSPHI-L1",
                NormalValue = 0.8,
                AbnormalValue = 0.8
            },
            new CustomKeyValuePair
            {
                Tag = "G2-COSPHI-L2",
                NormalValue = 0.8,
                AbnormalValue = 0.8
            },
            new CustomKeyValuePair
            {
                Tag = "G2-COSPHI-L3",
                NormalValue = 0.8,
                AbnormalValue = 0.8
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-RUN",
                NormalValue = 1,
                AbnormalValue = 1,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-GEN-FRQ",
                NormalValue = 50,
                AbnormalValue = 50
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-CURR",
                NormalValue = 207,
                AbnormalValue = 207
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-COS-PHI",
                NormalValue = 0.8,
                AbnormalValue = 0.8
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-CONN",
                NormalValue = 1,
                AbnormalValue = 0,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-BUS-V",
                NormalValue = 400,
                AbnormalValue = 400,
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-LOLO-S",
                NormalValue = 0,
                AbnormalValue = 1,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-STOP-BK",
                NormalValue = 0,
                AbnormalValue = 1,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "DG2-LOC",
                NormalValue = 0,
                AbnormalValue = 1,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "G2-REV-PWR",
                NormalValue = 0,
                AbnormalValue = 1,
                SignalType = SignalType.Digital
            },
            new CustomKeyValuePair
            {
                Tag = "G2-U-L1/L2",
                NormalValue = 400,
                AbnormalValue = 400
            },
            new CustomKeyValuePair
            {
                Tag = "G2-U-L1/L3",
                NormalValue = 400,
                AbnormalValue = 400,
            },
            new CustomKeyValuePair
            {
                Tag = "G2-U-L2/L3",
                NormalValue = 400,
                AbnormalValue = 400,
            },
        };

        private string[] allTagnames;

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
            this.allTagnames = this.TagDataForGen.Select(x => x.Tag).Where(y => onlyWantedTags.Select(z => z.Tag).Contains(y)).ToArray();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            currentlyGenerating = !currentlyGenerating;
            if (currentlyGenerating)
            {
                buttonGenerate.Text = "STOP SIMULATING!";
            }
            else
            {
                buttonGenerate.Text = "SIMULATE!";
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (currentlyGenerating && this.allTagnames != null && this.allTagnames.Length > 1)
            {
                List<measurementRequest> request = new List<measurementRequest>();
                foreach (var selectedItem in this.allTagnames)
                {
                    if (selectedItem is null)
                    {
                        continue;
                    }

                    var selectedTag = this.onlyWantedTags.First(x => x.Tag == selectedItem);
                    var msrmnt = new measurementRequest { Tag = selectedItem, Measurement = selectedTag.NormalValue };
                    // Reverse power.
                    if (checkBox1.Checked && selectedItem == "G2-REV-PWR")
                    {
                        if (reverseIsHigh)
                        {
                            msrmnt.Measurement = selectedTag.NormalValue;
                            reverseIsHigh = false;
                        }
                        else
                        {
                            msrmnt.Measurement = selectedTag.AbnormalValue;
                            reverseIsHigh = true;
                        }
                    }

                    if (selectedTag.SignalType == SignalType.Analog)
                    {
                        double percentage = rnd.Next(0, 26);
                        if (percentage != 0)
                        {
                            var newMeasurement = 0.0;
                            if (percentage >= 10)
                            {
                                newMeasurement = msrmnt.Measurement*Double.Parse(("0.0"+percentage));
                            }
                            else
                            {
                                newMeasurement = msrmnt.Measurement*Double.Parse(("0.00"+percentage));
                            }

                            if (rnd.Next(0,2) == 0)
                            {
                                msrmnt.Measurement += newMeasurement;
                            }
                            else
                            {
                                msrmnt.Measurement -= newMeasurement;
                            }
                        }
                    }
                    request.Add(msrmnt);
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
