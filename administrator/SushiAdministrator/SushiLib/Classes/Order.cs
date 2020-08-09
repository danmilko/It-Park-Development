using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiLib
{
    [Serializable]
    public class Order
    {
        public int id;
        //public int Id { get { return id; } }

        public int id_operator;
        public int id_povar;
        public int id_upakov;
        public int id_kurer;

        public int id_client;
        //public int Client { get { return id_client; } }

        public int id_point_work;
        //public int PointWork { get { return id_point_work; } }

        public int datetime_start;
        //public int DateTimeStart { get { return datetime_start; } }

        public int datetime_finish;
        //public int DateTimeFinish { get { return datetime_finish; } }

        public int duration;

        public int price;
        //public int Price { get { return price; } }

        public int skidka;
        //public int Skidka { get { return skidka; } }

        public int price_with_skidka;
        public int client_money;

        public int count_persons;
        //public int CountPersons { get { return count_persons; } }

        public string description;//1000симв
        //public string Description { get { return description; } }

        public string promocode;//20симв
        public int no_cash;
        public int operator_prodavec;
        public int povar_upakov;
        public int samovivoz;

        public int id_order_state;
        //public int OrderState { get { return id_order_state; } }

        public int on_work;
        public int send_fiscal_data;
        public Order()
        {
        }
    }
}
