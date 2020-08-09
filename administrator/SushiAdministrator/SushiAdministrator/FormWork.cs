using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SushiLib;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SushiAdministrator
{
    public partial class FormWork : Form
    {
        #region Переменные и константы
        const string ApiKey = "skdlfjhjw7834267bsbshudft673RBFDNB72R356VCSBX";
        const string Url = "http://nashisyshi.pluton-host.ru/api.php";
        API api;
        List<Order> orders;
        List<Log> logs;
        List<Point_Work> pointworks;
        List<City> cities;
        List<Rayon> rayons;
        List<Street> streets;
        List<AdvPoint> advpoints;
        List<Client> clients;
        List<Dolgnost> dolgnosti;
        List<Product> products;
        List<User> users;
        List<Order_State> states;
        List<Log_Action> actions;
        List<Order_Product> order_products;
        List<Kurer_Work> kurer_works;
        List<Povar_Work> povar_works;
        List<Upakov_Work> upakov_works;

        List<string> downloadstrs = new List<string>() { "Загрузка points_work", "Загрузка cities", "Загрузка rayons", "Загрузка streets", 
            "Загрузка advpoints", "Загрузка clients", "Загрузка dolgnosti", "Загрузка products", "Загрузка users", "Загрузка order_states", 
            "Загрузка log_actions", "Загрузка order_products", "Загрузка kurer_work", "Загрузка povar_work", "Загрузка upakov_work" };
        User u;
        FormLogin fl;
        FormLoading fld;
        #endregion

        #region Общие функции
        private void Download_alldata()
        {
            fld = new FormLoading();
            fld.Show();
            fld.Activate();
            backgroundWorker1.RunWorkerAsync(fld);
        }
        private int GetUnixTime(DateTime time)
        {
            int unixTime = Convert.ToInt32((time - new System.DateTime(1970, 1, 1, 3, 0, 0, 0)).TotalSeconds);
            return unixTime;
        }
        private string ConvertFromUnixTime(int time)
        {
            if (time != -1)
            {
                string res = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time).AddHours(3).ToString();
                return res;
            }
            return "Не завершен";
        }
        private string GetTime(int time)
        {
            int hours = time / 3600;
            time = time % 3600;
            int minutes = time / 60;
            time = time % 60;
            return hours.ToString() + " ч " + minutes.ToString() + " м " + time.ToString() + " с";
        }

        private void ExportToExcel(DataGridView dataGridView1)
        {
            saveFileDialogExcel.DefaultExt = "xls";
            saveFileDialogExcel.Filter = "Excel files(*.xls) | *.xls";
            saveFileDialogExcel.Title = "Excel Export";

            if (saveFileDialogExcel.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(saveFileDialogExcel.FileName, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.Unicode);


                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    streamWriter.Write(column.HeaderText + "\t");
                }
                streamWriter.WriteLine();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //int index = int.Parse(row.Cells[0].Value.ToString());
                    foreach (DataGridViewCell obj in row.Cells)
                    {
                        string data = obj.Value == null ? "" : obj.Value.ToString();
                        streamWriter.Write(data + "\t");
                    }
                    streamWriter.WriteLine();
                }
                streamWriter.Flush();

                fileStream.Close();
                MessageBox.Show("Файл успешно экспортирован");
                //Process.Start(fileStream.Name);
            }
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
        private string CheckEmpty(string s)
        {
            if (s == string.Empty)
            {
                return "-1";
            }
            return s;
        }
        private string IsNull(string s)
        {
            if (s == "-1")
            {
                return "Неизвестно";
            }
            return s;
        }
        #endregion

        #region Функции Orders
        private Task<DataTable> DownloadDataOrders()
        {
            return Task.Run(() =>
            {
                DataTable dt = new DataTable();
                for (int i = 0; i < 12; i++)
                {
                    dt.Columns.Add(new DataColumn());
                }
                DateTime timestart = new DateTime(dateTimePickerOrderStart.Value.Year, dateTimePickerOrderStart.Value.Month, dateTimePickerOrderStart.Value.Day, 0, 0, 0);
                DateTime timefinish = new DateTime(dateTimePickerOrderFinish.Value.Year, dateTimePickerOrderFinish.Value.Month, dateTimePickerOrderFinish.Value.Day, 23, 59, 59);
                orders = api.Orders_select_dts_to_dtf(GetUnixTime(timestart), GetUnixTime(timefinish));
                clients = api.Clients_select_all();

                for (int i = 0; i < orders.Count; i++)
                {
                    string[] row = new string[dt.Columns.Count];
                    row[0] = orders[i].id.ToString();
                    row[1] = ConvertFromUnixTime(orders[i].datetime_start);
                    row[2] = ConvertFromUnixTime(orders[i].datetime_finish);
                    for (int j = 0; j < states.Count; j++)
                    {
                        if (orders[i].id_order_state == states[j].id)
                        {
                            row[3] = states[j].name;
                        }
                    }
                    row[4] = orders[i].on_work == 1 ? "Да" : "Нет";

                    for (int j = 0; j < pointworks.Count; j++)
                    {
                        if (pointworks[j].id == orders[i].id_point_work)
                        {
                            row[5] = pointworks[j].name;
                            break;
                        }
                    }
                    for (int j = 0; j < clients.Count; j++)
                    {
                        if (clients[j].id == orders[i].id_client)
                        {
                            row[6] = clients[j].number_phone.ToString();
                            break;
                        }
                    }
                    row[7] = orders[i].skidka.ToString();
                    row[8] = orders[i].price_with_skidka.ToString();
                    row[9] = orders[i].count_persons.ToString();
                    row[10] = orders[i].description;

                    for (int j = 0; j < users.Count; j++)
                    {
                        if (users[j].id == orders[i].id_kurer)
                        {
                            row[11] = users[j].last_name;
                            break;
                        }
                    }
                    
                    dt.Rows.Add(row);
                }
                return dt;
            });
        }
        private async void buttonUpdateOrders_Click(object sender, EventArgs e)
        {
            dataGridViewOrders.DataSource = await DownloadDataOrders();
            dataGridViewOrders.Columns[0].HeaderText = "Id заказа";
            dataGridViewOrders.Columns[1].HeaderText = "Дата начала";
            dataGridViewOrders.Columns[2].HeaderText = "Дата окончания";
            dataGridViewOrders.Columns[3].HeaderText = "Статус";
            dataGridViewOrders.Columns[4].HeaderText = "В работе";
            dataGridViewOrders.Columns[5].HeaderText = "Точка работы";
            dataGridViewOrders.Columns[6].HeaderText = "Клиент";
            dataGridViewOrders.Columns[7].HeaderText = "Скидка";
            dataGridViewOrders.Columns[8].HeaderText = "Цена со скидкой";
            dataGridViewOrders.Columns[9].HeaderText = "Кол-во человек";
            dataGridViewOrders.Columns[10].HeaderText = "Примечание";
            dataGridViewOrders.Columns[11].HeaderText = "Курьер";
            foreach (DataGridViewColumn column in dataGridViewOrders.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void buttonExportOrders_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialogExcel.DefaultExt = "xls";
                saveFileDialogExcel.Filter = "Excel files(*.xls) | *.xls";
                saveFileDialogExcel.Title = "Excel Export";

                if (saveFileDialogExcel.ShowDialog() == DialogResult.OK)
                {
                    FileStream fileStream = new FileStream(saveFileDialogExcel.FileName, FileMode.Create);
                    StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.Unicode);


                    //foreach (DataGridViewColumn column in dataGridViewOrders.Columns)
                    //{
                    //    streamWriter.Write(column.HeaderText + "\t");
                    //}
                    streamWriter.Write("Id" + "\t");
                    streamWriter.Write("Время начала" + "\t");
                    streamWriter.Write("Время окончания" + "\t");
                    streamWriter.Write("Длительность выполнения" + "\t");

                    streamWriter.Write("Телефон клиента" + "\t");

                    streamWriter.Write("Цена" + "\t");
                    streamWriter.Write("Скидка" + "\t");
                    streamWriter.Write("Цена со скидкой" + "\t");
                    streamWriter.Write("Сколько дал клиент" + "\t");
                    streamWriter.Write("Кол-во человек" + "\t");
                    streamWriter.Write("Описание" + "\t");
                    streamWriter.Write("Промокод" + "\t");
                    streamWriter.Write("Безнал" + "\t");
                    streamWriter.Write("Оператор-продавец" + "\t");
                    streamWriter.Write("Повар-упаковщик" + "\t");
                    streamWriter.Write("Самовывоз" + "\t");
                    streamWriter.Write("Состояние заказа" + "\t");
                    streamWriter.Write("Отправлять фискальные данные" + "\t");

                    streamWriter.Write("Повар" + "\t");
                    streamWriter.Write("Время добавления (повар)" + "\t");
                    streamWriter.Write("Время начала работы (повар)" + "\t");
                    streamWriter.Write("Время окончания работы (повар)" + "\t");
                    streamWriter.Write("Длительность между добавлением и стартом работы (повар)" + "\t");
                    streamWriter.Write("Длительность между стартом и окончанием работы (повар)" + "\t");

                    streamWriter.Write("Упаковщик" + "\t");
                    streamWriter.Write("Время добавления (упаковщик)" + "\t");
                    streamWriter.Write("Время начала работы (упаковщик)" + "\t");
                    streamWriter.Write("Время окончания работы (упаковщик)" + "\t");
                    streamWriter.Write("Длительность между добавлением и стартом работы (упаковщик)" + "\t");
                    streamWriter.Write("Длительность между стартом и окончанием работы (упаковщик)" + "\t");

                    streamWriter.Write("Курьер" + "\t");
                    streamWriter.Write("Время добавления (курьер)" + "\t");
                    streamWriter.Write("Время начала работы (курьер)" + "\t");
                    streamWriter.Write("Время окончания работы (курьер)" + "\t");
                    streamWriter.Write("Длительность между добавлением и стартом работы (курьер)" + "\t");
                    streamWriter.Write("Длительность между стартом и окончанием работы (курьер)" + "\t");
                    streamWriter.Write("Дистанция (курьер)" + "\t");
                    streamWriter.Write("Пункт назначения (курьер)" + "\t");
                    
                    streamWriter.Write("Продукты" + "\t");
                    streamWriter.Write("Кол-во" + "\t");


                    streamWriter.WriteLine();
                    foreach (DataGridViewRow row in dataGridViewOrders.Rows)
                    {
                        //int index = int.Parse(row.Cells[0].Value.ToString());
                        //foreach (DataGridViewCell obj in row.Cells)
                        //{
                        //    string data = obj.Value == null ? "" : obj.Value.ToString();
                        //    streamWriter.Write(data + "\t");
                        //}
                        
                        int index = int.Parse(row.Cells[0].Value.ToString());
                        int i1 = 1;
                        for (int i = 0; i < orders.Count; i++)
                        {
                            if (orders[i].id == index)
                            {
                                i1 = i;
                                break;
                            }
                        }
                        streamWriter.Write(orders[i1].id.ToString() + "\t");
                        streamWriter.Write(ConvertFromUnixTime(orders[i1].datetime_start).ToString() + "\t");
                        streamWriter.Write(ConvertFromUnixTime(orders[i1].datetime_finish).ToString() + "\t");
                        streamWriter.Write((orders[i1].duration == -1 ? "Не завершен" : GetTime(orders[i1].duration)) + "\t");

                        streamWriter.Write(clients.Find(c => c.id == orders[i1].id_client).number_phone + "\t");


                        streamWriter.Write(orders[i1].price.ToString() + "\t");
                        streamWriter.Write(orders[i1].skidka.ToString() + "\t");
                        streamWriter.Write(orders[i1].price_with_skidka.ToString() + "\t");
                        streamWriter.Write(orders[i1].client_money.ToString() + "\t");
                        streamWriter.Write(orders[i1].count_persons.ToString() + "\t");
                        streamWriter.Write(orders[i1].description.Replace('\n',' ').Replace("\r", "") + "\t");
                        streamWriter.Write(orders[i1].promocode + "\t");
                        streamWriter.Write(CheckBool(orders[i1].no_cash) + "\t");
                        streamWriter.Write(CheckBool(orders[i1].operator_prodavec) + "\t");
                        streamWriter.Write(CheckBool(orders[i1].povar_upakov) + "\t");
                        streamWriter.Write(CheckBool(orders[i1].samovivoz) + "\t");
                        streamWriter.Write(states[orders[i1].id_order_state - 1].name + "\t");
                        streamWriter.Write(CheckBool(orders[i1].send_fiscal_data) + "\t");

                        bool povarFind = false;
                        for (int i = 0; i < povar_works.Count; i++)
                        {
                            if (povar_works[i].id_order == index)
                            {
                                for (int j = 0; j < users.Count; j++)
                                {
                                    if (povar_works[i].id_user == users[j].id)
                                    {
                                        streamWriter.Write(users[j].first_name + " " + users[j].last_name + "\t");
                                        break;
                                    }
                                }
                                streamWriter.Write(ConvertFromUnixTime(povar_works[i].datetime_add).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(povar_works[i].datetime_start).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(povar_works[i].datetime_finish).ToString() + "\t");
                                streamWriter.Write(GetTime(povar_works[i].duration_add_start) + "\t");
                                streamWriter.Write(GetTime(povar_works[i].duration_start_finish) + "\t");

                                povarFind = true;
                            }
                            
                        }
                        if (povarFind == false)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                streamWriter.Write("-" + "\t");
                            }
                        }



                        bool upakovFind = false;
                        for (int i = 0; i < upakov_works.Count; i++)
                        {
                            if (upakov_works[i].id_order == index)
                            {
                                for (int j = 0; j < users.Count; j++)
                                {
                                    if (upakov_works[i].id_user == users[j].id)
                                    {
                                        streamWriter.Write(users[j].first_name + " " + users[j].last_name + "\t");
                                        break;
                                    }
                                }
                                streamWriter.Write(ConvertFromUnixTime(upakov_works[i].datetime_add).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(upakov_works[i].datetime_start).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(upakov_works[i].datetime_finish).ToString() + "\t");
                                streamWriter.Write(GetTime(upakov_works[i].duration_add_start) + "\t");
                                streamWriter.Write(GetTime(upakov_works[i].duration_start_finish) + "\t");

                                upakovFind = true;
                            }
                        }
                        if (upakovFind == false)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                streamWriter.Write("-" + "\t");
                            }
                        }



                        bool kurerFind = false;
                        for (int i = 0; i < kurer_works.Count; i++)
                        {
                            if (kurer_works[i].id_order == index)
                            {
                                for (int j = 0; j < users.Count; j++)
                                {
                                    if (kurer_works[i].id_user == users[j].id)
                                    {
                                        streamWriter.Write(users[j].first_name + " " + users[j].last_name + "\t");
                                        break;
                                    }
                                }
                                streamWriter.Write(ConvertFromUnixTime(kurer_works[i].datetime_add).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(kurer_works[i].datetime_start).ToString() + "\t");
                                streamWriter.Write(ConvertFromUnixTime(kurer_works[i].datetime_finish).ToString() + "\t");
                                streamWriter.Write(GetTime(kurer_works[i].duration_add_start) + "\t");
                                streamWriter.Write(GetTime(kurer_works[i].duration_start_finish) + "\t");
                                streamWriter.Write(kurer_works[i].distance + " км" + "\t");
                                Client client = clients.Find(c => c.id == orders[i1].id_client);
                                City city = cities.Find(c => c.id == client.id_city);
                                Rayon rayon = rayons.Find(r => r.id == client.id_rayon);
                                Street street = streets.Find(s => s.id == client.id_street);
                                string pointDistination = String.Format("Город:{0} _ Район:{1} _ Улица:{2} _ Дом:{3}", 
                                    city==null?"?":city.name,
                                    rayon == null ? "?" : rayon.name,
                                    street == null ? "?" : street.name,
                                    client.number_home);
                                streamWriter.Write(pointDistination + "\t");

                                kurerFind = true;
                            }
                        }
                        if (kurerFind == false)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                streamWriter.Write("-" + "\t");
                            }
                        }

                        for (int i = 0; i < order_products.Count; i++)
                        {
                            if (order_products[i].id_order == index)
                            {
                                string name = "";
                                for (int j = 0; j < products.Count; j++)
                                {
                                    if (order_products[i].id_product == products[j].id)
                                    {
                                        name = products[j].name;
                                        streamWriter.Write(name + "\t");
                                        streamWriter.Write(order_products[i].count + "\t");
                                        streamWriter.WriteLine();
                                        break;
                                    }
                                }
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    streamWriter.Write("\t");
                                }
                                for (int j = 1; j < 28; j++)
                                {
                                    streamWriter.Write("\t");
                                }
                            }
                        }

                        
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();

                    fileStream.Close();
                    MessageBox.Show("Файл успешно экспортирован");
                    Process.Start(fileStream.Name);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка. Файл не экспортирован");
            }
        }
        private void dataGridViewOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            richTextBoxInfo.Clear();
            int i = dataGridViewOrders.SelectedCells[0].RowIndex;
            if (i > dataGridViewOrders.RowCount - 2) { return; }
            richTextBoxInfo.Text += "Id: " + orders[i].id.ToString() + "\n";
            richTextBoxInfo.Text += "Время начала:" + ConvertFromUnixTime(orders[i].datetime_start).ToString() + "\n";
            richTextBoxInfo.Text += "Время окончания: " + ConvertFromUnixTime(orders[i].datetime_finish).ToString() + "\n";
            richTextBoxInfo.Text += "Длительность выполнения: " + (orders[i].duration == -1 ? "Не завершен" : GetTime(orders[i].duration)) + "\n";
            richTextBoxInfo.Text += "Цена: " + orders[i].price.ToString() + "\n";
            richTextBoxInfo.Text += "Скидка: " + orders[i].skidka.ToString() + "\n";
            richTextBoxInfo.Text += "Цена со скидкой: " + orders[i].price_with_skidka.ToString() + "\n";
            richTextBoxInfo.Text += "Сколько дал клиент: " + orders[i].client_money.ToString() + "\n";
            richTextBoxInfo.Text += "Кол-во человек: " + orders[i].count_persons.ToString() + "\n";
            richTextBoxInfo.Text += "Описание: " + orders[i].description + "\n";
            richTextBoxInfo.Text += "Промокод: " + orders[i].promocode + "\n";
            richTextBoxInfo.Text += "Безнал: " + CheckBool(orders[i].no_cash) + "\n";
            richTextBoxInfo.Text += "Оператор-продавец: " + CheckBool(orders[i].operator_prodavec) + "\n";
            richTextBoxInfo.Text += "Повар-упаковщик: " + CheckBool(orders[i].povar_upakov) + "\n";
            richTextBoxInfo.Text += "Самовывоз: " + CheckBool(orders[i].samovivoz) + "\n";
            richTextBoxInfo.Text += "Состояние заказа: " + states[orders[i].id_order_state - 1].name + "\n";
            richTextBoxInfo.Text += "В работе: " + (orders[i].on_work == 1 ? "Да" : "Нет")+"\n";
            richTextBoxInfo.Text += "Отправлять фискальные данные: " + CheckBool(orders[i].send_fiscal_data) + "\n";
            for (int j = 0; j < advpoints.Count; j++)
            {
                if (advpoints[j].id == orders[i].id_point_work)
                {
                    richTextBoxInfo.Text += "Точка работы: " + pointworks[j].name + "\n";
                }
            }
            for (int j = 0; j < users.Count; j++)
            {
                if (users[j].id == orders[i].id_operator)
                {
                    richTextBoxInfo.Text += "Оператор: " + users[j].GetInfo(dolgnosti, pointworks);
                    break;
                }
            }
            for (int j = 0; j < users.Count; j++)
            {
                if (users[j].id == orders[i].id_povar)
                {
                    richTextBoxInfo.Text += "Повар: " + users[j].GetInfo(dolgnosti, pointworks);
                    break;
                }
            }
            for (int l = 0; l < povar_works.Count; l++)
            {
                if (povar_works[l].id_order == i)
                {
                    //for (int j = 0; j < users.Count; j++)
                    //{
                    //    if (povar_works[l].id_user == users[j].id)
                    //    {
                    //        richTextBoxInfo.Text += "Повар: " + users[j].login + "\n";
                    //        break;
                    //    }
                    //}
                    richTextBoxInfo.Text += "  Время добавления: " + (ConvertFromUnixTime(povar_works[l].datetime_add).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время начала работы: " + (ConvertFromUnixTime(povar_works[l].datetime_start).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время окончания работы: " + (ConvertFromUnixTime(povar_works[l].datetime_finish).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время между добавлением и стартом: " + (GetTime(povar_works[l].duration_add_start) + "\n");
                    richTextBoxInfo.Text += "  Время между стартом и окончанием: " + (GetTime(povar_works[l].duration_start_finish) + "\n");
                }
            }
            for (int j = 0; j < users.Count; j++)
            {
                if (users[j].id == orders[i].id_upakov)
                {
                    richTextBoxInfo.Text += "Упаковщик: " + users[j].GetInfo(dolgnosti, pointworks);
                    break;
                }
            }
            for (int l = 0; l < upakov_works.Count; l++)
            {
                if (upakov_works[l].id_order == i)
                {
                    //for (int j = 0; j < users.Count; j++)
                    //{
                    //    if (upakov_works[l].id_user == users[j].id)
                    //    {
                    //        richTextBoxInfo.Text += "Упаковщик: " + users[j].login + "\n";
                    //        break;
                    //    }
                    //}
                    richTextBoxInfo.Text += "  Время добавления: " + (ConvertFromUnixTime(upakov_works[l].datetime_add).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время начала работы: " + (ConvertFromUnixTime(upakov_works[l].datetime_start).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время окончания работы: " + (ConvertFromUnixTime(upakov_works[l].datetime_finish).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время между добавлением и стартом: " + (GetTime(upakov_works[l].duration_add_start) + "\n");
                    richTextBoxInfo.Text += "  Время между стартом и окончанием: " + (GetTime(upakov_works[l].duration_start_finish) + "\n");
                }
            }
            for (int j = 0; j < users.Count; j++)
            {
                if (users[j].id == orders[i].id_kurer)
                {
                    richTextBoxInfo.Text += "Курьер: " + users[j].GetInfo(dolgnosti, pointworks);
                    break;
                }
            }
            for (int l = 0; l < kurer_works.Count; l++)
            {
                if (kurer_works[l].id_order == i)
                {
                    //for (int j = 0; j < users.Count; j++)
                    //{
                    //    if (kurer_works[l].id_user == users[j].id)
                    //    {
                    //        richTextBoxInfo.Text += "Курьер: " + users[j].login + "\n";
                    //        break;
                    //    }
                    //}
                    richTextBoxInfo.Text += "  Время добавления: " + (ConvertFromUnixTime(kurer_works[l].datetime_add).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время начала работы: " + (ConvertFromUnixTime(kurer_works[l].datetime_start).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время окончания работы: " + (ConvertFromUnixTime(kurer_works[l].datetime_finish).ToString() + "\n");
                    richTextBoxInfo.Text += "  Время между добавлением и стартом: " + (GetTime(kurer_works[l].duration_add_start) + "\n");
                    richTextBoxInfo.Text += "  Время между стартом и окончанием: " + (GetTime(kurer_works[l].duration_start_finish) + "\n");
                    richTextBoxInfo.Text += "  Дистанция: " + (kurer_works[l].distance + " км" + "\n");
                }
            }
            for (int j = 0; j < clients.Count; j++)
            {
                if (clients[j].id == orders[i].id_client)
                {
                    richTextBoxInfo.Text += "Клиент: " + "\n" + clients[j].GetInfo(cities, rayons, streets, advpoints) + "\n";
                }
            }
            
            i = int.Parse(dataGridViewOrders.Rows[i].Cells[0].Value.ToString());
            richTextBoxInfo.Text += "Продукты: \n";
            for (int l = 0; l < order_products.Count; l++)
            {
                if (order_products[l].id_order == i)
                {
                    string name = "";
                    for (int j = 0; j < products.Count; j++)
                    {
                        if (order_products[l].id_product == products[j].id)
                        {
                            name = products[j].name;
                            richTextBoxInfo.Text += "  Имя продукта: " + (name) + "\n";
                            richTextBoxInfo.Text += "  Кол-во: " + (order_products[l].count.ToString()) + "\n";
                            break;
                        }
                    }
                }
            }
            
            
        }
        #endregion

        #region Функции Logs
        private Task<DataTable> DownloadDataLogs()
        {
            return Task.Run(() =>
            {
                DataTable dt = new DataTable();
                for (int i = 0; i < 5; i++)
                {
                    dt.Columns.Add(new DataColumn());
                }
                DateTime timestart = new DateTime(dateTimePickerLogStart.Value.Year, dateTimePickerLogStart.Value.Month, dateTimePickerLogStart.Value.Day, 0, 0, 0);
                DateTime timefinish = new DateTime(dateTimePickerLogFinish.Value.Year, dateTimePickerLogFinish.Value.Month, dateTimePickerLogFinish.Value.Day, 23, 59, 59);
                logs = api.Logs_select_dts_to_dtf(GetUnixTime(timestart), GetUnixTime(timefinish));
                for (int i = 0; i < logs.Count; i++)
                {
                    string[] row = new string[dt.Columns.Count];
                    row[0] = logs[i].id.ToString();
                    row[1] = ConvertFromUnixTime(logs[i].datetime).ToString();
                    for (int j = 0; j < users.Count; j++)
                    {
                        if (users[j].id == logs[i].id_user)
                        {
                            row[4] = users[j].login;
                            break;
                        }
                    }
                    for (int j = 0; j < actions.Count; j++)
                    {
                        if (actions[j].id == logs[i].id_log_action)
                        {
                            row[3] = actions[j].name;
                            break;
                        }
                    }
                    row[2] = logs[i].id_order.ToString();
                    dt.Rows.Add(row);
                }
                return dt;
            });
        }
        private async void buttonUpdateLogs_Click(object sender, EventArgs e)
        {
            dataGridViewLogs.DataSource = await DownloadDataLogs();
            dataGridViewLogs.Columns[0].HeaderText = "Id";
            dataGridViewLogs.Columns[1].HeaderText = "Дата";
            dataGridViewLogs.Columns[4].HeaderText = "Пользователь";
            dataGridViewLogs.Columns[3].HeaderText = "Действие";
            dataGridViewLogs.Columns[2].HeaderText = "Id заказа";
            foreach (DataGridViewColumn column in dataGridViewLogs.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void buttonExportLogs_Click(object sender, EventArgs e)
        {
            saveFileDialogExcel.DefaultExt = "xls";
            saveFileDialogExcel.Filter = "Excel files(*.xls) | *.xls";
            saveFileDialogExcel.Title = "Excel Export";

            if (saveFileDialogExcel.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(saveFileDialogExcel.FileName, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.Unicode);


                foreach (DataGridViewColumn column in dataGridViewLogs.Columns)
                {
                    streamWriter.Write(column.HeaderText + "\t");
                }
                streamWriter.Write("Фамилия" + "\t");
                streamWriter.Write("Имя" + "\t");
                streamWriter.Write("Телефон" + "\t");
                streamWriter.Write("Должность" + "\t");
                streamWriter.WriteLine();
                foreach (DataGridViewRow row in dataGridViewLogs.Rows)
                {
                    foreach (DataGridViewCell obj in row.Cells)
                    {
                        string data = obj.Value == null ? "" : obj.Value.ToString();
                        streamWriter.Write(data + "\t");
                    }
                    string index = row.Cells[4].Value.ToString();

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].login == index)
                        {
                            streamWriter.Write(users[i].last_name + "\t");
                            streamWriter.Write(users[i].first_name + "\t");
                            streamWriter.Write(users[i].number_phone + "\t");
                            for (int j = 0; j < dolgnosti.Count; j++)
                            {
                                if (dolgnosti[j].id == users[i].id_dolgnost)
                                {
                                    streamWriter.Write(dolgnosti[j].name + "\t");
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    streamWriter.WriteLine();
                }
                streamWriter.Flush();

                fileStream.Close();
                MessageBox.Show("Файл успешно экспортирован");
                //Process.Start(fileStream.Name);
            }
        }
        #endregion

        #region Функции справочников
        private void ShowSpravInfo()
        {
            DataTable dt = new DataTable();
            switch (comboBoxSelect.Text)
            {
                case "Продукты":
                    for (int i = 0; i < 4; i++)
                    {
                        dt.Columns.Add(new DataColumn());
                    }
                    break;
                default:
                    for (int i = 0; i < 2; i++)
                    {
                        dt.Columns.Add(new DataColumn());
                    }
                    break;
            }
            string[] row = new string[dt.Columns.Count];
            switch (comboBoxSelect.Text)
            {
                case "Точки рекламы":
                    for (int i = 0; i < advpoints.Count; i++)
                    {
                        row[0] = advpoints[i].id.ToString();
                        row[1] = advpoints[i].name;
                        dt.Rows.Add(row);
                    }
                    break;
                case "Города":
                    for (int i = 0; i < cities.Count; i++)
                    {
                        row[0] = cities[i].id.ToString();
                        row[1] = cities[i].name;
                        dt.Rows.Add(row);
                    }
                    break;
                case "Районы":
                    for (int i = 0; i < rayons.Count; i++)
                    {
                        row[0] = rayons[i].id.ToString();
                        row[1] = rayons[i].name;
                        dt.Rows.Add(row);
                    }
                    break;
                case "Улицы":
                    for (int i = 0; i < streets.Count; i++)
                    {
                        row[0] = streets[i].id.ToString();
                        row[1] = streets[i].name;
                        dt.Rows.Add(row);
                    }
                    break;
                case "Точки работы":
                    for (int i = 0; i < pointworks.Count; i++)
                    {
                        row[0] = pointworks[i].id.ToString();
                        row[1] = pointworks[i].name;
                        dt.Rows.Add(row);
                    }
                    break;
                case "Продукты":
                    for (int i = 0; i < products.Count; i++)
                    {
                        row[0] = products[i].id.ToString();
                        row[1] = products[i].name;
                        row[2] = products[i].ingredients;
                        row[3] = products[i].price.ToString();
                        dt.Rows.Add(row);
                    }
                    break;
            }
            switch (comboBoxSelect.Text)
            {
                case "Продукты":
                    label15.Visible = true;
                    label16.Visible = true;
                    textBoxIngr.Visible = true;
                    textBoxPrice.Visible = true;
                    dataGridViewSprav.DataSource = dt;
                    dataGridViewSprav.Columns[0].HeaderText = "Id";
                    dataGridViewSprav.Columns[1].HeaderText = "Имя";
                    dataGridViewSprav.Columns[2].HeaderText = "Ингредиенты";
                    dataGridViewSprav.Columns[3].HeaderText = "Цена";
                    break;
                default:
                    label15.Visible = false;
                    label16.Visible = false;
                    textBoxIngr.Visible = false;
                    textBoxPrice.Visible = false;
                    dataGridViewSprav.DataSource = dt;
                    dataGridViewSprav.Columns[0].HeaderText = "Id";
                    dataGridViewSprav.Columns[1].HeaderText = "Имя";
                    break;
            }
            foreach (DataGridViewColumn column in dataGridViewSprav.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void dataGridViewSprav_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = dataGridViewSprav.SelectedCells[0].RowIndex;
            //switch (comboBoxSelect.Text)
            //{
            //    case "Продукты":
            //        textBoxNameSprav.Text = products[rowindex].name;
            //        textBoxIngr.Text = products[rowindex].ingredients;
            //        textBoxPrice.Text = products[rowindex].price.ToString();
            //        break;
            //    default:
            //        textBoxNameSprav.Text = dataGridViewSprav.Rows[rowindex].Cells[1].Value.ToString();
            //        break;
            //}
            switch (comboBoxSelect.Text)
            {
                case "Продукты":
                    textBoxNameSprav.Text = dataGridViewSprav.Rows[rowindex].Cells[1].Value.ToString();
                    textBoxIngr.Text = dataGridViewSprav.Rows[rowindex].Cells[2].Value.ToString();
                    textBoxPrice.Text = dataGridViewSprav.Rows[rowindex].Cells[3].Value.ToString();
                    break;
                default:
                    textBoxNameSprav.Text = dataGridViewSprav.Rows[rowindex].Cells[1].Value.ToString();
                    break;
            }
        }
        private void buttonCreateSprav_Click(object sender, EventArgs e)
        {
            try
            {
                string res = "0";
                //if (int.Parse(res) == 0)
                //{
                //    throw new Exception();
                //}
                switch (comboBoxSelect.Text)
                {
                    case "Продукты":
                        res = api.Products_insert(textBoxNameSprav.Text, textBoxIngr.Text, int.Parse(textBoxPrice.Text), u.id);
                        if (res != "0") 
                        {
                            products.Add(new Product() { id = int.Parse(res), ingredients = textBoxIngr.Text, name = textBoxNameSprav.Text, price = int.Parse(textBoxPrice.Text) });
                        }
                        break;
                    case "Города":
                        res = api.Cities_insert(textBoxNameSprav.Text, u.id);
                        if (res != "0")
                        {
                            cities.Add(new City() { id = int.Parse(res), name = textBoxNameSprav.Text });
                        }
                        break;
                    case "Районы":
                        res = api.Rayons_insert(textBoxNameSprav.Text, u.id);
                        if (res != "0")
                        {
                            rayons.Add(new Rayon() { id = int.Parse(res), name = textBoxNameSprav.Text });
                        }
                        break;
                    case "Улицы":
                        res = api.Streets_insert(textBoxNameSprav.Text, u.id);
                        if (res != "0")
                        {
                            streets.Add(new Street() { id = int.Parse(res), name = textBoxNameSprav.Text });
                        }
                        break;
                    case "Точки работы":
                        res = api.Points_work_insert(textBoxNameSprav.Text, u.id);
                        if (res != "0")
                        {
                            pointworks.Add(new Point_Work() { id = int.Parse(res), name = textBoxNameSprav.Text });
                        }
                        break;
                    case "Точки рекламы":
                        res = api.Advpoints_insert(textBoxNameSprav.Text, u.id);
                        if (res != "0")
                        {
                            advpoints.Add(new AdvPoint() { id = int.Parse(res), name = textBoxNameSprav.Text });
                        }
                        break;
                }
                ShowSpravInfo();
            }
            catch
            {
                MessageBox.Show(this, "Ошибка при добавлении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonUpdateSprav_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewSprav.SelectedCells.Count != 0)
                {
                    string res = "error";
                    int rowindex = dataGridViewSprav.SelectedCells[0].RowIndex;
                    int i = int.Parse(dataGridViewSprav.Rows[rowindex].Cells[0].Value.ToString());
                    switch (comboBoxSelect.Text)
                    {
                        case "Города":
                            res = api.Cities_update_by_id(textBoxNameSprav.Text, i, u.id);
                            if (res == "ok")
                            {
                                cities[rowindex] = new City() { name = textBoxNameSprav.Text, id = i };
                            }
                            break;
                        case "Районы":
                            res = api.Rayons_update_by_id(textBoxNameSprav.Text, i, u.id);
                            if (res == "ok")
                            {
                                rayons[rowindex] = new Rayon() { name = textBoxNameSprav.Text, id = i };
                            }
                            break;
                        case "Улицы":
                            res = api.Streets_update_by_id(textBoxNameSprav.Text, i, u.id);
                            if (res == "ok")
                            {
                                streets[rowindex] = new Street() { name = textBoxNameSprav.Text, id = i };
                            }
                            break;
                        case "Точки рекламы":
                            res = api.Advpoints_update_by_id(textBoxNameSprav.Text, i, u.id);
                            if (res == "ok")
                            {
                                advpoints[rowindex] = new AdvPoint() { name = textBoxNameSprav.Text, id = i };
                            }
                            break;
                        case "Точки работы":
                            res = api.Points_work_update_by_id(textBoxNameSprav.Text, i, u.id);
                            if (res == "ok")
                            {
                                pointworks[rowindex] = new Point_Work() { name = textBoxNameSprav.Text, id = i };
                            }
                            break;
                        case "Продукты":
                            try
                            {
                                res = api.Products_update_by_id(textBoxNameSprav.Text, i, textBoxIngr.Text, int.Parse(textBoxPrice.Text), u.id);
                                if (res == "ok")
                                {
                                    products[rowindex] = new Product() { name = textBoxNameSprav.Text, id = i, ingredients = textBoxIngr.Text, price = int.Parse(textBoxPrice.Text) };
                                }
                                break;
                            }
                            catch
                            {
                                MessageBox.Show(this, "Не удалось добавить продукт. Проверьте корректность введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                    }
                    if (res == "error")
                    {
                        throw new Exception();
                    }
                    ShowSpravInfo();
                }
            }
            catch
            {
                MessageBox.Show(this, "Ошибка при изменении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonDeleteSprav_Click(object sender, EventArgs e)
        {
            if (dataGridViewSprav.SelectedCells.Count != 0)
            {
                //dataGridViewSprav.Sort(dataGridViewSprav.Columns[0], ListSortDirection.Ascending);
                string res = "error";
                int rowindex = dataGridViewSprav.SelectedCells[0].RowIndex;
                int i = int.Parse(dataGridViewSprav.Rows[rowindex].Cells[0].Value.ToString());
                switch (comboBoxSelect.Text)
                {
                    case "Города":
                        #region проверка удаления
                        if (clients.Find(c => c.id_city == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. В этом городе живут клиенты");
                            return;
                        }
                        #endregion
                        res = api.Cities_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            cities.RemoveAt(rowindex);
                        }
                        break;
                    case "Районы":
                        #region проверка удаления
                        if (clients.Find(c => c.id_rayon == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. В этом районе живут клиенты");
                            return;
                        }
                        #endregion
                        res = api.Rayons_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            rayons.RemoveAt(rowindex);
                        }
                        break;
                    case "Улицы":
                        #region проверка удаления
                        if (clients.Find(c => c.id_street == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. На этой улице живут клиенты");
                            return;
                        }
                        #endregion
                        res = api.Streets_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            streets.RemoveAt(rowindex);
                        }
                        break;
                    case "Точки рекламы":
                        #region проверка удаления
                        if (clients.Find(c => c.id_advpoint == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. С этой точки рекламы приходят клиенты");
                            return;
                        }
                        #endregion
                        res = api.Advpoints_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            advpoints.RemoveAt(rowindex);
                        }
                        break;
                    case "Точки работы":
                        #region проверка удаления
                        if (users.Find(u => u.id_point_work == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. В этой точке есть заказы");
                            return;
                        }
                        #endregion
                        res = api.Points_work_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            pointworks.RemoveAt(rowindex);
                        }
                        break;
                    case "Продукты":
                        #region проверка удаления
                        if (order_products.Find(op => op.id_product == i) != null)
                        {
                            MessageBox.Show("Удаление невозможно. Эти продукты используются в заказах");
                            return;
                        }
                        #endregion
                        res = api.Products_delete_by_id(i, u.id);
                        if (res != "error")
                        {
                            products.RemoveAt(rowindex);
                        }
                        break;
                }
                ShowSpravInfo();
            }
            else
            {
                MessageBox.Show(this, "Не выбран объект для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Функции Users
        private Task<DataTable> DownloadDataUsers()
        {
            return Task.Run(() =>
            {
                users = api.Users_select_all();
                DataTable dt = new DataTable();
                for (int i = 0; i < 10; i++)
                {
                    dt.Columns.Add(new DataColumn());
                }
                for (int i = 0; i < users.Count; i++)
                {
                    string[] row = new string[dt.Columns.Count];
                    row[0] = users[i].id.ToString();
                    row[1] = users[i].number_phone;
                    row[2] = users[i].login;
                    row[3] = users[i].password;
                    row[4] = IsNull(users[i].first_name);
                    row[5] = IsNull(users[i].last_name);
                    row[6] = IsNull(users[i].passport);
                    for (int j = 0; j < dolgnosti.Count; j++)
                    {
                        if (dolgnosti[j].id == users[i].id_dolgnost)
                        {
                            row[7] = dolgnosti[j].name;
                            break;
                        }
                    }
                    for (int j = 0; j < pointworks.Count; j++)
                    {
                        if (pointworks[j].id == users[i].id_point_work)
                        {
                            row[8] = pointworks[j].name;
                            break;
                        }
                    }
                    row[9] = CheckBool(users[i].online);
                    dt.Rows.Add(row);
                }
                return dt;
            });
        }
        private async void ShowUsers()
        {
            dataGridViewUsers.DataSource = await DownloadDataUsers();
            dataGridViewUsers.Columns[0].HeaderText = "Id";
            dataGridViewUsers.Columns[1].HeaderText = "Номер телефона";
            dataGridViewUsers.Columns[2].HeaderText = "Логин";
            dataGridViewUsers.Columns[3].HeaderText = "Пароль";
            dataGridViewUsers.Columns[4].HeaderText = "Имя";
            dataGridViewUsers.Columns[5].HeaderText = "Фамилия";
            dataGridViewUsers.Columns[6].HeaderText = "Паспорт";
            dataGridViewUsers.Columns[7].HeaderText = "Должность";
            dataGridViewUsers.Columns[8].HeaderText = "Точка работы";
            dataGridViewUsers.Columns[9].HeaderText = "На работе";
            foreach (DataGridViewColumn column in dataGridViewUsers.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            comboBoxDolgnost.Items.Clear();
            for (int j = 0; j < dolgnosti.Count; j++)
            {
                comboBoxDolgnost.Items.Add(dolgnosti[j].name);
            }
            comboBoxWorkPoint.Items.Clear();
            for (int j = 0; j < pointworks.Count; j++)
            {
                comboBoxWorkPoint.Items.Add(pointworks[j].name);
            }
        }
        private void dataGridViewUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowindex1 = dataGridViewUsers.SelectedCells[0].RowIndex;
                int row = Convert.ToInt32(dataGridViewUsers.Rows[rowindex1].Cells[0].Value);
                int rowindex = -1;
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].id == row)
                    {
                        rowindex = i;
                    }
                }
                textBoxPhone.Text = users[rowindex].number_phone;
                textBoxLogin.Text = users[rowindex].login;
                textBoxPassword.Text = users[rowindex].password;
                textBoxName.Text = IsNull(users[rowindex].first_name);
                textBoxLastName.Text = IsNull(users[rowindex].last_name);
                textBoxPassport.Text = IsNull(users[rowindex].passport);
                for (int j = 0; j < comboBoxWorkPoint.Items.Count; j++)
                {
                    if (comboBoxWorkPoint.Items[j].ToString() == dataGridViewUsers.Rows[rowindex].Cells[8].Value.ToString())
                    {
                        comboBoxWorkPoint.SelectedIndex = j;
                        break;
                    }
                }
                for (int j = 0; j < comboBoxDolgnost.Items.Count; j++)
                {
                    if (comboBoxDolgnost.Items[j].ToString() == dataGridViewUsers.Rows[rowindex].Cells[7].Value.ToString())
                    {
                        comboBoxDolgnost.SelectedIndex = j;
                        break;
                    }
                }
                if (users[rowindex].online == 0)
                {
                    checkBoxOnWork.Checked = false;
                }
                else
                {
                    checkBoxOnWork.Checked = true;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void buttonAddNewUser_Click(object sender, EventArgs e)
        {
            if (textBoxPhone.Text != "" && textBoxLogin.Text != "" && textBoxPassword.Text != "" &&  comboBoxDolgnost.Text != "" && comboBoxWorkPoint.Text != "")
            {
                int dolgnost = 1;
                for (int i = 0; i < comboBoxDolgnost.Items.Count; i++)
                {
                    if (dolgnosti[i].name == comboBoxDolgnost.Text)
                    {
                        dolgnost = i + 1;
                        break;
                    }
                }
                int point_work = 1;
                for (int i = 0; i < comboBoxWorkPoint.Items.Count; i++)
                {
                    if (pointworks[i].name == comboBoxWorkPoint.Text)
                    {
                        point_work = i + 1;
                        break;
                    }
                }
                string res = api.Users_insert_new(textBoxPhone.Text, textBoxLogin.Text, textBoxPassword.Text, CheckEmpty(textBoxName.Text), CheckEmpty(textBoxLastName.Text), CheckEmpty(textBoxPassport.Text), dolgnost, point_work, u.id);
                //if (res != "0")
                //{
                //    users.Add(new User() { id = int.Parse(res), number_phone = textBoxPhone.Text, login = textBoxLogin.Text, password = textBoxPassword.Text, first_name = CheckEmpty(textBoxName.Text), last_name = CheckEmpty(textBoxLastName.Text), passport = CheckEmpty(textBoxPassport.Text), id_dolgnost = dolgnost, id_point_work = point_work });
                //    ShowUsers();
                //}
                //else
                //{
                //    MessageBox.Show(this, "Ошибка при добавлении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                ShowUsers();
            }
            else
            {
                MessageBox.Show(this, "Необходимо ввести все данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonEditUser_Click(object sender, EventArgs e)
        {
            if (textBoxPhone.Text != "" && textBoxLogin.Text != "" && textBoxPassword.Text != "" && comboBoxDolgnost.Text != "" && comboBoxWorkPoint.Text != "")
            {
                //dataGridViewUsers.Sort(dataGridViewUsers.Columns[0], ListSortDirection.Ascending);
                int rowindex = dataGridViewUsers.SelectedCells[0].RowIndex;
                int id = int.Parse(dataGridViewUsers.Rows[rowindex].Cells[0].Value.ToString());
                int dolgnost = 1;
                for (int i = 0; i < comboBoxDolgnost.Items.Count; i++)
                {
                    if (dolgnosti[i].name == comboBoxDolgnost.Text)
                    {
                        dolgnost = i + 1;
                        break;
                    }
                }
                int point_work = 1;
                for (int i = 0; i < comboBoxWorkPoint.Items.Count; i++)
                {
                    if (pointworks[i].name == comboBoxWorkPoint.Text)
                    {
                        point_work = i + 1;
                        break;
                    }
                }
                int online = 0;
                if (checkBoxOnWork.Checked == true)
                {
                    online = 1;
                }
                string res = api.Users_update_by_id(id, textBoxPhone.Text, textBoxLogin.Text, textBoxPassword.Text, CheckEmpty(textBoxName.Text), CheckEmpty(textBoxLastName.Text), CheckEmpty(textBoxPassport.Text), dolgnost, point_work, online, u.id);
                
                //if (res == "ok")
                //{
                //    for (int i = 0; i < users.Count; i++)
                //    {
                //        if (users[i].id == id)
                //        {
                //            users[i] = new User() { id = id, number_phone = textBoxPhone.Text, login = textBoxLogin.Text, password = textBoxPassword.Text, first_name = textBoxName.Text, last_name = textBoxLastName.Text, id_dolgnost = dolgnost, id_point_work = point_work, online = online };
                //        }
                //    }
                //}
                ShowUsers();
            }
            else
            {
                MessageBox.Show(this, "Ошибка при добавлении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedCells.Count != 0)
            {
                //dataGridViewUsers.Sort(dataGridViewUsers.Columns[0], ListSortDirection.Ascending);
                int rowindex = dataGridViewUsers.SelectedCells[0].RowIndex;
                int id = int.Parse(dataGridViewUsers.Rows[rowindex].Cells[0].Value.ToString());

                #region проверка удаления
                if (users.Find(u=>u.id==id).login=="admin")
                {
                    MessageBox.Show("Удаление невозможно. Это же администратор!");
                    return;

                }

                if (povar_works.Find(pw => pw.id_user == id)!=null || upakov_works.Find(uw=>uw.id_user==id)!=null || kurer_works.Find(kw=>kw.id_user == id)!=null)
                {
                    MessageBox.Show("Удаление невозможно. Пользователь делал работу");
                    return;
                }
                #endregion

                string res = api.Users_delete_by_id(id, u.id);
                //if (res == "ok")
                //{
                //    for (int i = 0; i < users.Count; i++)
                //    {
                //        if (users[i].id == id)
                //        {
                //            users.RemoveAt(i);
                //            break;
                //        }
                //    }
                //}
                ShowUsers();
            }
            else
            {
                MessageBox.Show(this, "Не выбран пользователь", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Функции Clients
        private Task<DataTable> DownloadDataClients()
        {
            return Task.Run(() =>
            {
                DataTable dt = new DataTable();
                for (int i = 0; i < 15; i++)
                {
                    dt.Columns.Add(new DataColumn());
                }
                //clients = api.Clients_select_all();
                for (int i = 0; i < clients.Count; i++)
                {
                    string[] row = new string[dt.Columns.Count];
                    row[0] = clients[i].id.ToString();
                    row[1] = clients[i].number_phone;
                    row[2] = clients[i].first_name;
                    row[3] = clients[i].last_name;
                    for (int j = 0; j < cities.Count; j++)
                    {
                        if (cities[j].id == clients[i].id_city)
                        {
                            row[4] = cities[j].name;
                            break;
                        }
                    }
                    for (int j = 0; j < rayons.Count; j++)
                    {
                        if (rayons[j].id == clients[i].id_rayon)
                        {
                            row[5] = rayons[j].name;
                            break;
                        }
                    }
                    for (int j = 0; j < streets.Count; j++)
                    {
                        if (streets[j].id == clients[i].id_street)
                        {
                            row[6] = streets[j].name;
                            break;
                        }
                    }
                    if (clients[i].number_home != -1)
                    {
                        row[7] = clients[i].number_home.ToString();
                    }
                    if (clients[i].number_corp != -1)
                    {
                        row[8] = clients[i].number_corp.ToString();
                    }
                    if (clients[i].number_build != -1)
                    {
                        row[9] = clients[i].number_build.ToString();
                    }
                    if (clients[i].number_flat != -1)
                    {
                        row[10] = clients[i].number_flat.ToString();
                    }
                    if (clients[i].bd_day != -1)
                    {
                        row[11] = clients[i].bd_day.ToString();
                    }
                    if (clients[i].bd_month != -1)
                    {
                        row[12] = clients[i].bd_month.ToString();
                    }
                    if (clients[i].number_card != "")
                    {
                        row[13] = clients[i].number_card;
                    }
                    for (int j = 0; j < advpoints.Count; j++)
                    {
                        if (advpoints[j].id == clients[i].id_advpoint)
                        {
                            row[14] = advpoints[j].name;
                            break;
                        }
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            });
        }
        private async void ShowClients()
        {
            dataGridViewClients.DataSource = await DownloadDataClients();
            dataGridViewClients.Columns[0].HeaderText = "Id";
            dataGridViewClients.Columns[1].HeaderText = "Номер телефона";
            dataGridViewClients.Columns[2].HeaderText = "Имя";
            dataGridViewClients.Columns[3].HeaderText = "Фамилия";
            dataGridViewClients.Columns[4].HeaderText = "Город";
            dataGridViewClients.Columns[5].HeaderText = "Район";
            dataGridViewClients.Columns[6].HeaderText = "Улица";
            dataGridViewClients.Columns[7].HeaderText = "Дом";
            dataGridViewClients.Columns[8].HeaderText = "Корпус";
            dataGridViewClients.Columns[9].HeaderText = "Строение";
            dataGridViewClients.Columns[10].HeaderText = "Квартира";
            dataGridViewClients.Columns[11].HeaderText = "День рождения";
            dataGridViewClients.Columns[12].HeaderText = "Месяц рождения";
            dataGridViewClients.Columns[13].HeaderText = "Номер карты";
            dataGridViewClients.Columns[14].HeaderText = "Точка рекламы";
            foreach (DataGridViewColumn column in dataGridViewClients.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            advpoints = api.AdvPoints_select_all();
            comboBoxAdvpoints.Items.Clear();
            for (int j = 0; j < advpoints.Count; j++)
            {
                comboBoxAdvpoints.Items.Add(advpoints[j].name);
            }
            cities = api.Cities_select_all();
            comboBoxCity.Items.Clear();
            for (int j = 0; j < cities.Count; j++)
            {
                comboBoxCity.Items.Add(cities[j].name);
            }
            rayons = api.Rayons_select_all();
            comboBoxRayon.Items.Clear();
            for (int j = 0; j < rayons.Count; j++)
            {
                comboBoxRayon.Items.Add(rayons[j].name);
            }
            streets = api.Streets_select_all();
            comboBoxStreet.Items.Clear();
            for (int j = 0; j < streets.Count; j++)
            {
                comboBoxStreet.Items.Add(streets[j].name);
            }
        }
        private void buttonAddClient_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxClientPhone.Text != "")
                {
                    int advpoint = -1;
                    for (int i = 0; i < advpoints.Count; i++)
                    {
                        if (advpoints[i].name == comboBoxAdvpoints.Text)
                        {
                            advpoint = advpoints[i].id;
                            break;
                        }
                    }
                    int city = -1;
                    for (int i = 0; i < cities.Count; i++)
                    {
                        if (cities[i].name == comboBoxCity.Text)
                        {
                            city = cities[i].id;
                            break;
                        }
                    }
                    int rayon = -1;
                    for (int i = 0; i < rayons.Count; i++)
                    {
                        if (rayons[i].name == comboBoxRayon.Text)
                        {
                            rayon = rayons[i].id;
                            break;
                        }
                    }
                    int street = -1;
                    for (int i = 0; i < streets.Count; i++)
                    {
                        if (streets[i].name == comboBoxStreet.Text)
                        {
                            street = streets[i].id;
                            break;
                        }
                    }
                    string res = api.Clients_insert_new(textBoxClientName.Text, textBoxClientLastName.Text, textBoxClientPhone.Text, city, rayon, street, int.Parse(textBoxHouse.Text == "" ? "-1" : textBoxHouse.Text), int.Parse(textBoxCorp.Text == "" ? "-1" : textBoxCorp.Text), int.Parse(textBoxBuilding.Text == "" ? "-1" : textBoxBuilding.Text), int.Parse(textBoxFlat.Text == "" ? "-1" : textBoxFlat.Text), dateTimePickerBD.Value.Day, dateTimePickerBD.Value.Month, textBoxCardNumber.Text, advpoint, u.id);

                    if (int.Parse(res) != 0)
                    {
                        clients.Add(new Client() { id = int.Parse(res), first_name = textBoxClientName.Text, last_name = textBoxClientLastName.Text, number_phone = textBoxClientPhone.Text, id_city = city, id_rayon = rayon, id_street = street, number_home = int.Parse(textBoxHouse.Text == "" ? "-1" : textBoxHouse.Text), number_corp = int.Parse(textBoxCorp.Text == "" ? "-1" : textBoxCorp.Text), number_build = int.Parse(textBoxBuilding.Text == "" ? "-1" : textBoxBuilding.Text), number_flat = int.Parse(textBoxFlat.Text == "" ? "-1" : textBoxFlat.Text), bd_day = dateTimePickerBD.Value.Day, bd_month = dateTimePickerBD.Value.Month, number_card = textBoxCardNumber.Text, id_advpoint = advpoint });
                    }
                    ShowClients();
                }
            }
            catch
            {
                MessageBox.Show("ошибка при добавлении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonEditClient_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxClientPhone.Text != "")
                {
                    //dataGridViewClients.Sort(dataGridViewUsers.Columns[0], ListSortDirection.Ascending);
                    int rowindex = dataGridViewClients.SelectedCells[0].RowIndex;
                    int id = int.Parse(dataGridViewClients.Rows[rowindex].Cells[0].Value.ToString());
                    int advpoint = -1;
                    for (int i = 0; i < advpoints.Count; i++)
                    {
                        if (advpoints[i].name == comboBoxAdvpoints.Text)
                        {
                            advpoint = advpoints[i].id;
                            break;
                        }
                    }
                    int city = -1;
                    for (int i = 0; i < cities.Count; i++)
                    {
                        if (cities[i].name == comboBoxCity.Text)
                        {
                            city = cities[i].id;
                            break;
                        }
                    }
                    int rayon = -1;
                    for (int i = 0; i < rayons.Count; i++)
                    {
                        if (rayons[i].name == comboBoxRayon.Text)
                        {
                            rayon = rayons[i].id;
                            break;
                        }
                    }
                    int street = -1;
                    for (int i = 0; i < streets.Count; i++)
                    {
                        if (streets[i].name == comboBoxStreet.Text)
                        {
                            street = streets[i].id;
                            break;
                        }
                    }
                    string res = api.Clients_update_by_id(id, textBoxClientName.Text, textBoxClientLastName.Text, textBoxClientPhone.Text, city, rayon, street, int.Parse(textBoxHouse.Text=="" ? "-1": textBoxHouse.Text), int.Parse(textBoxCorp.Text==""?"-1": textBoxCorp.Text), int.Parse(textBoxBuilding.Text==""?"-1":textBoxBuilding.Text), int.Parse(textBoxFlat.Text==""?"-1": textBoxFlat.Text), dateTimePickerBD.Value.Day, dateTimePickerBD.Value.Month, textBoxCardNumber.Text, advpoint, u.id);
                    if (res == "ok")
                    {
                        for (int i = 0; i < clients.Count; i++)
                        {
                            if (id == clients[i].id)
                            {
                                clients[i] = new Client() { id = id, first_name = textBoxClientName.Text, last_name = textBoxClientLastName.Text, number_phone = textBoxClientPhone.Text, id_city = city, id_rayon = rayon, id_street = street, number_home = int.Parse(textBoxHouse.Text), number_corp = int.Parse(textBoxCorp.Text), number_build = int.Parse(textBoxBuilding.Text), number_flat = int.Parse(textBoxFlat.Text), bd_day = dateTimePickerBD.Value.Day, bd_month = dateTimePickerBD.Value.Month, number_card = textBoxCardNumber.Text, id_advpoint = advpoint };
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при изменении.");
                    }
                    ShowClients();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении. Проверьте соединение с интернетом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridViewClients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex1 = dataGridViewClients.SelectedCells[0].RowIndex;
            int row = Convert.ToInt32(dataGridViewClients.Rows[rowindex1].Cells[0].Value);
            int rowindex = -1;
            try
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].id == row)
                    {
                        rowindex = i;
                    }
                }
                textBoxClientPhone.Text = clients[rowindex].number_phone;
                textBoxClientName.Text = clients[rowindex].first_name;
                textBoxClientLastName.Text = clients[rowindex].last_name;
                textBoxCardNumber.Text = clients[rowindex].number_card;
                textBoxHouse.Text = clients[rowindex].number_home.ToString();
                textBoxCorp.Text = clients[rowindex].number_corp.ToString();
                textBoxBuilding.Text = clients[rowindex].number_build.ToString();
                textBoxFlat.Text = clients[rowindex].number_flat.ToString();
                textBoxPhone.Text = clients[rowindex].number_phone;
            }
            catch
            {

            }
            try
            {
                dateTimePickerBD.Value = new DateTime(DateTime.Now.Year, clients[rowindex].bd_month, clients[rowindex].bd_day);
            }
            catch
            {

            }
            for (int i = 0; i < comboBoxCity.Items.Count; i++)
            {
                if (comboBoxCity.Items[i].ToString() == dataGridViewClients.Rows[rowindex1].Cells[4].Value.ToString())
                {
                    comboBoxCity.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < comboBoxRayon.Items.Count; i++)
            {
                if (comboBoxRayon.Items[i].ToString() == dataGridViewClients.Rows[rowindex1].Cells[5].Value.ToString())
                {
                    comboBoxRayon.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < comboBoxStreet.Items.Count; i++)
            {
                if (comboBoxStreet.Items[i].ToString() == dataGridViewClients.Rows[rowindex1].Cells[6].Value.ToString())
                {
                    comboBoxStreet.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < comboBoxAdvpoints.Items.Count; i++)
            {
                if (comboBoxAdvpoints.Items[i].ToString() == dataGridViewClients.Rows[rowindex1].Cells[14].Value.ToString())
                {
                    comboBoxAdvpoints.SelectedIndex = i;
                    break;
                }
            }
        }
        private void buttonDeleteClient_Click(object sender, EventArgs e)
        {
           // dataGridViewClients.Sort(dataGridViewUsers.Columns[0], ListSortDirection.Ascending);
            int rowindex = dataGridViewClients.SelectedCells[0].RowIndex;
            int id = int.Parse(dataGridViewClients.Rows[rowindex].Cells[0].Value.ToString());

            #region проверка удаления
            //if (orders.Find(o => o.id_client == id ) != null)
            //{
            //    MessageBox.Show("Удаление невозможно. Клиент делал заказ");
            //    return;
            //}
            #endregion

            string res = api.Clients_delete_by_id(id, u.id);
            if (res == "ok")
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (id == clients[i].id)
                    {
                        clients.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка при удалении. Проверьте соединение с интернетом");
            }
            ShowClients();
        }
        #endregion

        #region Функции формы
        public FormWork(User user, FormLogin _fl)
        {
            InitializeComponent();
            u = user;
            fl = _fl;
        }
        private void FormWork_Load(object sender, EventArgs e)
        {
            this.Text = "Администратор";
            api = new API();
            Download_alldata();
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            api.User_finish_work(u.id);
            Application.Exit();
            //this.Close();
            //fl.Show();
        }
        private void FormWork_FormClosed(object sender, FormClosedEventArgs e)
        {
            api.User_finish_work(u.id);
            Application.Exit();
            //fl.Show();
        }
        private void buttonDownloadAll_Click(object sender, EventArgs e)
        {
            Download_alldata();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            fld.BeginInvoke(new Action(() => { fld.progressBar1.Maximum = downloadstrs.Count; }));
            int i = 0;
            pointworks = api.Points_work_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            cities = api.Cities_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            rayons = api.Rayons_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            streets = api.Streets_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            advpoints = api.AdvPoints_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            clients = api.Clients_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            dolgnosti = api.Dolgnosti_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            products = api.Products_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            users = api.Users_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            states = api.Order_states_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            actions = api.Log_actions_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            order_products = api.Order_products_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            kurer_works = api.Kurers_work_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            povar_works = api.Povars_work_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
            upakov_works = api.Upakovs_work_select_all();
            i++;
            backgroundWorker1.ReportProgress(i, e.Argument);
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            fld.BeginInvoke(new Action(() => { fld.progressBar1.Value = e.ProgressPercentage; }));
            fld.BeginInvoke(new Action(() => { fld.label1.Text = downloadstrs[e.ProgressPercentage - 1]; }));
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            fld.Close();
        }
        private void comboBoxSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSpravInfo();
        }
        private void tabControlAdmin_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlAdmin.SelectedTab.Text)
            {
                case "Пользователи":
                    ShowUsers();
                    break;
                case "Клиенты":
                    ShowClients();
                    break;
            }
        }
        #endregion

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxPhone.Clear();
            textBoxLogin.Clear();
            textBoxPassword.Clear();
            textBoxName.Clear();
            textBoxLastName.Clear();
            textBoxPassport.Clear();
            comboBoxDolgnost.SelectedIndex = -1;
            comboBoxWorkPoint.SelectedIndex = -1;
            checkBoxOnWork.Checked = false;
        }

        private void buttonClearSprav_Click(object sender, EventArgs e)
        {
            textBoxNameSprav.Clear();
            textBoxPrice.Clear();
            textBoxIngr.Clear();
        }

        private void buttonClearClient_Click(object sender, EventArgs e)
        {
            textBoxClientName.Clear();
            textBoxClientLastName.Clear();
            textBoxClientPhone.Clear();
            comboBoxAdvpoints.SelectedIndex = -1;
            comboBoxCity.SelectedIndex = -1;
            comboBoxRayon.SelectedIndex = -1;
            comboBoxStreet.SelectedIndex = -1;
            textBoxHouse.Clear();
            textBoxFlat.Clear();
            textBoxCorp.Clear();
            textBoxBuilding.Clear();
            textBoxCardNumber.Clear();
            dateTimePickerBD.Value = DateTime.Now;
        }

    }
}