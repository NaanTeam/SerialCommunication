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
            chart10.ChartAreas[0].AxisY.Maximum = 3200;
            chart10.ChartAreas[0].AxisY.Minimum = 3000;
            chart11.ChartAreas[0].AxisY.Maximum = 3200;
            chart11.ChartAreas[0].AxisY.Minimum = 3000;
            chart12.ChartAreas[0].AxisY.Maximum = 3200;
            chart12.ChartAreas[0].AxisY.Minimum = 3000;
            chart13.ChartAreas[0].AxisY.Maximum = 3200;
            chart13.ChartAreas[0].AxisY.Minimum = 3000;
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

            data_acclX.Clear();
            data_acclY.Clear();
            data_acclZ.Clear();

            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart3.Series[0].Points.Clear();

            chart1.Series[1].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart3.Series[1].Points.Clear();

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

            chart10.Series[0].Points.Clear();
            chart11.Series[0].Points.Clear();
            chart12.Series[0].Points.Clear();
            chart13.Series[0].Points.Clear();



            //chart7.ChartAreas[0].AxisY.Maximum = 610;
            //chart7.ChartAreas[0].AxisY.Minimum = 530;

            //chart3.ChartAreas[0].AxisY.Maximum = -60;
            //chart3.ChartAreas[0].AxisY.Minimum = -80;

        }
        private void button5_Click(object sender, EventArgs e)
        {
            //Dumps data to txt file
            int a = 5;

            a = 5*a + 5 + 4;

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
            public float accel_x_raw_avg;
            public float accel_y_raw_avg;
            public float accel_z_raw_avg;

            public float gyro_x;
            public float gyro_y;
            public float gyro_z;
            public float gyro_temp;
            public short gyro_x_raw;
            public short gyro_y_raw;
            public short gyro_z_raw;
            public short gyro_temp_raw;
            public float gyro_x_raw_avg;
            public float gyro_y_raw_avg;
            public float gyro_z_raw_avg;


            public float mag_x;
            public float mag_y;
            public float mag_z;
            public short mag_x_raw;
            public short mag_y_raw;
            public short mag_z_raw;

            public float pitch;
            public float yaw;
            public float roll;
            public float scaled_yaw;
            public float desired_pitch;
            public float desired_yaw;
            public float desired_roll;
            public float desired_throttle;

            public float motor1;
            public float motor2;
            public float motor3;
            public float motor4;

            public float v_batt;

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

        List<short> data_acclX = new List<short>();
        List<short> data_acclY = new List<short>();
        List<short> data_acclZ = new List<short>();


        private void timer1_Tick(object sender, EventArgs e)
        {
            //Interpret buffer
            if (RxBuffer.Count > 0)
            {
                if (RxBuffer.Count >= (RxBuffer[0]))
                {
                    int count = RxBuffer[0];
                    RxBuffer.RemoveAt(0);
                    boardRegisters regs = fromBytes(RxBuffer.GetRange(0, count - 1).ToArray());
                    RxBuffer.RemoveRange(0, count - 1);


                    double roll_ang = (regs.roll * 180.0 / Math.PI);
                    double pitch_ang = (regs.pitch * 180.0 / Math.PI);
                    double yaw_ang = (regs.yaw * 180.0 / Math.PI);

                    //textBox12.Text = regs.roll.ToString();
                    textBox1.Text = regs.accel_x.ToString();
                    textBox2.Text = regs.accel_y.ToString();
                    textBox3.Text = regs.accel_z.ToString();

                    textBox13.Text = regs.accel_x_raw.ToString();
                    textBox14.Text = regs.accel_y_raw.ToString();
                    textBox15.Text = regs.accel_z_raw.ToString();

                    textBox29.Text = regs.accel_x_raw_avg.ToString();
                    textBox30.Text = regs.accel_y_raw_avg.ToString();
                    textBox31.Text = regs.accel_z_raw_avg.ToString();


                    textBox4.Text = regs.gyro_x.ToString();
                    textBox5.Text = regs.gyro_y.ToString();
                    textBox6.Text = regs.gyro_z.ToString();

                    textBox7.Text = regs.mag_x.ToString();
                    textBox8.Text = regs.mag_y.ToString();
                    textBox9.Text = regs.mag_z.ToString();



                    textBox12.Text = regs.roll.ToString();
                    textBox10.Text = regs.pitch.ToString();
                    textBox11.Text = regs.yaw.ToString();

                    //textBox24.Text = roll_ang.ToString();
                    //textBox22.Text = pitch_ang.ToString();
                    //textBox23.Text = yaw_ang.ToString();
                    textBox40.Text = regs.scaled_yaw.ToString();

                    textBox26.Text = regs.desired_roll.ToString();
                    textBox27.Text = regs.desired_pitch.ToString();
                    textBox28.Text = regs.desired_yaw.ToString();
                    textBox39.Text = regs.desired_throttle.ToString();

                    textBox35.Text = regs.motor1.ToString();
                    textBox36.Text = regs.motor2.ToString();
                    textBox37.Text = regs.motor3.ToString();
                    textBox38.Text = regs.motor4.ToString();

                    textBox41.Text = regs.v_batt.ToString();

                    //Charts updating
                    chart1.Series[0].Points.Add(regs.roll);
                    chart2.Series[0].Points.Add(regs.pitch);
                    chart3.Series[0].Points.Add(regs.yaw);

                    chart1.Series[1].Points.Add(regs.desired_roll);
                    chart2.Series[1].Points.Add(regs.desired_pitch);
                    chart3.Series[1].Points.Add(regs.desired_yaw);


                    data_acclX.Add(regs.gyro_x_raw);
                    data_acclY.Add(regs.gyro_y_raw);
                    data_acclZ.Add(regs.gyro_z_raw);

                    chart4.Series[0].Points.Add(regs.gyro_x_raw);
                    chart7.Series[0].Points.Add(regs.gyro_x_raw_avg);

                    chart5.Series[0].Points.Add(regs.gyro_y_raw);
                    chart8.Series[0].Points.Add(regs.gyro_y_raw_avg);

                    chart6.Series[0].Points.Add(regs.gyro_z_raw);
                    chart9.Series[0].Points.Add(regs.gyro_z_raw_avg);

                    chart10.Series[0].Points.Add(regs.motor1);
                    chart11.Series[0].Points.Add(regs.motor2);
                    chart12.Series[0].Points.Add(regs.motor3);
                    chart13.Series[0].Points.Add(regs.motor4);


                    //Quad copter model updating
                    quadcopterModel1.UpdateModel(-1* regs.roll, -1*regs.pitch, -1*regs.scaled_yaw);
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
                                //scaled yaw
                                0x43, 
                                //Desired pitch, yaw, roll, throttle
                                0x44, 0x45, 0x46, 0x47,

                                //motor values
                                0x50, 0x51, 0x52, 0x53,

                                //voltage battery
                                0x60
                            };
            buffer2.Insert(0, 0x02); //Read reg
            buffer2.Insert(buffer2.Count, 0xFF);
            buffer2.Insert(0, (byte)(buffer2.Count() + 1)); //Count



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
