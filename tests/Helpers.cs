﻿using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace Tests
{
    public static class Helpers
    {
        public static async Task<IHtmlDocument> GetDocumentAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var document = await BrowsingContext.New()
                .OpenAsync(ResponseFactory, CancellationToken.None);
            return (IHtmlDocument)document;

            void ResponseFactory(VirtualResponse htmlResponse)
            {
                htmlResponse
                    .Address(response.RequestMessage.RequestUri)
                    .Status(response.StatusCode);

                MapHeaders(response.Headers);
                MapHeaders(response.Content.Headers);

                htmlResponse.Content(content);

                void MapHeaders(HttpHeaders headers)
                {
                    foreach (var header in headers)
                    {
                        foreach (var value in header.Value)
                        {
                            htmlResponse.Header(header.Key, value);
                        }
                    }
                }
            }
        }

        public static string ToHtml(this IHtmlDocument content)
        {
            var writer = new StringWriter();
            content.ToHtml(writer, new PrettyMarkupFormatter());
            var result = writer.ToString();
            return result;
        }
    }
}