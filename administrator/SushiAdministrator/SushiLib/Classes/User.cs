using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.IO;

namespace SushiLib
{
    [Serializable]
    public class User
    {
        public int id;
        public string number_phone;
        public string login;
        public string password;
        public string first_name;
        public string last_name;
        public string passport;
        public int id_dolgnost;
        public int id_point_work;
        public int online;
        public User()
        {

        }
        private string CheckBool(int i)
        {
            if (i == 0)
            {
                return "нет";
            }
            else
            {
                return "да";
            }
        }
        public string GetInfo(List<Dolgnost> dolgnosti, List<Point_Work> points_work)
        {
            string dolgnost = "";
            for (int i = 0; i < dolgnosti.Count; i++)
            {
                if (dolgnosti[i].id == id_dolgnost)
                {
                    dolgnost = dolgnosti[i].name;
                }
            }
            string pointwork = "";
            for (int i = 0; i < points_work.Count; i++)
            {
                if (points_work[i].id == id_point_work)
                {
                    pointwork = points_work[i].name;
                }
            }
            try
            {
                return String.Format("\n" + "  Номер телефона: " + number_phone + "\n" + "  Логин: " + login + "\n" + "  Пароль: " + password + "\n" + "  Имя: " + first_name + "\n" + "  Фамилия: " + last_name + "\n" + "  Паспорт: " + passport + "\n" + "  Должность: " + dolgnost + "\n" + "  Точка работы: " + pointwork + "\n" + "  Онлайн: " + CheckBool(online) + "\n");
            }
            catch
            {
                return "пользователь не найден";
            }
        }
        public string GetInfoExcel(List<Dolgnost> dolgnosti, List<Point_Work> points_work)
        {
            string dolgnost = "";
            for (int i = 0; i < dolgnosti.Count; i++)
            {
                if (dolgnosti[i].id == id_dolgnost)
                {
                    dolgnost = dolgnosti[i].name;
                }
            }
            string pointwork = "";
            for (int i = 0; i < points_work.Count; i++)
            {
                if (points_work[i].id == id_point_work)
                {
                    pointwork = points_work[i].name;
                }
            }
            try
            {
                return String.Format(number_phone + "\t" + login + "\t" + password + "\t" + first_name + "\t" + last_name + "\t" + passport + "\t" + dolgnost + "\t" + pointwork);
            }
            catch
            {
                return "пользователь не найден";
            }
        }
    }
}
