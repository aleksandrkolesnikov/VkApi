namespace VkApi.Core

open System.Threading.Tasks
open VkApi.TinyHttp


type Client private (info) =
    let apiVersion = "5.126"

    static member Create (login, password): Task<Client> =
        task {
            let clientId = 2274003
            let clientSecret = "hHbZxrka2uZ6jB1inYsH"
            let! response =
                http {
                    GET $"https://oauth.vk.com/token?grant_type=password&client_id={clientId}&client_secret={clientSecret}&username=%s{login}&password=%s{password}"
                }

            let info = ResponseParser.TryParse<AuthenticationInfo> response

            return Client (info)
        }


    member _.GetDocumentsAsync () =
        let TryGetDocuments () =
            task {
                let! text =
                    http {
                        GET $"https://api.vk.com/method/docs.get?access_token={info.AccessToken}&v={apiVersion}"
                    }

                let response = ResponseParser.TryParse<Response<Items<Document>>> text
                return response.Response.Items
            }

        retry 20 {
            let! docs = TryGetDocuments

            // TODO: implement return! docs
            return docs;
        }

    member _.UploadDocumentAsync (name, stream) =
        let GetUploadServerAsync () =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.getUploadServer?access_token={info.AccessToken}&v={apiVersion}"
                    }

                let response = ResponseParser.TryParse<Response<UploadServer>> response
                return response.Response
            }

        let UploadDocumentAsync (serverInfo: UploadServer) =
            task {
                let! response =
                    http {
                        POST serverInfo.Url
                        Content { Name = name; Content = stream }
                    }

                let response = ResponseParser.TryParse<UploadedFileInfo> response
                return response
            }

        let SaveDocumentAsync (info: AuthenticationInfo) (fileInfo: UploadedFileInfo) =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.save?access_token={info.AccessToken}&file={fileInfo.Info}&title={fileInfo.Title}&tags=&v={apiVersion}"
                    }

                let response = ResponseParser.TryParse<Response<Doc>> response;
                let mutable document = response.Response.Document
                document.Hash <- fileInfo.Hash

                return document                
            }

        task {
            let! serverInfo =
                retry 20 {
                    let! info = GetUploadServerAsync
                    return info
                }

            let! fileInfo =
                retry 20 {
                    let! info = fun () -> UploadDocumentAsync serverInfo
                    return info
                }

            let! doc =
                retry 20 {
                    let! doc = fun () -> SaveDocumentAsync info fileInfo
                    return doc
                }

            return doc
        }

    member _.RemoveDocumentAsync (doc: Document) =
        let TryRemoveDocumentAsync () =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.delete?access_token={info.AccessToken}&owner_id={doc.OwnerId}&doc_id={doc.Id}&v={apiVersion}"
                    }

                let result = ResponseParser.TryParse<Response<int>> response
                if result.Response <> 1 then failwith "Test"
            }

        retry 20 {
            let! tmp = TryRemoveDocumentAsync
            return tmp
        }
