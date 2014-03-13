using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
                int badRate = 115200;
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
            if (port1.IsOpen)
            {
                timer1.Interval = int.Parse(textBox25.Text);
                timer1.Enabled = true;
            }
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
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
        List<short> test1 = new List<short>();
        List<short> test2 = new List<short>();
        List<short> test3 = new List<short>();


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct boardRegisters
        {
            public float accel_x;
            public float accel_y;
            public float accel_z;
            public float accel_temp; 
            public short accel_x_raw;
            public short accel_y_raw;
            public short accel_z_raw;
            public short accel_temp_raw; 

            public float gyro_x;
            public float gyro_y;
            public float gyro_z;
            public float gyro_temp; 
            public short gyro_x_raw;
            public short gyro_y_raw;
            public short gyro_z_raw;
            public short gyro_temp_raw;
            //public short gyro_x_raw_avg; 


            public float mag_x;
            public float mag_y;
            public float mag_z; 
            public short mag_x_raw;
            public short mag_y_raw;
            public short mag_z_raw; 

            public float pitch;
            public float yaw;
            public float roll;  

        }


        //boardRegisters ByteArrayToNewStuff(byte[] bytes)
        //{
        //    GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        //    boardRegisters stuff = (boardRegisters)Marshal.PtrToStructure(
        //        handle.AddrOfPinnedObject(), typeof(boardRegisters));
        //    handle.Free();
        //    return stuff;
        //}

        boardRegisters fromBytes(byte[] arr)
        {
            boardRegisters str = new boardRegisters();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (boardRegisters)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
        //////////////////////////////////////


        private void timer1_Tick(object sender, EventArgs e)
        {
            //Interpret buffer
            if (RxBuffer.Count > 0)
            {
                if (RxBuffer.Count >= (RxBuffer[0]))
                {
                    int count = RxBuffer[0];
                    RxBuffer.RemoveAt(0);
                    boardRegisters regs = fromBytes(RxBuffer.ToArray());
                    RxBuffer.RemoveRange(0, count-1);

                    textBox12.Text = regs.roll.ToString();
                    textBox1.Text = regs.accel_x.ToString();
                    textBox2.Text = regs.accel_y.ToString();

                    textBox13.Text = regs.accel_x_raw.ToString();


                    //int count = RxBuffer[0];
                    //byte[] rxBuff = new byte[count - 1];
                    //for (int i = 1; i < count; i++)
                    //{
                    //    rxBuff[i - 1] = RxBuffer[i];
                    //}
                    //RxBuffer.RemoveRange(0, count);

                    //var floatBuff = new float[12 * sizeof(float)];
                    //Buffer.BlockCopy(rxBuff, 0, floatBuff, 0, floatBuff.Length);

                    //var shortBuff = new short[10 * sizeof(short)];
                    //Buffer.BlockCopy(rxBuff, floatBuff.Length, shortBuff, 0, shortBuff.Length);


                    ////*****Converted
                    ////accel
                    //textBox1.Text = floatBuff[0].ToString();
                    //textBox2.Text = floatBuff[1].ToString();
                    //textBox3.Text = floatBuff[2].ToString();
                    ////Gyro
                    //textBox4.Text = floatBuff[3].ToString();
                    //textBox5.Text = floatBuff[4].ToString();
                    //textBox6.Text = floatBuff[5].ToString();
                    ////Mag
                    //textBox7.Text = floatBuff[6].ToString();
                    //textBox8.Text = floatBuff[7].ToString();
                    //textBox9.Text = floatBuff[8].ToString();
                    ////Yaw pitch roll
                    //textBox10.Text = floatBuff[9].ToString();
                    //textBox11.Text = floatBuff[10].ToString();
                    //textBox12.Text = floatBuff[11].ToString();

                    //textBox22.Text = (floatBuff[9] * 180.0 / Math.PI).ToString("0.00");
                    //textBox23.Text = (floatBuff[10] * 180.0 / Math.PI).ToString("0.00");
                    //textBox24.Text = (floatBuff[11] * 180.0 / Math.PI).ToString("0.00");

                    ////*****Raw
                    ////accel
                    //test1.Add(shortBuff[3]);
                    //test2.Add(shortBuff[4]);
                    //test3.Add(shortBuff[5]);
                    //textBox13.Text = shortBuff[0].ToString();
                    //textBox14.Text = shortBuff[1].ToString();
                    //textBox15.Text = shortBuff[2].ToString();
                    ////gyro
                    //textBox16.Text = shortBuff[3].ToString();
                    //textBox17.Text = shortBuff[4].ToString();
                    //textBox18.Text = shortBuff[5].ToString();
                    ////mag
                    //textBox19.Text = shortBuff[6].ToString();
                    //textBox20.Text = shortBuff[7].ToString();
                    //textBox21.Text = shortBuff[8].ToString();

                    ////***Avg raw
                    //textBox32.Text = shortBuff[9].ToString();

                    //chart1.Series[0].Points.Add(shortBuff[3]); //gyro raw
                    //chart2.Series[0].Points.Add(shortBuff[9]); //gyro raw avg

                }
                
            }

            ////Send request
            //byte[] buffer = { 24, 0x02, 
            //                    //Accel
            //                    0x10, 0x11, 0x12, 0x13,
            //                    0x14, 0x15, 0x16, 0x17, 

            //                    //Gyro
            //                    0x20, 0x21, 0x22, 0x23,
            //                    0x24, 0x25, 0x26, 0x27, 
            //                    0x28, 

            //                    //mag
            //                    0x30, 0x31, 0x32, 
            //                    0x33, 0x34, 0x35,

            //                    //Roll yaw pitch
            //                    0x40, 0x41, 0x42,
            //                };

            List<byte> buffer2 = new List<byte> 
                               {0x10, 0x11, 0x12, 0x13,
                                0x14, 0x15, 0x16, 0x17, 

                                //Gyro
                                0x20, 0x21, 0x22, 0x23,
                                0x24, 0x25, 0x26, 0x27, 
                                0x28, 

                                //mag
                                0x30, 0x31, 0x32, 
                                0x33, 0x34, 0x35,

                                //Roll yaw pitch
                                0x40, 0x41, 0x42,
                            };
            buffer2.Insert(0, 0x02); //Read reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();



            ////Send request
            //byte[] buffer = { 24, 0x02, 
            //                    //Coverted Sensor data
            //                    0x10, 0x11, 0x12, 
            //                    0x20, 0x21, 0x22,
            //                    0x30, 0x31, 0x32,

            //                    //Roll yaw pitch
            //                    0x40, 0x41, 0x42,

            //                    //Raw sensor data
            //                    0x14, 0x15, 0x16, 
            //                    0x24, 0x25, 0x26,
            //                    0x33, 0x34, 0x35,
                                
            //                    //Average raw sensor data
            //                    0x28
            //                };
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

        private void button5_Click(object sender, EventArgs e)
        {
            float avg1 = 0;
            int total1 = 0;
            for (int i = 0; i < test1.Count; i++)
            {
                total1 += test1[i];
            }
            avg1 = (float)total1 / (float)test1.Count;

            float avg2 = 0;
            int total2 = 0;
            for (int i = 0; i < test2.Count; i++)
            {
                total2 += test2[i];
            }
            avg2 = (float)total2 / (float)test2.Count;

            float avg3 = 0;
            int total3 = 0;
            for (int i = 0; i < test3.Count; i++)
            {
                total3 += test3[i];
            }
            avg3 = (float)total3 / (float)test3.Count;


            textBox26.Text = avg1.ToString();
            textBox27.Text = avg2.ToString();
            textBox28.Text = avg3.ToString();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            test1.Clear();
            test2.Clear();
            test3.Clear();
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();

            //chart1.ChartAreas[0].AxisY.Maximum = 1000;
            //chart1.ChartAreas[0].AxisY.Minimum = -1000;

            //chart2.ChartAreas[0].AxisY.Maximum = 1000;
            //chart2.ChartAreas[0].AxisY.Minimum = -1000;
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
