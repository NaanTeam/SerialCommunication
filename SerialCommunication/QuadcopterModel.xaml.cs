using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;

namespace SerialCommunication
{
    /// <summary>
    /// Interaction logic for QuadcopterModel.xaml
    /// </summary>
    public partial class QuadcopterModel : UserControl
    {
        public QuadcopterModel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the quadcopter model based on roll pitch and yaw (in radians.)
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        public void UpdateModel(float roll, float pitch, float yaw)
        {
            //Credit to http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternion/index.htm
            // for the math


            double heading = -1 * yaw;
            //double heading = 0.0;
            double attitude = -1 * roll;
            double bank = pitch;

            double c1 = Math.Cos(heading / 2.0);
            double s1 = Math.Sin(heading / 2.0);
            double c2 = Math.Cos(attitude / 2.0);
            double s2 = Math.Sin(attitude / 2.0);
            double c3 = Math.Cos(bank / 2.0);
            double s3 = Math.Sin(bank / 2.0);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            double w = c1c2 * c3 - s1s2 * s3;
            double x = c1c2 * s3 + s1s2 * c3;
            double y = s1 * c2 * c3 + c1 * s2 * s3;
            double z = c1 * s2 * c3 - s1 * c2 * s3;


            Quaternion q = new Quaternion(x, y, z, w);
            this.angleRotation.Angle = q.Angle;
            this.angleRotation.Axis = q.Axis;


            this.Label_Roll.Content = "Roll: " + (roll * 180.0 / Math.PI).ToString("#0.00");
            this.Label_Pitch.Content = "Pitch: " + (pitch * 180.0 / Math.PI).ToString("#0.00");
            this.Label_Yaw.Content = "Yaw: " + (yaw * 180.0 / Math.PI).ToString("#0.00");
        }
    }
}
