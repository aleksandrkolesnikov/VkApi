namespace VkApi


type internal VkRequest =
    | Get of Url: string
    | Post of Url: string * FileName: string

type internal RequestBuilder () =
    member _.Yield (_) =
        Get ""

    [<CustomOperation "url">]
    member _.Url (req, text) =
        match req with
        | Get _ -> Get text
        | Post (_, filename) -> Post (text, filename)

    [<CustomOperation "filename">]
    member _.FileName (req, name) =
        match req with
        | Post (url, _) -> Post (url, name)
        | Get url -> Post (url, name)


[<AutoOpen>]
module internal ReqBuilder =
    let request = RequestBuilder ()