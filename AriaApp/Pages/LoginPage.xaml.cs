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

            // Créez un objet JSON pour envoyer au backend
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
                        // La connexion a réussi, extrayez le jeton d'accès (access token)
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<LoginResponseModel>(responseContent);

                        if (!string.IsNullOrEmpty(responseObject.access_token))
                        {
                            // Vous avez récupéré le jeton d'accès
                            string accessToken = responseObject.access_token;
                            await SecureStorage.SetAsync("AccessToken", accessToken);
                            // Faites ce que vous voulez avec le jeton d'accès (par exemple, le stocker pour des requêtes ultérieures)

                            await DisplayAlert("Succès", "Connexion réussie", "OK");
                        }
                        else
                        {
                            // Le backend n'a pas renvoyé de jeton d'accès
                            await DisplayAlert("Erreur", "Aucun jeton d'accès n'a été renvoyé par le backend", "OK");
                        }
                    }
                    else
                    {
                        // La connexion a échoué
                        await DisplayAlert("Erreur", "La connexion a échoué", "OK");
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
