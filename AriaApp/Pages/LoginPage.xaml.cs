using Microsoft.Extensions.Logging.Abstractions;
using System;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AriaApp.Pages
{
    public class LoginResponseModel
    {
        [JsonPropertyName("access_token")]
        public string access_token { get; set; }
    }

    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string mot_de_passe = MotDePasseEntry.Text;

            // Cr�ez un objet JSON pour envoyer au backend
            var data = new { email, mot_de_passe };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("http://localhost:5000/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // La connexion a r�ussi, extrayez le jeton d'acc�s (access token)
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<LoginResponseModel>(responseContent);

                        if (!string.IsNullOrEmpty(responseObject.access_token))
                        {
                            // Vous avez r�cup�r� le jeton d'acc�s
                            string accessToken = responseObject.access_token;
                            await SecureStorage.SetAsync("AccessToken", accessToken);
                            // Faites ce que vous voulez avec le jeton d'acc�s (par exemple, le stocker pour des requ�tes ult�rieures)

                            await DisplayAlert("Succ�s", "Connexion r�ussie", "OK");
                        }
                        else
                        {
                            // Le backend n'a pas renvoy� de jeton d'acc�s
                            await DisplayAlert("Erreur", "Aucun jeton d'acc�s n'a �t� renvoy� par le backend", "OK");
                        }
                    }
                    else
                    {
                        // La connexion a �chou�
                        await DisplayAlert("Erreur", "La connexion a �chou�", "OK");
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
