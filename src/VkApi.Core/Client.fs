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

            let info = match ResponseParser.Parse<AuthenticationInfo> response with
                       | Error error -> Exception.RaiseSpecificException error.Error
                       | Ok response -> response

            return Client (info)
        }


    member _.GetDocumentsAsync () =
        let TryGetDocuments () =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.get?access_token={info.AccessToken}&v={apiVersion}"
                    }

                return
                    match ResponseParser.Parse<Response<Items<Document>>> response with
                    | Error error -> Exception.RaiseSpecificException error.Error
                    | Ok resp -> resp.Response.Items
            }

        retry 20 {
            return! TryGetDocuments
        }

    member _.UploadDocumentAsync (name, stream) =
        let GetUploadServerAsync () =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.getUploadServer?access_token={info.AccessToken}&v={apiVersion}"
                    }

                return
                    match ResponseParser.Parse<Response<UploadServer>> response with
                    | Error error -> Exception.RaiseSpecificException error.Error
                    | Ok response -> response.Response
            }

        let UploadDocumentAsync (serverInfo: UploadServer) =
            task {
                let! response =
                    http {
                        POST serverInfo.Url
                        Content { Name = name; Content = stream }
                    }

                return
                    match ResponseParser.Parse<UploadedFileInfo> response with
                    | Error error -> Exception.RaiseSpecificException error.Error
                    | Ok response -> response
            }

        let SaveDocumentAsync (info: AuthenticationInfo) (fileInfo: UploadedFileInfo) =
            task {
                let! response =
                    http {
                        GET $"https://api.vk.com/method/docs.save?access_token={info.AccessToken}&file={fileInfo.Info}&title={fileInfo.Title}&tags=&v={apiVersion}"
                    }

                let mutable document =
                    match ResponseParser.Parse<Response<Doc>> response with
                    | Error error -> Exception.RaiseSpecificException error.Error
                    | Ok response -> response.Response.Document

                document.Hash <- fileInfo.Hash

                return document
            }

        task {
            let! serverInfo =
                retry 20 {
                    return! GetUploadServerAsync
                }

            let! fileInfo =
                retry 20 {
                    return! fun () -> UploadDocumentAsync serverInfo
                }

            let! doc =
                retry 20 {
                    return! fun () -> SaveDocumentAsync info fileInfo
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

                let result =
                    match ResponseParser.Parse<Response<int>> response with
                    | Error error -> Exception.RaiseSpecificException error.Error
                    | Ok response -> response.Response

                if result <> 1 then failwith "Test"
            }

        retry 20 {
            do! TryRemoveDocumentAsync
        }
