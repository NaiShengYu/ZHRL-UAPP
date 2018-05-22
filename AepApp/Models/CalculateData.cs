using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.View
{
    public class CalculateData
    {
        public CalculateData()
        {
        }
        public double CalculatePM25(double aqiValue, double value)
        {
            double caValue = 0.0;
            int aqiH = 0;
            int aqiL = 0;
            int bpH = 0;
            int bpL = 0;
            if (aqiValue <= 50)
            {
                aqiH = 50;
                aqiL = 0;
            }
            else if (aqiValue <= 100)
            {
                aqiH = 100;
                aqiL = 50;
            }
            else if (aqiValue <= 150)
            {
                aqiH = 150;
                aqiL = 100;
            }
            else if (aqiValue <= 200)
            {
                aqiH = 200;
                aqiL = 150;
            }
            else if (aqiValue <= 300)
            {
                aqiH = 300;
                aqiL = 200;
            }
            else if (aqiValue <= 400)
            {
                aqiH = 400;
                aqiL = 300;
            }
            else
            {
                aqiH = 500;
                aqiL = 400;
            }
            if (value <= 35)
            {
                bpH = 35;
                bpL = 0;
            }
            else if (value <= 75)
            {
                bpH = 75;
                bpL = 35;
            }
            else if (value <= 115)
            {
                bpH = 115;
                bpL = 75;
            }
            else if (value <= 150)
            {
                bpH = 150;
                bpL = 115;
            }
            else if (value <= 250)
            {
                bpH = 250;
                bpL = 150;
            }
            else if (value <= 350)
            {
                bpH = 350;
                bpL = 250;
            }
            else
            {
                bpH = 500;
                bpL = 350;
            }
            caValue = (aqiH - aqiL) / (bpH - bpL) * (value - bpL) + aqiL;
            return caValue;
        }
        public double CalculatePM10(double aqiValue, double value)
        {
            double caValue = 0.0;
            int aqiH = 0;
            int aqiL = 0;
            int bpH = 0;
            int bpL = 0;
            if (aqiValue <= 50)
            {
                aqiH = 50;
                aqiL = 0;
            }
            else if (aqiValue <= 100)
            {
                aqiH = 100;
                aqiL = 50;
            }
            else if (aqiValue <= 150)
            {
                aqiH = 150;
                aqiL = 100;
            }
            else if (aqiValue <= 200)
            {
                aqiH = 200;
                aqiL = 150;
            }
            else if (aqiValue <= 300)
            {
                aqiH = 300;
                aqiL = 200;
            }
            else if (aqiValue <= 400)
            {
                aqiH = 400;
                aqiL = 300;
            }
            else
            {
                aqiH = 500;
                aqiL = 400;
            }
            if (value <= 50)
            {
                bpH = 50;
                bpL = 0;
            }
            else if (value <= 150)
            {
                bpH = 150;
                bpL = 50;
            }
            else if (value <= 250)
            {
                bpH = 250;
                bpL = 150;
            }
            else if (value <= 350)
            {
                bpH = 350;
                bpL = 250;
            }
            else if (value <= 420)
            {
                bpH = 420;
                bpL = 350;
            }
            else if (value <= 500)
            {
                bpH = 500;
                bpL = 420;
            }
            else
            {
                bpH = 600;
                bpL = 500;
            }
            caValue = (aqiH - aqiL) / (bpH - bpL) * (value - bpL) + aqiL;
            return caValue;
        }
        public double CalculateO3(double aqiValue, double value)
        {
            double caValue = 0.0;
            int aqiH = 0;
            int aqiL = 0;
            int bpH = 0;
            int bpL = 0;
            if (aqiValue <= 50)
            {
                aqiH = 50;
                aqiL = 0;
            }
            else if (aqiValue <= 100)
            {
                aqiH = 100;
                aqiL = 50;
            }
            else if (aqiValue <= 150)
            {
                aqiH = 150;
                aqiL = 100;
            }
            else if (aqiValue <= 200)
            {
                aqiH = 200;
                aqiL = 150;
            }
            else if (aqiValue <= 300)
            {
                aqiH = 300;
                aqiL = 200;
            }
            else if (aqiValue <= 400)
            {
                aqiH = 400;
                aqiL = 300;
            }
            else
            {
                aqiH = 500;
                aqiL = 400;
            }

            if (value <= 160)
            {
                bpH = 160;
                bpL = 0;
            }
            else if (value <= 200)
            {
                bpH = 200;
                bpL = 160;
            }
            else if (value <= 300)
            {
                bpH = 300;
                bpL = 200;
            }
            else if (value <= 400)
            {
                bpH = 400;
                bpL = 300;
            }
            else if (value <= 800)
            {
                bpH = 800;
                bpL = 400;
            }
            else if (value <= 1000)
            {
                bpH = 1000;
                bpL = 800;
            }
            else
            {
                bpH = 1200;
                bpL = 1000;
            }
            caValue = (aqiH - aqiL) / (bpH - bpL) * (value - bpL) + aqiL;
            return caValue;
        }
        public double CalculateCO(double aqiValue, double value)
        {
            double caValue = 0.0;
            int aqiH = 0;
            int aqiL = 0;
            int bpH = 0;
            int bpL = 0;
            if (aqiValue <= 50)
            {
                aqiH = 50;
                aqiL = 0;
            }
            else if (aqiValue <= 100)
            {
                aqiH = 100;
                aqiL = 50;
            }
            else if (aqiValue <= 150)
            {
                aqiH = 150;
                aqiL = 100;
            }
            else if (aqiValue <= 200)
            {
                aqiH = 200;
                aqiL = 150;
            }
            else if (aqiValue <= 300)
            {
                aqiH = 300;
                aqiL = 200;
            }
            else if (aqiValue <= 400)
            {
                aqiH = 400;
                aqiL = 300;
            }
            else
            {
                aqiH = 500;
                aqiL = 400;
            }

            if (value <= 2)
            {
                bpH = 2;
                bpL = 0;
            }
            else if (value <= 4)
            {
                bpH = 4;
                bpL = 2;
            }
            else if (value <= 14)
            {
                bpH = 14;
                bpL = 4;
            }
            else if (value <= 24)
            {
                bpH = 24;
                bpL = 14;
            }
            else if (value <= 36)
            {
                bpH = 36;
                bpL = 24;
            }
            else if (value <= 48)
            {
                bpH = 48;
                bpL = 36;
            }
            else
            {
                bpH = 60;
                bpL = 48;
            }
            caValue = (aqiH - aqiL) / (bpH - bpL) * (value - bpL) + aqiL;
            return caValue;
        }
    }
}
