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
        }

        SerialPort port1;
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                string portName = comboBox1.Text;
                int badRate = 9600;
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
            //byte[] buffer = { (byte)'a', (byte)'b', (byte)'x', (byte)'e' };
            //port1.Write(buffer, 0, buffer.Length);

            timer1.Interval = 80;
            timer1.Enabled = true;

            //byte[] buffer = { 0x01, 0x02, 0x10, 0x11, 0x12, 0xFF };
            //byte[] buffer = { 0x01, 0x02, 0x10, 0x11, 0x12, 0x20, 0x21, 0x22, 0x30, 0x31, 0x32, 0xFF };
            //port1.Write(buffer, 0, buffer.Length);

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



        int reg = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] buffer = {0,0,0};

            switch (reg)
            {
                case 0:
                    buffer = new byte[] { 0x01, 0x02, 0x10, 0xFF };
                    break;
                case 1:
                    buffer = new byte[] { 0x01, 0x02, 0x11, 0xFF };
                    break;
                case 2:
                    buffer = new byte[] { 0x01, 0x02, 0x12, 0xFF };
                    break;
                case 3:
                    buffer = new byte[] { 0x01, 0x02, 0x20, 0xFF };
                    break;
                case 4:
                    buffer = new byte[] { 0x01, 0x02, 0x21, 0xFF };
                    break;
                case 5:
                    buffer = new byte[] { 0x01, 0x02, 0x22, 0xFF };
                    break;
                case 6:
                    buffer = new byte[] { 0x01, 0x02, 0x30, 0xFF };
                    break;
                case 7:
                    buffer = new byte[] { 0x01, 0x02, 0x31, 0xFF };
                    break;
                case 8:
                    buffer = new byte[] { 0x01, 0x02, 0x32, 0xFF };
                    break;
            }


            //breaks
            //byte[] buffer = { 0x01, 0x02, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x11, 0xFF };

            //works
            //byte[] buffer = { 0x01, 0x02, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x11, 0xFF };

            port1.Write(buffer, 0, buffer.Length);
        }

        int n = 0;
        private void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            double intrp = double.Parse(port1.ReadLine());
            reg++;
            switch (reg)
            {
                case 1:
                    textBox1.Text = intrp.ToString();
                    break;
                case 2:
                    textBox2.Text = intrp.ToString();
                    break;
                case 3:
                    textBox3.Text = intrp.ToString();
                    break;
                case 4:
                    textBox4.Text = intrp.ToString();
                    break;
                case 5:
                    textBox5.Text = intrp.ToString();
                    break;
                case 6:
                    textBox6.Text = intrp.ToString();
                    break;
                case 7:
                    textBox7.Text = intrp.ToString();
                    break;
                case 8:
                    textBox8.Text = intrp.ToString();
                    break;
                case 9:
                    textBox9.Text = intrp.ToString();
                    reg = 0;
                    break;
            }

            //n++;
            //chart1.Series[0].Points.AddXY(n, xAccel);

        }

        /*      #define SERIALCOMM_START_TOKEN      0x01
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
