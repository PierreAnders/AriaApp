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

                // Envoyez le message � votre backend et attendez la r�ponse
                string botReply = await GetBotReplyAsync(userMessage);

                Messages.Add(new Message { Text = $"Bot: {botReply}" });
            }
        }

        private async Task<string> GetBotReplyAsync(string userMessage)
        {
            // Appelez votre backend ici pour obtenir la r�ponse du chatbot
            try
            {
                string url = "http://localhost:5000/AIchatWithData"; // Remplacez par l'URL de votre backend
                string accessToken = await GetAccessTokenAsync();

                // Cr�ez un objet JSON avec les donn�es n�cessaires
                var requestData = new
                {
                    session_id = Guid.NewGuid().ToString(), // Utilisez un ID de session unique
                    query = userMessage
                };

                // Convertissez l'objet en une cha�ne JSON
                string jsonRequest = JsonConvert.SerializeObject(requestData);

                // Cr�ez une demande HTTP POST
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Ajoutez des en-t�tes d'autorisation si n�cessaire
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    // Envoyez la demande au backend
                    var response = await client.PostAsync(url, content);

                    // V�rifiez si la demande a r�ussi
                    if (response.IsSuccessStatusCode)
                    {
                        // Lisez la r�ponse du serveur
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Analysez la r�ponse JSON pour extraire la r�ponse du chatbot
                        var responseData = JsonConvert.DeserializeAnonymousType(jsonResponse, new
                        {
                            answer = ""
                        });

                        return responseData.answer;
                    }
                    else
                    {
                        // G�rez les erreurs de demande ici
                        return "D�sol�, une erreur s'est produite lors de la communication avec le serveur.";
                    }
                }
            }
            catch (Exception ex)
            {
                // G�rez les exceptions ici
                return "D�sol�, une erreur s'est produite : " + ex.Message;
            }
        }

    }

    public class Message
    {
        public string Text { get; set; }
    }
}
