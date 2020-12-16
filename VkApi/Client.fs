namespace VkApi


open FSharp.Control.Tasks.V2
open VkApi.Wrappers


type Client (login, password) =
    let apiVersion = "5.126"

    let authInfo =
            let clientId = 3697615
            let clientSecret = "AlVXZFMUqyrnABp8ncuU"
            Request.perform<AuthInfo> <| Get $"https://oauth.vk.com/token?grant_type=password&client_id={clientId}&client_secret={clientSecret}&username=%s{login}&password=%s{password}"

    member _.GetDocuments () =
        task {
            let! info = authInfo
            let! response = request {
                                url $"https://api.vk.com/method/docs.get?access_token={info.AccessToken}&v={apiVersion}"
                            } |> Request.perform<Response<Items<Document>>>

            return response.Response.Items
        }

    member _.RemoveDocument (doc: Document) =
        task {
            let! info = authInfo
            let! response = request {
                                url $"https://api.vk.com/method/docs.delete?access_token={info.AccessToken}&owner_id={doc.OwnerId}&doc_id=13&v={apiVersion}"
                            } |> Request.perform<Response<int>>

            ()
        }

    member _.UploadDocument path =
        task {
            let! info = authInfo
            let! response = request {
                                url $"https://api.vk.com/method/docs.getUploadServer?access_token={info.AccessToken}&v={apiVersion}"
                            } |> Request.perform<Response<UploadServer>>

            let! response = request {
                                url response.Response.Url
                                filename path
                            } |> Request.perform<UploadedFileInfo>

            let! response = request {
                                url $"https://api.vk.com/method/docs.save?access_token={info.AccessToken}&file={response.Info}&title={response.Title}&tags=&v={apiVersion}"
                            } |> Request.perform<Response<Doc<Document>>>

            return response.Response.Document
        }
