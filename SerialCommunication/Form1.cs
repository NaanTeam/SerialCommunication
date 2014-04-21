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
            if (ports.Length > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            chart10.ChartAreas[0].AxisY.Maximum = 3300;
            chart10.ChartAreas[0].AxisY.Minimum = 3000;
            chart11.ChartAreas[0].AxisY.Maximum = 3300;
            chart11.ChartAreas[0].AxisY.Minimum = 3000;
            chart12.ChartAreas[0].AxisY.Maximum = 3300;
            chart12.ChartAreas[0].AxisY.Minimum = 3000;
            chart13.ChartAreas[0].AxisY.Maximum = 3300;
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

        private void button8_Click(object sender, EventArgs e)
        {
            List<byte> buffer2 = new List<byte> 
                            {
                                //Reset PID
                                0x84, 
                        };


            buffer2.Insert(0, 0x03); //Write reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();

            port1.Write(buffer, 0, buffer.Length);
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
            public float desired_pitch;
            public float desired_yaw;
            public float desired_roll;
            public float desired_throttle;

            public float motor1;
            public float motor2;
            public float motor3;
            public float motor4;

            public float v_batt;

            public float filter_RollPitch_P;
            public float filter_RollPitch_I;
            public float filter_Yaw_P;
            public float filter_Yaw_I;

            public float PID_Roll_P;
            public float PID_Roll_I;
            public float PID_Roll_D;
            public float PID_Pitch_P;
            public float PID_Pitch_I;
            public float PID_Pitch_D;
            public float PID_Yaw_P;
            public float PID_Yaw_I;
            public float PID_Yaw_D;

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

        boardRegisters CurrRegs = new boardRegisters();

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Interpret buffer
            if (RxBuffer.Count > 0)
            {
                if (RxBuffer.Count >= (RxBuffer[0]))
                {
                    int count = RxBuffer[0];
                    RxBuffer.RemoveAt(0);
                    CurrRegs = fromBytes(RxBuffer.GetRange(0, count - 1).ToArray());
                    RxBuffer.RemoveRange(0, count - 1);


                    double roll_ang = (CurrRegs.roll * 180.0 / Math.PI);
                    double pitch_ang = (CurrRegs.pitch * 180.0 / Math.PI);
                    double yaw_ang = (CurrRegs.yaw * 180.0 / Math.PI);

                    //textBox12.Text = regs.roll.ToString();
                    textBox1.Text = CurrRegs.accel_x.ToString();
                    textBox2.Text = CurrRegs.accel_y.ToString();
                    textBox3.Text = CurrRegs.accel_z.ToString();

                    textBox13.Text = CurrRegs.accel_x_raw.ToString();
                    textBox14.Text = CurrRegs.accel_y_raw.ToString();
                    textBox15.Text = CurrRegs.accel_z_raw.ToString();

                    textBox16.Text = CurrRegs.gyro_x_raw.ToString();
                    textBox17.Text = CurrRegs.gyro_y_raw.ToString();
                    textBox18.Text = CurrRegs.gyro_z_raw.ToString();

                    textBox32.Text = CurrRegs.gyro_x_raw_avg.ToString();
                    textBox33.Text = CurrRegs.gyro_y_raw_avg.ToString();
                    textBox34.Text = CurrRegs.gyro_z_raw_avg.ToString();



                    textBox29.Text = CurrRegs.accel_x_raw_avg.ToString();
                    textBox30.Text = CurrRegs.accel_y_raw_avg.ToString();
                    textBox31.Text = CurrRegs.accel_z_raw_avg.ToString();


                    textBox4.Text = CurrRegs.gyro_x.ToString();
                    textBox5.Text = CurrRegs.gyro_y.ToString();
                    textBox6.Text = CurrRegs.gyro_z.ToString();

                    textBox7.Text = CurrRegs.mag_x.ToString();
                    textBox8.Text = CurrRegs.mag_y.ToString();
                    textBox9.Text = CurrRegs.mag_z.ToString();



                    textBox12.Text = CurrRegs.roll.ToString();
                    textBox10.Text = CurrRegs.pitch.ToString();
                    textBox11.Text = CurrRegs.yaw.ToString();

                    //textBox24.Text = roll_ang.ToString();
                    //textBox22.Text = pitch_ang.ToString();
                    //textBox23.Text = yaw_ang.ToString();

                    textBox26.Text = CurrRegs.desired_roll.ToString();
                    textBox27.Text = CurrRegs.desired_pitch.ToString();
                    textBox28.Text = CurrRegs.desired_yaw.ToString();
                    textBox39.Text = CurrRegs.desired_throttle.ToString();

                    textBox35.Text = CurrRegs.motor1.ToString();
                    textBox36.Text = CurrRegs.motor2.ToString();
                    textBox37.Text = CurrRegs.motor3.ToString();
                    textBox38.Text = CurrRegs.motor4.ToString();

                    textBox41.Text = CurrRegs.v_batt.ToString();

                    //Charts updating
                    chart1.Series[0].Points.Add(CurrRegs.roll);
                    chart2.Series[0].Points.Add(CurrRegs.pitch);
                    chart3.Series[0].Points.Add(CurrRegs.yaw);

                    chart1.Series[1].Points.Add(CurrRegs.desired_roll);
                    chart2.Series[1].Points.Add(CurrRegs.desired_pitch);
                    chart3.Series[1].Points.Add(CurrRegs.desired_yaw);


                    data_acclX.Add(CurrRegs.gyro_x_raw);
                    data_acclY.Add(CurrRegs.gyro_y_raw);
                    data_acclZ.Add(CurrRegs.gyro_z_raw);

                    chart4.Series[0].Points.Add(CurrRegs.accel_x_raw);
                    chart7.Series[0].Points.Add(CurrRegs.accel_x_raw_avg);

                    chart5.Series[0].Points.Add(CurrRegs.accel_y_raw);
                    chart8.Series[0].Points.Add(CurrRegs.accel_y_raw_avg);

                    chart6.Series[0].Points.Add(CurrRegs.accel_z_raw);
                    chart9.Series[0].Points.Add(CurrRegs.accel_z_raw_avg);

                    chart10.Series[0].Points.Add(CurrRegs.motor1);
                    chart11.Series[0].Points.Add(CurrRegs.motor2);
                    chart12.Series[0].Points.Add(CurrRegs.motor3);
                    chart13.Series[0].Points.Add(CurrRegs.motor4);


                    //Quad copter model updating
                    quadcopterModel1.UpdateModel( CurrRegs.roll, CurrRegs.pitch, CurrRegs.yaw);
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

                                //Desired pitch, yaw, roll, throttle
                                0x44, 0x45, 0x46, 0x47,

                                //motor values
                                0x50, 0x51, 0x52, 0x53,

                                //voltage battery
                                0x60,

                                //Filter RollPitchP, RollPitchI, YawP, YawI
                                0x80, 0x81, 0x82, 0x83,

                                //MotorPID
                                //Roll PID, Pitch PID, Yaw PID
                                0x90, 0x91, 0x92,
                                0x93, 0x94, 0x95,
                                0x96, 0x97, 0x98
                            };

            buffer2.Insert(0, 0x02); //Read reg
            buffer2.Insert(buffer2.Count, 0xFF); //add End of Transmission byte
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


        private void btReadPID_Click(object sender, EventArgs e)
        {
            //Motor PID
            //Roll
            tbMotorRollP.Text = CurrRegs.PID_Roll_P.ToString("0.0######");
            tbMotorRollI.Text = CurrRegs.PID_Roll_I.ToString("0.0######");
            tbMotorRollD.Text = CurrRegs.PID_Roll_D.ToString("0.0######");
            //pitch
            tbMotorPitchP.Text = CurrRegs.PID_Pitch_P.ToString("0.0######");
            tbMotorPitchI.Text = CurrRegs.PID_Pitch_I.ToString("0.0######");
            tbMotorPitchD.Text = CurrRegs.PID_Pitch_D.ToString("0.0######");
            //yaw
            tbMotorYawP.Text = CurrRegs.PID_Yaw_P.ToString("0.0######");
            tbMotorYawI.Text = CurrRegs.PID_Yaw_I.ToString("0.0######");
            tbMotorYawD.Text = CurrRegs.PID_Yaw_D.ToString("0.0######");

            //Filter
            //Roll,Pitch
            tbFilterRollPitchP.Text = CurrRegs.filter_RollPitch_P.ToString("0.0######");
            tbFilterRollPitchI.Text = CurrRegs.filter_RollPitch_I.ToString("0.0######");
            //yaw
            tbFilterYawP.Text = CurrRegs.filter_Yaw_P.ToString("0.0######");
            tbFilterYawI.Text = CurrRegs.filter_Yaw_I.ToString("0.0######");
        }

        private void btWritePID_Click(object sender, EventArgs e)
        {
            //filter
            float rollpitch_kp = 0;
            float rollpitch_ki = 0;
            float yaw_kp = 0;
            float yaw_ki = 0;

            float.TryParse(tbFilterRollPitchP.Text, out rollpitch_kp);
            float.TryParse(tbFilterRollPitchI.Text, out rollpitch_ki);
            float.TryParse(tbFilterYawP.Text, out yaw_kp);
            float.TryParse(tbFilterYawI.Text, out yaw_ki);

            byte[] rp_kp = StructureToByteArray(rollpitch_kp);
            byte[] rp_ki = StructureToByteArray(rollpitch_ki);
            byte[] y_kp = StructureToByteArray(yaw_kp);
            byte[] y_ki = StructureToByteArray(yaw_ki);


            float mRollP = 0;
            float mRollI = 0;
            float mRollD = 0;
            float.TryParse(tbMotorRollP.Text, out mRollP);
            float.TryParse(tbMotorRollI.Text, out mRollI);
            float.TryParse(tbMotorRollD.Text, out mRollD);
            byte[] mRollP_b = StructureToByteArray(mRollP);
            byte[] mRollI_b = StructureToByteArray(mRollI);
            byte[] mRollD_b = StructureToByteArray(mRollD);

            float mPitchP = 0;
            float mPitchI = 0;
            float mPitchD = 0;
            float.TryParse(tbMotorPitchP.Text, out mPitchP);
            float.TryParse(tbMotorPitchI.Text, out mPitchI);
            float.TryParse(tbMotorPitchD.Text, out mPitchD);
            byte[] mPitchP_b = StructureToByteArray(mPitchP);
            byte[] mPitchI_b = StructureToByteArray(mPitchI);
            byte[] mPitchD_b = StructureToByteArray(mPitchD);


            float mYawP = 0;
            float mYawI = 0;
            float mYawD = 0;
            float.TryParse(tbMotorYawP.Text, out mYawP);
            float.TryParse(tbMotorYawI.Text, out mYawI);
            float.TryParse(tbMotorYawD.Text, out mYawD);
            byte[] mYawP_b = StructureToByteArray(mYawP);
            byte[] mYawI_b = StructureToByteArray(mYawI);
            byte[] mYawD_b = StructureToByteArray(mYawD);



            List<byte> buffer2 = new List<byte> 
                            {
                                //Filter
                                0x80, rp_kp[0], rp_kp[1], rp_kp[2], rp_kp[3], 
                                0x81, rp_ki[0], rp_ki[1], rp_ki[2], rp_ki[3], 
                                0x82, y_kp[0], y_kp[1], y_kp[2], y_kp[3], 
                                0x83, y_ki[0], y_ki[1], y_ki[2], y_ki[3],

                                //PID
                                0x90, mRollP_b[0], mRollP_b[1], mRollP_b[2], mRollP_b[3],
                                0x91, mRollI_b[0], mRollI_b[1], mRollI_b[2], mRollI_b[3], 
                                0x92, mRollD_b[0], mRollD_b[1], mRollD_b[2], mRollD_b[3], 

                                0x93, mPitchP_b[0], mPitchP_b[1], mPitchP_b[2], mPitchP_b[3], 
                                0x94, mPitchI_b[0], mPitchI_b[1], mPitchI_b[2], mPitchI_b[3], 
                                0x95, mPitchD_b[0], mPitchD_b[1], mPitchD_b[2], mPitchD_b[3], 

                                0x96, mYawP_b[0], mYawP_b[1], mYawP_b[2], mYawP_b[3], 
                                0x97, mYawI_b[0], mYawI_b[1], mYawI_b[2], mYawI_b[3], 
                                0x98, mYawD_b[0], mYawD_b[1], mYawD_b[2], mYawD_b[3], 
                        };


            buffer2.Insert(0, 0x03); //Write reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();

            port1.Write(buffer, 0, buffer.Length);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            List<byte> buffer2 = new List<byte> 
                            {
                                //Reset PID
                                0x99, 
                        };


            buffer2.Insert(0, 0x03); //Write reg
            buffer2.Insert(0, (byte)(buffer2.Count() + 1));
            byte[] buffer = buffer2.ToArray();

            port1.Write(buffer, 0, buffer.Length);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            quadcopterModel1.UpdateModel(-.3F, .2F, -.65F);
        }

    }
}
