
using Plugin.Maui.Audio;
using System.Data.SQLite;
using System.Net;
using System.Net.Mail;

namespace ParkingSystem.Pages;

public partial class RegisterPage : ContentPage
{
    public readonly IAudioManager audioManager;
    int count;
	public RegisterPage(IAudioManager audioManager)
	{
		InitializeComponent();
        this.code.IsEnabled = false;
		count = 0;
        this.audioManager = audioManager;
    }

    private void Signup_Clicked(object sender, EventArgs e)
    {
		if(count == 0)
		{
            //register person and then wait for code
            var email = this.email.Text;
            var password = this.password.Text;
            var rand = new Random();
            int code = rand.Next(0, 19201);

            if (Send_Email(email, code))
            {
                this.code.IsEnabled = true;
                DisplayAlert("Verify Code", "We sent a code to your email! please verify the email", "OK");
                this.Signup.Text = "Verify Code";
            }
		}
		else
		{
			insert();
		}
		count++;
    }
	private bool Send_Email(string email,int code)
	{
		var smtpClient = new SmtpClient("smtp.gmail.com")
		{
			Port = 587,
			Credentials = new NetworkCredential("whizydan@gmail.com", "laolslatndgndand"),
			EnableSsl = true,
		};
		try {
            smtpClient.Send("Kerberos@kerberos.co.ke", email, "Email Verification", $"Please verify your account: {code}");
			return true;
        }
        catch(Exception ex) {
			DisplayAlert($"Error: {ex.Source}", $"An error has occured: {ex.Message}", "OK");
			return false;
		}
		
		
	}
	private void insert()
	{
        string path = AppContext.BaseDirectory;
        string cs = @$"URI=file:{path}\Database\programDB.db";

        using var con = new SQLiteConnection(cs);
        con.Open();
        string stm = $"INSERT INTO users VALUES(null,\"{this.email.Text}\",\"{this.password.Text}\",0,0) ";
        var cmd = new SQLiteCommand(stm, con);
		if(cmd.ExecuteNonQuery() > 0)
		{
			DisplayAlert("Success", "You can log in ", "OK");
		}
        App.Current.MainPage = new NavigationPage(new MainPage(audioManager));
    }
}