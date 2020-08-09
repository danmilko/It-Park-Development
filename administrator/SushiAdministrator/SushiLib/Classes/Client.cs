using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiLib
{
    public class Client
    {
        public int id;
        public string first_name;
        public string last_name;
        public string number_phone;
        public int id_city;
        public int id_rayon;
        public int id_street;
        public int number_home;
        public int number_corp;
        public int number_build;
        public int number_flat;
        public int bd_day;
        public int bd_month;
        public string number_card;
        public int id_advpoint;

        public string GetInfo(List<City> cities, List<Rayon> rayons, List<Street> streets, List<AdvPoint> advpoints)
        {
            string city = "";
            string rayon = "";
            string street = "";
            string adv = "";
            for (int i = 0; i < cities.Count; i++)
            {
                if (cities[i].id == id_city)
                {
                    city = cities[i].name;
                    break;
                }
            }
            for (int i = 0; i < rayons.Count; i++)
            {
                if (rayons[i].id == id_rayon)
                {
                    rayon = rayons[i].name;
                    break;
                }
            }
            for (int i = 0; i < streets.Count; i++)
            {
                if (streets[i].id == id_street)
                {
                    street = streets[i].name;
                    break;
                }
            }
            for (int i = 0; i < advpoints.Count; i++)
            {
                if (advpoints[i].id == id_advpoint)
                {
                    adv = advpoints[i].name;
                    break;
                }
            }
            return "  Id: " + id.ToString() + "\n" + "  Имя: " + first_name + "\n" + "  Фамилия: " + last_name + "\n" + "  Номер телефона: " + number_phone + "\n"
                 + "  Город: " + city + "\n" + "  Район: " + rayon + "\n" + "  Улица: " + street + "\n" + "  Дом: " + number_home.ToString() + "\n" + "  Корпус: " + number_corp + "\n"
                 + "  Строение: " + number_build.ToString() + "\n" + "  Квартира: " + number_flat.ToString() + "\n" + "  Дата рождения: " + bd_day + "/" + bd_month + "\n"
                 + "  Номер карты: " + number_card + "\n" + "  Точка рекламы: " + adv;
        }
        public string GetInfoExcel(List<City> cities, List<Rayon> rayons, List<Street> streets, List<AdvPoint> advpoints)
        {
            string city = "";
            string rayon = "";
            string street = "";
            string adv = "";
            for (int i = 0; i < cities.Count; i++)
            {
                if (cities[i].id == id_city)
                {
                    city = cities[i].name;
                    break;
                }
            }
            for (int i = 0; i < rayons.Count; i++)
            {
                if (rayons[i].id == id_rayon)
                {
                    rayon = rayons[i].name;
                    break;
                }
            }
            for (int i = 0; i < streets.Count; i++)
            {
                if (streets[i].id == id_street)
                {
                    street = streets[i].name;
                    break;
                }
            }
            for (int i = 0; i < advpoints.Count; i++)
            {
                if (advpoints[i].id == id_advpoint)
                {
                    adv = advpoints[i].name;
                    break;
                }
            }
            return first_name + "\t" + last_name + "\t" + number_phone + "\t"
                 + city + "\t" + rayon + "\t" + street + "\t" + number_home.ToString() + "\t" + number_corp + "\t"
                 + number_build.ToString() + "\t" + number_flat.ToString() + "\t" + bd_day + "\t" + bd_month + "\t"
                 + number_card + "\t" + adv;
        }
    }
}
