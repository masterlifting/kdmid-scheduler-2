﻿module internal EA.Worker.Countries.Slovenia

open Infrastructure.Domain
open Worker.Domain
open EA.Core.Domain
open EA.Worker.Embassies

let private Ljubljana =
    Graph.Node({ Name = "Ljubljana"; Task = None }, [ Russian.addTasks <| Slovenia Ljubljana ])

let Tasks = Graph.Node({ Name = "Slovenia"; Task = None }, [ Ljubljana ])
