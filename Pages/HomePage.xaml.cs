using Plugin.Maui.Audio;
using System.Data.SQLite;
using ParkingSystem.Pages;
using System.Security.Cryptography;
using IronBarCode;
using System.Net;

namespace ParkingSystem.Pages;

public partial class HomePage : ContentPage
{
    public readonly IAudioManager audioManager;
    string path = AppContext.BaseDirectory;
    string cs;
    int user_id;
    IDispatcherTimer timer;
    public HomePage(IAudioManager audioManager,int id)
	{
		InitializeComponent();
        user_id = id;
        cs = @$"URI=file:{path}\Database\programDB.db";
        Set_colours();

        this.audioManager = audioManager;

        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(10);
        timer.Tick += (s, e) => OnTimer();
    }
    private async void OnTimer()
    {
        //get remaining time and make noise after that time,if it reaches zero reset and free spaces
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("urgent.wav"));
        player.Play();
        DisplayAlert("Time's up", "Your parking period has expired please refill your credit", "OK");
        System.Threading.Thread.Sleep(5000);
        player.Stop();

        string sqm = $"DELETE FROM slots WHERE user_id= {user_id}";
        using var con = new SQLiteConnection(cs);
        con.Open();

        var cmd = new SQLiteCommand(sqm, con);
        if (cmd.ExecuteNonQuery() > 0)
        {
            Set_colours();
            timer.Stop();
        }
        Set_colours();
    }
    private void Set_colours()
    {
        using var con = new SQLiteConnection(cs);
        con.Open();

        string stm = $"SELECT * FROM slots";
        using var cmd = new SQLiteCommand(stm, con);
        using SQLiteDataReader rdr = cmd.ExecuteReader();
        if (rdr.HasRows)
        {
            while (rdr.Read())
            {
                //Red - Occupied , Green - Vacant, 
                string parking_slot = "a" + rdr.GetString(2);
                switch(parking_slot)
                {
                    case "a1":
                        a1.Background = Colors.Red;
                        a1.IsEnabled= false;
                        break;
                    case "a2":
                        a2.Background = Colors.Red;
                        a2.IsEnabled = false;
                        break;
                    case "a3":
                        a3.Background = Colors.Red;
                        a3.IsEnabled = false;
                        break;
                    case "a4":
                        a4.Background = Colors.Red;
                        a4.IsEnabled = false;
                        break;
                    case "a5":
                        a5.Background = Colors.Red;
                        a5.IsEnabled = false;
                        break;
                    case "a6":
                        a6.Background = Colors.Red;
                        a6.IsEnabled = false;
                        break;
                    case "a7":
                        a7.Background = Colors.Red;
                        a7.IsEnabled = false;
                        break;
                    case "a8":
                        a8.Background = Colors.Red;
                        a8.IsEnabled = false;
                        break;
                    case "a9":
                        a9.Background = Colors.Red;
                        a9.IsEnabled = false;
                        break;
                    case "a10":
                        a10.Background = Colors.Red;
                        a10.IsEnabled = false;
                        break;
                    case "a11":
                        a11.Background = Colors.Red;
                        a11.IsEnabled = false;
                        break;
                    case "a12":
                        a12.Background = Colors.Red;
                        a12.IsEnabled = false;
                        break;
                    case "a13":
                        a13.Background = Colors.Red;
                        a13.IsEnabled = false;
                        break;
                    case "a14":
                        a14.Background = Colors.Red;
                        a14.IsEnabled = false;
                        break;
                    case "a15":
                        a15.Background = Colors.Red;
                        a15.IsEnabled = false;
                        break;
                    case "a16":
                        a16.Background = Colors.Red;
                        a16.IsEnabled = false;
                        break;
                    case "a17":
                        a17.Background = Colors.Red;
                        a17.IsEnabled = false;
                        break;
                    case "a18":
                        a18.Background = Colors.Red;
                        a18.IsEnabled = false;
                        break;
                    case "a19":
                        a19.Background = Colors.Red;
                        a19.IsEnabled = false;
                        break;
                    case "a20":
                        a20.Background = Colors.Red;
                        a20.IsEnabled = false;
                        break;
                    case "a21":
                        a21.Background = Colors.Red;
                        a21.IsEnabled = false;
                        break;
                    case "a22":
                        a22.Background = Colors.Red;
                        a22.IsEnabled = false;
                        break;
                    case "a23":
                        a23.Background = Colors.Red;
                        a23.IsEnabled = false;
                        break;
                    case "a24":
                        a24.Background = Colors.Red;
                        a24.IsEnabled = false;
                        break;
                    case "a25":
                        a25.Background = Colors.Red;
                        a25.IsEnabled = false;
                        break;
                    case "a26":
                        a26.Background = Colors.Red;
                        a26.IsEnabled = false;
                        break;
                    case "a27":
                        a27.Background = Colors.Red;
                        a27.IsEnabled = false;
                        break;
                    case "a28":
                        a28.Background = Colors.Red;
                        a28.IsEnabled = false;
                        break;
                    case "a29":
                        a29.Background = Colors.Red;
                        a29.IsEnabled = false;
                        break;
                }
            }
        }
    }
    private void generateQr()
    {
        IronBarCode.License.LicenseKey = "IRONBARCODE.WHIZYDAN.14037-7BD2A8D299-AONLKF2OV7XLQ-UXQVNLA3YPZM-MSWTJLAI7JEP-JJEB4EQGDYMW-MVRNP2C6NS2Q-XN7DPM-T2ASSIWATPSJEA-DEPLOYMENT.TRIAL-Q4CKDZ.TRIAL.EXPIRES.29.MAR.2023";
        string path2 = @$"{path}\parking_ticket.png";
        QRCodeWriter.CreateQrCode($"user : {user_id} booked in at {DateTime.Now}", 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng(path2);
        DisplayAlert("success", "Your qr has been saved as parking_ticket.png", "OK");
        Set_colours();

        //start timer
        
        timer.Start();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string slot = "1";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked(object sender, EventArgs e)
    {
        string slot = "2";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if(content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }else if(content == "1"){
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if(cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
        
    }

    private async void a1_Clicked_1(object sender, EventArgs e)
    {
        string slot = "3";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_2(object sender, EventArgs e)
    {
        string slot = "4";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_3(object sender, EventArgs e)
    {
        string slot = "5";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_4(object sender, EventArgs e)
    {
        string slot = "6";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_5(object sender, EventArgs e)
    {
        string slot = "7";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_6(object sender, EventArgs e)
    {
        string slot = "8";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_7(object sender, EventArgs e)
    {
        string slot = "9";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_8(object sender, EventArgs e)
    {
        string slot = "10";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_9(object sender, EventArgs e)
    {
        string slot = "11";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_10(object sender, EventArgs e)
    {
        string slot = "12";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_11(object sender, EventArgs e)
    {
        string slot = "13";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_12(object sender, EventArgs e)
    {
        string slot = "14";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_13(object sender, EventArgs e)
    {
        string slot = "15";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_14(object sender, EventArgs e)
    {
        string slot = "16";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_15(object sender, EventArgs e)
    {
        string slot = "17";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_16(object sender, EventArgs e)
    {
        string slot = "18";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_17(object sender, EventArgs e)
    {
        string slot = "19";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_18(object sender, EventArgs e)
    {
        string slot = "20";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_19(object sender, EventArgs e)
    {
        string slot = "21";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_20(object sender, EventArgs e)
    {
        string slot = "22";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_21(object sender, EventArgs e)
    {
        string slot = "23";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_22(object sender, EventArgs e)
    {
        string slot = "24";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_23(object sender, EventArgs e)
    {
        string slot = "25";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_24(object sender, EventArgs e)
    {
        string slot = "26";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_25(object sender, EventArgs e)
    {
        string slot = "27";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_26(object sender, EventArgs e)
    {
        string slot = "28";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }

    private async void a1_Clicked_27(object sender, EventArgs e)
    {
        string slot = "29";
        string card_number = await DisplayPromptAsync("Chekout", "Enter card number");
        string card_cvv = await DisplayPromptAsync("Chekout", "Enter card cvv");
        int amount = 100;
        string url = $"https://kerberos.co.ke/test/?number={card_number}&op=0&cvv={card_cvv}&amount={amount}";
        using var client = new HttpClient();
        var content = await client.GetStringAsync(url);
        if (content == "0")
        {
            DisplayAlert("Response", "Insufficient funds", "OK");
        }
        else if (content == "1")
        {
            //insert into database and generate QR code
            string sql = $"INSERT INTO slots VALUES(null,\"{user_id}\",\"{slot}\")";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(sql, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                generateQr();
            }
            else
            {
                DisplayAlert("Error", "Could not insert records", "OK");
            }
        }
    }
}