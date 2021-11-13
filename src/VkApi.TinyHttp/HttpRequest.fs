namespace VkApi.TinyHttp


[<NoComparison>]
type Content = {
    Name: string
    Content: byte array
}

[<NoComparison>]
type HttpRequest =
    | Get of string
    | Post of string * Content

[<RequireQualifiedAccess; CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal HttpRequest =

    open System.Net.Http


    let private client = new HttpClient ()

    let private GetAsync (url: string) =
        url
        |> client.GetStringAsync

    let private PostAsync (url: string) content =
        task {
            let httpContent = new MultipartFormDataContent ()
            use innerContent = new ByteArrayContent (content.Content)
            httpContent.Add (innerContent, "file", content.Name)
            let! response =
                (url, httpContent)
                |> client.PostAsync

            return! response.Content.ReadAsStringAsync ()
        }

    let SendAsync request =
        match request with
        | Get url -> url |> GetAsync
        | Post (url, content) -> PostAsync url content
