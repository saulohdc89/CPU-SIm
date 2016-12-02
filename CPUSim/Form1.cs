﻿using System;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using CsvHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace CPUSim
{
    public partial class Form1 : Form
    {

        Source.CPU _cpu = new Source.CPU();
        DataSet ds = new DataSet();

        bool[] twoesp = new bool[256];
        public Form1()
        {
            InitializeComponent();
        }
        DataTable table = new DataTable();
        int IndexRow;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            dataGridView1.Rows[e.RowIndex].ReadOnly = true;
            IndexRow = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[IndexRow];
            textBox1.Text = row.Cells[2].Value.ToString();
        }



        private void Form1_Load_1(object sender, EventArgs e)
        {
            textBox1.MaxLength = 3;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;

            for (int i = 0; i <= 255; i++)
            {
                if (_cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.NOT || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.HLT || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.NOP)
                {
                    if (i != 0)
                        dataGridView1.Rows.Add(" ", i.ToString(), _cpu._words[i].ToString(), _cpu.Decode(_cpu._words[i]).ToString());

                    if (i == 0)
                        dataGridView1.Rows.Add("*", "0".ToString(), _cpu._words[0].ToString(), _cpu.Decode(_cpu._words[0]).ToString());



                    if (i != 255)
                        twoesp[i + 1] = false;
                }

                else if (_cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.JMP || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.AND || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.ADD || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.JN || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.JZ || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.LDA || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.OR || _cpu.Decode(_cpu._words[i]) == Source.CPU.opcode.STA)
                {
                    dataGridView1.Rows.Add(" ", i.ToString(), _cpu._words[i].ToString(), _cpu.Decode(_cpu._words[i]).ToString() + " " + _cpu._words[i + 1].ToString());

                    if (i != 255)
                        twoesp[i + 1] = true;
                }


            }
            dataGridView1.AllowUserToAddRows = false;
            //  dataGridView1.DataSource =
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //enter key is down
                DataGridViewRow newDataRow = dataGridView1.Rows[IndexRow];
                if (Int32.Parse(textBox1.Text) > 255)
                {
                    textBox1.Text = "255";
                }
                newDataRow.Cells[2].Value = textBox1.Text;
                int NextRow = IndexRow + 1;
                int PrevRow = IndexRow - 1;

                newDataRow.Cells[3].Value = _cpu.Decode(Byte.Parse(textBox1.Text));
                _cpu._words[IndexRow] = Byte.Parse(textBox1.Text);

                if ((!twoesp[IndexRow]) && (_cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.NOT || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.HLT || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.NOP))
                {
                    newDataRow.Cells[3].Value = _cpu.Decode(Byte.Parse(textBox1.Text));
                    if (twoesp[NextRow])
                    {
                        twoesp[NextRow] = false;
                        dataGridView1.Rows[NextRow].Cells[3].Value = "???";
                    }
                }
                else if ((!twoesp[IndexRow]) && (_cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.JMP || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.AND || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.ADD || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.JN || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.JZ || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.LDA || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.OR || _cpu.Decode(Byte.Parse(textBox1.Text)) == Source.CPU.opcode.STA))
                {

                    NextRow = IndexRow + 1;
                    newDataRow.Cells[3].Value = _cpu.Decode(Byte.Parse(textBox1.Text)) + " " +
                   dataGridView1.Rows[NextRow].Cells[2].Value;
                    dataGridView1.Rows[NextRow].Cells[3].Value = " ";
                    twoesp[NextRow] = true;
                }
                else if ((twoesp[IndexRow]))
                {
                    dataGridView1.Rows[PrevRow].Cells[3].Value = _cpu.Decode(_cpu._words[PrevRow]) + " " + dataGridView1.Rows[IndexRow].Cells[2].Value;
                    newDataRow.Cells[3].Value = " ";
                }
       

                if (IndexRow < dataGridView1.RowCount)
                {
                    dataGridView1.Rows[IndexRow].Selected = false;
                    dataGridView1.Rows[++IndexRow].Selected = true;
                }
                dataGridView1.Refresh();
                dataGridView1.Update();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            {
                string value = textBox1.Text;
                int number = 255;
                if (int.TryParse(textBox1.Text, out number))

                {
                    if (number < 0)
                    {
                        textBox1.Text = "0";
                    }
                    else if (number > 255)
                    {
                        textBox1.Text = "255";
                    }

                }
            }
        }






        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = " ";
            _cpu.Step();

            if ((int)_cpu.GetPC() < dataGridView1.RowCount)
            {

                dataGridView1.Rows[(int)_cpu.GetPC()].Selected = true;
            }
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = "*";
            textBox2.Text = (_cpu.GetPC().ToString());
            textBox3.Text = (_cpu.GetAC().ToString());
            textBox4.Text = (_cpu.GetMemoryAcesses().ToString());
            textBox5.Text = (_cpu.GetInstructionCount().ToString());
            _cpu.updateFLAGS();
            if (_cpu.GetN() == true)
            {
                textBox6.Text = "1";
            }
            else
            {
                textBox6.Text = "0";
            }
            if (_cpu.GetZ() == true)
            {
                textBox7.Text = "1";
            }
            else
            {
                textBox7.Text = "0";
            }
            for (int i = 0; i <= 255; i++)
            {
                
                    dataGridView1.Rows[i].Cells[2].Value = _cpu._words[i];
                

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = " ";
            _cpu.Run();
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = "*";
            if ((int)_cpu.GetPC() < dataGridView1.RowCount)
            {

                dataGridView1.Rows[(int)_cpu.GetPC()].Selected = true;
            }
            textBox2.Text = (_cpu.GetPC().ToString());
            textBox3.Text = (_cpu.GetAC().ToString());
            textBox4.Text = (_cpu.GetMemoryAcesses().ToString());
            textBox5.Text = (_cpu.GetInstructionCount().ToString());
            _cpu.updateFLAGS();
            if (_cpu.GetN() == true)
            {
                textBox6.Text = "1";
            }
            else
            {
                textBox6.Text = "0";
            }
            if (_cpu.GetZ() == true)
            {
                textBox7.Text = "1";
            }
            else
            {
                textBox7.Text = "0";
            }

            for (int i = 0; i <= 255; i++)
            {

                dataGridView1.Rows[i].Cells[2].Value = _cpu._words[i];


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
            string file = "instructions.neasm";
            using (BinaryWriter bw = new BinaryWriter(File.Open(file, FileMode.Create)))
            {
                bw.Write(dataGridView1.Columns.Count);
                bw.Write(dataGridView1.Rows.Count);
                foreach (DataGridViewRow dgvR in dataGridView1.Rows)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                    {
                        object val = dgvR.Cells[j].Value;
                        if (val == null)
                        {
                            bw.Write(false);
                            bw.Write(false);
                        }
                        else
                        {
                            bw.Write(true);
                            bw.Write(val.ToString());
                        }
                    }
                }
            }
            */
            /* SaveFileDialog saveFileDialog1 = new SaveFileDialog();
              if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                  using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew)) 
                  using (StreamWriter sw = new StreamWriter(s)) {
                      sw.Write(dataGridView1.Columns.Count);
                      sw.Write(dataGridView1.Rows.Count);
                      foreach (DataGridViewRow dgvR in dataGridView1.Rows)
                      {
                          for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                          {
                              object val = dgvR.Cells[j].Value;
                              if (val == null)
                              {
                                  sw.Write(false);
                                  sw.Write(false);
                              }
                              else
                              {
                                  sw.Write(true);
                                  sw.Write(val.ToString());
                              }
                          }
                      }
                  }


              }
              */
            /* SaveFileDialog file = new SaveFileDialog();
             DialogResult dialogResult = file.ShowDialog();
             if (dialogResult == DialogResult.OK)
             {
                 if (String.IsNullOrEmpty(file.FileName))
                 {
                     //Inform the user
                 }
                 string path = file.FileName;
                 FileInfo fi = new FileInfo(path);

                 // Open the stream for writing.
                 using (FileStream fs = fi.OpenWrite())
                 {
                     Byte[] info = Encoding.ASCII.GetBytes(richTextBox1.Text);

                     // Add some information to the file.
                     fs.Write(info, 0, info.Length);
                 }
                 */
            StreamWriter Write;
            SaveFileDialog Open = new SaveFileDialog();
            Open.Filter = ("Text Document|*.txt|All Files|*.*");
            Open.FilterIndex = 1;
            Open.RestoreDirectory = true;

            if (Open.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    Write = new StreamWriter(Open.FileName);
                    for (int I = 0; I < dataGridView1.RowCount; I++)
                    {
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[0].Value));
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[1].Value));
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[2].Value));
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[3].Value));
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[1].Value));
                        Write.WriteLine(Convert.ToString(dataGridView1.Rows[I].Cells[2].Value));
                    }
                    Write.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Convert.ToString(ex.Message));
                    return;
                }
            }
             
        }

        private void button4_Click(object sender, EventArgs e)
        {

            /*            dataGridView1.Rows.Clear();
                        string file = "instructions.neasm";
                        using (BinaryReader bw = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            int n = bw.ReadInt32();
                            int m = bw.ReadInt32();
                            for (int i = 0; i < m; ++i)
                            {
                                dataGridView1.Rows.Add();
                                for (int j = 0; j < n; ++j)
                                {
                                    if (bw.ReadBoolean())
                                    {
                                        dataGridView1.Rows[i].Cells[j].Value = bw.ReadString();
                                    }
                                    else bw.ReadBoolean();
                                }
                            }
                        }
                        */
            {
 
                OpenFileDialog Open = new OpenFileDialog();
                Open.Filter = "Text Document|*.txt|All Files|*.*";
                Open.FilterIndex = 1;
                Open.RestoreDirectory = true;
                if (Open.ShowDialog() == DialogResult.OK) {
                    dataGridView1.Rows.Clear();
                    try
                    {
//                        Open.ShowDialog();
                        
                        StreamReader Import = new StreamReader(Convert.ToString(Open.FileName));
                      //  int contador = -1;
                        while (Import.Peek() >= 0) {
                            dataGridView1.Rows.Add(Convert.ToString(Import.ReadLine()), Convert.ToString(Import.ReadLine()), Convert.ToString(Import.ReadLine()), Convert.ToString(Import.ReadLine()));
                            _cpu._words[Convert.ToInt32(Import.ReadLine())]  = Convert.ToByte(Import.ReadLine());
                        }
                           
                        
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Convert.ToString(ex.Message));
                        return;
                    }
                    for (int i = 0; i <= 255; i++)
                    {
                        twoesp[i] = false;
                        //    byte[] convert = ObjectToByteArray(dataGridView1.Rows[i].Cells[3].Value);
                        // _cpu._words = ObjectToByteArray(dataGridView1.Rows[i].Cells[3].Value);
                    //    Source.Dados.Instance.endereco(i, ObjectToByteArray(dataGridView1.Rows[i].Cells[3].Value));
                    string str = dataGridView1.Rows[i].Cells[3].Value.ToString();
                        int result;
                        if (int.TryParse(str, out result))
                        {
                            _cpu._words[i] = (byte)result;
                        } /*
                    //    int conversor = Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                      //  _cpu._words[i] = (byte)conversor;**/
                    }

                    _cpu.Stop();
                    for (int i = 0; i <= 255; i++)
                    {
                        if (i != 0)
                        {
                            dataGridView1.Rows[i].Cells[0].Value = " ";
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[0].Value = "*";
                        }

                    }

                    IndexRow = 0;
                    textBox2.Text = (_cpu.GetPC().ToString());
                    textBox3.Text = (_cpu.GetAC().ToString());
                    textBox4.Text = (_cpu.GetMemoryAcesses().ToString());
                    textBox5.Text = (_cpu.GetInstructionCount().ToString());
                    _cpu.updateFLAGS();

                }


            }
                 



        }
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = " ";
            _cpu.Stop();
            dataGridView1.Rows[_cpu.GetPC()].Cells[0].Value = "*";
            IndexRow = 0;
            _cpu.updateFLAGS();
            if (_cpu.GetN() == true)
            {
                textBox6.Text = "1";
            }
            else
            {
                textBox6.Text = "0";
            }
            if (_cpu.GetZ() == true)
            {
                textBox7.Text = "1";
            }
            else
            {
                textBox7.Text = "0";
            }
            textBox2.Text = (_cpu.GetPC().ToString());
            textBox3.Text = (_cpu.GetAC().ToString());
            textBox4.Text = (_cpu.GetMemoryAcesses().ToString());
            textBox5.Text = (_cpu.GetInstructionCount().ToString());

        }
    }
}

