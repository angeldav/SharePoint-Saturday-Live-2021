using O365.ActivityManagementAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace O365.ActivityManagementAPI.Business
{
    public class ManagementAPI
    {
        public static void ManageSubscription(string tenantId, OAuthMessageHandler messageHandler, OperationSubscriptions.OperationSubscription operationSubscription)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient(messageHandler, false))
                {
                    httpClient.BaseAddress = new Uri("https://manage.office.com");
                    httpClient.Timeout = new TimeSpan(0, 2, 0);

                    string endpoint = String.Empty;
                    switch (operationSubscription)
                    {
                        case OperationSubscriptions.OperationSubscription.Start:
                            endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/start?contentType=Audit.Sharepoint";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/start?contentType=Audit.General";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/start?contentType=Audit.Exchange";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/start?contentType=Audit.AzureActiveDirectory";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/start?contentType=DLP.All";
                            break;
                        case OperationSubscriptions.OperationSubscription.Stop:
                            endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/stop?contentType=Audit.Sharepoint";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/stop?contentType=Audit.General";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/stop?contentType=Audit.Exchange";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/stop?contentType=Audit.AzureActiveDirectory";
                            //endpoint = $"/api/v1.0/{tenantId}/activity/feed/subscriptions/stop?contentType=DLP.All";
                            break;
                        default:
                            break;
                    }

                    string endpointName = endpoint.Substring(endpoint.IndexOf('=') + 1, endpoint.Length - endpoint.IndexOf('=') - 1);
                    Logging.LogMessage("\tEndpoint: " + endpointName);
                    Logging.LogMessage("\tOperación de suscripción a realizar: " + operationSubscription.ToString().ToUpper());

                    HttpRequestMessage message = new HttpRequestMessage(new HttpMethod("POST"), endpoint);
                    var response = httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Logging.LogMessage("Operación realizada correctamente");
                    }
                    else
                    {
                        Logging.LogMessage("Operación de suscripción fallida: " + response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage("EXCEPCIÓN en ManageSubscription: " + ex.Message);
                throw ex;
            }
        }

        public static void ListSubscription(string tenantId, OAuthMessageHandler messageHandler)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient(messageHandler, false))
                {
                    httpClient.BaseAddress = new Uri("https://manage.office.com");
                    httpClient.Timeout = new TimeSpan(0, 2, 0);
                    var response = httpClient.GetAsync($"https://manage.office.com/api/v1.0/{tenantId}/activity/feed/subscriptions/list").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        Logging.LogMessage("Operación de suscripción a lista realizada correctamente");
                    }
                    else
                    {
                        Logging.LogMessage("Operación de suscripción a lista fallida: " + response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage("EXCEPCIÓN en ListSubscription: " + ex.Message);
                throw ex;
            }
        }

        public static List<O365Subscription> RetrieveContentBlobs(string tenantId, OAuthMessageHandler messageHandler)
        {
            List<O365Subscription> subscriptionList = new List<O365Subscription>();
            bool QueryIncomplete;

            try
            {
                using (HttpClient httpClient = new HttpClient(messageHandler, false))
                {
                    httpClient.BaseAddress = new Uri("https://manage.office.com");
                    httpClient.Timeout = new TimeSpan(0, 2, 0);

                    var response = httpClient.GetAsync($"https://manage.office.com/api/v1.0/{tenantId}/activity/feed/subscriptions/content?contentType=Audit.SharePoint&PublisherIdentifier={tenantId}");
                    //var response = httpClient.GetAsync($"https://manage.office.com/api/v1.0/{tenantId}/activity/feed/subscriptions/content?contentType=Audit.SharePoint&PublisherIdentifier={tenantId}&amp;startTime={start}&amp;endTime={end}");
                    //var response = httpClient.GetAsync($"https://manage.office.com/api/v1.0/{tenantId}/activity/feed/subscriptions/content?contentType=Audit.SharePoint&PublisherIdentifier={tenantId}&amp;startTime={01-01-2021}&amp;endTime={01-06-2021}");

                    if (response.Result.IsSuccessStatusCode)
                    {
                        do
                        {
                            QueryIncomplete = false;
                            string json = response.Result.Content.ReadAsStringAsync().Result;
                            subscriptionList.AddRange(JsonConvert.DeserializeObject<List<O365Subscription>>(json));
                            if (response.Result.Headers.Contains("NextPageUri"))
                            {
                                response = httpClient.GetAsync(response.Result.Headers.GetValues("NextPageUri").First());
                                if (response.Result.IsSuccessStatusCode)
                                {
                                    QueryIncomplete = true;
                                }
                            }
                        } while (QueryIncomplete);
                    }

                    Logging.LogMessage("Se han obtenido " + subscriptionList.Count + " content blobs.");
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage("EXCEPCIÓN en RetrieveContentBlobs: " + ex.Message);
                throw ex;
            }

            return subscriptionList;
        }

        public static List<SharePointDataAudit> GetDataContentBlobs(OAuthMessageHandler messageHandler, List<O365Subscription> contentBlobs)
        {
            List<SharePointDataAudit> operationList = new List<SharePointDataAudit>();

            Logging.LogMessage("Se van a recuperar los datos de los Content Blobs. Por favor, espere...");
            try
            {
                using (HttpClient httpClient = new HttpClient(messageHandler, false))
                {
                    httpClient.BaseAddress = new Uri("https://manage.office.com");
                    httpClient.Timeout = new TimeSpan(0, 2, 0);

                    foreach (var content in contentBlobs)
                    {
                        string contentUri = content.ContentUri;
                        var response = httpClient.GetAsync($"{contentUri}").Result;
                        //var response = httpClient.GetAsync($"https://manage.office.com/api/v1.0/${tenantId}/activity/feed/audit/${contentId}$audit_sharepoint$Audit_SharePoint");

                        if (response.IsSuccessStatusCode)
                        {
                            string json = response.Content.ReadAsStringAsync().Result;
                            operationList.AddRange(JsonConvert.DeserializeObject<List<SharePointDataAudit>>(json));
                        }
                    }
                    Logging.LogMessage("Elementos encontrados en los Content Blobs: " + operationList.Count);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage("EXCEPCIÓN en GetDataContentBlobs: " + ex.Message);
                throw ex;
            }

            return operationList;
        }
    }
}
