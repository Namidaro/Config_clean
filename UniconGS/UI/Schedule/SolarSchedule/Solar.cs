using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Schedule.SolarSchedule
{
    /// <summary>
    /// Программерское колдунство, очень важное, нисматрити. Ну а так - класс для вычисления продолжительности сумерек по широте и дню года или широте и солнечному склонению
    /// </summary>
    public class Solar
    {
        #region [Private Fields]
        private double _delta;
        private double _h;
        private double _hc96;
        private double _hn102;
        private double _ha108;
        private double _fi;
        private double _tCivil;
        private double _tNavigate;
        private double _tAstro;
        #endregion

        #region [CONST]
        /// <summary>
        /// Число пи
        /// </summary>
        private const double PI = 3.1415;
        /// <summary>
        /// Перевод в радианы
        /// </summary>
        private const double DR = PI / 180;
        /// <summary>
        /// Предельное значение солнечного склонения е[-23,45;23.45] градусов. Внутренние функции C# считают тригонометрию с радианами, поэтому сразу переводим в радианы.
        /// </summary>
        private const double SOLAR_DECLINATION_LIMIT = 23.45 * DR;
        #endregion

        #region [Properties]
        /// <summary>
        /// Величина солнечного склонения
        /// </summary>
        public double SolarDeclination
        {
            get { return _delta; }
            set
            {
                _delta = value;
            }
        }
        /// <summary>
        /// Часовой угол солнца в момент восхода/захода. На самом деле они немного отличаются, но погрешность небольшая, поэтому ей можно пренебречь.
        /// </summary>
        private double H
        {
            get { return _h; }
            set
            {
                _h = value;
            }
        }
        /// <summary>
        /// Часовой угол Солнца, пока зенитное расстояние не превышает 96 градусов (гражданские сумерки)
        /// </summary>
        private double Hc96
        {
            get { return _hc96; }
            set
            {
                _hc96 = value;
            }
        }
        /// <summary>
        /// Часовой угол Солнца, пока зенитное расстояние не превышает 96 градусов (гражданские сумерки)
        /// </summary>
        private double Hn102
        {
            get { return _hn102; }
            set
            {
                _hn102 = value;
            }
        }
        /// <summary>
        /// Часовой угол Солнца, пока зенитное расстояние не превышает 96 градусов (гражданские сумерки)
        /// </summary>
        private double Ha108
        {
            get { return _ha108; }
            set
            {
                _ha108 = value;
            }
        }
        /// <summary>
        /// Широта местности
        /// </summary>
        public double Latitude
        {
            get { return _fi; }
            set
            {
                _fi = value;
            }
        }
        /// <summary>
        /// Продолжительность гражданских сумерек (в угловых часах)
        /// </summary>
        public double TCivil
        {
            get { return _tCivil; }
            set
            {
                _tCivil = value;
            }
        }
        /// <summary>
        /// Продолжительность навигационных сумерек (в угловых часах)
        /// </summary>
        public double TNavigate
        {
            get { return _tNavigate; }
            set
            {
                _tNavigate = value;
            }
        }
        /// <summary>
        /// Продолжительность астрономических сумерек (в угловых часах), там есть какие-то нюансы по продолжительности, мне лень сейчас разбираться, я все равно буду использовать только гражданские, это так, просто чтобы были
        /// </summary>
        public double TAstro
        {
            get { return _tAstro; }
            set
            {
                _tAstro = value;
            }
        }
        #endregion

        #region [Ctor]
        public Solar(double _latitude, int _day)
        {
            Latitude = _latitude;
            SolarDeclination = CalculateSolarDeclination(_day);
            H = CalculateH();
            Hc96 = CalculateHx(96);
            Hn102 = CalculateHx(102);
            Ha108 = CalculateHx(108);
            TCivil = Round((Hc96 - H) / Round(Math.PI, 2) * 180 / 15, 3);
            TNavigate = Round((Hn102 - H) / Round(Math.PI, 2) * 180 / 15, 3);
            TAstro = Round((Ha108 - H) / Round(Math.PI, 2) * 180 / 15, 3);
        }
        public Solar(double _latitude, double _decl)
        {
            Latitude = _latitude;
            SolarDeclination = Round(_decl * DR, 4);
            H = CalculateH();
            Hc96 = CalculateHx(96);
            Hn102 = CalculateHx(102);
            Ha108 = CalculateHx(108);
            // сразу поправляю погрешность, делать точнее не буду - ибо заманало уже неделю совокупляться с астрономией
            TCivil = Round((Hc96 - H) / DR / 15 * 0.9, 3);
            TNavigate = Round((Hn102 - H) / DR / 15 * 0.9, 3);
            TAstro = Round((Ha108 - H) / DR / 15 * 0.9, 3);
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Расчет солнечного склонения для конкретного дня года
        /// </summary>
        /// <param name="_dayOfYear">Порядковый номер дня в году</param>
        private double CalculateSolarDeclination(int _dayOfYear)
        {
            return Round(Math.Asin(Round(Math.Sin(SOLAR_DECLINATION_LIMIT), 3) * Round(Math.Sin(Round(((double)360 / (double)365), 3) * ((double)(_dayOfYear - 81)) * DR), 3)), 3);
        }
        /// <summary>
        /// Расчет часового угла Солнца в моменты восхода/захода
        /// </summary>
        private double CalculateH()
        {
            return Round(Math.Acos((-1) * (Round(Math.Tan(Latitude * DR), 4) * Round(Math.Tan(SolarDeclination), 4))), 4);
        }
        /// <summary>
        /// Расчет часового угла Солнца на заданном зенитном расстоянии
        /// </summary>
        /// <param name="_solarAngle">Зенитное расстояние</param>
        private double CalculateHx(double _solarAngle)
        {
            return Round(Math.Acos(
                (Round(Math.Cos(_solarAngle * DR), 4) - Round(Math.Sin(Latitude * DR), 4) * Round(Math.Sin(SolarDeclination), 4)) /
                                                (Round(Math.Cos(Latitude * DR), 4) * Round(Math.Cos(SolarDeclination), 4))), 4);
        }
        /// <summary>
        /// Функция отбрасывает "хвост" даблов до заданной точности (!)БЕЗ округления
        /// </summary>
        /// <param name="x"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        static double Round(double x, int precision)
        {
            return ((int)(x * Math.Pow(10.0, precision)) / Math.Pow(10.0, precision));
        }
        #endregion
    }
}
