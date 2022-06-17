using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalAss
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            Thread trd = new Thread(new ThreadStart(runForm));
            trd.Start();
            Thread.Sleep(850);
            InitializeComponent();
            linkLabel1.Hide();
            trd.Abort();
        }
        string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\voras\source\repos\FinalAss\FinalAss\TouchingGrass.mdf; Integrated Security = True";
        Site site1 = new Site("Nice camping site");
        Camping camping1 = new Camping();
        private void runForm()
        {
            Application.Run(new Form_LoadingScreen());
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {

        }
        //Live validation for input in the reservation form
        //which expects proper input from the user and assigns live boolean values to make sure every value is correct.

        bool correctInputNumOfPpl = false;
        bool correctInputDateStart = false;
        bool correctInputDateEnd = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int numOfPeople = 0;
            bool isNumeric = int.TryParse(textBoxNumOfPpl.Text, out numOfPeople);
            if (isNumeric == true && numOfPeople > 0)
            {
                correctInputNumOfPpl = true;
                feedbackNumOfPpl.Text = "v";
                feedbackNumOfPpl.ForeColor = Color.Lime;
            }
            else
            {
                correctInputNumOfPpl = false;
                feedbackNumOfPpl.Text = "x";
                feedbackNumOfPpl.ForeColor = Color.Red;
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if ((dateStartPicker.Value - DateTime.Now).TotalMilliseconds >= 0 && (dateEndPicker.Value - dateStartPicker.Value).TotalMilliseconds > 0)
            {
                correctInputDateStart = true;
                feedbackDateStart.Text = "v";
                feedbackDateStart.ForeColor = Color.Lime;
            }
            else
            {
                correctInputDateStart = false;
                feedbackDateStart.Text = "x";
                feedbackDateStart.ForeColor = Color.Red;
            }
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1_ValueChanged(sender, e);
            if ((dateEndPicker.Value - DateTime.Now).TotalDays >= 0 && (dateEndPicker.Value - dateStartPicker.Value).TotalMilliseconds > 0)
            {
                correctInputDateEnd = true;
                feedbackDateEnd.Text = "v";
                feedbackDateEnd.ForeColor = Color.Lime;
            }
            else
            {
                correctInputDateEnd = false;
                feedbackDateEnd.Text = "x";
                feedbackDateEnd.ForeColor = Color.Red;
            }
        }
        //Calculate how much a client has to pay and allowing them to click the final link to "pay"
        int numberOfPeople;
        DateTime startDate;
        DateTime endDate;
        bool carIncluded;
        double totalPayment;
        private void submit_Click(object sender, EventArgs e)
        {
            int extraCarPayment = 0;
            if (carPresentBox.Checked)
            {
                extraCarPayment = 3;
            }
            if (correctInputNumOfPpl == true && correctInputDateStart == true && correctInputDateEnd == true)
            {
                double payment = (site1.price + 2.5 * int.Parse(textBoxNumOfPpl.Text) + extraCarPayment) * (dateEndPicker.Value - dateStartPicker.Value).Days;

                if (DateTime.IsLeapYear(dateStartPicker.Value.Year))
                {
                    if (dateStartPicker.Value.DayOfYear >= 193 && dateStartPicker.Value.DayOfYear <= 228)
                    {
                        payment = payment * 1.25;
                    }
                }
                else
                {
                    if (dateStartPicker.Value.DayOfYear >= 192 && dateStartPicker.Value.DayOfYear <= 227)
                    {
                        payment = payment * 1.25;
                    }
                    linkLabel1.Show();
                    linkLabel1.Text = "Total price: " + payment + "€. Click here to pay.\r\nIf data has been mistyped, resubmit the form.";
                    numberOfPeople = int.Parse(textBoxNumOfPpl.Text);
                    startDate = dateStartPicker.Value;
                    endDate = dateEndPicker.Value;
                    carIncluded = carPresentBox.Checked;
                    totalPayment = payment;
                    
                }
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            camping1.reservations.Add(new Reservation(numberOfPeople, startDate, endDate, carIncluded, site1));
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int carAvailability = 0;
                if(carIncluded)
                {
                    carAvailability = 1;
                }
                string query = "INSERT INTO Reservation (NumOfPeople, DateStart, DateEnd, CarIncluded, Price) VALUES" +
                               "('" + numberOfPeople + "', '" + startDate + "', '" + endDate + "', '" + carAvailability + "', '" + totalPayment + "');";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 ReservationID FROM Reservation ORDER BY ReservationID DESC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessageBox.Show("Reservation created, your unique code is: " + reader["ReservationID"]);
                        }
                    }
                }
                connection.Close();
            }
            textBoxNumOfPpl.Text = "";
            dateStartPicker.Value = DateTime.Now;
            dateEndPicker.Value = DateTime.Now;
            carPresentBox.Checked = false;
            linkLabel1.Hide();
        }
        //
        //Check reservation
        //
        private void uniqueNumberBox_TextChanged_1(object sender, EventArgs e)
        {
            int uniqueCode = 0;
            bool isNumeric = int.TryParse(uniqueNumberBox.Text, out uniqueCode);
            if (isNumeric == true && uniqueCode > 0)
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT NumOfPeople, DateStart, DateEnd, CarIncluded, Price FROM Reservation WHERE ReservationID='" + uniqueCode + "';";
                    using(SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                checkNumOfPpl.Text = "Number of people: " + reader["NumOfPeople"].ToString();
                                checkDate.Text = "Duration: " + reader["DateStart"].ToString() + " - " + reader["DateEnd"].ToString();
                                if(int.Parse(reader["CarIncluded"].ToString()) == 1)
                                {
                                    checkCar.Text = "Car included: Yes";
                                }
                                else
                                {
                                    checkCar.Text = "Car included: No";
                                }
                                checkPrice.Text = "Price: " + reader["Price"].ToString() + "€";
                            }
                        }
                    }
                }
            }
        }
        //Statistics, run when the refresh button is clicked

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            totalIncomeLabel.Text = "Total income: " + camping1.getEarnings().ToString() + " €";
            avgDaysLabel.Text = "Average amount of days spent at the park: " + camping1.getAverageDaysRented().ToString() + " days";
            totalPeopleLabel.Text = "Total amount of people who visited us: " + camping1.getTotalAmountOfPeople().ToString() + " people";
        }

        private void aboutLabel_Click(object sender, EventArgs e)
        {
            using (AboutBox1 box = new AboutBox1())
            {
                box.ShowDialog(this);
            }
        }

        private void openTab1_Click(object sender, EventArgs e)
        {
            Show();
            this.tabControl1.SelectTab(0);
        }

        private void openTab2_Click(object sender, EventArgs e)
        {
            Show();
            this.tabControl1.SelectTab(1);
        }

        private void openTab3_Click(object sender, EventArgs e)
        {
            Show();
            this.tabControl1.SelectTab(2);
        }

        private void open_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void close_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}