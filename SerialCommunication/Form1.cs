using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//REQUIRED
using System.IO.Ports;


namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            for (int i = 0; i < ports.Length; i++)
            {
                comboBox1.Items.Add(ports[i]);
            }
            comboBox1.SelectedIndex = 0;
        }

        List<byte> RxBuffer = new List<byte>();
        SerialPort port1;

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                string portName = comboBox1.Text;
                int badRate = 38400;
                Parity par = Parity.None;
                int dataBits = 8;
                StopBits stpbits = StopBits.One;

                port1 = new SerialPort(comboBox1.Text, badRate, par, dataBits, stpbits);
                port1.Open();

                port1.DataReceived += new SerialDataReceivedEventHandler(port1_DataReceived);
            }
            else
            {
                MessageBox.Show("Please select a Comm port. If none are listed, press refresh. If none appear" +
                    " then no COM ports are open.");
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 300;
            timer1.Enabled = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            for (int i = 0; i < ports.Length; i++)
            {
                comboBox1.Items.Add(ports[i]);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Interpret buffer
            if (RxBuffer.Count > 0)
            {
                if (RxBuffer.Count >= RxBuffer[0])
                {
                    RxBuffer.RemoveAt(0);
                    byte[] rxBuff = RxBuffer.ToArray();
                    RxBuffer.Clear();

                    var floatBuff = new float[rxBuff.Length / 4];
                    Buffer.BlockCopy(rxBuff, 0, floatBuff, 0, rxBuff.Length);
                    if (floatBuff.Length == 9)
                    {
                        textBox1.Text = floatBuff[0].ToString();
                        textBox2.Text = floatBuff[1].ToString();
                        textBox3.Text = floatBuff[2].ToString();
                        textBox4.Text = floatBuff[3].ToString();
                        textBox5.Text = floatBuff[4].ToString();
                        textBox6.Text = floatBuff[5].ToString();
                        textBox7.Text = floatBuff[6].ToString();
                        textBox8.Text = floatBuff[7].ToString();
                        textBox9.Text = floatBuff[8].ToString();

                        chart1.Series[0].Points.Add(floatBuff[0]);
                    }
                    else
                    {
                        textBox1.Text = "error";
                    }
                }
                
            }

            //Send request
            byte[] buffer = { 11, 0x02, 
                                0x10, 0x11, 0x12, 
                                0x20, 0x21, 0x22,
                                0x30, 0x31, 0x32};
            port1.Write(buffer, 0, buffer.Length);
        }


        private void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int c = port1.BytesToRead;
            for (int i = 0; i < c; i++)
            {
                RxBuffer.Add((byte)(port1.ReadByte()));
            }
        }



        /*      #define SERIALCOMM_START_TOKEN      0x01
         * 
                #define SERIALCOMM_READ_REGISTER    0x02
                #define SERIALCOMM_WRITE_REGISTER   0x03
                #define SERIALCOMM_END_TOKEN        0xFF


                #define SERIALCOMM_REGISTER_XAcceleration       0x10
                #define SERIALCOMM_REGISTER_YAcceleration       0x11
                #define SERIALCOMM_REGISTER_ZAcceleration       0x12
                #define SERIALCOMM_REGISTER_AcceloTemperature   0x13

                #define SERIALCOMM_REGISTER_XAngularRate        0x20
                #define SERIALCOMM_REGISTER_YAngularRate        0x21
                #define SERIALCOMM_REGISTER_ZAngularRate        0x22
                #define SERIALCOMM_REGISTER_GyroTemperature     0x23

                #define SERIALCOMM_REGISTER_XMagneticVector     0x30
                #define SERIALCOMM_REGISTER_YMagneticVector     0x31
                #define SERIALCOMM_REGISTER_ZMagneticVector     0x32*/

    }
}
