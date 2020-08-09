using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;
using SushiLib;
using System.Web.Script.Serialization;
using System.Collections;
using SushiLib.Classes;

namespace SushiAdministrator
{
    public class API
    {
        public const string EmptyJSON = "{}";
        const string ApiKey = "skdlfjhjw7834267bsbshudft673RBFDNB72R356VCSBX";
        const string Url = "http://nashisyshi.pluton-host.ru/api.php";
        private string ExecuteCommand(NameValueCollection parameters)
        {
            try
            {
                parameters["key"] = ApiKey;
                WebClient wc = new WebClient();
                byte[] temp = wc.UploadValues(Url, parameters);
                string responce = Encoding.UTF8.GetString(temp);
                wc.Dispose();
                return responce;
            }
            catch
            {
                return "";
            }
        }
        public string UsualFunction(string json, string command, string other)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters["command"] = command;
            parameters["parameters"] = json;
            parameters["other"] = other;
            string response = ExecuteCommand(parameters);
            return response;
        }
        public User Users_select_by_login_password(string Login, string Password)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { login = Login, password = Password });
            string response = UsualFunction(json, "users.select.by_login_password", "");
            if (response == EmptyJSON)
            {
                return null;
            }
            User u = serializer.Deserialize<User>(response);
            return u;
        }

        public List<Order> Orders_select_dts_to_dtf(int startTime, int finishTime)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { datetime_start = startTime, datetime_finish = finishTime });
            string response = UsualFunction(json, "orders.select.dts_to_dtf_orders", "");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(response);
            return orders;
        }
        public List<Log> Logs_select_dts_to_dtf(int startTime, int finishTime)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { datetime_start = startTime, datetime_finish = finishTime });
            string response = UsualFunction(json, "logs.select.dts_to_dtf_logs", "");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Log> logs = JsonConvert.DeserializeObject<List<Log>>(response);
            return logs;
        }

        //public ArrayList CommandString(string command, ArrayList args)
        //{
        //    switch (command)
        //    {
        //        case "orders.select.dts_to_dtf_orders":
        //            List<Order> o = Orders_select_dts_to_dtf((int)args[0], (int)args[1]);
        //            return new ArrayList(o);
        //        case "logs.select.dts_to_dtf_logs":
        //            List<Log> l = Logs_select_dts_to_dtf((int)args[0], (int)args[1]);
        //            return new ArrayList(l);
        //        default:
        //            return null;
        //    }
        //}
        public string User_start_work(int id_user, int id_point_work)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id_user, id_point_work = id_point_work });
            string response = UsualFunction(json, "users.update.start_work", "");
            return response;
            //Console.WriteLine(response);
        }
        public string User_finish_work(int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id_user.ToString();
            string response = UsualFunction(json, "users.update.finish_work", "");
            return response;
        }

        public string GetSprav(string command)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { });
            string response = UsualFunction(json, command, "");
            return response;
        }
        public List<Point_Work> Points_work_select_all()
        {
            string response = GetSprav("points_work.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Point_Work> temp = JsonConvert.DeserializeObject<List<Point_Work>>(response);
            return temp;
        }
        public List<User> Users_select_all()
        {
            string response = GetSprav("users.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<User> temp = JsonConvert.DeserializeObject<List<User>>(response);
            return temp;
        }
        public List<City> Cities_select_all()
        {
            string response = GetSprav("cities.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<City> temp = JsonConvert.DeserializeObject<List<City>>(response);
            return temp;
        }
        public List<Rayon> Rayons_select_all()
        {
            string response = GetSprav("rayons.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Rayon> temp = JsonConvert.DeserializeObject<List<Rayon>>(response);
            return temp;
        }
        public List<Street> Streets_select_all()
        {
            string response = GetSprav("streets.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Street> temp = JsonConvert.DeserializeObject<List<Street>>(response);
            return temp;
        }
        public List<AdvPoint> AdvPoints_select_all()
        {
            string response = GetSprav("advpoints.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<AdvPoint> temp = JsonConvert.DeserializeObject<List<AdvPoint>>(response);
            return temp;
        }
        public List<Client> Clients_select_all()
        {
            string response = GetSprav("clients.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Client> temp = JsonConvert.DeserializeObject<List<Client>>(response);
            return temp;
        }
        public List<Dolgnost> Dolgnosti_select_all()
        {
            string response = GetSprav("dolgnosti.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Dolgnost> temp = JsonConvert.DeserializeObject<List<Dolgnost>>(response);
            return temp;
        }
        public List<Product> Products_select_all()
        {
            string response = GetSprav("products.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Product> temp = JsonConvert.DeserializeObject<List<Product>>(response);
            return temp;
        }
        public List<Order_State> Order_states_select_all()
        {
            string response = GetSprav("order_states.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Order_State> temp = JsonConvert.DeserializeObject<List<Order_State>>(response);
            return temp;
        }
        public List<Log_Action> Log_actions_select_all()
        {
            string response = GetSprav("log_actions.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Log_Action> temp = JsonConvert.DeserializeObject<List<Log_Action>>(response);
            return temp;
        }
        public List<Order_Product> Order_products_select_all()
        {
            string response = GetSprav("order_products.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Order_Product> temp = JsonConvert.DeserializeObject<List<Order_Product>>(response);
            return temp;
        }
        public List<Kurer_Work> Kurers_work_select_all()
        {
            string response = GetSprav("kurers_work.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Kurer_Work> temp = JsonConvert.DeserializeObject<List<Kurer_Work>>(response);
            return temp;
        }
        public List<Upakov_Work> Upakovs_work_select_all()
        {
            string response = GetSprav("upakovs_work.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Upakov_Work> temp = JsonConvert.DeserializeObject<List<Upakov_Work>>(response);
            return temp;
        }
        public List<Povar_Work> Povars_work_select_all()
        {
            string response = GetSprav("povars_work.select.all");
            if (response == EmptyJSON)
            {
                return null;
            }
            //Console.WriteLine(response);
            List<Povar_Work> temp = JsonConvert.DeserializeObject<List<Povar_Work>>(response);
            return temp;
        }

        public string Points_work_insert(string name, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name });
            string response = UsualFunction(json, "points_work.insert.new_point_work", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Cities_insert(string name, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name });
            string response = UsualFunction(json, "cities.insert.new_city", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Rayons_insert(string name, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name });
            string response = UsualFunction(json, "rayons.insert.new_rayon", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Streets_insert(string name, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name });
            string response = UsualFunction(json, "streets.insert.new_street", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Advpoints_insert(string name, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name });
            string response = UsualFunction(json, "advpoints.insert.new_advpoint", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Products_insert(string name, string ingredients, int price, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { name = name, ingredients = ingredients, price = price });
            string response = UsualFunction(json, "products.insert.new_product", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }

        public string Cities_update_by_id(string name, int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name });
            string response = UsualFunction(json, "cities.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Rayons_update_by_id(string name, int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name });
            string response = UsualFunction(json, "rayons.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Streets_update_by_id(string name, int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name });
            string response = UsualFunction(json, "streets.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Points_work_update_by_id(string name, int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name });
            string response = UsualFunction(json, "points_work.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Advpoints_update_by_id(string name, int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name });
            string response = UsualFunction(json, "advpoints.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Products_update_by_id(string name, int id, string ingredients, int price, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, name = name, ingredients = ingredients, price = price });
            string response = UsualFunction(json, "products.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }

        public string Cities_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "cities.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Rayons_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "rayons.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Streets_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "streets.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Points_work_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "points_work.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Advpoints_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "advpoints.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Products_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "products.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }

        public string Users_insert_new(string number_phone, string login, string password, string first_name, string last_name, string passport, int id_dolgnost, int id_point_work, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { number_phone = number_phone, login = login, password = password, first_name = first_name, last_name = last_name, passport = passport, id_dolgnost = id_dolgnost, id_point_work = id_point_work, online = 0 });
            string response = UsualFunction(json, "users.insert.new_user", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Users_update_by_id(int id, string number_phone, string login, string password, string first_name, string last_name, string passport, int id_dolgnost, int id_point_work, int online, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, number_phone = number_phone, login = login, password = password, first_name = first_name, last_name = last_name, passport = passport, id_dolgnost = id_dolgnost, id_point_work = id_point_work, online = online });
            string response = UsualFunction(json, "users.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Users_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "users.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }

        public string Clients_insert_new(string first_name, string last_name, string number_phone, int id_city, int id_rayon, int id_street, int number_home, int number_corp, int number_build, int number_flat, int bd_day, int bd_month, string number_card, int id_advpoint, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { number_phone = number_phone, first_name = first_name, last_name = last_name, id_city = id_city, id_rayon = id_rayon, id_street = id_street, number_home = number_home, number_corp = number_corp, number_build = number_build, number_flat = number_flat, bd_day = bd_day, bd_month = bd_month, number_card = number_card, id_advpoint = id_advpoint });
            string response = UsualFunction(json, "clients.insert.new_client", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Clients_update_by_id(int id, string first_name, string last_name, string number_phone, int id_city, int id_rayon, int id_street, int number_home, int number_corp, int number_build, int number_flat, int bd_day, int bd_month, string number_card, int id_advpoint, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new { id = id, number_phone = number_phone, first_name = first_name, last_name = last_name, id_city = id_city, id_rayon = id_rayon, id_street = id_street, number_home = number_home, number_corp = number_corp, number_build = number_build, number_flat = number_flat, bd_day = bd_day, bd_month = bd_month, number_card = number_card, id_advpoint = id_advpoint });
            string response = UsualFunction(json, "clients.update.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
        public string Clients_delete_by_id(int id, int id_user)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = id.ToString();
            string response = UsualFunction(json, "clients.delete.by_id", id_user.ToString());
            Status temp = JsonConvert.DeserializeObject<Status>(response);
            return temp.status;
        }
    }
}
