using ParkingSystem.Pages;
using Plugin.Maui.Audio;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace ParkingSystem;

public partial class MainPage : ContentPage
{
    string path = AppContext.BaseDirectory;
    public readonly IAudioManager audioManager;
    string cs;
    int id;

    public MainPage(IAudioManager audioManager)
	{
		InitializeComponent();
		registerclickFucn();
        cs = @$"URI=file:{path}\Database\programDB.db";
        this.audioManager = audioManager;
    }

    private void login_Clicked(object sender, EventArgs e)
    {
		/*
		 Login from localStorage and if success go to HomePage.
		 */
		string email = this.email.Text;
		string password = this.password.Text;
        using var con = new SQLiteConnection(cs);
        con.Open();
        string stm = $"SELECT * FROM users WHERE email='{email}' AND password='{password}'";
        using var cmd = new SQLiteCommand(stm, con);
        using SQLiteDataReader rdr = cmd.ExecuteReader();
        if (rdr.HasRows)
        {
            while (rdr.Read())
            {
                id = rdr.GetInt32(0);
                App.Current.MainPage = new NavigationPage(new HomePage(audioManager,id));
            }
        }
        else
        {
            DisplayAlert("Error", "Wrong Credentials!", "OK");
        }
        

    }
	void registerclickFucn()
	{
		register.GestureRecognizers.Add(new TapGestureRecognizer()
		{
			Command = new Command(() =>
			{
                App.Current.MainPage = new NavigationPage(new RegisterPage(audioManager));
            })
		});
	}
}

