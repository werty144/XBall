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

    val connectionManager = ConnectionManager()
    val gamesManager = GamesManager(connectionManager)
    val lobbyManager = LobbyManager(gamesManager)
    val authenticationManager = AuthenticationManager()
    val gameStartManager = GameStartManager(gamesManager, lobbyManager, connectionManager)

    configureRouting(gamesManager, authenticationManager, connectionManager, gameStartManager)

    val logger = Logger(lobbyManager, gamesManager, connectionManager)
    launch { logger.logPeriodically() }

    val cleaner = Cleaner(lobbyManager, gamesManager, connectionManager)
    launch { cleaner.cleanPeriodically() }
}
