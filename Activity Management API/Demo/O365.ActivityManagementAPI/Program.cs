using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using O365.ActivityManagementAPI.Business;
using O365.ActivityManagementAPI.Entities;

namespace O365.ActivityManagementAPI
{
    public class Program
    {
        private static readonly string tenantId = ConfigurationManager.AppSettings["TenantID"];
        private static readonly string clientId = ConfigurationManager.AppSettings["ClientID"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private static OAuthMessageHandler messageHandler;
        
        private static List<O365Subscription> contentBlobs;
        private static List<SharePointDataAudit> operationListCB;

        public static void Main(string[] args)
        {
            Logging.LogMessage("Iniciando proceso...");

            messageHandler = new OAuthMessageHandler(tenantId, clientId, clientSecret, new HttpClientHandler());
            if (messageHandler != null)
            {
                try
                {
                    Logging.LogMessage("Iniciando suscripción a los servicios de la Activity Management API");
                    // Cancelando Suscripción                   
                    ManagementAPI.ManageSubscription(tenantId, messageHandler, OperationSubscriptions.OperationSubscription.Stop);
                    // Iniciando Suscripción                 
                    ManagementAPI.ManageSubscription(tenantId, messageHandler, OperationSubscriptions.OperationSubscription.Start);
                    // Iniciando ListSubscription                  
                    ManagementAPI.ListSubscription(tenantId, messageHandler);
                    // Obteniendo Content Blobs
                    contentBlobs = ManagementAPI.RetrieveContentBlobs(tenantId, messageHandler);
                    if ((contentBlobs != null) && (contentBlobs.Count > 0))
                    {
                        operationListCB = ManagementAPI.GetDataContentBlobs(messageHandler, contentBlobs);
                        if (operationListCB != null && operationListCB.Count > 0)
                        {
                            List<SharePointDataAudit> filteredDocumentsList = new List<SharePointDataAudit>();
                            filteredDocumentsList = operationListCB.Where(data => (
                                (data.Operation == "FileAccessed") ||
                                (data.Operation == "FileDownloaded") ||
                                (data.Operation == "FileDeleted") ||
                                (data.Operation == "FileUploaded") ||
                                (data.Operation == "FileModified") ||
                                (data.Operation == "FileRenamed") ||
                                (data.Operation == "SharingSet"))
                            ).ToList();
                            Logging.LogMessage("\tSe han identificado " + filteredDocumentsList.Count + " operaciones con documentos");

                            int i = 0;
                            foreach (var data in filteredDocumentsList)
                            {
                                Console.WriteLine("\t- CreationTime: " + data.CreationTime);
                                Console.WriteLine("\t- ID: " + data.Id);
                                Console.WriteLine("\t- Operation: " + data.Operation);
                                Console.WriteLine("\t- OrganizationId: " + data.OrganizationId);
                                Console.WriteLine("\t- RecordType: " + data.RecordType);
                                Console.WriteLine("\t- UserKey: " + data.UserKey);
                                Console.WriteLine("\t- UserType: " + data.UserType);
                                Console.WriteLine("\t- Version: " + data.Version);
                                Console.WriteLine("\t- Workload: " + data.Workload);
                                Console.WriteLine("\t- ClientIp: " + data.ClientIp);
                                Console.WriteLine("\t- ObjectID: " + data.ObjectId);
                                Console.WriteLine("\t- CorrelationID: " + data.CorrelationId);
                                Console.WriteLine("\t- EventSource: " + data.EventSource);
                                Console.WriteLine("\t- ItemType: " + data.ItemType); //List, File or ListItem                              
                                Console.WriteLine("\t- ListID: " + data.ListID);
                                Console.WriteLine("\t- ListItemUniqueID: " + data.ListItemUniqueId);
                                Console.WriteLine("\t- Site: " + data.Site);
                                Console.WriteLine("\t- UserAgent: " + data.UserAgent);
                                Console.WriteLine("\t- SiteURL: " + data.SiteUrl);
                                Console.WriteLine("\t- SourceFileName: " + data.SourceFileName);
                                Console.WriteLine("\t- SourceRelativeURL: " + data.SourceRelativeURL);
                                Console.WriteLine("\t- SourceFileExtension: " + data.SourceFileExtension);
                                Console.WriteLine("\t- UserId: " + data.UserId);
                                Console.WriteLine("\t- WebID: " + data.WebID);
                                Console.WriteLine("\t------------------------------------------------");
                                
                                i++;
                                if(i == 5) { break; }
                            }

                            List<SharePointDataAudit> filteredPagesList = new List<SharePointDataAudit>();
                            filteredPagesList = operationListCB.Where(data => (
                                (data.Operation == "PageViewed") ||
                                (data.Operation == "PageAccessed"))
                            ).ToList();
                            Logging.LogMessage("\tSe han identificado " + filteredPagesList.Count + " operaciones con páginas");

                            //TODO: Volcar información a una base de datos para manipular la información con Power BI
                        }
                        else
                        {
                            Logging.LogMessage("No se han encontrado datos en los Contents Blobs");
                        }
                    }
                    else
                    {
                        Logging.LogMessage("No se han encontrado Content Blobs");
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogMessage("EXCEPCION: " + ex.Message);
                    throw ex;
                }
            }

            Logging.LogMessage("El proceso ha finalizado.");
            Console.ReadLine();
        }
    }
}
