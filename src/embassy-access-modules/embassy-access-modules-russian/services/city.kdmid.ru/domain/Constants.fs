﻿[<RequireQualifiedAccess>]
module EA.Embassies.Russian.Kdmid.Domain.Constants

let internal SUPPORTED_SUB_DOMAINS =
    Map
        [ "belgrad", "EMB.RUS.SRB.BEG"
          "budapest", "EMB.RUS.HUN.BUD"
          "sarajevo", "EMB.RUS.BIH.SJJ"
          "berlin", "EMB.RUS.DEU.BER"
          "podgorica", "EMB.RUS.MNE.POD"
          "tirana", "EMB.RUS.ALB.TIA"
          "paris", "EMB.RUS.FRA.PAR"
          "rome", "EMB.RUS.ITA.ROM"
          "dublin", "EMB.RUS.IRL.DUB"
          "bern", "EMB.RUS.CHE.BER"
          "helsinki", "EMB.RUS.FIN.HEL"
          "hague", "EMB.RUS.NLD.HAG"
          "ljubljana", "EMB.RUS.SVN.LJU" ]

module ErrorCode =

    [<Literal>]
    let PAGE_HAS_ERROR = "PageHasError"

    [<Literal>]
    let NOT_CONFIRMED = "RequestNotConfirmed"

    [<Literal>]
    let CONFIRMATION_EXISTS = "ConfirmationExists"

    [<Literal>]
    let REQUEST_DELETED = "RequestDeleted"
