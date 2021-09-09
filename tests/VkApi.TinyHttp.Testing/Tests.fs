module Tests

open System.IO
open System.Text
open Xunit
open VkApi.TinyHttp
open Newtonsoft.Json


type File = {
    [<JsonProperty "file">]
    File: string
}

type Response = {
    [<JsonProperty "url">]
    Url: string

    [<JsonProperty "files">]
    Files: File
}


let text = """Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
              Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
              Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
              Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."""


[<Fact>]
let ``Get Response`` () =
    let response =
        http {
            GET "https://httpbin.org/get"
        }

    let obj =
        response.Result
        |> JsonConvert.DeserializeObject<Response>

    Assert.Null obj.Files
    Assert.Equal ("https://httpbin.org/get", obj.Url)

[<Fact>]
let ``Post Request`` () =
    let content = Encoding.UTF8.GetBytes text

    let response =
        http {
            POST "https://httpbin.org/post"
            Content {
                Content = content;
                Name = "Test.txt"
            }
        }

    let obj =
        response.Result
        |> JsonConvert.DeserializeObject<Response>

    Assert.Equal (text, obj.Files.File)

[<Fact>]
let ``Post Request (Content First)`` () =
    let content = Encoding.UTF8.GetBytes text

    let response =
        http {
            Content {
                Content = content;
                Name = "Test.txt"
            }
            POST "https://httpbin.org/post"
        }

    let obj =
        response.Result
        |> JsonConvert.DeserializeObject<Response>

    Assert.Equal (text, obj.Files.File)

[<Fact>]
let ``Throw Exception If Change From Post to Get`` () =
    let TestCode () =
        let content = Encoding.UTF8.GetBytes text

        let response =
            http {
                Content {
                    Content = content;
                    Name = "Test.txt"
                }
                POST "https://httpbin.org/post"
                GET "https://httpbin.org/get"
            }

        response.Result
        |> JsonConvert.DeserializeObject<Response>
        |> ignore

    Assert.Throws<System.ArgumentException> (TestCode)
