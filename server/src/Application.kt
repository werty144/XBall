package com.example

import com.example.infrastructure.InvitesManager
import com.example.infrastructure.GamesManager
import com.example.infrastructure.Logger
import com.example.routing.Connection
import com.example.routing.configureRouting
import io.ktor.application.*
import io.ktor.websocket.*
import kotlinx.coroutines.launch
import java.util.*
import kotlin.collections.LinkedHashSet

fun main(args: Array<String>): Unit = io.ktor.server.netty.EngineMain.main(args)


@Suppress("unused") // Referenced in application.conf
@kotlin.jvm.JvmOverloads
fun Application.module(testing: Boolean = false) {
    install(WebSockets)

    val invitesManager = InvitesManager()
    val gamesManager = GamesManager()
    val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())
    configureRouting(gamesManager, invitesManager, connections)

    val logger = Logger(invitesManager, gamesManager, connections)
    launch { logger.logPeriodically() }
}
