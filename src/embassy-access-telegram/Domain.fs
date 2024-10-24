﻿module EA.Telegram.Domain

open System.Threading
open Microsoft.Extensions.Configuration
open Infrastructure
open EA.Domain
open Web.Telegram.Domain
open Web.Telegram.Domain.Producer

module Key =

    [<Literal>]
    let internal EMBASSY_ACCESS_TELEGRAM_BOT_TOKEN = "EMBASSY_ACCESS_TELEGRAM_BOT_TOKEN"

    [<Literal>]
    let Chats = "chats"

type Chat =
    { Id: ChatId
      Subscriptions: Set<RequestId> }

module External =

    type Chat() =
        member val Id = 0L with get, set
        member val Subscriptions = List.empty<string> with get, set
