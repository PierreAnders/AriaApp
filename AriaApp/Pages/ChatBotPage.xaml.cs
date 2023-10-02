using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace AriaApp
{
    public partial class ChatBotPage : ContentPage, INotifyPropertyChanged
    {
        private ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();

        public ChatBotPage()
        {
            InitializeComponent();
            MessageListView.ItemsSource = Messages;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string accessToken = await SecureStorage.GetAsync("AccessToken");
            return accessToken;
        }


        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            string userMessage = MessageEntry.Text;
            if (!string.IsNullOrWhiteSpace(userMessage))
            {
                Messages.Add(new Message { Text = $"You: {userMessage}" });
                MessageEntry.Text = string.Empty;

                // Envoyez le message à votre backend et attendez la réponse
                string botReply = await GetBotReplyAsync(userMessage);

                Messages.Add(new Message { Text = $"Bot: {botReply}" });
            }
        }

        private async Task<string> GetBotReplyAsync(string userMessage)
        {
            // Appelez votre backend ici pour obtenir la réponse du chatbot
            try
            {
                string url = "http://localhost:5000/AIchatWithData"; // Remplacez par l'URL de votre backend
                string accessToken = await GetAccessTokenAsync();

                // Créez un objet JSON avec les données nécessaires
                var requestData = new
                {
                    session_id = Guid.NewGuid().ToString(), // Utilisez un ID de session unique
                    query = userMessage
                };

                // Convertissez l'objet en une chaîne JSON
                string jsonRequest = JsonConvert.SerializeObject(requestData);

                // Créez une demande HTTP POST
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Ajoutez des en-têtes d'autorisation si nécessaire
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    // Envoyez la demande au backend
                    var response = await client.PostAsync(url, content);

                    // Vérifiez si la demande a réussi
                    if (response.IsSuccessStatusCode)
                    {
                        // Lisez la réponse du serveur
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Analysez la réponse JSON pour extraire la réponse du chatbot
                        var responseData = JsonConvert.DeserializeAnonymousType(jsonResponse, new
                        {
                            answer = ""
                        });

                        return responseData.answer;
                    }
                    else
                    {
                        // Gérez les erreurs de demande ici
                        return "Désolé, une erreur s'est produite lors de la communication avec le serveur.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérez les exceptions ici
                return "Désolé, une erreur s'est produite : " + ex.Message;
            }
        }

    }

    public class Message
    {
        public string Text { get; set; }
    }
}
