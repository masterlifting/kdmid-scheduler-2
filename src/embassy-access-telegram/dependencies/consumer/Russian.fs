﻿[<RequireQualifiedAccess>]
module EA.Telegram.Dependencies.Consumer.Russian

open EA.Telegram.Domain
open Infrastructure.Domain
open Infrastructure.Prelude
open EA.Core.Domain
open Web.Telegram.Domain
open EA.Core.DataAccess
open EA.Telegram.DataAccess

type Dependencies =
    { ChatId: ChatId
      MessageId: int
      ServiceGraph: Async<Result<Graph.Node<ServiceNode>, Error'>>
      createOrUpdateChat: Chat -> Async<Result<Chat, Error'>>
      createOrUpdateRequest: Request -> Async<Result<Request, Error'>> }

    static member create(deps: Core.Dependencies) =
        let result = ResultBuilder()

        result {

            let serviceGraph =
                "RussianServices"
                |> deps.initServiceGraphStorage
                |> ResultAsync.wrap ServiceGraph.get

            let createOrUpdateChat chat =
                deps.chatStorage |> Chat.Command.createOrUpdate chat

            let createOrUpdateRequest request =
                deps.requestStorage |> Request.Command.createOrUpdate request

            return
                { ChatId = deps.ChatId
                  MessageId = deps.MessageId
                  ServiceGraph = serviceGraph
                  createOrUpdateChat = createOrUpdateChat
                  createOrUpdateRequest = createOrUpdateRequest }
        }
