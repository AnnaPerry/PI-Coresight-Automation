using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoresightAutomation.Types;
using Microsoft.Framework.WebEncoders;
using CoresightAutomation.PIWebAPI.DTO;
using Newtonsoft.Json;
using System.Net;

namespace CoresightAutomation.PIWebAPI
{
    public class PIWebAPIClient : IDisposable
    {
        public PIWebAPIClient(Uri server, ICredentials credentials = null)
        {
            if (!server.AbsolutePath.EndsWith("/"))
            {
                throw new ArgumentException("Server Uri must end with a trailing /", "server");
            }

            Server = server;
            clientHandler = credentials == null ? new HttpClientHandler() { UseDefaultCredentials = true } : new HttpClientHandler() { Credentials = credentials };
            httpClient = new HttpClient(clientHandler) { BaseAddress = Server };
        }

        public Uri Server { get; private set; }

        private HttpClientHandler clientHandler;
        private HttpClient httpClient;

        public Task<AFElementDTO> GetElementDTOByPathAsync(string absolutePath)
        {
            string requestUri = ConstructPathQuery("elements", absolutePath);
            return GetObjectAsync<AFElementDTO>(requestUri);
        }

        public Task<AFElementDTO> GetElementDTOAsync(string webId)
        {
            string requestUri = string.Format("elements/{0}", webId);
            return GetObjectAsync<AFElementDTO>(requestUri);
        }

        public Task<AFElementTemplateSlim> GetElementTemplateSlimAsync(AFElementDTO forElement, string alternateTemplateName = null)
        {
            string templateName = string.IsNullOrWhiteSpace(alternateTemplateName) ? forElement.TemplateName : alternateTemplateName;

            string[] pathTokens = forElement.Path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string serverName = pathTokens[0];
            string databaseName = pathTokens[1];
            string templatePath = string.Format("\\\\{0}\\{1}\\ElementTemplates[{2}]", serverName, databaseName, templateName);
            return GetElementTemplateSlimAsync(templatePath);
        }

        public async Task<AFElementTemplateSlim> GetElementTemplateSlimAsync(AFElementTemplateDTO elementTemplateDTO)
        {
            List<AFAttributeTemplateDTO> attributeTemplates = await GetAllAttributeTemplateDTOsAsync(elementTemplateDTO);
            return elementTemplateDTO.ToSlim(attributeTemplates);
        }

        public async Task<AFElementTemplateSlim> GetElementTemplateSlimAsync(string absolutePath)
        {
            AFElementTemplateDTO elementTemplateDTO = await GetElementTemplateDTOAsync(absolutePath);
            return await GetElementTemplateSlimAsync(elementTemplateDTO).ConfigureAwait(false);
        }

        private Task<AFElementTemplateDTO> GetElementTemplateDTOAsync(string absolutePath)
        {
            string requestUri = ConstructPathQuery("elementtemplates", absolutePath);
            return GetObjectAsync<AFElementTemplateDTO>(requestUri);
        }

        private async Task<List<AFAttributeTemplateDTO>> GetAllAttributeTemplateDTOsAsync(AFElementTemplateDTO elementTemplateDTO)
        {
            List<AFAttributeTemplateDTO> topLevelAttributes = await GetTopLevelAttributeTemplateDTOsAsync(elementTemplateDTO).ConfigureAwait(false);

            List<Task<List<AFAttributeTemplateDTO>>> attributeQueryTasks = new List<Task<List<AFAttributeTemplateDTO>>>();
            foreach (AFAttributeTemplateDTO topLevelAttribute in topLevelAttributes)
            {
                Task<List<AFAttributeTemplateDTO>> attributeQueryTask = GetAllAttributeTemplateDTOs(topLevelAttribute);
                attributeQueryTasks.Add(attributeQueryTask);
            }
            IEnumerable<List<AFAttributeTemplateDTO>> attributeQueryTaskResults = await Task.WhenAll<List<AFAttributeTemplateDTO>>(attributeQueryTasks).ConfigureAwait(false);
            return attributeQueryTaskResults.SelectMany(aq => aq).ToList();
        }

        private async Task<List<AFAttributeTemplateDTO>> GetAllAttributeTemplateDTOs(AFAttributeTemplateDTO attributeTemplateDTO)
        {
            List<AFAttributeTemplateDTO> selfAndChildren = new List<AFAttributeTemplateDTO>() { attributeTemplateDTO };
            if (attributeTemplateDTO.HasChildren)
            {
                List<AFAttributeTemplateDTO> children = await GetImmediateChildAttributeTemplateDTOsAsync(attributeTemplateDTO.WebId);
                foreach (AFAttributeTemplateDTO child in children)
                {
                    List<AFAttributeTemplateDTO> childAndAllDescendants = await GetAllAttributeTemplateDTOs(child);
                    selfAndChildren.AddRange(childAndAllDescendants);
                }
            }
            return selfAndChildren;
        }

        private async Task<List<AFAttributeTemplateDTO>> GetTopLevelAttributeTemplateDTOsAsync(AFElementTemplateDTO elementTemplateDTO)
        {
            List<AFAttributeTemplateDTO> allTopLevelAttributes = new List<AFAttributeTemplateDTO>();

            string requestUri = string.Format("elementtemplates/{0}/attributetemplates", elementTemplateDTO.WebId);
            List<AFAttributeTemplateDTO> topLevelAttributes = await GetObjectAsync<ItemCollectionDTO<AFAttributeTemplateDTO>>(requestUri);

            //Recurse to read attributes from the chain of base templates
            if (elementTemplateDTO.HasBaseTemplate)
            {
                AFElementTemplateDTO baseTemplateDTO = await GetObjectAsync<AFElementTemplateDTO>(elementTemplateDTO.BaseTemplateUri);
                List<AFAttributeTemplateDTO> baseAttributes = await GetTopLevelAttributeTemplateDTOsAsync(baseTemplateDTO);
                topLevelAttributes.AddRange(baseAttributes);
            }

            return topLevelAttributes;
        }

        private Task<ItemCollectionDTO<AFAttributeTemplateDTO>> GetImmediateChildAttributeTemplateDTOsAsync(string attributeTemplateWebId)
        {
            string requestUri = string.Format("attributetemplates/{0}/attributetemplates", attributeTemplateWebId);
            return GetObjectAsync<ItemCollectionDTO<AFAttributeTemplateDTO>>(requestUri);
        }

        private Task<AFAttributeTemplateDTO> GetAttributeTemplateDTOAsync(string webId)
        {
            string requestUriRaw = "attributeTemplates/" + webId;
            return GetObjectAsync<AFAttributeTemplateDTO>(requestUriRaw);
        }

        private async Task<T> GetObjectAsync<T>(string requestUri)
        {
            HttpResponseMessage objectResponseMessage = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

            try
            {
                objectResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("GET request failed for {0}", objectResponseMessage.RequestMessage.RequestUri);
                Console.WriteLine("The response was {0}: {1}", (int)objectResponseMessage.StatusCode, objectResponseMessage.ReasonPhrase);
                throw;
            }

            string objectJson = await objectResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(objectJson);
        }

        private string ConstructPathQuery(string uriPath, string afObjectPath)
        {
            UrlEncoder urlEncoder = new UrlEncoder();
            string afObjectPathEncoded = urlEncoder.UrlEncode(afObjectPath);
            return string.Format("{0}?path={1}", uriPath, afObjectPathEncoded);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                    clientHandler.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
