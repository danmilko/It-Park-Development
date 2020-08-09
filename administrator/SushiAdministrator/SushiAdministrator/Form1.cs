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

namespace SushiAdministrator
{
    public partial class FormLogin : Form
    {
        API api = new API();
        List<Point_Work> points_work;
        public FormLogin()
        {
            InitializeComponent();
        }
        private User LogIn(string login, string password)
        {
            User user = api.Users_select_by_login_password(login, password);
            if (user != null && user.id_dolgnost == 1)
            {
                int index = 0;
                for (int i = 0; i < points_work.Count; i++)
                {
                    if (comboBoxPointWork.Text == points_work[i].name)
                    {
                        index = points_work[i].id;
                    }
                }
                api.User_start_work(user.id, index);
                return user;
            }
            return null;
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            if (textBoxLogin.Text != String.Empty && textBoxPassword.Text != String.Empty && comboBoxPointWork.SelectedIndex != -1)
            {
                User u = LogIn(textBoxLogin.Text, textBoxPassword.Text);
                if (u != null)
                {
                    FormWork fw = new FormWork(u, this);
                    textBoxLogin.Text = "";
                    textBoxPassword.Text = "";
                    comboBoxPointWork.SelectedIndex = -1;
                    this.Hide(); 
                    fw.Show();
                }
                else
                {
                    MessageBox.Show("Не удалось выполнить вход. Проверьте правильность введенных данных.");
                }
            }
            else
            {
                MessageBox.Show("Не введены какие-то данные.");
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                points_work = api.Points_work_select_all();
                for (int i = 0; i < points_work.Count; i++)
                {
                    comboBoxPointWork.Items.Add(points_work[i].name);
                }
            }
            catch
            {
                MessageBox.Show("Отсутствует список point_work. Проверьте соединение с интернетом.");
                Application.Exit();
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
