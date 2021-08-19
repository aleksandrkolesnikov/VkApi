namespace VkApi.Core

open System
open Newtonsoft.Json


type Document [<JsonConstructor>] internal (id: uint64, owner_id: uint64, title: string, size: uint64, date: uint64, url: string) =
    let ToDateTime =
        let unixEpoch = DateTime (1970, 1, 1, 0, 0, 0, 0)
        float >> unixEpoch.AddSeconds

    let mutable hash: byte array = null

    interface IEquatable<Document> with
        member _.Equals document = (id = document.Id) && (owner_id = document.OwnerId)

    override self.Equals obj =
        match obj with
        | :? IEquatable<Document> as doc -> doc.Equals self
        | _ -> false

    override _.GetHashCode () = (id ^^^ owner_id) |> int

    member _.Id = id
    member _.OwnerId = owner_id
    member _.Title = title
    member _.Size = size
    member _.Date = date |> ToDateTime
    member _.Url = url
    member _.Hash
        with get () = hash
        and internal set (value) = hash <- value
