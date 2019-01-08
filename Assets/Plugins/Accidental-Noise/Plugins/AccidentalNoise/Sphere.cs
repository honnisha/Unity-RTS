using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class Sphere : ModuleBase
    {
        private double m_cx = 0;
        private double m_cy = 0;
        private double m_cz = 0;
        private double m_cw = 0;
        private double m_cu = 0;
        private double m_cv = 0;
        private double m_radius = 1;

        public Sphere(double radius, double x, double y, double z)
        {
            this.setRadius(radius);
            this.setCenterX(x);
            this.setCenterY(y);
            this.setCenterZ(z);

        }

        void setCenter(double cx,double cy,double cz,double cw,double cu,double cv)
        {
            m_cx=cx; m_cy=cy; m_cz=cz; m_cw=cw; m_cu=cu; m_cv=cv;
        }

        void setCenterX(double cx){m_cx=cx;}
        void setCenterY(double cy){m_cy=cy;}
        void setCenterZ(double cz){m_cz=cz;}
        void setCenterW(double cw){m_cw=cw;}
        void setCenterU(double cu){m_cu=cu;}
        void setCenterV(double cv){m_cv=cv;}
        void setRadius(double r)
        {
            m_radius = r;
        }



        public override double Get(double x, double y)
        {
            double dx = x - m_cx, dy = y - m_cy;
            double len = Math.Sqrt(dx * dx + dy * dy);
            double radius = m_radius;
            double i = (radius - len) / radius;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }
    }
}
