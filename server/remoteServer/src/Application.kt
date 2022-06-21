package com.xballserver.remoteserver

import com.xballserver.remoteserver.routing.Connection
import com.xballserver.remoteserver.routing.configureRouting
import com.xballserver.remoteserver.infrastructure.*
import io.ktor.serialization.kotlinx.json.*
import io.ktor.server.application.*
import io.ktor.server.plugins.contentnegotiation.*
import io.ktor.server.websocket.*
import kotlinx.coroutines.launch
import java.util.*
import kotlin.collections.LinkedHashSet

fun main(args: Array<String>): Unit = io.ktor.server.netty.EngineMain.main(args)


@Suppress("unused") // Referenced in application.conf
@kotlin.jvm.JvmOverloads
fun Application.module(testing: Boolean = false) {
    install(WebSockets)
    install(ContentNegotiation) {
        json()
    }

    val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())
    val gamesManager = GamesManager(connections)
    val lobbyManager = LobbyManager(gamesManager)
    val authenticationManager = AuthenticationManager()

    configureRouting(gamesManager, lobbyManager, authenticationManager, connections)

    val logger = Logger(lobbyManager, gamesManager, connections)
    launch { logger.logPeriodically() }

    val cleaner = Cleaner(lobbyManager, gamesManager, connections)
    launch { cleaner.cleanPeriodically() }
}
