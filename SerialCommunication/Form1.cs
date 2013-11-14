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
            timer1.Interval = 100;
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


        int n = 0;
        private void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            double xAccel = double.Parse(port1.ReadLine());
            textBox2.Text = xAccel.ToString();

            n++;
            chart1.Series[0].Points.AddXY(n, xAccel);

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            //See SerialComm.h for magic number descriptions

            //<StartTransmission> <Read Register> <Register Address> <End Transmission>
            byte[] buffer = { 0x01, 0x02, 0x10, 0xFF };
            port1.Write(buffer, 0, buffer.Length);
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
