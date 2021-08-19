namespace VkApi.Core

open System.Threading.Tasks
open VkApi.TinyHttp
open FSharp.Control.Tasks.V2


type Client private (info) =
    let apiVersion = "5.126"

    static member Create (login, password): Task<Client> =
        task {
            let clientId = 2274003
            let clientSecret = "hHbZxrka2uZ6jB1inYsH"
            let! stream =
                http {
                    GET $"https://oauth.vk.com/token?grant_type=password&client_id={clientId}&client_secret={clientSecret}&username=%s{login}&password=%s{password}"
                }

            let! info = ResponseConverter.ConvertAsync<AuthenticationInfo> stream

            return Client (info)
        }


    member _.GetDocumentsAsync () =
        task {
            let! stream =
                http {
                    GET $"https://api.vk.com/method/docs.get?access_token={info.AccessToken}&v={apiVersion}"
                }

            let! response = ResponseConverter.ConvertAsync<Response<Items<Document>>> stream
            return response.Response.Items
        }

    member _.UploadDocumentAsync (name, stream) =
        let GetUploadServerAsync () =
            task {
                let! stream =
                    http {
                        GET $"https://api.vk.com/method/docs.getUploadServer?access_token={info.AccessToken}&v={apiVersion}"
                    }

                let! response = ResponseConverter.ConvertAsync<Response<UploadServer>> stream
                return response.Response
            }

        let UploadDocumentAsync (serverInfo: UploadServer) =
            task {
                let! stream =
                    http {
                        POST serverInfo.Url
                        Content { Name = name; Content = stream }
                    }

                let! response = ResponseConverter.ConvertAsync<UploadedFileInfo> stream
                return response
            }

        let SaveDocumentAsync (info: AuthenticationInfo) (fileInfo: UploadedFileInfo) =
            task {
                let! stream =
                    http {
                        GET $"https://api.vk.com/method/docs.save?access_token={info.AccessToken}&file={fileInfo.Info}&title={fileInfo.Title}&tags=&v={apiVersion}"
                    }

                let! response = ResponseConverter.ConvertAsync<Response<Doc>> stream;
                let mutable document = response.Response.Document
                document.Hash <- fileInfo.Hash

                return document                
            }

        task {
            let! serverInfo = GetUploadServerAsync ()
            let! fileInfo = UploadDocumentAsync serverInfo
            return! SaveDocumentAsync info fileInfo
        }

    member _.RemoveDocumentAsync (doc: Document) =
        task {
            let! stream =
                http {
                    GET $"https://api.vk.com/method/docs.delete?access_token={info.AccessToken}&owner_id={doc.OwnerId}&doc_id={doc.Id}&v={apiVersion}"
                }

            let! result = ResponseConverter.ConvertAsync<Response<int>> stream
            if result.Response <> 1 then failwith "Test"
        }
