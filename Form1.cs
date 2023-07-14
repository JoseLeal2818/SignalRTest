using System;

using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.SignalR;
using System.Data.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SignalRTest
{
    public partial class Form1 : Form
    {
        private string HUB = "OPSCWebHub";
        private string API_HUB ;
        private HubConnection connection;
        private IHubProxy hubProxy;

        private string API_KEY = "SecretKey1";
        private HttpClient httpClient;
        private string apiUrl = "http://cml-app01.transpak.com/ocwdev/api/v1/signal/right/user/";
        private string userId = "21066";

        public Form1()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(apiUrl);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            API_HUB = "OPSCWebHub";
            string serverUrl = "http://cml-app01.transpak.com/ocwdev/" + API_HUB;

            // Crea la conexión con el servidor SignalR
            connection = new HubConnection(serverUrl);
            hubProxy = connection.CreateHubProxy(API_HUB);

            connection.Headers.Add("API_KEY", "SecretKey1");

            // Registra el método de escucha para los eventos del servidor
            hubProxy.On<string>("ReceiveRightsMessage-" + userId, mensaje =>
            {
                // Muestra el mensaje recibido en el formulario
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Mensaje recibido: " + mensaje);
                });
            });


            // Inicia la conexión
            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    MessageBox.Show("Error al conectar con el servidor SignalR. ERROR: " + connection.LastError) ;
                }
                else
                {
                    MessageBox.Show("Conexión exitosa con el servidor SignalR.");
                }
            });
        }


        /** AQUI ENVIA EL USUARIO AL ENDPOINT PARA OPCS WEB RECIBE LA RESPUESTA */
        private async void button1_Click(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    await UpdateUser(userId);
                    //MessageBox.Show("PUT request successful", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error");
                }
            }
        }

        private async Task UpdateUser(string apiUrl)
        {
            string url = userId;
            string message = "Hello, API!";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("API-KEY", "SecretKey1");

            var content = new StringContent(message);
            content.Headers.ContentType.MediaType = "text/plain";

            HttpResponseMessage response = await httpClient.PutAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

    }
}