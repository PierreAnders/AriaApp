//using Microsoft.Extensions.Logging.Abstractions;
//using System.Text.Json;

//namespace AriaApp.Pages;

//public partial class RegistrationPage : ContentPage
//{
//	public RegistrationPage()
//	{
//		InitializeComponent();
//	}
//}

using Microsoft.Extensions.Logging.Abstractions;
using System;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AriaApp.Pages
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string nom = NomEntry.Text;
            string email = EmailEntry.Text;
            string mot_de_passe = MotDePasseEntry.Text;

            // Cr�ez un objet JSON pour envoyer au backend
            var data = new { nom, email, mot_de_passe };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("http://localhost:5000/register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // L'inscription a r�ussi
                        await DisplayAlert("Succ�s", "Inscription r�ussie", "OK");
                    }
                    else
                    {
                        // L'inscription a �chou�
                        await DisplayAlert("Erreur", "L'inscription a �chou�", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
        }
    }
}
