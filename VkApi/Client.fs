namespace VkApi

open System.IO
open FSharp.Control.Tasks.V2
open VkApi.Wrappers


type Client (login, password) =
    let apiVersion = "5.126"

    let authInfo =
            let clientId = 3697615
            let clientSecret = "AlVXZFMUqyrnABp8ncuU"
            Get $"https://oauth.vk.com/token?grant_type=password&client_id={clientId}&client_secret={clientSecret}&username=%s{login}&password=%s{password}"
            |> Request.performAsync<AuthInfo>

    member _.GetDocumentsAsync () =
        task {
            let! info = authInfo
            let! response =
                Get $"https://api.vk.com/method/docs.get?access_token={info.AccessToken}&v={apiVersion}"
                |> Request.performAsync<Response<Items<Document>>>

            return response.Response.Items
        }

    member _.RemoveDocumentAsync (doc: Document) =
        task {
            let! info = authInfo
            let! _ =
                Get $"https://api.vk.com/method/docs.delete?access_token={info.AccessToken}&owner_id={doc.OwnerId}&doc_id={doc.Id}&v={apiVersion}"
                |> Request.performAsync<Response<int>>

            ()
        }

    member self.UploadDocumentAsync (stream: Stream, name) =
        task {
            use memory = new MemoryStream ()
            do! stream.CopyToAsync memory

            return! self.UploadDocumentAsync (memory.ToArray (), name)
        }

    member _.UploadDocumentAsync (source, name) =
        task {
            let! info = authInfo
            let! response =
                Get $"https://api.vk.com/method/docs.getUploadServer?access_token={info.AccessToken}&v={apiVersion}"
                |> Request.performAsync<Response<UploadServer>>

            let! response =
                Post (response.Response.Url, name, source)
                |> Request.performAsync<UploadedFileInfo>

            let hash = response.Hash

            let! response =
                Get $"https://api.vk.com/method/docs.save?access_token={info.AccessToken}&file={response.Info}&title={response.Title}&tags=&v={apiVersion}"
                |> Request.performAsync<Response<Doc>>

            let mutable document = response.Response.Document
            document.Hash <- hash

            return document
        }        
