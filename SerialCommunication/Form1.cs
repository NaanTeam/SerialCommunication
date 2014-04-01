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
using System.Windows.Media.Media3D;

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
            tabControl1.SelectedIndex = 1;
        }



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
        private void button7_Click(object sender, EventArgs e)
        {
            float rollpitch_kp = 0;
            float rollpitch_ki = 0;
            float yaw_kp = 0;
            float yaw_ki = 0;


            float.TryParse(textBox_rollpitchkp.Text, out rollpitch_kp);
            float.TryParse(textBox_rollpitchki.Text, out rollpitch_ki);
            float.TryParse(textBox_yawkp.Text, out yaw_kp);
            float.TryParse(textBox_yawki.Text, out yaw_ki);


            byte[] rp_kp = StructureToByteArray(rollpitch_kp);
            byte[] rp_ki = StructureToByteArray(rollpitch_ki);
            byte[] y_kp = StructureToByteArray(yaw_kp);
            byte[] y_ki = StructureToByteArray(yaw_ki);


            List<byte> buffer2 = new List<byte> 
                            {0x80, rp_kp[0], rp_kp[1], rp_kp[2], rp_kp[3], 
                                0x81, rp_ki[0], rp_ki[1], rp_ki[2], rp_ki[3], 
                                0x82, y_kp[0], y_kp[1], y_kp[2], y_kp[3], 
                                0x83, y_ki[0], y_ki[1], y_ki[2], y_ki[3]
                        };
            buffer2.Insert(0, 0x03); //Write reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();

            port1.Write(buffer, 0, buffer.Length);
        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart3.Series[0].Points.Clear();

            //chart1.ChartAreas[0].AxisY.Maximum = -1;
            //chart1.ChartAreas[0].AxisY.Minimum = -4;

            //chart2.ChartAreas[0].AxisY.Maximum = 6;
            //chart2.ChartAreas[0].AxisY.Minimum = 3;

            //chart3.ChartAreas[0].AxisY.Maximum = -60;
            //chart3.ChartAreas[0].AxisY.Minimum = -80;


            chart4.Series[0].Points.Clear();
            chart5.Series[0].Points.Clear();
            chart6.Series[0].Points.Clear();

            chart7.Series[0].Points.Clear();
            chart8.Series[0].Points.Clear();
            chart9.Series[0].Points.Clear();

            //chart4.ChartAreas[0].AxisY.Maximum = 610;
            //chart4.ChartAreas[0].AxisY.Minimum = 530;

            //chart7.ChartAreas[0].AxisY.Maximum = 610;
            //chart7.ChartAreas[0].AxisY.Minimum = 530;

            //chart3.ChartAreas[0].AxisY.Maximum = -60;
            //chart3.ChartAreas[0].AxisY.Minimum = -80;

        }




        /////////Communication Stuff//////////////////////////////////////////


        List<byte> RxBuffer = new List<byte>();
        SerialPort port1;
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
            public short accel_x_raw_avg;
            public short accel_y_raw_avg;
            public short accel_z_raw_avg;

            public float gyro_x;
            public float gyro_y;
            public float gyro_z;
            public float gyro_temp; 
            public short gyro_x_raw;
            public short gyro_y_raw;
            public short gyro_z_raw;
            public short gyro_temp_raw;
            public short gyro_x_raw_avg;
            public short gyro_y_raw_avg;
            public short gyro_z_raw_avg; 


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
        public static byte[] StructureToByteArray(object obj)
        {
            //creds not to me
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);

            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;

        }

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
                    RxBuffer.RemoveRange(0, count - 1);


                    double roll_ang = (regs.roll * 180.0 / Math.PI);
                    double pitch_ang = (regs.pitch * 180.0 / Math.PI);
                    double yaw_ang = (regs.yaw * 180.0 / Math.PI);

                    //textBox12.Text = regs.roll.ToString();
                    textBox1.Text = regs.accel_x.ToString();
                    textBox2.Text = regs.accel_y.ToString();
                    textBox3.Text = regs.accel_z.ToString();

                    textBox29.Text = regs.accel_x.ToString();
                    textBox30.Text = regs.accel_y.ToString();
                    textBox31.Text = regs.accel_z.ToString();


                    textBox4.Text = regs.gyro_x.ToString();
                    textBox5.Text = regs.gyro_y.ToString();
                    textBox6.Text = regs.gyro_z.ToString();

                    textBox7.Text = regs.mag_x.ToString();
                    textBox8.Text = regs.mag_y.ToString();
                    textBox9.Text = regs.mag_z.ToString();



                    textBox12.Text = regs.roll.ToString();
                    textBox10.Text = regs.pitch.ToString();
                    textBox11.Text = regs.yaw.ToString();

                    textBox24.Text = roll_ang.ToString();
                    textBox22.Text = pitch_ang.ToString();
                    textBox23.Text = yaw_ang.ToString();

                    //Charts updating
                    chart1.Series[0].Points.Add(roll_ang);
                    chart2.Series[0].Points.Add(pitch_ang);
                    chart3.Series[0].Points.Add(yaw_ang);


                    chart4.Series[0].Points.Add(regs.accel_x_raw);
                    chart7.Series[0].Points.Add(regs.accel_x_raw_avg);

                    chart5.Series[0].Points.Add(regs.accel_y_raw);
                    chart8.Series[0].Points.Add(regs.accel_y_raw_avg);

                    chart6.Series[0].Points.Add(regs.accel_z_raw);
                    chart9.Series[0].Points.Add(regs.accel_z_raw_avg);


                    //Quad copter model updating
                    quadcopterModel1.UpdateModel(regs.roll, regs.pitch, regs.yaw);
                }

            }






            List<byte> buffer2 = new List<byte> 
                               {0x10, 0x11, 0x12, 0x13,
                                0x14, 0x15, 0x16, 0x17,
                                0x18, 0x19, 0x1A,

                                //Gyro
                                0x20, 0x21, 0x22, 0x23,
                                0x24, 0x25, 0x26, 0x27, 
                                0x28, 0x29, 0x2A, 

                                //mag
                                0x30, 0x31, 0x32, 
                                0x33, 0x34, 0x35,

                                //Roll yaw pitch
                                0x40, 0x41, 0x42,
                            };
            buffer2.Insert(0, 0x02); //Read reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();

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





    }
}
