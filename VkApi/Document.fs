namespace VkApi

open Newtonsoft.Json
open System


type Document =
    struct
        val Id: uint64
        val OwnerId: uint64
        val Title: string
        val Size: uint64
        val Date: DateTime

        [<JsonConstructor>]
        internal new (id, owner_id, title, size, date) =
            let toDateTime date =
                let unixEpoch = DateTime (1970, 1, 1, 0, 0, 0, 0)
                date |> unixEpoch.AddSeconds
            {
                Id = id
                OwnerId = owner_id
                Title = title
                Size = size
                Date = date |> float |> toDateTime
            }
    end

(*type Document = {
    [<JsonProperty "id">]
    Id: uint64

    [<JsonProperty "owner_id">]
    OwnerId: uint64

    [<JsonProperty "title">]
    Title: string

    [<JsonProperty "size">]
    Size: uint64

    [<JsonProperty "date">]
    UnixDate: uint64
}
with
    member self.Date =
        let unixEpoch = DateTime (1970, 1, 1, 0, 0, 0, 0)
        self.UnixDate |> float |> unixEpoch.AddSeconds*)